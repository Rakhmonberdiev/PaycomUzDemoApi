using PaycomUz.Abstractions;
using PaycomUz.Core.Errors;
using PaycomUz.Models.Errors;
using PaycomUz.Models.Requests;
using PaycomUz.Models.Requests.Params;
using PaycomUz.Models.Responses;
using PaycomUzDemoApi.Errors;
using PaycomUzDemoApi.Services;
using System.Text.Json;

namespace PaycomUzDemoApi.PaycomHandlers
{
    public class CheckPerformTransactionHandler : ICheckPerformTransactionHandler
    {
        private readonly MockDataService _data;
        public CheckPerformTransactionHandler(MockDataService data) => _data = data;
        public Task<PaycomResponse> HandleAsync(PaycomRequest request)
        {
            // Десериализуем параметры запроса из JSON в объект нужного типа
            var param = JsonSerializer.Deserialize<CheckPerformTransactionParams>(request.Params.GetRawText());
            if (param == null)
                // Если не удалось распознать тело запроса — выбрасываем ошибку некорректного запроса
                throw new TransactionError(PaymeError.CantDoOperation, request.Id, "Invalid request");

            // Пытаемся получить значение user_id из словаря Account
            if (!param.Account.TryGetValue("user_id", out var userIdRaw) || !Guid.TryParse(userIdRaw, out var userId))
                // Если user_id отсутствует или не является корректным GUID — выбрасываем ошибку
                throw new TransactionError(PaymeError.CantDoOperation, request.Id, "user_id");
            // Аналогично пытаемся получить product_id
            if (!param.Account.TryGetValue("product_id", out var productIdRaw) || !Guid.TryParse(productIdRaw, out var productId))
                // Если product_id отсутствует или невалиден — выбрасываем ошибку
                throw new TransactionError(PaymeError.CantDoOperation, request.Id, "product_id");

            // Ищем в базе (или коллекции) заказ по комбинации userId и productId
            var order =  _data.Orders.FirstOrDefault(o =>o.UserId == userId &&o.ProductId == productId);
            if (order == null)
                // Если такого заказа нет — выбрасываем кастомную ошибку "заказ не найден"
                throw new TransactionError(CustomPaymeErrors.OrderNotFound, request.Id, "order");

            // Сравниваем сумму заказа (умноженную на 100, т.к. Paycom оперирует копейками)
            if (order.Amount * 100 != param.Amount)
                // Если переданная в запросе сумма не совпадает с ожидаемой — ошибка некорректной суммы
                throw new TransactionError(PaymeError.InvalidAmount, request.Id, "amount");


            // Получаем информацию о самом продукте (для формирования чека)
            var product = _data.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
                // Если продукт не найден в базе — выдаем общую ошибку операции
                throw new TransactionError(PaymeError.CantDoOperation, request.Id, "product");


            // Формируем успешный ответ, указываем, что транзакцию можно провести (allow = true)
            // Можно опустить секцию detail, если не требуется формирование товарного чека.
            // Но при фиксации товара в налоговых органах через Payme обязательно нужно отправить блок detail.
            var result = PaycomResponse.CreateResult(request.Id,
                new
                {
                    allow = true,
                    detail = new
                    {
                        receipt_type = 0,
                        items = new[]
                        {
                                new
                                {
                                    title = product.Name, // Название товара в чеке
                                    price = product.Price * 100, // Цена товара в тийинах
                                    count = 1, // Количество единиц товара
                                    code =  product.IkpuCode, // код *ИКПУ
                                    vat_percent = 0, // НДС в процентах
                                    package_code = product.UnitCode, //Код упаковки для конкретного товара или услуги
                                }
                        }
                    },

                });
            // Возвращаем сформированный ответ
            return Task.FromResult(result);

        }
    }
}

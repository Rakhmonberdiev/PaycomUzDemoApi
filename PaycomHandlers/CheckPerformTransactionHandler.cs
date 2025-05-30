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
            var param = JsonSerializer.Deserialize<CheckPerformTransactionParams>(request.Params.GetRawText());
            if (param == null)
                throw new TransactionError(PaymeError.CantDoOperation, request.Id, "Invalid request");


            if (!param.Account.TryGetValue("user_id", out var userIdRaw) || !Guid.TryParse(userIdRaw, out var userId))
                throw new TransactionError(PaymeError.CantDoOperation, request.Id, "user_id");
            if (!param.Account.TryGetValue("product_id", out var productIdRaw) || !Guid.TryParse(productIdRaw, out var productId))
                throw new TransactionError(PaymeError.CantDoOperation, request.Id, "product_id");

            var order =  _data.Orders.FirstOrDefault(o =>o.UserId == userId &&o.ProductId == productId);
            if (order == null)
                throw new TransactionError(CustomPaymeErrors.OrderNotFound, request.Id, "order");
            
            if (order.Amount * 100 != param.Amount)
                throw new TransactionError(PaymeError.InvalidAmount, request.Id, "amount");


            var product = _data.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
                throw new TransactionError(PaymeError.CantDoOperation, request.Id, "product");
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
                                    title = product.Name,
                                    price = (int)(product.Price * 100),
                                    count = 1,
                                    code =  product.IkpuCode,
                                    vat_percent = 0,
                                    package_code = product.UnitCode,
                                }
                        }
                    },

                });
            return Task.FromResult(result);

        }
    }
}

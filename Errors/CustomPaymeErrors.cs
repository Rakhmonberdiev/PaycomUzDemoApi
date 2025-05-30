using PaycomUz.Models.Common;
using PaycomUz.Models.Errors;

namespace PaycomUzDemoApi.Errors
{
    public static class CustomPaymeErrors
    {
        public static PaymeError UserNotFound => new PaymeError
        {
            Name = "UserNotFound",
            Code = -31050,
            Message = new LocalizedMessage
            {
                Uz = "Foydalanuvchi topilmadi",
                Ru = "Пользователь не найден",
                En = "User not found"
            }
        };
        public static PaymeError OrderNotFound => new PaymeError
        {
            Name = "OrderNotFound",
            Code = -31050,
            Message = new LocalizedMessage
            {
                Uz = "Buyurtma topilmadi",
                Ru = "Заказ не найден",
                En = "Order not found"
            }
        };
    }
}

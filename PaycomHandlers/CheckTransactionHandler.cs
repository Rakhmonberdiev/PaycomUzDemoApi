using PaycomUz.Abstractions;
using PaycomUz.Models.Requests;
using PaycomUz.Models.Responses;

namespace PaycomUzDemoApi.PaycomHandlers
{
    public class CheckTransactionHandler : ICheckTransactionHandler
    {
        public Task<PaycomResponse> HandleAsync(PaycomRequest request)
        {
            throw new NotImplementedException();
        }
    }
}

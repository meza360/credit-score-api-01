using Functions.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Functions.Querying
{
    public class BancoUnionFunction : BaseFunction
    {
        private readonly Services.Querying.BancoUnion.Customer _customerService;
        private const string basePath = "v1/querying/banco-union";
        public BancoUnionFunction(Services.Querying.BancoUnion.Customer service)
        {
            _customerService = service;
        }


        [Function(nameof(GetAllBancoUnionCustomers))]
        public async Task<HttpResponseData> GetAllBancoUnionCustomers(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/customers")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<List<Domain.Relational.BancoUnion.Customer>>(await _customerService.GetCustomers(req), req, context);
        }
    }

}
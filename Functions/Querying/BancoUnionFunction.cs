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

        [Function(nameof(GetAllBancoUnionCustomersWithLoans))]
        public async Task<HttpResponseData> GetAllBancoUnionCustomersWithLoans(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/customers/credit-type/loans")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<List<Domain.Relational.BancoUnion.Customer>>(await _customerService.GetCustomersWithLoans(req), req, context);
        }

        [Function(nameof(GetAllBancoUnionCustomersWithAllProducts))]
        public async Task<HttpResponseData> GetAllBancoUnionCustomersWithAllProducts(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/customers/credit-type/all")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<List<Domain.Relational.BancoUnion.Customer>>(await _customerService.GetCustomersWithAllProducts(req), req, context);
        }
    }

}
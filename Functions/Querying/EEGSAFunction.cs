using Functions.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Functions.Querying
{
    public class EEGSAFunction : BaseFunction
    {
        private readonly Services.Querying.EEGSA.Customer _customerService;
        private const string basePath = "v1/querying/eegsa";
        public EEGSAFunction(Services.Querying.EEGSA.Customer service)
        {
            _customerService = service;
        }

        [Function(nameof(ListAllCustomers))]
        public async Task<HttpResponseData> ListAllCustomers(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/customers")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<List<Domain.Relational.EEGSA.Customer>>(await _customerService.ListAll(req), req, context);
        }

        [Function(nameof(ListAllCustomersWithContractsL1))]
        public async Task<HttpResponseData> ListAllCustomersWithContractsL1(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/customers/contracts")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<List<Domain.Relational.EEGSA.Customer>>(await _customerService.ListAllWithContractsL1(req), req, context);
        }
        [Function(nameof(ListAllCustomersWithContractsL2))]
        public async Task<HttpResponseData> ListAllCustomersWithContractsL2(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/customers/contracts/bills")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<List<Domain.Relational.EEGSA.Customer>>(await _customerService.ListAllWithContractsL2(req), req, context);
        }


    }
}
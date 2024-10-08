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
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/customer")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<List<Domain.Relational.EEGSA.Customer>>(await _customerService.ListAll(req), req, context);
        }

        [Function(nameof(ListAllCustomersWithContractsL1))]
        public async Task<HttpResponseData> ListAllCustomersWithContractsL1(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/customer/contracts")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<List<Domain.Relational.EEGSA.Customer>>(await _customerService.ListAllWithContractsL1(req), req, context);
        }
        [Function(nameof(ListAllCustomersWithContractsL2))]
        public async Task<HttpResponseData> ListAllCustomersWithContractsL2(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/customer/contracts/bills")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<List<Domain.Relational.EEGSA.Customer>>(await _customerService.ListAllWithContractsL2(req), req, context);
        }

        [Function(nameof(ListAllCustomersWithContractsReport))]
        public async Task<HttpResponseData> ListAllCustomersWithContractsReport(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/customer/credit-score")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<List<Domain.NoSQL.Private.EEGSA.EEGSSACustomer>>(await _customerService.ListAllCustomersReport(req), req, context);
        }

        [Function(nameof(GetCustomesWithContractsReport))]
        public async Task<HttpResponseData> GetCustomesWithContractsReport(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/customer/credit-score/report")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<Domain.NoSQL.Private.EEGSA.EEGSSACustomer>(await _customerService.GetCustomersReportByCui(req), req, context);
        }
    }
}
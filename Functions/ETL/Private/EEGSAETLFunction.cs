using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Functions.ETL.Private
{
    public class EEGSAETLFunction : Core.BaseFunction
    {
        private readonly Services.ETL.Private.EEGSACustomerETL _customerService;
        private const string basePath = "v1/etl/private/eegsa";
        public EEGSAETLFunction(Services.ETL.Private.EEGSACustomerETL service)
        {
            this._customerService = service;
        }

        [Function(nameof(TransformToMonthlyBillsAsRecord))]
        public async Task<HttpResponseData> TransformToMonthlyBillsAsRecord(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/customers")] HttpRequestData req, FunctionContext context
        )
        {
            return await HandleResult<List<Domain.NoSQL.Private.EEGSA.EEGSSACustomer>?>(await _customerService.TransformToMonthlyBills(req), req, context);
        }

    }
}
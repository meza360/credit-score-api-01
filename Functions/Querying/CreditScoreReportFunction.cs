using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Functions.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Functions.Querying
{
    public class CreditScoreReportFunction : BaseFunction
    {
        private readonly Services.Querying.Reports.EntityReportService _entityService;
        private const string basePath = "v1/querying/reports/credit-score";
        public CreditScoreReportFunction(Services.Querying.Reports.EntityReportService service)
        {
            _entityService = service;
        }

        [Function(nameof(GetCreditScoreReports))]
        public async Task<HttpResponseData> GetCreditScoreReports(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<List<Domain.NoSQL.Report.EntityReport>>(await _entityService.GetCreditScoreReports(req), req, context);
        }

        [Function(nameof(GetCreditScoreReport))]
        public async Task<HttpResponseData> GetCreditScoreReport(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/entity")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<Domain.NoSQL.Report.EntityReport>(await _entityService.GetCreditScoreReport(req), req, context);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Services.ETL.Report;

namespace Functions.ETL.Report
{
    public class ConsolidateReportETLFunction : Core.BaseFunction
    {
        private const string _basePath = "v1/etl/report/consolidate";
        private readonly ConsolidateService _consolidateService;

        public ConsolidateReportETLFunction(ConsolidateService consolidateService)
        {
            this._consolidateService = consolidateService;
        }

        [Function(nameof(ConsolidateAllCitizens))]
        public async Task<HttpResponseData> ConsolidateAllCitizens(
                   [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{_basePath}/citizen")] HttpRequestData req, FunctionContext context)
        {
            return await HandleResult<List<Domain.NoSQL.Report.EntityReport>?>(await _consolidateService.Consolidate(req), req, context);
        }
    }
}
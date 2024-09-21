using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Functions.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Functions.ETL.SAT
{
    public class SatETLFunction : BaseFunction
    {
        private readonly Services.ETL.Sat.ContributorETL _constributorService;
        private const string basePath = "v1/etl/sat";
        public SatETLFunction(Services.ETL.Sat.ContributorETL service)
        {
            this._constributorService = service;
        }

        [Function(nameof(TransformToMonthlyImpositions))]
        public async Task<HttpResponseData> TransformToMonthlyImpositions(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/contributor/impositions")] HttpRequestData req, FunctionContext context
        )
        {
            return await HandleResult<List<Domain.NoSQL.SAT.Contributor>?>(await _constributorService.TransformToMonthlyImpositions(), req, context);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Functions.ETL.Bank
{
    public class BancoUnionETLFunction : Core.BaseFunction
    {
        private readonly Services.ETL.Bank.BancoUnion _bankEtlService;
        private const string basePath = "v1/etl/bank/banco-union";
        public BancoUnionETLFunction(Services.ETL.Bank.BancoUnion service)
        {
            _bankEtlService = service;
        }

        [Function(nameof(TransformCustomersToETLScoring))]
        public async Task<HttpResponseData> TransformCustomersToETLScoring(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{basePath}/customers/transform")]
                HttpRequestData req,
                FunctionContext context)
        {
            return await HandleResult<List<Domain.NoSQL.Bank.BancoUnionCustomerETL>?>
            (await _bankEtlService.TransformCreditsToMonthlyDeclarations(req), req, context);
        }
    }
}
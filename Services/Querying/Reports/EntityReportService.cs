using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Domain.NoSQL.Report;
using Infra;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Persistence.NoSQL;
using Services.Core;

namespace Services.Querying.Reports
{
    public class EntityReportService
    {
        private readonly ILogger<EntityReportService> _logger;
        private readonly CosmosContext _cosmosContext;
        private readonly IMongoDatabase _database;
        private readonly string _databaseName;

        public EntityReportService(Persistence.NoSQL.CosmosContext cosmosContext, ILogger<EntityReportService> logger)
        {
            this._cosmosContext = cosmosContext;
            this._logger = logger;
            this._databaseName = CosmosEnv.COSMOS_REPORT_DB;
            var camelCaseConvention = new ConventionPack
            {
                new CamelCaseElementNameConvention()
            };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
            this._database = _cosmosContext.getCosmosClient().GetDatabase(_databaseName);
        }

        public async Task<Result<EntityReport?>> GetCreditScoreReport(HttpRequestData req)
        {
            string? searchMethod = "";
            string? searchValue = "";
            NameValueCollection query = req.Query;
            foreach (string? key in query.AllKeys)
            {
                string[]? values = query.GetValues(key);
                if (values?.Length > 0)
                {
                    foreach (string value in values)
                    {
                        System.Console.WriteLine($"{key}: {value}");
                        if (key == "cui")
                        {
                            searchMethod = key;
                            searchValue = value;
                            MongoDB.Driver.IAsyncCursor<EntityReport> entity = await _database.GetCollection<EntityReport>(CosmosEnv.COSMOS_REPORT_DB_COL)
                                                                                                .FindAsync(x => x.Cui == value);
                            if (entity != null || entity.Any())
                            {
                                return Result<EntityReport?>.Success(entity.FirstOrDefault());
                            }
                        }
                    }
                }
            }
            return Result<EntityReport?>.Failure("Entity not found");
        }

        public async Task<Result<List<EntityReport>?>> GetCreditScoreReports(HttpRequestData req)
        {
            MongoDB.Driver.IAsyncCursor<EntityReport> entities = await _database.GetCollection<EntityReport>(CosmosEnv.COSMOS_REPORT_DB_COL)
                                                                                                .FindAsync(_ => true);
            if (entities != null || entities.Any())
            {
                return Result<List<EntityReport>?>.Success(await entities.ToListAsync());
            }
            return Result<List<EntityReport>?>.Failure("Entities not found");
        }
    }
}
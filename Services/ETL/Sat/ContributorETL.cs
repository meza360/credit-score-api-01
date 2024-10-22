using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Relational.SAT;
using Infra;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Persistence.NoSQL;
using Services.Core;
using Services.Querying.Sat;

namespace Services.ETL.Sat
{
    public class ContributorETL
    {
        private readonly Services.Querying.Sat.Contributor _contributorService;
        private readonly ILogger<ContributorETL> _logger;
        private readonly CosmosContext _cosmosContext;
        private readonly IMongoDatabase _database;
        private readonly string _databaseName;

        public ContributorETL(Services.Querying.Sat.Contributor contributorService, ILogger<ContributorETL> logger, CosmosContext cosmosContext)
        {
            this._contributorService = contributorService;
            this._logger = logger;
            this._cosmosContext = cosmosContext;
            this._databaseName = CosmosEnv.COSMOS_SAT_DB;
            var camelCaseConvention = new ConventionPack
            {
                new CamelCaseElementNameConvention()
            };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
            this._database = _cosmosContext.getCosmosClient().GetDatabase(_databaseName);
        }

        public async Task<Result<List<Domain.NoSQL.SAT.Contributor>?>> TransformToMonthlyImpositions()
        {
            List<Domain.NoSQL.SAT.Contributor>? contributorsTransformed = new List<Domain.NoSQL.SAT.Contributor>();
            _contributorService.ListAllContributorsWithImpositionAndPayments()
            .Result?
            .Value?
            .ForEach(c =>
            {
                //_logger.LogInformation($"ContributorId: {c.Cui}");
                // for each contributor resets list
                List<Domain.NoSQL.SAT.StatementETL> contributorStatements = new List<Domain.NoSQL.SAT.StatementETL>();
                List<Domain.NoSQL.SAT.ImpositionETL> contributorImpositions = new List<Domain.NoSQL.SAT.ImpositionETL>();
                // with all contributor statements, transforms data into a list of StatementETL
                c.Statements
                    //.DistinctBy(i => i.ContributorId)
                    .Where(i => i.ContributorId != 0)
                    .OrderBy(i => new DateTime(i.StatementYear, i.StatementMonth, 1))
                    .ToList()
                    .ForEach((s) =>
                    {
                        //logger.LodDebug($"ContributorId: {s.StatementId}");
                        contributorStatements.Add(
                                new Domain.NoSQL.SAT.StatementETL
                                {
                                    Year = s.StatementYear,
                                    Month = s.StatementMonth,
                                    WasDue = s.StatementOverdue,
                                    DaysOverdue = 1,
                                    StatementAmount = s.StatementAmount
                                }
                            );
                    });



                c.Impositions
                .OrderBy(i => i.PaymentDate)
                .ToList()
                .ForEach((imposition) =>
                {
                    contributorImpositions.Add(
                        new Domain.NoSQL.SAT.ImpositionETL
                        {
                            PaymentDate = imposition.PaymentDate,
                            PaymentAmount = imposition.PaymentAmount,
                            ContributorId = c.Id,
                            Year = imposition.PaymentDate.Year,
                            Month = imposition.PaymentDate.Month
                        }
                    );
                });


                double accumulatedDebt = 0;//c.Impositions.Sum(i => i.PaymentAmount);
                c.Statements.ForEach(s =>
                {
                    if (s.StatementOverdue)
                    {
                        //_logger.LogDebug($"StatementId: {s.StatementId} is Overdue");
                        //_logger.LogDebug($"StatementAmount: {s.StatementAmount}");
                        //_logger.LogDebug($"PaymentAmount: {s.Payment?.PaymentAmount ?? 0}");
                        accumulatedDebt += s.StatementAmount - (s.Payment?.PaymentAmount ?? 0);
                    }
                });

                //_logger.LogDebug($"AccumulatedDebt: {accumulatedDebt} for ContributorId: {c.Nit}");

                contributorsTransformed.Add(
                        new Domain.NoSQL.SAT.Contributor
                        {
                            Cui = c.Cui,
                            Nit = c.Nit,
                            FullName = $"{c.FirstName} {c.LastName}",
                            LastUpdate = DateTime.Now,
                            StatementHistoricalRecord = contributorStatements,
                            ImpositionHistoricalRecord = contributorImpositions,
                            AccumulatedDebt = accumulatedDebt
                        }
                    );
            });
            // logger.LogInformation("ImpositionETLs:");
            //impositionETLs.ForEach(i => logger.LogInformation($"ContributorId: {i.ContributorId}, Year: {i.Year}, Month: {i.Month}, PaymentAmount: {i.PaymentAmount}"));
            bool isFinished = await SaveETLImpositions(contributorsTransformed);
            if (!isFinished)
            {
                return Result<List<Domain.NoSQL.SAT.Contributor>?>.Failure("Error saving contributors etl");
            }
            return Result<List<Domain.NoSQL.SAT.Contributor>?>.Success(contributorsTransformed?.Count > 0 ? contributorsTransformed : null);
        }

        public async Task<bool> SaveETLImpositions(List<Domain.NoSQL.SAT.Contributor> impositions)
        {
            // saves contributor transformed data into CosmosDB
            await _database.GetCollection<Domain.NoSQL.SAT.Contributor>(CosmosEnv.COSMOS_SAT_IMPOSITION_COL)
            .InsertManyAsync(impositions);
            return true;
        }
    }
}
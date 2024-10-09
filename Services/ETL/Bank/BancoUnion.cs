using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infra;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Persistence.NoSQL;
using Services.Core;

namespace Services.ETL.Bank
{
    public class BancoUnion
    {
        private readonly Services.Querying.BancoUnion.Customer _customerQueryingService;
        private readonly string _databaseName;
        private readonly IMongoDatabase _database;
        private ILogger<Services.ETL.Bank.BancoUnion> _logger;
        private readonly CosmosContext _cosmosContext;
        public BancoUnion(ILogger<Services.ETL.Bank.BancoUnion> logger, CosmosContext context, Services.Querying.BancoUnion.Customer customerQueryingService)
        {
            this._cosmosContext = context;
            this._logger = logger;
            this._customerQueryingService = customerQueryingService;
            this._databaseName = CosmosEnv.COSMOS_BANCO_UNION_DB;
            var camelCaseConvention = new ConventionPack
            {
                new CamelCaseElementNameConvention()
            };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
            this._database = _cosmosContext.getCosmosClient().GetDatabase(_databaseName);
        }

        public async Task<Result<List<Domain.NoSQL.Bank.BancoUnionCustomerETL>?>> TransformCreditsToMonthlyDeclarations(HttpRequestData req)
        {
            Result<List<Domain.Relational.BancoUnion.Customer>?> customersNotTransformed = await _customerQueryingService.GetCustomersWithAllProducts(req);
            List<Domain.NoSQL.Bank.BancoUnionCustomerETL> customersTransformed = new List<Domain.NoSQL.Bank.BancoUnionCustomerETL>();
            if (customersNotTransformed == null || customersNotTransformed?.Value?.Count == 0)
            {
                return Result<List<Domain.NoSQL.Bank.BancoUnionCustomerETL>?>
                                                                        .Failure("No customers found");
            }
            customersNotTransformed?
            .Value?
            .ForEach(customer =>
            {
                List<Domain.NoSQL.Bank.CreditHistoricalRecordETL> creditRecord = new List<Domain.NoSQL.Bank.CreditHistoricalRecordETL>();
                List<Domain.NoSQL.Bank.LoanHistoricalRecordETL> loanRecord = new List<Domain.NoSQL.Bank.LoanHistoricalRecordETL>();
                double calculatedAccumulatedDebt = 0;
                customer.Credits?
                    .ForEach(credit =>
                    {
                        credit.Statements?
                             .ForEach(statement =>
                             {
                                 if (statement.StatementOverdue != null && statement.StatementOverdue == true)
                                 {
                                     calculatedAccumulatedDebt += statement.StatementAmount;
                                 }
                                 creditRecord.Add(new Domain.NoSQL.Bank.CreditHistoricalRecordETL
                                 {
                                     Id = Guid.NewGuid().ToString(),
                                     Month = statement.StatementMonth,
                                     Year = statement.StatementYear,
                                     WasDue = statement.StatementOverdue ?? false,
                                     DaysDue = statement.DaysOverdue ?? 0,
                                     Amount = statement.StatementAmount
                                 });
                             });
                    });

                customer.Loans?
                    .ForEach(loan =>
                    {
                        loan.Installments?
                             .ForEach(installment =>
                             {
                                 if (installment.InstallmentOverdue != null && installment.InstallmentOverdue == true)
                                 {
                                     calculatedAccumulatedDebt += installment.Amount;
                                 }
                                 loanRecord.Add(new Domain.NoSQL.Bank.LoanHistoricalRecordETL
                                 {
                                     Id = Guid.NewGuid().ToString(),
                                     Month = installment.DueDate.Month,
                                     Year = installment.DueDate.Year,
                                     WasDue = installment.InstallmentOverdue ?? false,
                                     DaysDue = installment.DaysOverdue ?? 0,
                                     Amount = installment.Amount
                                 });
                             });
                    });

                _logger.LogInformation($"Customer {customer.FirstName} has accumulated debt of {calculatedAccumulatedDebt}");
                customersTransformed.Add(
                    new Domain.NoSQL.Bank.BancoUnionCustomerETL
                    {
                        Cui = customer.Cui,
                        FullName = $"{customer.FirstName} {customer.LastName}",
                        LastUpdate = DateTime.Now,
                        BankScore = "0",
                        LoanHistoricalRecord = loanRecord,
                        CreditHistoricalRecord = creditRecord,
                        AccumulatedDebt = calculatedAccumulatedDebt
                    });
            });

            bool isFinished = await SaveCustomersETL(customersTransformed);

            if (!isFinished)
            {
                return Result<List<Domain.NoSQL.Bank.BancoUnionCustomerETL>?>
                                                                        .Failure("Error saving customers ETL");
            }
            return Result<List<Domain.NoSQL.Bank.BancoUnionCustomerETL>?>
                                                                        .Success(customersTransformed);
        }

        public async Task<bool> SaveCustomersETL(List<Domain.NoSQL.Bank.BancoUnionCustomerETL> customersToSave)
        {
            await _database
                        .GetCollection<Domain.NoSQL.Bank.BancoUnionCustomerETL>(CosmosEnv.COSMOS_BANCO_UNION_CX_COL)
                        .InsertManyAsync(customersToSave);
            return true;
        }
    }
}
using System.Collections.Specialized;
using Infra;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Persistence.NoSQL;
using Persistence.Relational;
using Services.Core;

namespace Services.Querying.BancoUnion
{
    public class Customer
    {
        private readonly BancoUnionContext _context;
        private readonly CosmosContext _cosmosContext;
        private readonly IMongoDatabase _database;
        private readonly string? _databaseName;

        public Customer(BancoUnionContext context, CosmosContext cosmosContext)
        {
            this._cosmosContext = cosmosContext;
            _context = context;
            this._databaseName = CosmosEnv.COSMOS_BANCO_UNION_DB;
            var camelCaseConvention = new ConventionPack
            {
                new CamelCaseElementNameConvention()
            };
            ConventionRegistry.Register("CamelCase", camelCaseConvention, type => true);
            this._database = _cosmosContext.getCosmosClient().GetDatabase(_databaseName);
        }

        public async Task<Result<List<Domain.Relational.BancoUnion.Customer>?>> GetCustomers(HttpRequestData req)
        {
            return Result<List<Domain.Relational.BancoUnion.Customer>>.Success(await _context.Customers.ToListAsync());
        }
        public async Task<Result<Domain.Relational.BancoUnion.Customer?>> GetCustomerByCUI(HttpRequestData req, string cui)
        {
            return Result<Domain.Relational.BancoUnion.Customer?>
            .Success(await _context.Customers.FirstOrDefaultAsync(c => c.Cui == cui));
        }

        public async Task<Result<List<Domain.Relational.BancoUnion.Customer>?>> GetCustomersWithLoans(HttpRequestData req)
        {
            List<Domain.Relational.BancoUnion.Customer> customersWithOuthPayments = await _context.Customers
                                                                                                    .Include(c => c.Loans)
                                                                                                    .ToListAsync();
            customersWithOuthPayments
            .ForEach(c =>
                {
                    c.Loans?
                         .ToList()
                         .ForEach(loan =>
                         {
                             List<Domain.Relational.BancoUnion.Installment> installmentsFromLoan = _context.Installments
                                                                                                            .Where(p => p.LoanId == loan.Id)
                                                                                                            .ToList();
                             loan.Installments = installmentsFromLoan;
                             /* installmentsFromLoan.ForEach(installment =>
                             {
                                 installment.Payments = _context.Payments.Where(p => p.InstallmentId == installment.Id).ToList();
                             }); */
                         });
                });

            return Result<List<Domain.Relational.BancoUnion.Customer>>
                .Success(customersWithOuthPayments);
        }

        public async Task<Result<List<Domain.Relational.BancoUnion.Customer>?>> GetCustomersWithAllProducts(HttpRequestData req)
        {
            List<Domain.Relational.BancoUnion.Customer> customersWithOuthPayments = await _context.Customers
                                                                                                    .Include(c => c.Loans)
                                                                                                    .Include(c => c.Credits)
                                                                                                    .ToListAsync();
            customersWithOuthPayments
            .ForEach(c =>
                {
                    //retrieve Loans and Installments
                    c.Loans?
                         .ToList()
                         .ForEach(loan =>
                         {
                             List<Domain.Relational.BancoUnion.Installment> installmentsFromLoan = _context.Installments
                                                                                                            .Where(p => p.LoanId == loan.Id)
                                                                                                            .ToList();
                             loan.Installments = installmentsFromLoan;
                             /* installmentsFromLoan.ForEach(installment =>
                             {
                                 installment.Payments = _context.Payments.Where(p => p.InstallmentId == installment.Id).ToList();
                             }); */
                         });

                    //retrieve Credits and Statements
                    c.Credits?
                    .ToList()
                    .ForEach(credit =>
                    {
                        List<Domain.Relational.BancoUnion.Statement> statementsFromCredit = _context.Statements
                                                                                                    .Where(p => p.CreditId == credit.Id)
                                                                                                    .ToList();
                        credit.Statements = statementsFromCredit;
                        /* installmentsFromCredit.ForEach(installment =>
                        {
                            installment.Payments = _context.Payments.Where(p => p.InstallmentId == installment.Id).ToList();
                        }); */
                    });
                });

            return Result<List<Domain.Relational.BancoUnion.Customer>?>
                                                            .Success(customersWithOuthPayments);
        }

        public async Task<Result<Domain.NoSQL.Bank.BancoUnionCustomerETL>?> GetCustomerReportById(HttpRequestData req)
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
                        if (key == "nit")
                        {
                            searchValue = query?.Get("nit");
                            searchMethod = "nit";
                            return Result<Domain.NoSQL.Bank.BancoUnionCustomerETL>
                            .Success(
                                await _database.GetCollection<Domain.NoSQL.Bank.BancoUnionCustomerETL>(CosmosEnv.COSMOS_BANCO_UNION_CX_COL)
                                .FindAsync(c => c.Cui == searchValue).Result.FirstOrDefaultAsync()
                            );
                        }
                        if (key == "cui")
                        {
                            searchValue = query?.Get("cui");
                            searchMethod = "cui";
                            return Result<Domain.NoSQL.Bank.BancoUnionCustomerETL>
                           .Success(
                               await _database.GetCollection<Domain.NoSQL.Bank.BancoUnionCustomerETL>(CosmosEnv.COSMOS_BANCO_UNION_CX_COL)
                               .FindAsync(c => c.Cui == searchValue).Result.FirstOrDefaultAsync()
                            );
                        }
                    }
                }
            }

            return Result<Domain.NoSQL.Bank.BancoUnionCustomerETL>
                .Failure("No search method provided");
        }
        public async Task<Result<Domain.NoSQL.Bank.BancoUnionCustomerETL>?> GetCustomerReportById(string searchMethod, string searchValue)
        {

            if (searchMethod == "nit")
            {
                return Result<Domain.NoSQL.Bank.BancoUnionCustomerETL>
                .Success(
                    await _database.GetCollection<Domain.NoSQL.Bank.BancoUnionCustomerETL>(CosmosEnv.COSMOS_BANCO_UNION_CX_COL)
                    .FindAsync(c => c.Cui == searchValue).Result.FirstOrDefaultAsync()
                );
            }
            if (searchMethod == "cui")
            {
                return Result<Domain.NoSQL.Bank.BancoUnionCustomerETL>
               .Success(
                   await _database.GetCollection<Domain.NoSQL.Bank.BancoUnionCustomerETL>(CosmosEnv.COSMOS_BANCO_UNION_CX_COL)
                   .FindAsync(c => c.Cui == searchValue).Result.FirstOrDefaultAsync()
                );
            }

            return Result<Domain.NoSQL.Bank.BancoUnionCustomerETL>
                .Failure("No search method provided");
        }
    }
}
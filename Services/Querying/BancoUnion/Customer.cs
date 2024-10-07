using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Persistence.Relational;
using Services.Core;

namespace Services.Querying.BancoUnion
{
    public class Customer
    {
        private readonly BancoUnionContext _context;

        public Customer(BancoUnionContext context)
        {
            _context = context;
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
    }
}
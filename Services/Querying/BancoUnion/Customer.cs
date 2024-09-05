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

        public async Task<Result<List<Domain.Relational.BancoUnion.Customer>>> GetCustomers(HttpRequestData req)
        {
            return Result<List<Domain.Relational.BancoUnion.Customer>>.Success(await _context.Customers.ToListAsync());
        }
        public async Task<Result<Domain.Relational.BancoUnion.Customer>> GetCustomerByCUI(HttpRequestData req, string cui)
        {
            return Result<Domain.Relational.BancoUnion.Customer>.Success(await _context.Customers.FirstOrDefaultAsync(c => c.CUI == cui));
        }
    }
}
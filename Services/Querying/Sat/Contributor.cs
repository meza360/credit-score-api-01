using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Services.Core;

namespace Services.Querying.Sat
{
    public class Contributor
    {
        private Persistence.Relational.SatContext _context;
        public Contributor(Persistence.Relational.SatContext repository)
        {
            _context = repository;
        }

        public async Task<Result<List<Domain.Relational.SAT.Contributor>>> ListAll(HttpRequestData req)
        {
            return Result<List<Domain.Relational.SAT.Contributor>>.Success(await _context.Contributors.ToListAsync());
        }

        public async Task<Result<List<Domain.Relational.SAT.Contributor>>> ListAllContributorsWithImposition(HttpRequestData req)
        {
            return Result<List<Domain.Relational.SAT.Contributor>>
            .Success(
                await _context.Contributors?
                .Include(c => c.Impositions)
                .ToListAsync()
                );
        }
    }
}
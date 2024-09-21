using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services.Core;

namespace Services.Querying.Sat
{
    public class Contributor
    {
        private Persistence.Relational.SatContext _context;
        private readonly ILogger<Contributor> logger;

        public Contributor(Persistence.Relational.SatContext repository, ILogger<Services.Querying.Sat.Contributor> logger)
        {
            _context = repository;
            this.logger = logger;
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

        public async Task<Result<List<Domain.Relational.SAT.Contributor>>> ListAllContributorsWithImposition()
        {
            return Result<List<Domain.Relational.SAT.Contributor>>
            .Success(
                await _context.Contributors?
                .Include(c => c.Statements).Include(c => c.Impositions)
                .ToListAsync()
                );
        }
    }
}
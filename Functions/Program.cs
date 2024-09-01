using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Functions.Core;
using Infra.Env;

namespace Functions;

class Program
{
    static async Task Main(string[] args)
    {
        var host = new HostBuilder()
                .ConfigureFunctionsWebApplication((env, workerApp) =>
                {
                    var serializeOptions = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        NullValueHandling = NullValueHandling.Include,
                        Formatting = Formatting.Indented,
                        //TypeNameHandling = TypeNameHandling.Objects,
                        //TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                    };

                    workerApp.Services.Configure<KestrelServerOptions>((options) =>
                    {
                        options.AllowSynchronousIO = true;
                    });
                    workerApp.UseNewtonsoftJson(serializeOptions);
                    workerApp.UseMiddleware<ExceptionMiddleware>();
                })
                .ConfigureServices(services =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.ConfigureFunctionsApplicationInsights();
                    services.AddDbContext<SatContext>(options =>
                        options.UseNpgsql(SatEnv.PGSQL_SAT_DB)
                    );
                    services.AddDbContext<RenapContext>(options =>
                        options.UseNpgsql(RenapEnv.PGSQL_RENAP_DB)
                    );
                    services.AddDbContext<EEGSAContext>(options =>
                        options.UseNpgsql(EEGSAEnv.PGSQL_EEGSA_DB)
                    );
                    services.AddSingleton<Services.Querying.Renap.Citizen>();
                    services.AddSingleton<Services.Querying.Sat.Contributor>();
                    services.AddSingleton<Services.Querying.EEGSA.Customer>();
                    services.AddLogging(s =>
                    {
                        s.AddAzureWebAppDiagnostics();
                        s.AddConsole();
                    });
                })
                .ConfigureOpenApi()
                .Build();
        await host.RunAsync();
    }

}
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
                        ReferenceLoopHandling = ReferenceLoopHandling.Error,
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
                        options.UseNpgsql(Environment.GetEnvironmentVariable("POSTGRES_SAT_CONNECTION_STRING"))
                    );
                    services.AddDbContext<RenapContext>(options =>
                        options.UseNpgsql(Environment.GetEnvironmentVariable("POSTGRES_RENAP_CONNECTION_STRING"))
                    );
                    services.AddSingleton<Services.Querying.Renap.Citizen>();
                    services.AddSingleton<Services.Querying.Sat.Contributor>();
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
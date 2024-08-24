namespace Infra;

public class CosmosEnv
{
    public String CosmosAcc { get; private set; } = Environment.GetEnvironmentVariable("COSMOS_ACC", EnvironmentVariableTarget.Process) ?? "mongodb://oddbbsrv01.gt2software.dev:34019";
}

namespace Infra;

public class CosmosEnv
{
    public String CosmosAcc { get; private set; } = Environment.GetEnvironmentVariable("COSMOS_ACC", EnvironmentVariableTarget.Process) ?? "mongodb://oddbbsrv01.gt2software.dev:34019";
    public static string COSMOS_SAT_DB { get; private set; } = Environment.GetEnvironmentVariable("COSMOS_SAT_DB", EnvironmentVariableTarget.Process) ?? "credit-score-sat-db";
    public static string COSMOS_SAT_IMPOSITION_COL { get; private set; } = Environment.GetEnvironmentVariable("COSMOS_SAT_IMPOSITION_COL", EnvironmentVariableTarget.Process) ?? "impositions";
}

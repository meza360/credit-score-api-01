namespace Infra;

public class CosmosEnv
{
    public String CosmosAcc { get; private set; } = Environment.GetEnvironmentVariable("COSMOS_ACC", EnvironmentVariableTarget.Process) ?? "mongodb://oddbbsrv01.gt2software.dev:34019";
    public static string COSMOS_SAT_DB { get; private set; } = Environment.GetEnvironmentVariable("COSMOS_SAT_DB", EnvironmentVariableTarget.Process) ?? "credit-score-sat-db";
    public static string COSMOS_SAT_IMPOSITION_COL { get; private set; } = Environment.GetEnvironmentVariable("COSMOS_SAT_IMPOSITION_COL", EnvironmentVariableTarget.Process) ?? "contributors";
    public static string COSMOS_EEGSA_DB { get; private set; } = Environment.GetEnvironmentVariable("COSMOS_EEGSA_DB", EnvironmentVariableTarget.Process) ?? "credit-score-eegsa-db";
    public static string COSMOS_EEGSA_CX_COL { get; private set; } = Environment.GetEnvironmentVariable("COSMOS_EEGSA_CX_COL", EnvironmentVariableTarget.Process) ?? "customers";
    public static string COSMOS_BANCO_UNION_DB { get; set; } = Environment.GetEnvironmentVariable("COSMOS_BANCO_UNION_DB", EnvironmentVariableTarget.Process) ?? "credit-score-banco-union-db";
    public static string COSMOS_BANCO_UNION_CX_COL { get; set; } = Environment.GetEnvironmentVariable("COSMOS_BANCO_UNION_CX_COL", EnvironmentVariableTarget.Process) ?? "customers";
}

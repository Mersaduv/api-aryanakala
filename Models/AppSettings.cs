using System.Runtime.Serialization;

namespace ApiAryanakala.Models;

public class AppSettings
{
    public AuthSettings AuthSettings { get; set; } = default!;

    public StripeSettings StripeSettings { get; set; } = default!;

    public AzureCosmosSettings AzureCosmosSettings { get; set; } = default!;

    public DbProvider? DbProvider { get; set; }
}

public class AuthSettings
{
    public string TokenKey { get; set; }
    public int TokenTimeout { get; set; }
    public int RefreshTokenTimeout { get; set; }
}

public class StripeSettings
{
    public string ApiKey { get; set; } = string.Empty;

    public string SigningSecret { get; set; } = string.Empty;
}

public class AzureCosmosSettings
{
    public string AccountEndpoint { get; set; } = string.Empty;

    public string AccountKey { get; set; } = string.Empty;

    public string DatabaseName { get; set; } = string.Empty;
}
public enum DbProvider
{
    [EnumMember(Value = "SQL Server")] SqlServer,
    [EnumMember(Value = "PostgreSql")] PostgreSql,

    [EnumMember(Value = "SQLite")] Sqlite,

    [EnumMember(Value = "In Memory")] InMemory,

    [EnumMember(Value = "Azure Cosmos")] AzureCosmos
}

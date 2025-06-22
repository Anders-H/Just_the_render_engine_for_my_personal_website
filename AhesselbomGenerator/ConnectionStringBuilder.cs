namespace AhesselbomGenerator;

public static class ConnectionStringBuilder
{
    private const string DSource = "Data Source=.";
    private const string DName = "Initial Catalog=WebSiteTweetDatabase";
    private const string Security = "Integrated Security=True";
    private const string Cert = "Trust Server Certificate=True";
    public const string ConnectionString = $"{DSource};{DName};{Security};{Cert}";

}
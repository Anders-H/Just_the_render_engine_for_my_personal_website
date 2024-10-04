namespace AhesselbomGenerator;

public static class ConnectionStringBuilder
{
    private const string dSource = "Data Source=.";
    private const string dName = "Initial Catalog=WebSiteTweetDatabase";
    private const string iSecurity = "Integrated Security=True";
    private const string tCert = "Trust Server Certificate=True";
    public const string ConnectionString = $"{dSource};{dName};{iSecurity};{tCert}";

}
using Microsoft.Data.SqlClient;

namespace AspireApp1.ApiService.Data;

public class SqlConnectionProvider : ISqlConnectionProvider
{
    private readonly string _connStr;
    
    public SqlConnectionProvider(IConfiguration configuration)
    {
        _connStr = configuration[Constants.Configuration.ConnectionString]!;
    }

    public SqlConnection Create()
    {
        return new SqlConnection(_connStr);
    }
}
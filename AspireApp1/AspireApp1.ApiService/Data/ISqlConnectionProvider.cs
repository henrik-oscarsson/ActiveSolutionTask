using Microsoft.Data.SqlClient;

namespace AspireApp1.ApiService.Data;

public interface ISqlConnectionProvider
{
    SqlConnection Create();
}
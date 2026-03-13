namespace ECOP.AccesoDatos.Data.Dapper;

public interface IDapperConnectionFactory
{
    IDbConnection CreateConnection();
}

public class DapperConnectionFactory(IConfiguration configuration) : IDapperConnectionFactory
{
    private readonly string _connectionString =
        configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' no configurado.");

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}

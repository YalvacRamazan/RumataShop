using Microsoft.Data.SqlClient;
using System.Data;

namespace RumataShop.Context
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        // Bu apı, projeye başlarken appsettings.json dosyasını okyuormuş
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            // appsettings.json içindeki "DefaultConnection" isimini arar
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        // Bize lazım olduğunda SQL bağlantısını üreten metod budur
        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}

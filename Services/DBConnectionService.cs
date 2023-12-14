using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace BugTrackerAPI.Services
{
    public class DBConnectionService
    {
        private readonly string _connectionString;

        public DBConnectionService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}

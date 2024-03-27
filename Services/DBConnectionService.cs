using Microsoft.Extensions.Configuration;
using Npgsql;

namespace BugTrackerAPI.Services
{
    public class DBConnectionService
    {
        private readonly string _connectionString;

        public DBConnectionService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public NpgsqlConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}

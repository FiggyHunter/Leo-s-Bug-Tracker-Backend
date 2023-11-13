using BugTrackerAPI.Entities;
using BugTrackerAPI.Models;
using System.Data.SqlClient;

namespace BugTrackerAPI.Services
{
    public interface IUserService
    {
        User LoginUser(UserLogin details);
    }

    public class UserService : IUserService
    {
        private readonly string _connectionString;

        public UserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public User FindByEmail(string email)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = $"SELECT * FROM Users WHERE Email = '{email}'";
                    Console.WriteLine(sql);
                    SqlCommand command = new SqlCommand(sql, connection);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new User
                            {
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Password = reader["Password"].ToString(),
                                Role = reader["Role"].ToString(),
                                Avatar = reader["Avatar"].ToString()
                            };
                            return user;
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                {
                    return null;
                }
            }
        }

        public User LoginUser(UserLogin details)
        {
            Console.WriteLine(details);
            User user = FindByEmail(details.Email);
            Console.WriteLine(user);
            if (user.Password == details.Password)
                return user;
            return null;
        }
    }
}
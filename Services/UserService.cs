using BugTrackerAPI.Entities;
using BugTrackerAPI.Models;
using System.Data.SqlClient;
using System;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BugTrackerAPI.Services
{
    public interface IUserService
    {
        User LoginUser(UserLogin details);
        User RegisterUser(UserRegister details);
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
                Console.WriteLine(ex);
                return null; 
            }
        }

        public User LoginUser(UserLogin details)
        {
            User user = FindByEmail(details.Email);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }
            if (user.Password != details.Password)
            {
                throw new ArgumentException("Incorrect password.");
            }
            return user;
        }

        public User RegisterUser(UserRegister details)
        {
            // INSERT INTO users (user_id, email, password, role, avatar, name)
            //VALUES('{user_id}', '{email}', '{password}', '{role}', '{avatar}', '{name}');

            Guid uuid = Guid.NewGuid();
            Console.WriteLine(details);
            User user = FindByEmail(details.Email);
            if(user == null) {
                try
                {
                    string sql = $"INSERT INTO Users (Id, email, password, role, avatar, name) VALUES ('{uuid}','{details.Email}','{details.Password}', 'default', 'default','default')";
                    Console.WriteLine(sql);
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                        command.ExecuteNonQuery();
                }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return user;
        }

    }
}
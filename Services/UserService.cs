using BugTrackerAPI.Entities;
using BugTrackerAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BugTrackerAPI.Services
{
    public interface IUserService
    {
        User LoginUser(UserLogin details);
        User RegisterUser(UserRegister details);
        User FindByEmail(string email);
        public User UpdateRoleAvatar(UserUpdate details);
        public User UpdateName(UserUpdate details);
        public string GenerateToken(User user);
    }

    public class UserService : IUserService
    {
        private readonly string _connectionString;
        private IConfiguration _configuration;

        public UserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection"); 
            _configuration = configuration;
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
                                Id = Guid.Parse(reader["Id"].ToString()),
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

        public User FindById(Guid id)
        {
            try
            {
                var stringId = id.ToString();
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = $"SELECT * FROM Users WHERE Id = '{stringId}'";
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
            User user = FindByEmail(details.Email);
            if (user != null && user.Email != null)
            {
                throw new ArgumentException("User already exists!");
            }
            if (user == null)
            {
                try
                {
                    Guid uuid = Guid.NewGuid();
                    string sql = $"INSERT INTO Users (Id, email, password, role, avatar, name) VALUES ('{uuid}','{details.Email}','{details.Password}', 'unset', 'unset','unset')";
                    using (SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(sql, connection);
                        command.ExecuteNonQuery();

                    }
                    return new User { Id = uuid, Email = details.Email, Password = details.Password, Role = "unset", Avatar = "unset", Name = "unset" };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw new ArgumentException("Internal Server Error.");
                }
            }
            return null;
        }
        public User UpdateRoleAvatar(UserUpdate details)
        {
            try
            {
                string sql = $"UPDATE Users SET Role = @Role, Avatar = @Avatar WHERE ID = @Id";
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Role", details.Role);
                    command.Parameters.AddWithValue("@Avatar", details.Avatar);
                    command.Parameters.AddWithValue("@Id", details.Id.ToString());
                    command.ExecuteNonQuery();

                }
                Console.WriteLine("done");
                return FindById(details.Id);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new ArgumentException("Internal Server Error.");
            }
        }

        public User UpdateName(UserUpdate details)
        {
            Console.WriteLine($"details:{details} ");
            try
            {
                string sql = $"UPDATE Users SET Name = @Name WHERE ID = @Id";
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Name", details.Name);
                    command.Parameters.AddWithValue("@Id", details.Id.ToString());
                    command.ExecuteNonQuery();

                }
                Console.WriteLine("done");
                return FindById(details.Id);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new ArgumentException("Internal Server Error.");
            }
        }

        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("Name", user.Name),
                new Claim("Email", user.Email),
                new Claim("Role", user.Role),
                new Claim("Avatar", user.Avatar),
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
              _configuration["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(1),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
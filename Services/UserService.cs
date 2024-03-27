using BugTrackerAPI.Entities;
using BugTrackerAPI.Models;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
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
        private readonly DBConnectionService _dbConnectionService;
        private IConfiguration _configuration;

        public UserService(DBConnectionService dbConnectionService, IConfiguration configuration)
        {
            _dbConnectionService = dbConnectionService;
            _configuration = configuration;
        }

        public User FindByEmail(string email)
        {
            try
            {
                using (Npgsql.NpgsqlConnection connection = _dbConnectionService.CreateConnection())
                {
                    connection.Open();
                    string sql = $"SELECT * FROM Users WHERE Email = '{email}'";
                    Console.WriteLine(sql);
                    Npgsql.NpgsqlCommand command = new Npgsql.NpgsqlCommand(sql, connection);

                    using (Npgsql.NpgsqlDataReader reader = command.ExecuteReader())
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
                using (Npgsql.NpgsqlConnection connection = _dbConnectionService.CreateConnection())
                {
                    connection.Open();
                    string sql = $"SELECT * FROM Users WHERE Id = '{stringId}'";
                    Console.WriteLine(sql);
                    Npgsql.NpgsqlCommand command = new Npgsql.NpgsqlCommand(sql, connection);

                    using (Npgsql.NpgsqlDataReader reader = command.ExecuteReader())
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

        public User LoginUser(UserLogin details)
        {
                User user = FindByEmail(details.Email);

                if (user == null)
                {
                    throw new ArgumentException("User not found.");
                }

                if (BCrypt.Net.BCrypt.Verify(details.Password, user.Password) == false)
                {
                    throw new ArgumentException("Incorrect password.");
                }
                else return user;
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
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(details.Password);
                    string sql = $"INSERT INTO Users (Id, email, password, role, avatar, name) VALUES ('{uuid}','{details.Email}','{hashedPassword}', 'unset', 'unset','unset')";
                    using (Npgsql.NpgsqlConnection connection = _dbConnectionService.CreateConnection())
                    {
                        connection.Open();
                        Npgsql.NpgsqlCommand command = new Npgsql.NpgsqlCommand(sql, connection);
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
                Console.WriteLine(details.Id);
                string sql = $"UPDATE Users SET Role = @Role, Avatar = @Avatar WHERE Id = @Id";
                using (Npgsql.NpgsqlConnection connection = _dbConnectionService.CreateConnection())
                {
                    connection.Open();
                    Npgsql.NpgsqlCommand command = new Npgsql.NpgsqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@Role", details.Role);
                    command.Parameters.AddWithValue("@Avatar", details.Avatar);
                    command.Parameters.AddWithValue("@Id", details.Id.ToString());
                    command.ExecuteNonQuery();

                }
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
                using (Npgsql.NpgsqlConnection connection = _dbConnectionService.CreateConnection())
                {
                    connection.Open();
                    Npgsql.NpgsqlCommand command = new Npgsql.NpgsqlCommand(sql, connection);
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
              expires: DateTime.Now.AddMinutes(5),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using InventoryManagement.Core.Enums;

namespace InventoryManagement.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool HasAnyUsers()
        {
            string query = @"
        SELECT CASE
            WHEN EXISTS (SELECT 1 FROM Users)
            THEN 1
            ELSE 0
        END";

            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command =
                       new SqlCommand(query, connection))
                {
                    int result = Convert.ToInt32(
                        command.ExecuteScalar());

                    return result == 1;
                }
            }
        }

        public User GetByEmail(string email)
        {
            string query = @"
        SELECT Id, FirstName, LastName, Email,
               PasswordHash, PasswordSalt, Role,
               IsActive, CreatedAt
        FROM Users
        WHERE Email = @Email";

            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command =
                       new SqlCommand(query, connection))
                {
                    command.Parameters
                        .Add("@Email", SqlDbType.NVarChar, 256)
                        .Value = email;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return null;
                        }

                        // 1. Read the Role from the database
                        string roleText = (string)reader["Role"];

                        // 2. Validate the Role
                        UserRole role;

                        if (!Enum.TryParse(
                                roleText,
                                ignoreCase: true,
                                result: out role)
                            || !Enum.IsDefined(typeof(UserRole), role))
                        {
                            throw new InvalidOperationException(
                                "Invalid role value in database: " + roleText);
                        }

                        // 3. Create the User object
                        User user = new User
                        {
                            Id = (int)reader["Id"],
                            FirstName = (string)reader["FirstName"],
                            LastName = (string)reader["LastName"],
                            Email = (string)reader["Email"],
                            PasswordHash = (byte[])reader["PasswordHash"],
                            PasswordSalt = (byte[])reader["PasswordSalt"],
                            Role = role,
                            IsActive = (bool)reader["IsActive"],
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        };
                        return user;
                    }
                }
            }
        }

        public bool EmailExists(string email)
        {
            string query = @"
        SELECT CASE
            WHEN EXISTS (
                SELECT 1
                FROM Users
                WHERE Email = @Email
            )
            THEN 1
            ELSE 0
        END";

            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command =
                       new SqlCommand(query, connection))
                {
                    command.Parameters
                        .Add("@Email", SqlDbType.NVarChar, 256)
                        .Value = email;

                    int result = Convert.ToInt32(
                        command.ExecuteScalar());

                    return result == 1;
                }
            }
        }

        public int Add(User user)
        {
            string query = @"
        INSERT INTO Users
            (FirstName, LastName, Email, PasswordHash,
             PasswordSalt, Role, IsActive)
        OUTPUT INSERTED.Id
        VALUES
            (@FirstName, @LastName, @Email, @PasswordHash,
             @PasswordSalt, @Role, @IsActive)";

            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command =
                       new SqlCommand(query, connection))
                {
                    command.Parameters.Add(
                        "@FirstName", SqlDbType.NVarChar, 50).Value =
                        user.FirstName;

                    command.Parameters.Add(
                        "@LastName", SqlDbType.NVarChar, 50).Value =
                        user.LastName;

                    command.Parameters.Add(
                        "@Email", SqlDbType.NVarChar, 256).Value =
                        user.Email;

                    command.Parameters.Add(
                        "@PasswordHash", SqlDbType.VarBinary, 64).Value =
                        user.PasswordHash;

                    command.Parameters.Add(
                        "@PasswordSalt", SqlDbType.VarBinary, 32).Value =
                        user.PasswordSalt;

                    command.Parameters.Add(
                        "@Role", SqlDbType.NVarChar, 10).Value =
                        user.Role.ToString();

                    command.Parameters.Add(
                        "@IsActive", SqlDbType.Bit).Value =
                        user.IsActive;

                    int newUserId = Convert.ToInt32(
                        command.ExecuteScalar());

                    return newUserId;
                }
            }
        }


        public User GetById(int id)
        {
            const string query = @"
        SELECT Id, FirstName, LastName, Email,
               PasswordHash, PasswordSalt, Role,
               IsActive, CreatedAt
        FROM Users
        WHERE Id = @Id";

            using (SqlConnection connection =
                   new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command =
                       new SqlCommand(query, connection))
                {
                    command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        string roleText = (string)reader["Role"];

                        UserRole role;

                        if (!Enum.TryParse(
                                roleText,
                                true,
                                out role)
                            || !Enum.IsDefined(typeof(UserRole), role))
                        {
                            throw new InvalidOperationException(
                                "Invalid role value in database: " + roleText);
                        }

                        return new User
                        {
                            Id = (int)reader["Id"],
                            FirstName = (string)reader["FirstName"],
                            LastName = (string)reader["LastName"],
                            Email = (string)reader["Email"],
                            PasswordHash = (byte[])reader["PasswordHash"],
                            PasswordSalt = (byte[])reader["PasswordSalt"],
                            Role = role,
                            IsActive = (bool)reader["IsActive"],
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        };
                    }
                }
            }
        }
    }


}
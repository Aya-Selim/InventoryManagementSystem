
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;

namespace InventoryManagement.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly SqlConnection _connection;
        private readonly Func<SqlTransaction> _getTransaction;

        public CategoryRepository(
            SqlConnection connection,
            Func<SqlTransaction> getTransaction)
        {
            _connection = connection
                ?? throw new ArgumentNullException(nameof(connection));

            _getTransaction = getTransaction
                ?? throw new ArgumentNullException(nameof(getTransaction));
        }

        private void EnsureConnectionOpen()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        public List<Category> GetActiveCategories()
        {
            const string query = @"
                SELECT Id, Name, Description, IsDeleted,
                       CreatedAt, UpdatedAt
                FROM Categories
                WHERE IsDeleted = 0
                ORDER BY Name;";

            var categories = new List<Category>();

            EnsureConnectionOpen();

            using (var command = new SqlCommand(
                query, _connection, _getTransaction()))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    categories.Add(MapCategory(reader));
                }
            }

            return categories;
        }

        public Category GetById(int id)
        {
            const string query = @"
                SELECT Id, Name, Description, IsDeleted,
                       CreatedAt, UpdatedAt
                FROM Categories
                WHERE Id = @Id;";

            EnsureConnectionOpen();

            using (var command = new SqlCommand(
                query, _connection, _getTransaction()))
            {
                command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    return MapCategory(reader);
                }
            }
        }

        public bool ExistsByName(string name)
        {
            const string query = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1
                    FROM Categories
                    WHERE Name = @Name
                      AND IsDeleted = 0
                ) THEN 1 ELSE 0 END;";

            EnsureConnectionOpen();

            using (var command = new SqlCommand(
                query, _connection, _getTransaction()))
            {
                command.Parameters
                    .Add("@Name", SqlDbType.NVarChar, 100).Value = name;

                return Convert.ToInt32(command.ExecuteScalar()) == 1;
            }
        }

        public bool ExistsByName(string name, int excludeCategoryId)
        {
            const string query = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1
                    FROM Categories
                    WHERE Name = @Name
                      AND Id <> @ExcludeCategoryId
                      AND IsDeleted = 0
                ) THEN 1 ELSE 0 END;";

            EnsureConnectionOpen();

            using (var command = new SqlCommand(
                query, _connection, _getTransaction()))
            {
                command.Parameters
                    .Add("@Name", SqlDbType.NVarChar, 100).Value = name;

                command.Parameters
                    .Add("@ExcludeCategoryId", SqlDbType.Int)
                    .Value = excludeCategoryId;

                return Convert.ToInt32(command.ExecuteScalar()) == 1;
            }
        }

        public int Add(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            const string query = @"
                INSERT INTO Categories (Name, Description)
                OUTPUT INSERTED.Id
                VALUES (@Name, @Description);";

            EnsureConnectionOpen();

            using (var command = new SqlCommand(
                query, _connection, _getTransaction()))
            {
                AddCategoryParameters(command, category);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public bool Update(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            const string query = @"
                UPDATE Categories
                SET Name = @Name,
                    Description = @Description,
                    UpdatedAt = SYSDATETIME()
                WHERE Id = @Id
                  AND IsDeleted = 0;";

            EnsureConnectionOpen();

            using (var command = new SqlCommand(
                query, _connection, _getTransaction()))
            {
                AddCategoryParameters(command, category);

                command.Parameters.Add("@Id", SqlDbType.Int)
                    .Value = category.Id;

                return command.ExecuteNonQuery() == 1;
            }
        }

        public bool SoftDelete(int id)
        {
            const string query = @"
                UPDATE Categories
                SET IsDeleted = 1,
                    UpdatedAt = SYSDATETIME()
                WHERE Id = @Id
                  AND IsDeleted = 0;";

            EnsureConnectionOpen();

            using (var command = new SqlCommand(
                query, _connection, _getTransaction()))
            {
                command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                return command.ExecuteNonQuery() == 1;
            }
        }

        private static void AddCategoryParameters(
            SqlCommand command, Category category)
        {
            command.Parameters
                .Add("@Name", SqlDbType.NVarChar, 100)
                .Value = category.Name;

            command.Parameters
                .Add("@Description", SqlDbType.NVarChar, 500)
                .Value = (object)category.Description ?? DBNull.Value;
        }

        private static Category MapCategory(SqlDataReader reader)
        {
            return new Category
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Description = reader["Description"] == DBNull.Value
                    ? null
                    : (string)reader["Description"],
                IsDeleted = (bool)reader["IsDeleted"],
                CreatedAt = (DateTime)reader["CreatedAt"],
                UpdatedAt = reader["UpdatedAt"] == DBNull.Value
                    ? (DateTime?)null
                    : (DateTime)reader["UpdatedAt"]
            };
        }
    }
}

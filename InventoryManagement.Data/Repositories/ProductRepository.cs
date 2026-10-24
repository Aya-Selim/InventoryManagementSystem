
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;

namespace InventoryManagement.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
       
        private const int LowStockThreshold = 5;

        private readonly SqlConnection _connection;
        private readonly Func<SqlTransaction> _getTransaction;

        public ProductRepository(
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
                _connection.Open();
        }

        public List<Product> GetActiveProducts()
        {
            const string query = @"
                SELECT Id, Name, Description, Quantity, Price,
                       SupplierName, CategoryId, IsDeleted,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Products
                WHERE IsDeleted = 0
                ORDER BY Name;";

            var products = new List<Product>();

            EnsureConnectionOpen();

            using (var command = new SqlCommand(
                query, _connection, _getTransaction()))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                    products.Add(MapProduct(reader));
            }

            return products;
        }

        public Product GetById(int id)
        {
            const string query = @"
                SELECT Id, Name, Description, Quantity, Price,
                       SupplierName, CategoryId, IsDeleted,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Products
                WHERE Id = @Id
                  AND IsDeleted = 0;";

            EnsureConnectionOpen();

            using (var command = new SqlCommand(
                query, _connection, _getTransaction()))
            {
                command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                        return null;

                    return MapProduct(reader);
                }
            }
        }

        public List<Product> Search(
            string name,
            int? categoryId,
            bool? lowStockOnly)
        {
            const string query = @"
                SELECT Id, Name, Description, Quantity, Price,
                       SupplierName, CategoryId, IsDeleted,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Products
                WHERE IsDeleted = 0
                  AND (@Name IS NULL OR Name LIKE @NamePattern)
                  AND (@CategoryId IS NULL OR CategoryId = @CategoryId)
                  AND (
                        @LowStockOnly IS NULL
                        OR (@LowStockOnly = 1
                            AND Quantity <= @Threshold)
                        OR (@LowStockOnly = 0
                            AND Quantity > @Threshold)
                      )
                ORDER BY Name;";

            var products = new List<Product>();

            EnsureConnectionOpen();

            using (var command = new SqlCommand(
                query, _connection, _getTransaction()))
            {
                string normalizedName = string.IsNullOrWhiteSpace(name)
                    ? null
                    : name.Trim();

                command.Parameters
                    .Add("@Name", SqlDbType.NVarChar, 150)
                    .Value = (object)normalizedName ?? DBNull.Value;

                command.Parameters
                    .Add("@NamePattern", SqlDbType.NVarChar, 152)
                    .Value = normalizedName == null
                        ? (object)DBNull.Value
                        : "%" + normalizedName + "%";

                command.Parameters
                    .Add("@CategoryId", SqlDbType.Int)
                    .Value = (object)categoryId ?? DBNull.Value;

                command.Parameters
                    .Add("@LowStockOnly", SqlDbType.Bit)
                    .Value = (object)lowStockOnly ?? DBNull.Value;

                command.Parameters
                    .Add("@Threshold", SqlDbType.Int)
                    .Value = LowStockThreshold;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        products.Add(MapProduct(reader));
                }
            }

            return products;
        }

        public int Add(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            const string query = @"
                INSERT INTO Products
                (
                    Name, Description, Quantity, Price,
                    SupplierName, CategoryId, CreatedBy
                )
                OUTPUT INSERTED.Id
                VALUES
                (
                    @Name, @Description, @Quantity, @Price,
                    @SupplierName, @CategoryId, @CreatedBy
                );";

            EnsureConnectionOpen();

            using (var command = new SqlCommand(
                query, _connection, _getTransaction()))
            {
                AddProductParameters(command, product);

                command.Parameters
                    .Add("@CreatedBy", SqlDbType.Int)
                    .Value = product.CreatedBy;

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public bool Update(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            const string query = @"
                UPDATE Products
                SET Name = @Name,
                    Description = @Description,
                    Quantity = @Quantity,
                    Price = @Price,
                    SupplierName = @SupplierName,
                    CategoryId = @CategoryId,
                    UpdatedAt = SYSDATETIME(),
                    UpdatedBy = @UpdatedBy
                WHERE Id = @Id
                  AND IsDeleted = 0;";

            EnsureConnectionOpen();

            using (var command = new SqlCommand(
                query, _connection, _getTransaction()))
            {
                AddProductParameters(command, product);

                command.Parameters
                    .Add("@Id", SqlDbType.Int).Value = product.Id;

                command.Parameters
                    .Add("@UpdatedBy", SqlDbType.Int)
                    .Value = (object)product.UpdatedBy ?? DBNull.Value;

                return command.ExecuteNonQuery() == 1;
            }
        }

        public bool SoftDelete(int id)
        {
            const string query = @"
                UPDATE Products
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

        public List<Product> GetByCategoryId(int categoryId)
        {
            const string query = @"
                SELECT Id, Name, Description, Quantity, Price,
                       SupplierName, CategoryId, IsDeleted,
                       CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
                FROM Products
                WHERE CategoryId = @CategoryId
                  AND IsDeleted = 0
                ORDER BY Name;";

            var products = new List<Product>();

            EnsureConnectionOpen();

            using (var command = new SqlCommand(
                query, _connection, _getTransaction()))
            {
                command.Parameters
                    .Add("@CategoryId", SqlDbType.Int)
                    .Value = categoryId;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        products.Add(MapProduct(reader));
                }
            }

            return products;
        }

        public bool HasActiveProducts(int categoryId)
        {
            const string query = @"
                SELECT CASE WHEN EXISTS (
                    SELECT 1
                    FROM Products
                    WHERE CategoryId = @CategoryId
                      AND IsDeleted = 0
                ) THEN 1 ELSE 0 END;";

            EnsureConnectionOpen();

            using (var command = new SqlCommand(
                query, _connection, _getTransaction()))
            {
                command.Parameters
                    .Add("@CategoryId", SqlDbType.Int)
                    .Value = categoryId;

                return Convert.ToInt32(command.ExecuteScalar()) == 1;
            }
        }

        public int SoftDeleteByCategoryId(int categoryId)
        {
            const string query = @"
                UPDATE Products
                SET IsDeleted = 1,
                    UpdatedAt = SYSDATETIME()
                WHERE CategoryId = @CategoryId
                  AND IsDeleted = 0;";

            EnsureConnectionOpen();

            using (var command = new SqlCommand(
                query, _connection, _getTransaction()))
            {
                command.Parameters
                    .Add("@CategoryId", SqlDbType.Int)
                    .Value = categoryId;

                return command.ExecuteNonQuery();
            }
        }

        private static void AddProductParameters(
            SqlCommand command, Product product)
        {
            command.Parameters
                .Add("@Name", SqlDbType.NVarChar, 150)
                .Value = product.Name;

            command.Parameters
                .Add("@Description", SqlDbType.NVarChar, 500)
                .Value = (object)product.Description ?? DBNull.Value;

            command.Parameters
                .Add("@Quantity", SqlDbType.Int)
                .Value = product.Quantity;

            var priceParameter =
                command.Parameters.Add("@Price", SqlDbType.Decimal);

            priceParameter.Precision = 18;
            priceParameter.Scale = 2;
            priceParameter.Value = product.Price;

            command.Parameters
                .Add("@SupplierName", SqlDbType.NVarChar, 150)
                .Value = product.SupplierName;

            command.Parameters
                .Add("@CategoryId", SqlDbType.Int)
                .Value = product.CategoryId;
        }

        private static Product MapProduct(SqlDataReader reader)
        {
            return new Product
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Description = reader["Description"] == DBNull.Value
                    ? null
                    : (string)reader["Description"],
                Quantity = (int)reader["Quantity"],
                Price = (decimal)reader["Price"],
                SupplierName = (string)reader["SupplierName"],
                CategoryId = (int)reader["CategoryId"],
                IsDeleted = (bool)reader["IsDeleted"],
                CreatedAt = (DateTime)reader["CreatedAt"],
                CreatedBy = (int)reader["CreatedBy"],
                UpdatedAt = reader["UpdatedAt"] == DBNull.Value
                    ? (DateTime?)null
                    : (DateTime)reader["UpdatedAt"],
                UpdatedBy = reader["UpdatedBy"] == DBNull.Value
                    ? (int?)null
                    : (int)reader["UpdatedBy"]
            };
        }
    }
}

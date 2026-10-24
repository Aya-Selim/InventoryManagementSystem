
using System;
using System.Data;
using System.Data.SqlClient;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Interfaces;

namespace InventoryManagement.Data.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly SqlConnection _connection;
        private readonly Func<SqlTransaction> _getTransaction;

        public AuditLogRepository(
            SqlConnection connection,
            Func<SqlTransaction> getTransaction)
        {
            _connection = connection
                ?? throw new ArgumentNullException(nameof(connection));

            _getTransaction = getTransaction
                ?? throw new ArgumentNullException(nameof(getTransaction));
        }

        public void Add(AuditLog auditLog)
        {
            if (auditLog == null)
                throw new ArgumentNullException(nameof(auditLog));

            var transaction = _getTransaction();

            if (transaction == null)
                throw new InvalidOperationException(
                    "Audit logs must be added within a transaction.");

            if (_connection.State != ConnectionState.Open)
                throw new InvalidOperationException(
                    "The database connection must be open.");

            const string query = @"
                INSERT INTO AuditLogs
                (
                    ActionType,
                    EntityType,
                    ProductId,
                    CategoryId,
                    UserId,
                    OldValues,
                    NewValues
                )
                VALUES
                (
                    @ActionType,
                    @EntityType,
                    @ProductId,
                    @CategoryId,
                    @UserId,
                    @OldValues,
                    @NewValues
                );";

            using (var command = new SqlCommand(
                query, _connection, transaction))
            {
                command.Parameters
                    .Add("@ActionType", SqlDbType.NVarChar, 20)
                    .Value = auditLog.ActionType;

                command.Parameters
                    .Add("@EntityType", SqlDbType.NVarChar, 20)
                    .Value = auditLog.EntityType;

                command.Parameters
                    .Add("@ProductId", SqlDbType.Int)
                    .Value = (object)auditLog.ProductId ?? DBNull.Value;

                command.Parameters
                    .Add("@CategoryId", SqlDbType.Int)
                    .Value = (object)auditLog.CategoryId ?? DBNull.Value;

                command.Parameters
                    .Add("@UserId", SqlDbType.Int)
                    .Value = auditLog.UserId;

                command.Parameters
                    .Add("@OldValues", SqlDbType.NVarChar, -1)
                    .Value = (object)auditLog.OldValues ?? DBNull.Value;

                command.Parameters
                    .Add("@NewValues", SqlDbType.NVarChar, -1)
                    .Value = (object)auditLog.NewValues ?? DBNull.Value;

                command.ExecuteNonQuery();
            }
        }
    }
}

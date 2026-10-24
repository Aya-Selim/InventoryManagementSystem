
using System;
using System.Data;
using System.Data.SqlClient;
using InventoryManagement.Core.Interfaces;
using InventoryManagement.Data.Repositories;

namespace InventoryManagement.Data
{
    public class SqlUnitOfWork : IUnitOfWork
    {
        private readonly SqlConnection _connection;
        private SqlTransaction _transaction;

        public ICategoryRepository Categories { get; }
        public IProductRepository Products { get; }
        public IAuditLogRepository AuditLogs { get; }

        public SqlUnitOfWork(string connectionString)
        {
            _connection = new SqlConnection(connectionString);

            Categories = new CategoryRepository(
                _connection,
                () => _transaction);

            Products = new ProductRepository(
                _connection,
                () => _transaction);

            AuditLogs = new AuditLogRepository(
                _connection,
                () => _transaction);
        }

        public void BeginTransaction()
        {
            if (_transaction != null)
                throw new InvalidOperationException(
                    "A transaction is already active.");

            if (_connection.State != ConnectionState.Open)
                _connection.Open();

            _transaction = _connection.BeginTransaction();
        }

        public void Commit()
        {
            if (_transaction == null)
                throw new InvalidOperationException(
                    "There is no active transaction.");

            try
            {
                _transaction.Commit();
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void Rollback()
        {
            if (_transaction == null)
                return;

            try
            {
                _transaction.Rollback();
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                try
                {
                    Rollback();
                }
                catch
                {
                    // Preserve disposal even if rollback fails.
                }
            }

            _connection.Dispose();
        }

    }
}

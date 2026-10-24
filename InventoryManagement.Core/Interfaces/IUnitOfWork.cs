using System;

namespace InventoryManagement.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository Categories { get; }

        IProductRepository Products { get; }

        IAuditLogRepository AuditLogs { get; }

        void BeginTransaction();

        void Commit();

        void Rollback();
    }
}
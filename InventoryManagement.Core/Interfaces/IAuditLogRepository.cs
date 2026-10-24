using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces
{
    public interface IAuditLogRepository
    {
        void Add(AuditLog auditLog);
    }
}
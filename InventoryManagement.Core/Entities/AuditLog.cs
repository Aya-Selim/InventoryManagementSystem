using System;

namespace InventoryManagement.Core.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string ActionType { get; set; }

        public string EntityType { get; set; }

        public int? ProductId { get; set; }

        public int? CategoryId { get; set; }

        public int UserId { get; set; }

        public string OldValues { get; set; }

        public string NewValues { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
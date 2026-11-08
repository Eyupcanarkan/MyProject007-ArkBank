using System;

namespace MyProject007.Entities
{
    public class AuditLog
    {
        public long AuditLogId { get; set; }
        public int? UserId { get; set; }
        public string EventType { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string MachineInfo { get; set; }
    }
}
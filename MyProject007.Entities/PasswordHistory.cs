using System;

namespace MyProject007.Entities
{
    public class PasswordHistory
    {
        public long PasswordHistoryId { get; set; }
        public int UserId { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public int PasswordIterations { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
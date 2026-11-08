using System;

namespace MyProject007.Entities
{
    public class PasswordResetToken
    {
        public long TokenId { get; set; }
        public int UserId { get; set; }
        public string TokenHash { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
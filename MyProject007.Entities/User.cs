using System;

namespace MyProject007.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public int PasswordIterations { get; set; }

       
        public string DuressPasswordHash { get; set; }
        public string DuressPasswordSalt { get; set; }
        public decimal VaultBalance { get; set; }           
        public DateTime? VaultUnlockDate { get; set; }     
        public bool IsDuressLogin { get; set; }             

        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public decimal Balance { get; set; }
        public string AccountNumber { get; set; }
        public bool IsActive { get; set; }
        public bool MustChangePassword { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
    }
}
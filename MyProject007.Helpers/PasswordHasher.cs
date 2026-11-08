using System;
using System.Security.Cryptography;

namespace MyProject007.Helpers
{
    public sealed class PasswordHashResult
    {
        public string Hash { get; set; }
        public string Salt { get; set; }
        public int Iterations { get; set; }
    }

    public static class PasswordHasher
    {
        private const int SaltSize = 16;      
        private const int HashSize = 32;       
        public const int DefaultIterations = 210000; 

        public static PasswordHashResult HashPassword(string password, int iterations = DefaultIterations)
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);
                return new PasswordHashResult
                {
                    Hash = Convert.ToBase64String(hash),
                    Salt = Convert.ToBase64String(salt),
                    Iterations = iterations
                };
            }
        }

        public static bool Verify(string password, string storedHash, string storedSalt, int iterations)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(storedSalt))
                return false;

            byte[] salt = Convert.FromBase64String(storedSalt);
            byte[] expectedHash = Convert.FromBase64String(storedHash);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                byte[] actualHash = pbkdf2.GetBytes(expectedHash.Length);

                
                int diff = expectedHash.Length ^ actualHash.Length;
                for (int i = 0; i < expectedHash.Length && i < actualHash.Length; i++)
                {
                    diff |= expectedHash[i] ^ actualHash[i];
                }
                return diff == 0;
            }
        }
    }
}
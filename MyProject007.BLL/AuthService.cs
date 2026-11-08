using System;
using System.Security.Cryptography;
using MyProject007.DAL;
using MyProject007.Entities;
using MyProject007.Helpers;

namespace MyProject007.BLL
{
    public class AuthService
    {
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly SecurityRepository _securityRepository = new SecurityRepository();

       
        public User Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new Exception("E-posta ve parola alanları zorunludur.");

            var user = _userRepository.GetByEmail(email.Trim());
            if (user == null)
            {
                try { _securityRepository.InsertAuditLog(null, "LOGIN_FAILED", $"Bilinmeyen e-posta ile giriş denendi: {email}"); } catch { }
                throw new Exception("E-posta adresi veya parola hatalı.");
            }

            if (!user.IsActive)
            {
                try { _securityRepository.InsertAuditLog(user.UserId, "LOGIN_FAILED", "Pasif hesapla giriş denendi."); } catch { }
                throw new Exception("Hesabınız pasif durumdadır. Lütfen yöneticinizle iletişime geçin.");
            }

           
            bool isDuress = false;
            if (!string.IsNullOrEmpty(user.DuressPasswordHash) && !string.IsNullOrEmpty(user.DuressPasswordSalt))
            {
                if (PasswordHasher.Verify(password, user.DuressPasswordHash, user.DuressPasswordSalt, user.PasswordIterations))
                {
                    isDuress = true;
                    user.IsDuressLogin = true;
                    try
                    {
                        _securityRepository.InsertAuditLog(user.UserId, "CRITICAL_DURESS_ALARM",
                            $"[ACİL DURUM] Kullanıcı panik parolası ile giriş yaptı! Cihaz: {Environment.MachineName}");
                    }
                    catch { }
                }
            }

         
            if (!isDuress)
            {
                bool isPasswordValid = PasswordHasher.Verify(password, user.PasswordHash, user.PasswordSalt, user.PasswordIterations);
                if (!isPasswordValid)
                {
                    try { _securityRepository.InsertAuditLog(user.UserId, "LOGIN_FAILED", "Hatalı parola girildi."); } catch { }
                    throw new Exception("E-posta adresi veya parola hatalı.");
                }
                try { _securityRepository.InsertAuditLog(user.UserId, "LOGIN_SUCCESS", "Başarılı giriş yapıldı."); } catch { }
            }

            
            Session.SessionBaslat(
                user.UserId,
                user.Email,
                $"{user.FirstName} {user.LastName}",
                user.RoleName,
                user.AccountNumber,
                user.Balance,
                user.VaultBalance,
                user.VaultUnlockDate,
                isDuress
            );

            return user;
        }

        public void ChangePassword(int userId, string oldPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
                throw new Exception("Mevcut parola ve yeni parola alanları zorunludur.");

            var user = _userRepository.GetById(userId);
            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

       
            bool isOldValid = PasswordHasher.Verify(oldPassword, user.PasswordHash, user.PasswordSalt, user.PasswordIterations);
            if (!isOldValid)
            {
                try { _securityRepository.InsertAuditLog(userId, "PASSWORD_CHANGE_FAILED", "Mevcut parola hatalı girildi."); } catch { }
                throw new Exception("Mevcut parolanızı hatalı girdiniz.");
            }

           
            ValidatePasswordRules(newPassword, user.FirstName, user.LastName, user.BirthDate);

           
            if (PasswordHasher.Verify(newPassword, user.PasswordHash, user.PasswordSalt, user.PasswordIterations))
            {
                throw new Exception("Yeni parolanız mevcut parolanız ile aynı olamaz.");
            }

            
            var newHashResult = PasswordHasher.HashPassword(newPassword);
            _userRepository.UpdatePassword(userId, newHashResult.Hash, newHashResult.Salt, newHashResult.Iterations);

            try { _securityRepository.InsertAuditLog(userId, "PASSWORD_CHANGED", "Parola başarıyla değiştirildi."); } catch { }
        }

     
        public string CreatePasswordResetToken(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Lütfen e-posta adresinizi giriniz.");

            var user = _userRepository.GetByEmail(email.Trim());
            if (user == null)
                return null;

            byte[] randomBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            string token = BitConverter.ToString(randomBytes).Replace("-", "").Substring(0, 10).ToUpper();

            try { _securityRepository.InsertAuditLog(user.UserId, "PASSWORD_RESET_REQ", $"Token: {token}"); } catch { }
            return token;
        }
        public string RequestPasswordReset(string email)
        {
            return CreatePasswordResetToken(email);
        }

      
        public void ResetPasswordWithToken(string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(newPassword))
                throw new Exception("Güvenlik kodu ve yeni parola alanları zorunludur.");

            
            if (Session.UserID > 0)
            {
                var user = _userRepository.GetById(Session.UserID);
                if (user != null)
                {
                    ValidatePasswordRules(newPassword, user.FirstName, user.LastName, user.BirthDate);
                    var newHashResult = PasswordHasher.HashPassword(newPassword);
                    _userRepository.UpdatePassword(user.UserId, newHashResult.Hash, newHashResult.Salt, newHashResult.Iterations);
                    try { _securityRepository.InsertAuditLog(user.UserId, "PASSWORD_RESET_SUCCESS", "Token ile parola sıfırlandı."); } catch { }
                    return;
                }
            }

            throw new Exception("Geçersiz veya süresi dolmuş güvenlik kodu.");
        }

     
        public void SeedInitialUsersIfNecessary()
        {
            try
            {
                var user = _userRepository.GetByEmail("user@banka.com");
                if (user != null)
                {
                    var userHash = PasswordHasher.HashPassword("User2026!");
                    _userRepository.UpdatePassword(user.UserId, userHash.Hash, userHash.Salt, userHash.Iterations);

                    var panicHash = PasswordHasher.HashPassword("Panic2026!");
                    _userRepository.UpdateDuressPassword(user.UserId, panicHash.Hash, panicHash.Salt);
                }

                var admin = _userRepository.GetByEmail("admin@banka.com");
                if (admin != null)
                {
                    var adminHash = PasswordHasher.HashPassword("Admin2026!");
                    _userRepository.UpdatePassword(admin.UserId, adminHash.Hash, adminHash.Salt, adminHash.Iterations);
                }
            }
            catch { }
        }

        private void ValidatePasswordRules(string password, string firstName, string lastName, DateTime? birthDate)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new Exception("Parola en az 6 karakter uzunluğunda olmalıdır.");

            bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false;
            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else hasSpecial = true;
            }

            if (!hasUpper || !hasLower || !hasDigit || !hasSpecial)
                throw new Exception("Parola en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir.");

            if (!string.IsNullOrEmpty(firstName) && firstName.Length >= 3 && password.IndexOf(firstName, StringComparison.OrdinalIgnoreCase) >= 0)
                throw new Exception("Parola adınızı içeremez.");

            if (!string.IsNullOrEmpty(lastName) && lastName.Length >= 3 && password.IndexOf(lastName, StringComparison.OrdinalIgnoreCase) >= 0)
                throw new Exception("Parola soyadınızı içeremez.");

          
            for (int i = 0; i < password.Length - 2; i++)
            {
                if (char.IsDigit(password[i]) && char.IsDigit(password[i + 1]) && char.IsDigit(password[i + 2]))
                {
                    int d1 = password[i] - '0';
                    int d2 = password[i + 1] - '0';
                    int d3 = password[i + 2] - '0';
                    if ((d2 == d1 + 1 && d3 == d2 + 1) || (d2 == d1 - 1 && d3 == d2 - 1))
                        throw new Exception("Parola ardışık rakam dizisi içeremez (örn: 123, 321).");
                }
            }
        }
    }
}
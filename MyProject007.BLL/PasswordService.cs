using System;
using MyProject007.DAL;
using MyProject007.Helpers;

namespace MyProject007.BLL
{
    public class PasswordService
    {
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly SecurityRepository _securityRepository = new SecurityRepository();

        
        public void EnsureNotReusedPassword(int userId, string newPassword, string currentHash, string currentSalt, int currentIterations)
        {
            
            if (PasswordHasher.Verify(newPassword, currentHash, currentSalt, currentIterations))
                throw new Exception("Yeni parolanız mevcut parolanız ile aynı olamaz.");

            
            var histories = _securityRepository.GetLastPasswordHistories(userId, 3);
            foreach (var history in histories)
            {
                if (PasswordHasher.Verify(newPassword, history.PasswordHash, history.PasswordSalt, history.PasswordIterations))
                    throw new Exception("Yeni parolanız, daha önce kullandığınız son 3 paroladan biri olamaz.");
            }
        }

        
        public void ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
                throw new Exception("Kullanıcı bulunamadı.");

            
            if (!PasswordHasher.Verify(oldPassword, user.PasswordHash, user.PasswordSalt, user.PasswordIterations))
                throw new Exception("Mevcut parolanızı hatalı girdiniz.");

           
            var validation = PasswordPolicyValidator.Validate(newPassword, user.FirstName, user.LastName, user.BirthDate);
            if (!validation.IsValid)
                throw new Exception(string.Join("\n", validation.Errors));

           
            EnsureNotReusedPassword(userId, newPassword, user.PasswordHash, user.PasswordSalt, user.PasswordIterations);

            using (var conn = DbConnectionFactory.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        
                        _securityRepository.AddPasswordHistory(user.UserId, user.PasswordHash, user.PasswordSalt, user.PasswordIterations, conn, trans);

                  
                        var hashResult = PasswordHasher.HashPassword(newPassword);
                        _userRepository.UpdatePassword(user.UserId, hashResult.Hash, hashResult.Salt, hashResult.Iterations);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw new Exception("Parola güncelleme işlemi sırasında bir hata oluştu.");
                    }
                }
            }

            _securityRepository.InsertAuditLog(userId, "PASSWORD_CHANGED", "Kullanıcı parolasını değiştirdi.");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MyProject007.DAL;
using MyProject007.Entities;
using MyProject007.Helpers;

namespace MyProject007.BLL
{
    public class UserService
    {
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly SecurityRepository _securityRepository = new SecurityRepository();

  
        public List<User> GetAllUsers()
        {
            if (Session.Rol != "Admin")
                throw new Exception("Bu işlem için yetkiniz bulunmamaktadır.");

            return _userRepository.GetAll();
        }

        
        public void SetUserActiveStatus(int targetUserId, bool isActive)
        {
            if (Session.Rol != "Admin")
                throw new Exception("Yetkisiz işlem.");

            _userRepository.UpdateActiveStatus(targetUserId, isActive);
        }

    
        public (string TempPassword, string PanicPassword) CreateUserWithSecurity(
            string email, string firstName, string lastName, DateTime birthDate, int roleId, decimal balance)
        {
            if (Session.Rol != "Admin")
                throw new Exception("Yetkisiz işlem.");

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new Exception("Lütfen tüm zorunlu alanları doldurunuz.");

            var existing = _userRepository.GetByEmail(email.Trim());
            if (existing != null)
                throw new Exception("Bu e-posta adresiyle kayıtlı bir kullanıcı zaten mevcut.");

          
            string tempPassword = GenerateSecureRandomPassword();
            string panicPassword = GenerateSecureRandomPassword() + "!";

            var tempHash = PasswordHasher.HashPassword(tempPassword);
            var panicHash = PasswordHasher.HashPassword(panicPassword);

            var user = new User
            {
                Email = email.Trim(),
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                BirthDate = birthDate,
                RoleId = roleId,
                Balance = balance,
                PasswordHash = tempHash.Hash,
                PasswordSalt = tempHash.Salt,
                PasswordIterations = tempHash.Iterations,
                DuressPasswordHash = panicHash.Hash,
                DuressPasswordSalt = panicHash.Salt,
                MustChangePassword = true,
                IsActive = true
            };

            _userRepository.InsertUserWithDuress(user);
            return (tempPassword, panicPassword);
        }

 
        public bool IsHighRiskTransfer(int senderUserId, decimal amount)
        {
            var sender = _userRepository.GetById(senderUserId);
            if (sender == null) return false;

            if (amount >= 5000.00m || (sender.Balance > 0 && amount >= (sender.Balance * 0.50m)))
            {
                return true;
            }
            return false;
        }

        
        public void TransferMoney(int senderUserId, string receiverAccountNumber, decimal amount, string description)
        {
            if (Session.IsDuressSession)
            {
                _securityRepository.InsertAuditLog(senderUserId, "DURESS_TRANSFER_BLOCKED",
                    $"Panik modunda transfer engellendi -> Hedef: {receiverAccountNumber}, Tutar: {amount:C2}");
                throw new Exception("Şu anda bankamızda güvenlik nedeniyle anlık transferler geçici olarak durdurulmuştur.");
            }

            if (amount <= 0)
                throw new Exception("Transfer tutarı sıfırdan büyük olmalıdır.");

            var sender = _userRepository.GetById(senderUserId);
            if (sender == null || !sender.IsActive)
                throw new Exception("Gönderici hesap bulunamadı veya pasif.");

            if (sender.Balance < amount)
                throw new Exception($"Yetersiz bakiye! Kullanılabilir: {sender.Balance:C2}");

            var receiver = _userRepository.GetByAccountNumber(receiverAccountNumber.Trim());
            if (receiver == null || !receiver.IsActive)
                throw new Exception("Alıcı hesap bulunamadı.");

            if (sender.AccountNumber == receiver.AccountNumber)
                throw new Exception("Kendi hesabınıza transfer yapamazsınız.");

            using (var conn = DbConnectionFactory.GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        _userRepository.UpdateBalance(sender.UserId, sender.Balance - amount, conn, trans);
                        _userRepository.UpdateBalance(receiver.UserId, receiver.Balance + amount, conn, trans);

                        const string sqlTrans = @"
                            INSERT INTO AccountTransactions (SenderUserId, ReceiverAccountNumber, Amount, Description, TransactionDate)
                            VALUES (@SenderId, @ReceiverAcc, @Amount, @Desc, SYSDATETIME())";

                        using (var cmd = new SqlCommand(sqlTrans, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@SenderId", sender.UserId);
                            cmd.Parameters.AddWithValue("@ReceiverAcc", receiver.AccountNumber);
                            cmd.Parameters.AddWithValue("@Amount", amount);
                            cmd.Parameters.AddWithValue("@Desc", (object)description ?? DBNull.Value);
                            cmd.ExecuteNonQuery();
                        }

                        trans.Commit();
                        Session.Balance -= amount;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw new Exception("Transfer işlemi sırasında bir veritabanı hatası oluştu.");
                    }
                }
            }

            _securityRepository.InsertAuditLog(sender.UserId, "MONEY_TRANSFER", $"{amount:C2} tutarında transfer yapıldı.");
        }

   
        public DataTable GetUserTransactions(int userId, string accountNumber)
        {
            if (Session.IsDuressSession)
            {
                var dt = new DataTable();
                dt.Columns.Add("İşlem No");
                dt.Columns.Add("Tarih");
                dt.Columns.Add("Tür");
                dt.Columns.Add("Karşı Hesap");
                dt.Columns.Add("Tutar (TL)");
                dt.Columns.Add("Açıklama");

                dt.Rows.Add("1", DateTime.Now.AddDays(-1).ToString("dd.MM.yyyy HH:mm"), "GİDEN TRANSFER", "TR8000000099", "120,00", "Market Harcaması");
                return dt;
            }

            return _userRepository.GetAccountTransactions(userId, accountNumber);
        }

    
        private string GenerateSecureRandomPassword()
        {
            const string lowers = "abcdefghijkmnopqrstuvwxyz";
            const string uppers = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string digits = "23456789";
            const string specials = "!@$?_-";

            var rnd = new Random();
            char[] chars = new char[10];
            chars[0] = uppers[rnd.Next(uppers.Length)];
            chars[1] = lowers[rnd.Next(lowers.Length)];
            chars[2] = digits[rnd.Next(digits.Length)];
            chars[3] = specials[rnd.Next(specials.Length)];

            string all = lowers + uppers + digits + specials;
            for (int i = 4; i < 10; i++)
            {
                chars[i] = all[rnd.Next(all.Length)];
            }

            for (int i = chars.Length - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                char temp = chars[i];
                chars[i] = chars[j];
                chars[j] = temp;
            }

            return new string(chars);
        }
        public void UpdateUserDuressPassword(int userId, string newPanicPassword)
        {
            var hashResult = PasswordHasher.HashPassword(newPanicPassword);
            _userRepository.UpdateDuressPassword(userId, hashResult.Hash, hashResult.Salt);
            _securityRepository.InsertAuditLog(userId, "DURESS_PASSWORD_CHANGED", "Kullanıcı panik parolasını güncelledi.");
        }
    }
}
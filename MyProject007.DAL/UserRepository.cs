using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MyProject007.Entities;

namespace MyProject007.DAL
{
    public class UserRepository
    {
        private const string UserSelectFields = @"
            u.UserId, u.Email, u.FirstName, u.LastName, u.BirthDate, 
            u.PasswordHash, u.PasswordSalt, u.PasswordIterations, 
            u.DuressPasswordHash, u.DuressPasswordSalt, u.VaultBalance, u.VaultUnlockDate,
            u.RoleId, r.RoleName, u.Balance, u.AccountNumber, 
            u.IsActive, u.MustChangePassword, u.CreatedAt, u.CreatedBy";

        public User GetByEmail(string email)
        {
            string sql = $@"
                SELECT {UserSelectFields}
                FROM Users u
                INNER JOIN Roles r ON u.RoleId = r.RoleId
                WHERE u.Email = @Email";

            using (var conn = DbConnectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = email;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) return MapUser(reader);
                }
            }
            return null;
        }

        public User GetById(int userId)
        {
            string sql = $@"
                SELECT {UserSelectFields}
                FROM Users u
                INNER JOIN Roles r ON u.RoleId = r.RoleId
                WHERE u.UserId = @UserId";

            using (var conn = DbConnectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) return MapUser(reader);
                }
            }
            return null;
        }

        public User GetByAccountNumber(string accountNumber)
        {
            string sql = $@"
                SELECT {UserSelectFields}
                FROM Users u
                INNER JOIN Roles r ON u.RoleId = r.RoleId
                WHERE u.AccountNumber = @AccountNumber";

            using (var conn = DbConnectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@AccountNumber", SqlDbType.NVarChar, 20).Value = accountNumber;
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) return MapUser(reader);
                }
            }
            return null;
        }

        public List<User> GetAll()
        {
            var list = new List<User>();
            string sql = $@"
                SELECT {UserSelectFields}
                FROM Users u
                INNER JOIN Roles r ON u.RoleId = r.RoleId
                ORDER BY u.UserId DESC";

            using (var conn = DbConnectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) list.Add(MapUser(reader));
                }
            }
            return list;
        }

        public void UpdateActiveStatus(int userId, bool isActive)
        {
            const string sql = "UPDATE Users SET IsActive = @IsActive WHERE UserId = @UserId";
            using (var conn = DbConnectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@IsActive", isActive);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdatePassword(int userId, string hash, string salt, int iterations)
        {
            const string sql = @"
                UPDATE Users 
                SET PasswordHash = @Hash, PasswordSalt = @Salt, PasswordIterations = @Iter, MustChangePassword = 0 
                WHERE UserId = @UserId";

            using (var conn = DbConnectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Hash", hash);
                cmd.Parameters.AddWithValue("@Salt", salt);
                cmd.Parameters.AddWithValue("@Iter", iterations);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateDuressPassword(int userId, string hash, string salt)
        {
            const string sql = "UPDATE Users SET DuressPasswordHash = @Hash, DuressPasswordSalt = @Salt WHERE UserId = @UserId";
            using (var conn = DbConnectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Hash", hash);
                cmd.Parameters.AddWithValue("@Salt", salt);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateBalance(int userId, decimal newBalance, SqlConnection conn, SqlTransaction trans)
        {
            const string sql = "UPDATE Users SET Balance = @Balance WHERE UserId = @UserId";
            using (var cmd = new SqlCommand(sql, conn, trans))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Balance", newBalance);
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertUserWithDuress(User user)
        {
            const string sql = @"
                INSERT INTO Users 
                (Email, FirstName, LastName, BirthDate, PasswordHash, PasswordSalt, PasswordIterations, 
                 DuressPasswordHash, DuressPasswordSalt, RoleId, Balance, AccountNumber, IsActive, MustChangePassword, CreatedAt)
                VALUES 
                (@Email, @FirstName, @LastName, @BirthDate, @PasswordHash, @PasswordSalt, @PasswordIterations, 
                 @DuressPasswordHash, @DuressPasswordSalt, @RoleId, @Balance, @AccountNumber, @IsActive, @MustChangePassword, SYSDATETIME())";

            using (var conn = DbConnectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@BirthDate", (object)user.BirthDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@PasswordSalt", user.PasswordSalt);
                cmd.Parameters.AddWithValue("@PasswordIterations", user.PasswordIterations);
                cmd.Parameters.AddWithValue("@DuressPasswordHash", (object)user.DuressPasswordHash ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DuressPasswordSalt", (object)user.DuressPasswordSalt ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
                cmd.Parameters.AddWithValue("@Balance", user.Balance);
                cmd.Parameters.AddWithValue("@AccountNumber", "TR" + new Random().Next(100000000, 999999999).ToString());
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                cmd.Parameters.AddWithValue("@MustChangePassword", user.MustChangePassword);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetAccountTransactions(int userId, string accountNumber)
        {
            var dt = new DataTable();
            const string sql = @"
                SELECT 
                    t.TransactionId AS [İşlem No],
                    t.TransactionDate AS [Tarih],
                    CASE 
                        WHEN t.SenderUserId = @UserId THEN 'GİDEN TRANSFER'
                        ELSE 'GELEN TRANSFER'
                    END AS [Tür],
                    CASE 
                        WHEN t.SenderUserId = @UserId THEN t.ReceiverAccountNumber
                        ELSE uSender.AccountNumber
                    END AS [Karşı Hesap],
                    t.Amount AS [Tutar (TL)],
                    t.Description AS [Açıklama]
                FROM AccountTransactions t
                INNER JOIN Users uSender ON t.SenderUserId = uSender.UserId
                WHERE t.SenderUserId = @UserId OR t.ReceiverAccountNumber = @AccountNumber
                ORDER BY t.TransactionDate DESC";

            using (var conn = DbConnectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);
                conn.Open();
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        private static User MapUser(SqlDataReader reader)
        {
            return new User
            {
                UserId = Convert.ToInt32(reader["UserId"]),
                Email = reader["Email"].ToString(),
                FirstName = reader["FirstName"].ToString(),
                LastName = reader["LastName"].ToString(),
                BirthDate = reader["BirthDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["BirthDate"]),
                PasswordHash = reader["PasswordHash"].ToString(),
                PasswordSalt = reader["PasswordSalt"].ToString(),
                PasswordIterations = Convert.ToInt32(reader["PasswordIterations"]),
                DuressPasswordHash = reader["DuressPasswordHash"] == DBNull.Value ? null : reader["DuressPasswordHash"].ToString(),
                DuressPasswordSalt = reader["DuressPasswordSalt"] == DBNull.Value ? null : reader["DuressPasswordSalt"].ToString(),
                VaultBalance = reader["VaultBalance"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["VaultBalance"]),
                VaultUnlockDate = reader["VaultUnlockDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["VaultUnlockDate"]),
                RoleId = Convert.ToInt32(reader["RoleId"]),
                RoleName = reader["RoleName"].ToString(),
                Balance = Convert.ToDecimal(reader["Balance"]),
                AccountNumber = reader["AccountNumber"].ToString(),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                MustChangePassword = Convert.ToBoolean(reader["MustChangePassword"]),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                CreatedBy = reader["CreatedBy"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["CreatedBy"])
            };
        }
    }
}
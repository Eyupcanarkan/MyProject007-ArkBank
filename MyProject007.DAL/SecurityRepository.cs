using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MyProject007.Entities;

namespace MyProject007.DAL
{
    public class SecurityRepository
    {
     
        public List<PasswordHistory> GetLastPasswordHistories(int userId, int count = 3)
        {
            var list = new List<PasswordHistory>();
            const string sql = @"
                SELECT TOP (@Count) PasswordHistoryId, UserId, PasswordHash, PasswordSalt, PasswordIterations, CreatedAt
                FROM PasswordHistories
                WHERE UserId = @UserId
                ORDER BY CreatedAt DESC";

            using (var conn = DbConnectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@Count", SqlDbType.Int).Value = count;

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PasswordHistory
                        {
                            PasswordHistoryId = Convert.ToInt64(reader["PasswordHistoryId"]),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            PasswordHash = reader["PasswordHash"].ToString(),
                            PasswordSalt = reader["PasswordSalt"].ToString(),
                            PasswordIterations = Convert.ToInt32(reader["PasswordIterations"]),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        });
                    }
                }
            }
            return list;
        }

        
        public void AddPasswordHistory(int userId, string hash, string salt, int iterations, SqlConnection conn, SqlTransaction trans)
        {
            const string sql = @"
                INSERT INTO PasswordHistories (UserId, PasswordHash, PasswordSalt, PasswordIterations, CreatedAt)
                VALUES (@UserId, @PasswordHash, @PasswordSalt, @PasswordIterations, SYSDATETIME())";

            using (var cmd = new SqlCommand(sql, conn, trans))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 500).Value = hash;
                cmd.Parameters.Add("@PasswordSalt", SqlDbType.NVarChar, 500).Value = salt;
                cmd.Parameters.Add("@PasswordIterations", SqlDbType.Int).Value = iterations;
                cmd.ExecuteNonQuery();
            }
        }

       
        public void CreateResetToken(int userId, string tokenHash, DateTime expiresAt)
        {
            const string sql = @"
                INSERT INTO PasswordResetTokens (UserId, TokenHash, ExpiresAt, CreatedAt)
                VALUES (@UserId, @TokenHash, @ExpiresAt, SYSDATETIME())";

            using (var conn = DbConnectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@TokenHash", SqlDbType.Char, 64).Value = tokenHash;
                cmd.Parameters.Add("@ExpiresAt", SqlDbType.DateTime2).Value = expiresAt;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

    
        public PasswordResetToken GetValidResetToken(string tokenHash)
        {
            const string sql = @"
                SELECT TokenId, UserId, TokenHash, ExpiresAt, UsedAt, CreatedAt
                FROM PasswordResetTokens
                WHERE TokenHash = @TokenHash 
                  AND UsedAt IS NULL 
                  AND ExpiresAt > SYSDATETIME()";

            using (var conn = DbConnectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@TokenHash", SqlDbType.Char, 64).Value = tokenHash;
                conn.Open();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new PasswordResetToken
                        {
                            TokenId = Convert.ToInt64(reader["TokenId"]),
                            UserId = Convert.ToInt32(reader["UserId"]),
                            TokenHash = reader["TokenHash"].ToString(),
                            ExpiresAt = Convert.ToDateTime(reader["ExpiresAt"]),
                            UsedAt = reader["UsedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["UsedAt"]),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                        };
                    }
                }
            }
            return null;
        }

       
        public void MarkTokenUsed(long tokenId, SqlConnection conn, SqlTransaction trans)
        {
            const string sql = "UPDATE PasswordResetTokens SET UsedAt = SYSDATETIME() WHERE TokenId = @TokenId";

            using (var cmd = new SqlCommand(sql, conn, trans))
            {
                cmd.Parameters.Add("@TokenId", SqlDbType.BigInt).Value = tokenId;
                cmd.ExecuteNonQuery();
            }
        }

    
        public void InsertAuditLog(int? userId, string eventType, string description)
        {
            const string sql = @"
                INSERT INTO AuditLogs (UserId, EventType, Description, CreatedAt, MachineInfo)
                VALUES (@UserId, @EventType, @Description, SYSDATETIME(), @MachineInfo)";

            using (var conn = DbConnectionFactory.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = (object)userId ?? DBNull.Value;
                cmd.Parameters.Add("@EventType", SqlDbType.NVarChar, 50).Value = eventType;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 500).Value = description;
                cmd.Parameters.Add("@MachineInfo", SqlDbType.NVarChar, 100).Value = Environment.MachineName;

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
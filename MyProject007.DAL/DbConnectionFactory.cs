using System.Configuration;
using System.Data.SqlClient;

namespace MyProject007.DAL
{
    public static class DbConnectionFactory
    {
        private static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["BankaDbConnection"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
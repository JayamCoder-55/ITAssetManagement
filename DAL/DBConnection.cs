using System.Configuration;
using System.Data.SqlClient;

namespace ITAssetManagement.DAL
{
    public static class DbConnection
    {
        private static string connString = ConfigurationManager.ConnectionStrings["ITAssetDB"].ConnectionString;

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connString);
        }
    }
}
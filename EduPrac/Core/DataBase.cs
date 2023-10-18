using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduPrac
{
    internal class DataBase
    {
        SqlConnection sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;
                                                          AttachDbFilename=C:\Users\Teniks_V\Documents\GitHub\2023_EduPrac_3-2-ISiP-2\EduPrac\LD.mdf;
                                                          Integrated Security=True");

        public void openConection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }

        public void closeConection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }

        public SqlConnection GetSqlConection()
        {
            return sqlConnection;
        }
    }
}

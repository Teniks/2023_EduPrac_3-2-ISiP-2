using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Xml;
using System.Collections;
using System.Reflection;

namespace EduPrac
{
    internal class DataBase
    {
        SqlConnection sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;
                                                          AttachDbFilename=C:\Users\Teniks_V\Documents\GitHub\2023_EduPrac_3-2-ISiP-2\EduPrac\data\Model\LD.mdf;
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

        public static string GetAttribut(in string[][] Data, in int massAttr, bool log = false)
        {
            string listAttribute;

            listAttribute = "(";
            for (int i = log ? 1 : 0 ; i < Data[massAttr].Length; i++)
            {
                if (i < Data[massAttr].Length - 1)
                {
                    listAttribute += $"{Data[massAttr][i]}" + ", ";
                }
                else
                {
                    listAttribute += $"{Data[massAttr][i]}";
                }
            }
            listAttribute += ")";
            return listAttribute;
        }

        public static void conectTableSQL(in string querySQL,in DataGrid dataGrid)
        {
            try
            {
                DataBase localDB = new DataBase();
                SqlDataAdapter adapter = new SqlDataAdapter();
                DataTable dataTable = new DataTable();

                localDB.openConection();

                using (SqlCommand sqlCommand = new SqlCommand(querySQL, localDB.GetSqlConection()))
                {
                    dataGrid.Visibility = Visibility.Visible;
                    dataGrid.Columns.Clear();
                    dataGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

                    adapter.SelectCommand = sqlCommand;
                    adapter.Fill(dataTable);

                    dataGrid.ItemsSource = dataTable.DefaultView;
                    dataGrid.AutoGenerateColumns = true;
                    dataGrid.CanUserAddRows = false;
                    dataGrid.CanUserDeleteRows = false;
                }

                localDB.closeConection();
            }
            catch
            {
                MessageBox.Show("При выводе данных в таблицу из базы данных возникла ошибка");
            }
        }

        public static bool checkIDisExists(in DataGrid dataGrid,in string Nameid,in string nameTable,in string nameValue,in string value)
        {
            int counter = 0;
            string querySQL = $"SELECT {Nameid} FROM {nameTable} WHERE {nameValue} = N{value}";

            DataBase localDB = new DataBase();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlDataReader reader;

            try
            {
                localDB.openConection();

                using (SqlCommand sqlCommand = new SqlCommand(querySQL, localDB.GetSqlConection()))
                {
                    dataGrid.Visibility = Visibility.Visible;
                    dataGrid.Columns.Clear();

                    adapter.SelectCommand = sqlCommand;
                    reader = sqlCommand.ExecuteReader();
                    reader.Read();
                    if (reader.HasRows)
                    {
                        counter++;
                    }

                }
            }
            catch
            {
                MessageBox.Show("При проверке записи на наличие в базе данных возникла ошибка.");
            }

            localDB.closeConection();

            if (counter != 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static int newID(in string nameTable, in string nameId)
        {
            int newID = 0;
            try
            {
                DataBase localDB = new DataBase();
                SqlDataAdapter adapter = new SqlDataAdapter();

                localDB.openConection();

                string querySQL = $"SELECT MAX({nameId}) FROM {nameTable}";
                using (SqlCommand sqlCommand = new SqlCommand(querySQL, localDB.GetSqlConection()))
                {
                    adapter.SelectCommand = sqlCommand;
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);

                    try
                    {
                        newID = (int)dataTable.Rows[0][0] + 1;
                    }
                    catch
                    {
                        newID = 1;
                    }
                    

                }

                localDB.closeConection();
            }
            catch
            {
                MessageBox.Show("При вычислении ID используемого элемента возникла ошибка. Проверьте правильность данных.");
            }
            return newID;
        }

        /// <summary>
        /// Only for INSERT, DELETE, UPDATE
        /// </summary>
        /// <param name="querySQL"></param>
        public static void querySQL(in string querySQL)
        {
            try
            {
                DataBase localDB = new DataBase();
                SqlDataAdapter adapter = new SqlDataAdapter();

                localDB.openConection();

                SqlCommand sqlCommand = new SqlCommand(querySQL, localDB.GetSqlConection());
                sqlCommand.ExecuteNonQuery();

                localDB.closeConection();

            }
            catch
            {
                MessageBox.Show("При запросе на добавление произошла ошибка. Проверьте правильность отправляемых данных.");
            }
        }

        public static void SearchInTable<T>(T searchSTR, string nameTable ,string[] attributs, DataGrid dataGrid)
        {
            string query = "";

            for (int i = attributs.Length; i >= 0; i--)
            {
                query += $"SELECT * FROM {nameTable} WHERE {attributs[i]} = '{searchSTR}'";
            }
            conectTableSQL(query, dataGrid);
        }
        public static void SearchEach<T>(T searchSTR, string[] namesTable, string[] attributs, DataGrid dataGrid)
        {
            string query = "";
            for(int j = namesTable.Length; j >= 0; j--)
            {
                for (int i = attributs.Length; i >= 0; i--)
                {
                    query += $"SELECT * FROM {namesTable[j]} WHERE {attributs[i]} = '{searchSTR}' \n";
                }
            }
            conectTableSQL(query, dataGrid);
        }
        public static int SearchID(in string Nameid, in string nameTable, in string nameValue, in string value)
        {
            int id = -1;
            string querySQL = $"SELECT {Nameid} FROM {nameTable} WHERE {nameValue} = N{value}";

            DataBase localDB = new DataBase();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlDataReader reader;

            try
            {
                localDB.openConection();

                using (SqlCommand sqlCommand = new SqlCommand(querySQL, localDB.GetSqlConection()))
                {
                    adapter.SelectCommand = sqlCommand;
                    reader = sqlCommand.ExecuteReader();
                    reader.Read();
                    if (reader.HasRows)
                    {
                        id = (int)reader[0];
                    }

                }
            }
            catch
            {
                MessageBox.Show($"При запросе к базе данных произошла ошибка. Проверьте правильность отправляемых данных.");
            }
            return id;
        }
    }
}

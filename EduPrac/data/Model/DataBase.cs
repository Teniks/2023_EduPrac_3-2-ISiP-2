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
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace EduPrac
{
    //Набор методов передачи сообщений в базу данных под необходимые случаи
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

        public static void SearchInTable(in string searchSTR,in string nameTable ,in string[] attributs, in DataGrid dataGrid)
        {
            string query = $"SELECT * FROM {nameTable}";

            for (int i = 0; i < attributs.Length; i++)
            {
                query += $"SELECT * FROM {nameTable} WHERE {attributs[i]} = N'{searchSTR.Trim()}'  ";
            }
            conectTableSQL(query, dataGrid);
        }
        public static void SearchEach<T>(in T searchSTR, in string[] namesTable, in string[] attributs, in DataGrid dataGrid)
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
        /// <summary>
        /// The query must request a single value
        /// Запрос должен запрашивать одно значение
        /// </summary>
        /// <param name="IdArtist"></param>
        /// <returns></returns>
        public static int GetSumMinuteWork(in int IdArtist, in int Month)
        {
            string querySQL = "Select (FORMAT(EndTime, 'HH')*60 + FORMAT(EndTime, 'mm') - FORMAT(DateGoingWork, 'HH')*60+FORMAT(DateGoingWork, 'mm')) " +
                    "from LogWork where FORMAT(EndTime, 'yyyy/MM/dd') = FORMAT(DateGoingWork, 'yyyy/MM/dd')\r\nand FORMAT(EndTime, 'yyyy/MM') = FORMAT(DateGoingWork, 'yyyy/MM')" +
                    $" and IdArtist = {IdArtist} and FORMAT(DateGoingWork, 'MM') = {Month}";
            int answer = 0;
            try
            {
                DataBase localDB = new DataBase();
                SqlDataAdapter adapter = new SqlDataAdapter();

                localDB.openConection();

                using (SqlCommand sqlCommand = new SqlCommand(querySQL, localDB.GetSqlConection()))
                {
                    adapter.SelectCommand = sqlCommand;
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);
                    try
                    {
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            answer += (int)dataTable.Rows[i][0];
                        }
                    }
                    catch
                    {
                        
                    }
                }

                localDB.closeConection();
            }
            catch
            {
                MessageBox.Show("При получении ответа на запрос возникла ошибка. Попробуйте снова");
            }
            return answer;
        }
        public static string GetFullName(in int IdArtist)
        {
            string querySQL = $"SELECT FullNameArtist from Artists where IdArtist = {IdArtist}";
            string answer = "";
            try
            {
                DataBase localDB = new DataBase();
                SqlDataAdapter adapter = new SqlDataAdapter();

                localDB.openConection();

                using (SqlCommand sqlCommand = new SqlCommand(querySQL, localDB.GetSqlConection()))
                {
                    adapter.SelectCommand = sqlCommand;
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);
                    try
                    {
                        answer = (string)dataTable.Rows[0][0];
                    }
                    catch
                    {

                    }
                }

                localDB.closeConection();
            }
            catch
            {
                MessageBox.Show("При получении ответа на запрос возникла ошибка. Попробуйте снова");
            }
            return answer;
        }
        public static int GetFirstYear()
        {
            int year = 0000;

            string querySQL = $"SELECT MIN(FORMAT(DateGoingWork, 'yyyy')) FROM LogWork";
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
                        string yearS = (string)reader[0];
                        year = Convert.ToInt32(yearS);
                    }
                }
            }
            catch
            {

            }

            return year;
        }
        public static int GetLastYear()
        {
            int year = 0000;

            string querySQL = $"SELECT MAX(FORMAT(DateGoingWork, 'yyyy')) FROM LogWork";
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
                        string yearS = (string)reader[0];
                        year = Convert.ToInt32(yearS);
                    }
                }
            }
            catch
            {

            }

            return year;
        }
        public static int GetFirstMonth(in int year)
        {
            int month = 0;

            string querySQL = $"SELECT MIN(FORMAT(DateGoingWork, 'MM')) FROM LogWork WHERE FORMAT(DateGoingWork, 'yyyy') = {year}";
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
                        string monthS= (string)reader[0];
                        month = Convert.ToInt32(monthS);
                    }
                }
            }
            catch
            {

            }

            return month;
        }
        public static int GetLastMonth(in int year)
        {
            int month = 0000;

            string querySQL = $"SELECT MAX(FORMAT(DateGoingWork, 'MM')) FROM LogWork WHERE FORMAT(DateGoingWork, 'yyyy') = {year}";
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
                        string monthS = (string)reader[0];
                        month = Convert.ToInt32(monthS);
                    }
                }
            }
            catch
            {

            }

            return month;
        }
        /// <summary>
        /// Returns ranges of day and cchanges the count of day in total.
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="idArtist"></param>
        /// <param name="countDay"></param>
        /// <returns></returns>
        public static string GetWorkDay(in int year, in int month, in int idArtist, ref int countDay)
        {
            string querySQL = $"SELECT FORMAT(DateGoingWork, 'dd') FROM LogWork where FORMAT(DateGoingWork, 'MM') = {month} and FORMAT(DateGoingWork, 'yyyy') = {year} and IdArtist = {idArtist}";
            string answer = ""; countDay = 0;
            try
            {
                DataBase localDB = new DataBase();
                SqlDataAdapter adapter = new SqlDataAdapter();

                localDB.openConection();

                using (SqlCommand sqlCommand = new SqlCommand(querySQL, localDB.GetSqlConection()))
                {
                    adapter.SelectCommand = sqlCommand;
                    DataTable dataTable = new DataTable();

                    adapter.Fill(dataTable);
                    try
                    {
                        countDay = dataTable.Rows.Count;

                        if (countDay != 1 && countDay != 0)
                        {
                            for (int i = 0; i < dataTable.Rows.Count; i++)
                            {
                                if(i != dataTable.Rows.Count-1)
                                {
                                    if (Convert.ToInt32((string)dataTable.Rows[i + 1][0]) - Convert.ToInt32((string)dataTable.Rows[i][0]) != 1)
                                    {
                                        answer += Convert.ToInt32((string)dataTable.Rows[i][0]) + " ";
                                    }
                                    if (Convert.ToInt32((string)dataTable.Rows[i][0]) + 1 == Convert.ToInt32((string)dataTable.Rows[i + 1][0]))
                                    {
                                        if (!answer.EndsWith("."))
                                            answer += Convert.ToInt32((string)dataTable.Rows[i][0]) + ".";
                                        else answer += ".";
                                    }
                                }
                                else
                                {
                                    if (answer.EndsWith("."))
                                        answer += "." + Convert.ToInt32((string)dataTable.Rows[i][0]) + " ";
                                    else answer += " " + Convert.ToInt32((string)dataTable.Rows[i][0]);
                                }

                            }
                        }
                        else
                        {
                            if (countDay != 0)
                                    answer += Convert.ToInt32((string)dataTable.Rows[0][0]);
                        }
                        
                    }
                    catch
                    {

                    }
                }

                localDB.closeConection();
            }catch { }

            return answer;
        }
    }
}

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

            int max = Data[massAttr].Length;

            listAttribute = "(";
            for (int i = log ? 1 : 0 ; i < max; i++)
            {
                if (i < max - 1)
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

                localDB.openConection();

                using (SqlCommand sqlCommand = new SqlCommand(querySQL, localDB.GetSqlConection()))
                {
                    dataGrid.Visibility = Visibility.Visible;
                    dataGrid.Columns.Clear();
                    dataGrid.HorizontalAlignment = HorizontalAlignment.Stretch;

                    DataTable dataTable = new DataTable();
                    adapter.SelectCommand = sqlCommand;
                    adapter.Fill(dataTable);
                    MainWindow.dTable.Clear();
                    MainWindow.dTable.Columns.Clear();
                    adapter.Fill(MainWindow.dTable);

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

        public static bool checkIDisExistsDouble(in string nameTable, in string nameValue, in string value, in string nameValue2, in string value2 )
        {
            int counter = 0;
            string querySQL = $"select * from {nameTable} where {nameValue} = N'{value}'  and {nameValue2} = N{value2}";

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
                        counter++;
                    }

                }
            }
            catch
            {

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

        public static bool checkIDisExists(in DataGrid dataGrid,in string nameid,in string nameTable,in string nameValue,in string value)
        {
            int counter = 0;
            string querySQL = $"SELECT {nameid} FROM {nameTable} WHERE {nameValue} = N'{value}'";

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
                    MainWindow.dTable.Clear();
                    MainWindow.dTable.Columns.Clear();
                    adapter.Fill(MainWindow.dTable);

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

        public static void SearchInTable(in string searchSTR,in string nameTable ,in string attribut, in DataGrid dataGrid, int oneOrTwo = 0, in string searchSTR2 = "" , in string attribut2 = "")
        {
            string query = $"";

            switch (oneOrTwo)
            {
                case 0:
                    query = $"SELECT IdRecordLog AS '№', FORMAT(DateGoingWork, N'yyyy/MM/dd HH:mm') as 'Вышел на работу', Artists.FullNameArtist as 'Полное имя'," +
                    $"CircusArea.NameCircusArea as 'Цирковая площадка', FORMAT(EndTime, N'yyyy/MM/dd HH:mm') as 'Закончил работу', " +
                    $"LaunchBreak as 'Перерыв' FROM {nameTable} left join Artists cross join CircusArea " +
                    $" on LogWork.IdArtist = Artists.IdArtist and LogWork.IdArea = CircusArea.IdArea WHERE {attribut} = N'{searchSTR.Trim()}' ORDER BY DateGoingWork DESC";
                    break;
                case 1:
                    query = $"SELECT IdRecordLog AS '№', FORMAT(DateGoingWork, N'yyyy/MM/dd HH:mm') as 'Вышел на работу', Artists.FullNameArtist as 'Полное имя'," +
                    $"CircusArea.NameCircusArea as 'Цирковая площадка', FORMAT(EndTime, N'yyyy/MM/dd HH:mm') as 'Закончил работу', " +
                    $"LaunchBreak as 'Перерыв' FROM {nameTable} left join Artists cross join CircusArea " +
                    $" on LogWork.IdArtist = Artists.IdArtist and LogWork.IdArea = CircusArea.IdArea WHERE LogWork.{attribut} = N'{searchSTR.Trim()}' ORDER BY DateGoingWork DESC";
                    break;
                case 2:
                    query = $"SELECT IdRecordLog AS '№', FORMAT(DateGoingWork, N'yyyy/MM/dd HH:mm') as 'Вышел на работу', Artists.FullNameArtist as 'Полное имя'," +
                    $"CircusArea.NameCircusArea as 'Цирковая площадка', FORMAT(EndTime, N'yyyy/MM/dd HH:mm') as 'Закончил работу', " +
                    $"LaunchBreak as 'Перерыв'  FROM {nameTable} left join Artists cross join CircusArea " +
                    $" on LogWork.IdArtist = Artists.IdArtist and LogWork.IdArea = CircusArea.IdArea " +
                    $"WHERE LogWork.{attribut} = N'{searchSTR.Trim()}'  and {attribut2} = N'{searchSTR2.Trim()}'  ORDER BY DateGoingWork DESC";
                    break;
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
            string querySQL = "Select ((FORMAT(EndTime, 'HH')*60 + FORMAT(EndTime, 'mm') - FORMAT(DateGoingWork, 'HH')*60+FORMAT(DateGoingWork, 'mm')) - DATEPART(HOUR, LaunchBreak)*60 + DATEPART(MINUTE, LaunchBreak)) " +
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
                    MainWindow.dTable.Clear();
                    MainWindow.dTable.Columns.Clear();
                    adapter.Fill(MainWindow.dTable);
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
                    MainWindow.dTable.Clear();
                    MainWindow.dTable.Columns.Clear();
                    adapter.Fill(MainWindow.dTable);
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
        /// Returns ranges of day and changes the count of day in total.
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
                    MainWindow.dTable.Clear();
                    MainWindow.dTable.Columns.Clear();
                    adapter.Fill(MainWindow.dTable);
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
                                        if (!answer.EndsWith("-"))
                                            answer += Convert.ToInt32((string)dataTable.Rows[i][0]) + "-";
                                        else answer += "";
                                    }
                                }
                                else
                                {
                                    if (answer.EndsWith("-"))
                                        answer += "-" + Convert.ToInt32((string)dataTable.Rows[i][0]) + " ";
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

        public static void BackUp()
        {
            try
            {
                string query = $@"DECLARE @db_Name nvarchar(max); DECLARE @backup_path nvarchar(max); set @db_Name = (select DB_NAME() AS [CURENT DATABASE]); set @backup_path = 'C:\Users\Public\Documents\LD.bak'; BACKUP DATABASE @db_Name TO DISK = @backup_path";
                querySQL(query);
                MessageBox.Show($"Путь: \n" + @"C:\Users\Public\Documents\LD.bak");
            }
            catch { }
        }
        public static void Restore(in string path)
        {
            try
            {
                string query = $@"DECLARE @backup_path nvarchar(max); SET @backup_path = '{path}'; RESTORE DATABASE LD.mdf FROM DISK @backup_path WITH NORECOVERY; RESTORE DATABASE LD.mdf WITH RECOVERY;";
                querySQL(query);

                MessageBox.Show("Восстановление завершено");
            }
            catch
            {

            }
        }
    }
}

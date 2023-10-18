using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace EduPrac
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        string query;
        public MainWindow()
        {
            InitializeComponent();

        }

        private void CloseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void MinButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void ButtonLog_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            query = "SELECT * FROM LogWork";
            querySQL(query);
        }

        private void ButtonListPerson_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            query = "SELECT * FROM Artists";
            querySQL(query);
        }

        private void ButtonListCategory_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            query = "SELECT * FROM ArtistCategory";
            querySQL(query);
        }

        private void ButtonCircusArea_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            query = "SELECT * FROM CircusArea";
            querySQL(query);
        }

        private void AddRow_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void DeleteRow_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void CreateDocs_PreviewMouseUp(object sender, MouseButtonEventArgs e)
                {

                }

        public void querySQL(in string querySQL)
        {
            try
            {
                DataBase localDB = new DataBase();
                SqlDataAdapter adapter = new SqlDataAdapter();
                DataTable DataTable = new DataTable();

                localDB.openConection();

                using (SqlCommand sqlCommand = new SqlCommand(querySQL, localDB.GetSqlConection()))
                {
                    DataGridTableArea.Columns.Clear();

                    adapter.SelectCommand = sqlCommand;
                    adapter.Fill(DataTable);

                    DataGridTableArea.ItemsSource = DataTable.DefaultView;
                    DataGridTableArea.AutoGenerateColumns = true;
                    DataGridTableArea.CanUserAddRows = false;
                    DataGridTableArea.CanUserDeleteRows = false;
                }

                localDB.closeConection();
            }
            catch
            {

            }
        }

        private void Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}

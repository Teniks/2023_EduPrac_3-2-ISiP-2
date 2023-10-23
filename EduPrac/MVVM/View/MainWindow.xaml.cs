using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml;

namespace EduPrac
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string query;
        string[][] TableData = 
        {   new string[] { "LogWork", "Artists", "ArtistCategory", "CircusArea"},
            new string[] { "IdRecordLog", "DateGoingWork", "IdArtist", "IdArea", "EndTime", "LaunchBreak"},
            new string[] { "IdArtist", "IdCategory", "FullNameArtist", "AddressArtist", "YearBirthArtist", "YearEntryArtist", "GenderArtist", "NumberPhoneArtist"},
            new string[] { "IdCategory", "NameCategory"},
            new string[] { "IdArea", "NameCircusArea", "AddressArea"}
        };

        string nameTable;
        string Attributs;
        string idname;
        int countAttribute;

        public MainWindow()
        {
            InitializeComponent();

            ButtonBorderSaveRecord.Visibility = Visibility.Collapsed;
            TextBoxes.Visibility = Visibility.Collapsed;
        }

        private void ButtonLog_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ButtonBorderSaveRecord.Visibility = Visibility.Collapsed;
            TextBoxes.Visibility = Visibility.Collapsed;

            nameTable = TableData[0][0];
            
            query = $"select DateGoingWork as 'Вышел на работу', Artists.FullNameArtist as 'Полное имя', " +
                    "CircusArea.NameCircusArea as 'Цирковая площадка', EndTime as 'Закончил работу', " +
                    "LaunchBreak as 'Перерыв' " +
                    "from LogWork left join Artists cross join CircusArea " +
                    "on LogWork.IdArtist = Artists.IdArtist and LogWork.IdArea = CircusArea.IdArea";

            DataBase.conectTableSQL(query, DataGridTableArea);
            Attributs = DataBase.GetAttribut(TableData, 1);
            countAttribute = 6;
            idname = TableData[1][0];

        }

        private void ButtonListPerson_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ButtonBorderSaveRecord.Visibility = Visibility.Collapsed;
            TextBoxes.Visibility = Visibility.Collapsed;

            nameTable = TableData[0][1];
            query = "SELECT FullNameArtist AS 'Имя артиста', ArtistCategory.NameCategory as 'Категория' , AddressArtist AS 'Адрес', YearBirthArtist AS 'День рождения'," +
                " YearEntryArtist AS 'Дата приема на работу', GenderArtist AS 'Пол', NumberPhoneArtist AS 'Номер телефона' " +
                "FROM Artists LEFT JOIN ArtistCategory ON Artists.IdCategory = ArtistCategory.IdCategory";

            DataBase.conectTableSQL(query, DataGridTableArea);
            Attributs = DataBase.GetAttribut(TableData, 2);
            countAttribute = 6;
            idname = TableData[2][0];
        }

        private void ButtonListCategory_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ButtonBorderSaveRecord.Visibility = Visibility.Collapsed;
            TextBoxes.Visibility = Visibility.Collapsed;

            nameTable = TableData[0][2];
            query = "SELECT NameCategory AS 'Категория' FROM ArtistCategory";

            DataBase.conectTableSQL(query, DataGridTableArea);
            Attributs = DataBase.GetAttribut(TableData, 3);
            countAttribute = 1;
            idname = TableData[3][0];
        }

        private void ButtonCircusArea_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ButtonBorderSaveRecord.Visibility = Visibility.Collapsed;
            TextBoxes.Visibility = Visibility.Collapsed;

            nameTable = TableData[0][3];
            query = "SELECT NameCircusArea AS 'Цирковая площадка', AddressArea AS 'Адрес площадки' FROM  CircusArea";

            DataBase.conectTableSQL(query, DataGridTableArea);
            Attributs = DataBase.GetAttribut(TableData, 4);
            countAttribute = 2;
            idname = TableData[4][0];
        }

        private void AddRow_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            DataGridTableArea.Visibility = Visibility.Collapsed;
            ButtonBorderSaveRecord.Visibility = Visibility.Visible;
            TextBoxes.Visibility = Visibility.Visible;

            FirstTextBox.Clear();
            SecondTextBox.Clear();
            ThirdTextBox.Clear();
            FourthTextBox.Clear();
            FifthTextBox.Clear();
            SixthTextBox.Clear();

            switch (countAttribute)
            {
                case 1:
                    SecondTextBox.Visibility = Visibility.Collapsed;
                    ThirdTextBox.Visibility = Visibility.Collapsed;
                    FourthTextBox.Visibility = Visibility.Collapsed;
                    FifthTextBox.Visibility = Visibility.Collapsed;
                    SixthTextBox.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    ThirdTextBox.Visibility = Visibility.Collapsed;
                    FourthTextBox.Visibility = Visibility.Collapsed;
                    FifthTextBox.Visibility = Visibility.Collapsed;
                    SixthTextBox.Visibility = Visibility.Collapsed;
                    break;
                case 6:
                    try
                    {
                        SecondTextBox.Visibility = Visibility.Visible;
                        ThirdTextBox.Visibility = Visibility.Visible;
                        FourthTextBox.Visibility = Visibility.Visible;
                        FifthTextBox.Visibility = Visibility.Visible;
                        SixthTextBox.Visibility = Visibility.Visible;
                    }
                    catch
                    {

                    }
                    break;
            }


        }  
        private void SaveChanges_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
             if(nameTable == TableData[0][1])
             {
                if(!DataBase.checkIDisExists(DataGridTableArea, TableData[2][0], TableData[0][1], TableData[2][2], $"'{SecondTextBox}'"))
                {
                    return;
                }
                
             }

            string valuesAttributs = "";
            string[] txtboxes = { FirstTextBox.Text, SecondTextBox.Text, ThirdTextBox.Text, FourthTextBox.Text, FifthTextBox.Text, SixthTextBox.Text };

            for (int i = 0; i < countAttribute; i++)
            {
                if (Char.IsLetter(txtboxes[i].Trim().ToCharArray().ElementAt(0)))
                {
                    if (i < countAttribute - 1)
                    {
                        valuesAttributs += "N'" + txtboxes[i].Trim() + "'" + ",";
                    }
                    else
                    {
                        valuesAttributs += "N'" + txtboxes[i].Trim() + "'";
                    }
                }
                else
                {
                    if (i < countAttribute - 1)
                    {
                        valuesAttributs += "'" + txtboxes[i].Trim() + "'" + ",";
                    }
                    else
                    {
                        valuesAttributs += "'" + txtboxes[i].Trim() + "'";
                    }
                }
            }
            string query = $"INSERT INTO {nameTable}{Attributs} VALUES ({DataBase.newID(nameTable, idname)}, {valuesAttributs})";
            DataBase.querySQL(query);
        }

        private void DeleteRow_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if(DataGridTableArea.SelectedItem != null)
            {
                DataGridTableArea.SelectedValue.ToString();
            }
        }

        private void CreateDocs_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }
        private void Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {

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
                if(this.WindowState != WindowState.Maximized)
                {
                    this.DragMove();
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 1280;
                    this.Height = 720;
                }
            }
        }
    }
}

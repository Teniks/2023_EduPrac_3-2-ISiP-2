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
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
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

        string[] rusNameAttributes;

        string nameTable;
        string Attributs;
        string idname;
        int countAttribute;
        bool AddorDel;

        public MainWindow()
        {
            InitializeComponent();

            TableVisElementEdit();
        }

        private void ButtonLog_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TableVisElementEdit();

            nameTable = TableData[0][0];
            
            query = $"select IdRecordLog AS '№', FORMAT(DateGoingWork, N'yyyy/MM/dd HH:mm') as 'Вышел на работу', Artists.FullNameArtist as 'Полное имя', " +
                    "CircusArea.NameCircusArea as 'Цирковая площадка', FORMAT(EndTime, N'yyyy/MM/dd HH:mm') as 'Закончил работу', " +
                    "LaunchBreak as 'Перерыв' " +
                    "from LogWork left join Artists cross join CircusArea " +
                    "on LogWork.IdArtist = Artists.IdArtist and LogWork.IdArea = CircusArea.IdArea";

            rusNameAttributes = new string[] { "Вышел", "Полное имя", "Цирковая площадка", "Закончил работу", "Перерыв" };
            DataBase.conectTableSQL(query, DataGridTableArea);
            Attributs = DataBase.GetAttribut(TableData, 1);
            idname = TableData[1][0];

        }

        private void ButtonListPerson_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TableVisElementEdit();

            nameTable = TableData[0][1];
            query = "SELECT IdArtist AS '№', FullNameArtist AS 'Имя артиста', ArtistCategory.NameCategory as 'Категория' , AddressArtist AS 'Адрес', FORMAT(YearBirthArtist, N'yyyy/MM/dd ') AS 'День рождения'," +
                    " FORMAT(YearEntryArtist, N'yyyy/MM/dd ') AS 'Дата приема на работу', GenderArtist AS 'Пол', NumberPhoneArtist AS 'Номер телефона' " +
                    "FROM Artists LEFT JOIN ArtistCategory ON Artists.IdCategory = ArtistCategory.IdCategory";

            rusNameAttributes = new string[] { "Имя артиста", "Категория", "Адрес", "День рождения", "Дата приема на работу", "Пол", "Номер телефона" };
            DataBase.conectTableSQL(query, DataGridTableArea);
            Attributs = DataBase.GetAttribut(TableData, 2);
            idname = TableData[2][0];
        }

        private void ButtonListCategory_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TableVisElementEdit();

            nameTable = TableData[0][2];
            query = "SELECT IdCategory AS '№', NameCategory AS 'Категория' FROM ArtistCategory";

            rusNameAttributes = new string[] { "Категория" };
            DataBase.conectTableSQL(query, DataGridTableArea);
            Attributs = DataBase.GetAttribut(TableData, 3);
            idname = TableData[3][0];
        }

        private void ButtonCircusArea_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TableVisElementEdit();

            nameTable = TableData[0][3];
            query = "SELECT IdArea AS '№', NameCircusArea AS 'Цирковая площадка', AddressArea AS 'Адрес площадки' FROM  CircusArea";

            rusNameAttributes = new string[] { "Цирковая площадка", "Адрес площадки" };
            DataBase.conectTableSQL(query, DataGridTableArea);
            Attributs = DataBase.GetAttribut(TableData, 4);
            idname = TableData[4][0];
        }

        private void AddRow_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //Операция чередования элементов при взаимодействии с одной из двух кнопок
            AddorDel = true;
            ChangesEditVisElementEdit();
            boolButton.Source = new BitmapImage(new Uri(@"Images/Add.png", UriKind.Relative));
            DeleteRowBorder.Visibility = Visibility.Visible;
            AddRowBorder.Visibility = Visibility.Hidden;
        }  
        private void SaveChanges_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //Проверка полей на необходимые данные и последующий вызов обработчиков запросов
            if (AddorDel)
            {
                switch (nameTable)
                {
                    case "Artists":
                        //Проверка на наличие необходимой записи в справочной таблице
                        if (!DataBase.checkIDisExists(DataGridTableArea, TableData[3][0], TableData[0][2], TableData[3][1], $"'{SecondTextBox.Text}'"))
                        {
                            MessageBox.Show("Такая категория отсутствует в списках. \n\t Проверьте правильность написания и наличие в списках");
                            return;
                        }
                        //Проверка на наличие данных в необходимых полях и активация выполнения сборщика запроса
                        if (FirstTextBox.Text.Trim() != "" && SecondTextBox.Text.Trim() != "" && ThirdTextBox.Text.Trim() != "" && FourthTextBox.Text.Trim() != "" && FifthTextBox.Text.Trim() != "")
                        {
                            DataBase.querySQL(CheckString());
                        }
                        else
                        {
                            MessageBox.Show("Пропущенные поля(все) не могут быть пустыми");
                        }
                        break;
                    case "LogWork":
                        if (!DataBase.checkIDisExists(DataGridTableArea, TableData[2][0], TableData[0][1], TableData[2][2], $"'{SecondTextBox.Text}'"))
                        {
                            MessageBox.Show("Такое имя отсутствует в списках. \n\t Проверьте правильность написания и наличие в списках");
                            return;
                        }
                        if (!DataBase.checkIDisExists(DataGridTableArea, TableData[4][0], TableData[0][3], TableData[4][1], $"'{ThirdTextBox.Text}'"))
                        {
                            MessageBox.Show("Такая площадка отсутствует в списках. \n\t Проверьте правильность написания и наличие в списках");
                            return;
                        }
                        if (FirstTextBox.Text.Trim() != "" && SecondTextBox.Text.Trim() != "")
                        {
                            DataBase.querySQL(CheckString());
                        }
                        else
                        {
                            MessageBox.Show("Пропущенные поля(1-2) не могут быть пустыми");
                        }
                        break;
                    case "CircusArea":
                        if (FirstTextBox.Text.Trim() != "" && SecondTextBox.Text.Trim() != "")
                        {
                            DataBase.querySQL(CheckString());
                        }
                        else
                        {
                            MessageBox.Show("Пропущенные поля(1-2) не могут быть пустыми");
                        }
                        break;
                    case "ArtistCategory":
                        if (FirstTextBox.Text.Trim() != "")
                        {
                            DataBase.querySQL(CheckString());
                        }
                        else
                        {
                            MessageBox.Show("Пропущенные поля(все) не могут быть пустыми");
                        }
                        break;
                }
            }
            if (!AddorDel)
            {
                if (nameTable == TableData[0][1])
                {
                    if (DataBase.checkIDisExists(DataGridTableArea, TableData[2][1], TableData[0][1], TableData[1][2], FirstTextBox.Text))
                    {
                        MessageBox.Show("Имя используется в другой записи. Таблица Artists");
                        return;
                    }
                }
                if (nameTable == TableData[0][2])
                {
                    if (DataBase.checkIDisExists(DataGridTableArea, TableData[2][1], TableData[0][2], TableData[1][2], FirstTextBox.Text))
                    {
                        MessageBox.Show("Категория используется в другой записи. Таблица Artists");
                        return;
                    }
                }
                string query = $"DELETE FROM {nameTable} WHERE {idname} = {FirstTextBox.Text}";
                DataBase.querySQL(query);
            }
        }

        private void DeleteRow_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //Операция чередования элементов при взаимодействии с одной из двух кнопок
            AddorDel = false;
            ChangesEditVisElementEdit();
            AddRowBorder.Visibility = Visibility.Visible;
            DeleteRowBorder.Visibility = Visibility.Collapsed;
            boolButton.Source = new BitmapImage(new Uri(@"Images/Delete.png", UriKind.Relative));
        }

        private void CreateDocs_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }
        private void Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(SettingsPanel.Visibility == Visibility.Collapsed)
                SettingsPanel.Visibility = Visibility.Visible;
            else SettingsPanel.Visibility = Visibility.Collapsed;
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
        private void SearchSortBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangesEditVisElementEdit();
        }
        private void TableVisElementEdit()
        {
            AddRowBorder.Visibility = Visibility.Visible;
            DeleteRowBorder.Visibility = Visibility.Visible;
            ButtonBorderSaveChanges.Visibility = Visibility.Collapsed;
            TextBoxes.Visibility = Visibility.Collapsed;

            FirstTxt.Visibility = Visibility.Collapsed;
            SecondTxt.Visibility = Visibility.Collapsed;
            ThirdTxt.Visibility = Visibility.Collapsed;
            FourthTxt.Visibility = Visibility.Collapsed;
            FifthTxt.Visibility = Visibility.Collapsed;
            SixthTxt.Visibility = Visibility.Collapsed;
            SeventhTxt.Visibility = Visibility.Collapsed;
        }
        private void ChangesEditVisElementEdit()
        {
            //Скрытие и проявление элементов окна. Подготовка к работе
            NameTableChange.Text = $"{nameTable}";
            DataGridTableArea.Visibility = Visibility.Collapsed;
            ButtonBorderSaveChanges.Visibility = Visibility.Visible;
            TextBoxes.Visibility = Visibility.Visible;

            FirstTextBox.Clear();
            SecondTextBox.Clear();
            ThirdTextBox.Clear();
            FourthTextBox.Clear();
            FifthTextBox.Clear();
            SixthTextBox.Clear();

            //Начало подбора необходмого количества полей и их подсказок
            switch (nameTable)
            {
                case "LogWork":
                    countAttribute = 5;
                    break;
                case "Artists":
                    countAttribute = 7;
                    break;
                case "ArtistCategory":
                    countAttribute = 1;
                    break;
                case "CircusArea":
                    countAttribute = 2;
                    break;

            }

            countAttribute = AddorDel ? countAttribute : 1;
            switch (countAttribute)
            {
                case 1:
                    SecondTextBox.Visibility = Visibility.Collapsed;
                    ThirdTextBox.Visibility = Visibility.Collapsed;
                    FourthTextBox.Visibility = Visibility.Collapsed;
                    FifthTextBox.Visibility = Visibility.Collapsed;
                    SixthTextBox.Visibility = Visibility.Collapsed;
                    SeventhTextBox.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    SecondTextBox.Visibility = Visibility.Visible;
                    ThirdTextBox.Visibility = Visibility.Collapsed;
                    FourthTextBox.Visibility = Visibility.Collapsed;
                    FifthTextBox.Visibility = Visibility.Collapsed;
                    SixthTextBox.Visibility = Visibility.Collapsed;
                    SeventhTextBox.Visibility = Visibility.Collapsed;
                    break;
                case 5:
                    SecondTextBox.Visibility = Visibility.Visible;
                    ThirdTextBox.Visibility = Visibility.Visible;
                    FourthTextBox.Visibility = Visibility.Visible;
                    FifthTextBox.Visibility = Visibility.Visible;
                    SixthTextBox.Visibility = Visibility.Collapsed;
                    SeventhTextBox.Visibility = Visibility.Collapsed;
                    break;
                case 7:
                    SecondTextBox.Visibility = Visibility.Visible;
                    ThirdTextBox.Visibility = Visibility.Visible;
                    FourthTextBox.Visibility = Visibility.Visible;
                    FifthTextBox.Visibility = Visibility.Visible;
                    SixthTextBox.Visibility = Visibility.Visible;
                    SeventhTextBox.Visibility = Visibility.Visible;
                    break;
            }
            try
            {
                switch (countAttribute)
                {
                    case 1:
                        FirstTxt.Visibility = Visibility.Visible;
                        FirstTxt.Text = AddorDel ? rusNameAttributes[0].ToString() : "Номер удаляемой записи";
                        SecondTxt.Visibility = Visibility.Collapsed;
                        ThirdTxt.Visibility = Visibility.Collapsed;
                        FourthTxt.Visibility = Visibility.Collapsed;
                        FifthTxt.Visibility = Visibility.Collapsed;
                        SixthTxt.Visibility = Visibility.Collapsed;
                        SeventhTxt.Visibility = Visibility.Collapsed;
                        break;
                    case 2:
                        FirstTxt.Visibility = Visibility.Visible;
                        FirstTxt.Text = rusNameAttributes[0].ToString();
                        SecondTxt.Visibility = Visibility.Visible;
                        SecondTxt.Text = rusNameAttributes[1].ToString();
                        ThirdTxt.Visibility = Visibility.Collapsed;
                        FourthTxt.Visibility = Visibility.Collapsed;
                        FifthTxt.Visibility = Visibility.Collapsed;
                        SixthTxt.Visibility = Visibility.Collapsed;
                        SeventhTxt.Visibility = Visibility.Collapsed;
                        break;
                    case 5:
                        FirstTxt.Visibility = Visibility.Visible;
                        FirstTxt.Text = rusNameAttributes[0].ToString() + "(год.месяц.день час:минута)";
                        SecondTxt.Visibility = Visibility.Visible;
                        SecondTxt.Text = rusNameAttributes[1].ToString();
                        ThirdTxt.Visibility = Visibility.Visible;
                        ThirdTxt.Text = rusNameAttributes[2].ToString();
                        FourthTxt.Visibility = Visibility.Visible;
                        FourthTxt.Text = rusNameAttributes[3].ToString();
                        FifthTxt.Visibility = Visibility.Visible;
                        FifthTxt.Text = rusNameAttributes[4].ToString();
                        SixthTxt.Visibility = Visibility.Collapsed;
                        SeventhTxt.Visibility = Visibility.Collapsed;
                        break;
                    case 7:
                        FirstTxt.Visibility = Visibility.Visible;
                        FirstTxt.Text = rusNameAttributes[0].ToString();
                        SecondTxt.Visibility = Visibility.Visible;
                        SecondTxt.Text = rusNameAttributes[1].ToString();
                        ThirdTxt.Visibility = Visibility.Visible;
                        ThirdTxt.Text = rusNameAttributes[2].ToString();
                        FourthTxt.Visibility = Visibility.Visible;
                        FourthTxt.Text = rusNameAttributes[3].ToString();
                        FifthTxt.Visibility = Visibility.Visible;
                        FifthTxt.Text = rusNameAttributes[4].ToString();
                        SixthTxt.Visibility = Visibility.Visible;
                        SixthTxt.Text = rusNameAttributes[5].ToString();
                        SeventhTxt.Visibility = Visibility.Visible;
                        SeventhTxt.Text = rusNameAttributes[6].ToString();
                        break;
                }
                //Конец подбора
            }
            catch
            {

            }
        }
        private string CheckString()
        {
            //Конвеер создания запроса SQL
            string valuesAttributs = "";
            string[] txtboxes = { FirstTextBox.Text, SecondTextBox.Text, ThirdTextBox.Text, FourthTextBox.Text, FifthTextBox.Text, SixthTextBox.Text, SeventhTextBox.Text };

            //Исправление порядка полей ввода для конкретной таблицы
            if (nameTable == "Artists")
            {
                txtboxes[1] = DataBase.SearchID("IdCategory", "ArtistCategory", "NameCategory", "'" + txtboxes[1] + "'").ToString();
                txtboxes[0] = txtboxes[1].ToString();
                txtboxes[1] = FirstTextBox.Text;
            }
            if (nameTable == "LogWork")
            {
                txtboxes[1] = DataBase.SearchID("IdArtist", "Artists", "FullNameArtist", "'" + txtboxes[1] + "'").ToString();
                txtboxes[2] = DataBase.SearchID("IdArea", "CircusArea", "NameCircusArea", "'" + txtboxes[2] + "'").ToString();
            }
            //Сам процесс подстановки символов для создания корректного запроса
            for (int i = 0; i < countAttribute; i++)
            {
                if (txtboxes[i].Trim() != "")
                {
                    if (Char.IsDigit(txtboxes[i].Trim().ToCharArray().ElementAt(0)) && txtboxes[i].Trim().ToCharArray().Length < 2)
                    {
                        if (i < countAttribute - 1)
                        {
                            valuesAttributs += txtboxes[i].Trim() + ",";
                        }
                        else
                        {
                            valuesAttributs += txtboxes[i].Trim();
                        }
                    }
                    else
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
                }
                if (txtboxes[i].Trim() == "")
                {
                    if (i < countAttribute - 1)
                    {
                        valuesAttributs += "NULL" + ",";
                    }
                    else
                    {
                        valuesAttributs += "NULL";
                    }
                }
            }
            //Реализация возможности изменения записи в едиснтвенной необходимой таблице
            if(nameTable == "LogWork")
            {
                if (DataBase.SearchID("IdRecordLog", "LogWork", "DateGoingWork", $"'{txtboxes[0]}'") == DataBase.SearchID("IdRecordLog", "LogWork", "IdArtist", $"'{txtboxes[1]}'"))
                {
                    string buffer3 = txtboxes[3].Trim() == "" ? "NULL" : $"N'{txtboxes[3]}'";
                    string buffer4 = txtboxes[4].Trim() == "" ? "NULL" : $"N'{txtboxes[4]}'";
                    return query = $"UPDATE {nameTable} SET IdArea = N'{txtboxes[2]}', EndTime = {buffer3}, LaunchBreak = {buffer4} WHERE IdRecordLog = {DataBase.SearchID("IdRecordLog", nameTable, "DateGoingWork", $"'{txtboxes[0]}'")}";
                }
            }
            
            return query = $"INSERT INTO {nameTable}{Attributs} VALUES ({DataBase.newID(nameTable, idname)}, {valuesAttributs})";
        }
            

        private void FirstTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FirstTxt.Visibility = Visibility.Collapsed;
        }

        private void SecondTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SecondTxt.Visibility = Visibility.Collapsed;
        }

        private void ThirdTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ThirdTxt.Visibility = Visibility.Collapsed;
        }

        private void FourthTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FourthTxt.Visibility = Visibility.Collapsed;
        }

        private void FifthTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            FifthTxt.Visibility = Visibility.Collapsed;
        }

        private void SixthTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SixthTxt.Visibility = Visibility.Collapsed;
        }

        private void SeventhTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SeventhTxt.Visibility = Visibility.Collapsed;
        }

        private void FirstTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (FirstTextBox.Text == "")
            {
                FirstTxt.Visibility = Visibility.Visible;
            }
        }

        private void SecondTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SecondTextBox.Text == "")
            {
                SecondTxt.Visibility = Visibility.Visible;
            }
        }

        private void ThirdTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ThirdTextBox.Text == "")
            {
                ThirdTxt.Visibility = Visibility.Visible;
            }
        }

        private void FourthTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (FourthTextBox.Text == "")
            {
                FourthTxt.Visibility = Visibility.Visible;
            }
        }

        private void FifthTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (FifthTextBox.Text == "")
            {
                FifthTxt.Visibility = Visibility.Visible;
            }
        }

        private void SixthTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SixthTextBox.Text == "")
            {
                SixthTxt.Visibility = Visibility.Visible;
            }
        }

        private void SeventhTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SeventhTextBox.Text == "")
            {
                SeventhTxt.Visibility = Visibility.Visible;
            }
        }

        private void SettingsPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            SettingsPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowSlidePanel(Grid grid)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0;
            animation.To = 500;  // задайте необходимую ширину для выдвигаемой панели
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.1)); // необходимая продолжительность анимации

            Storyboard.SetTargetName(animation, grid.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Grid.WidthProperty));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            storyboard.Begin(this);

            animation = new DoubleAnimation();






            animation.From = 0;
            animation.To = 400; // задайте необходимую ширину для выдвигаемой панели
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.1)); // задайте необходимую продолжительность анимации

            Storyboard.SetTargetName(animation, grid.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Border.HeightProperty));

            storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            storyboard.Begin(this);

        }
        private void HideSlidePanel(Grid grid)
        {
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 500;
            animation.To = 0; // задайте необходимую ширину для выдвигаемой панели
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3)); // задайте необходимую продолжительность анимации

            Storyboard.SetTargetName(animation, grid.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Grid.WidthProperty));

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            storyboard.Begin(this);

            animation = new DoubleAnimation();

            animation.From = 400;
            animation.To = 0; // задайте необходимую ширину для выдвигаемой панели
            animation.Duration = new Duration(TimeSpan.FromSeconds(0.3)); // задайте необходимую продолжительность анимации

            Storyboard.SetTargetName(animation, grid.Name);
            Storyboard.SetTargetProperty(animation, new PropertyPath(Border.HeightProperty));

            storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            storyboard.Begin(this);
        }
    }
}

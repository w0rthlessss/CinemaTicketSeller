using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CinemaTicketSeller.UserControls
{
    /// <summary>
    /// Логика взаимодействия для ScreeningUserControl.xaml
    /// </summary>
    public partial class ScreeningUserControl : UserControl
    {
        AdminConnection connection;

        ObservableCollection<Screenings> databaseScreenings = new ObservableCollection<Screenings>();
        List<Halls> halls = new List<Halls>();
        List<Movie> movies = new List<Movie>();
        bool containsThisID = false;

        public ScreeningUserControl(AdminConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            databaseScreenings = new ObservableCollection<Screenings>(connection.GetListOfScreenings());
            this.Table.ItemsSource = databaseScreenings;
            this.idTextBox.Text = connection.GetLastRecordIdFromSpecificTable("ScreeningID", "screenings").ToString();

            SetupComboBox();

            this.addScreening.MouseLeftButtonDown += AddScreeningClick;
            this.Random.MouseLeftButtonDown += RandomFillScreening;

            this.idTextBoxEdit.TextChanged += SetTextFields;
            this.ApplyChanges.MouseLeftButtonDown += EditRecords;
            this.DeleteScreening.MouseLeftButtonDown += DeleteRecord;

            this.DataContext = this;
        }       

        private void SetupComboBox()
        {
            halls = connection.GetListOfHalls();
            movies = connection.GetListOfMovies();

            this.movieIDComboBox.ItemsSource = movies.Select(m => m.Title).ToArray();
            this.movieIDComboBoxEdit.ItemsSource = movies.Select(m => m.Title);

            this.hallIDComboBox.ItemsSource = halls.Select(h => h.HallNumber);
            this.hallIDComboBoxEdit.ItemsSource = halls.Select(h => h.HallNumber);
        }

        private void UpdateTable()
        {
            this.Table.ItemsSource = databaseScreenings.ToList();
        }

        private bool CheckValue(string value, string regex)
        {
            Regex valCheck = new Regex(regex);
            return valCheck.IsMatch(value);
        }

        private bool IfContainsThisID()
        {
            if (!containsThisID)
            {
                MessageBox.Show("В базе данных не содержится записи с таким id!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                idTextBoxEdit.Focus();
                idTextBoxEdit.SelectAll();
            }
            return containsThisID;
        }


        #region AddRegion

        private void RemoveSelectionAdd()
        {
            this.movieIDComboBox.BorderThickness = new Thickness(0);
            this.hallIDComboBox.BorderThickness = new Thickness(0);
            this.datePicker.BorderThickness = new Thickness(0);
            this.timePicker.BorderThickness = new Thickness(0);
            this.priceAmplification.BorderThickness = new Thickness(0);
        }

        private void ClearFieldsAdd()
        {
            this.movieIDComboBox.Text = string.Empty;
            this.hallIDComboBox.Text = string.Empty;
            this.datePicker.Text = string.Empty;
            this.timePicker.Text = string.Empty;
            this.priceAmplification.Text = string.Empty;
        }

        private void AddScreeningClick(object sender, MouseButtonEventArgs e)
        {
            int id = Convert.ToInt32(this.idTextBox.Text);
            string title = this.movieIDComboBox.Text;
            string tmpHallNumber = this.hallIDComboBox.Text;
            DateTime date = DateTime.Parse(this.datePicker.Text);
            string time = this.timePicker.Text;
            string tmpPriceAmpl = this.priceAmplification.Text;

            int hallNumber = 0;
            double priceAmpl = 0;

            bool res = true;

            if(!CheckValue(tmpHallNumber, "^[1-9][0-9]*$") || !int.TryParse(tmpHallNumber, out hallNumber))
            {
                res = false;
                this.hallIDComboBox.BorderBrush = Brushes.Red;
                this.hallIDComboBox.BorderThickness = new Thickness(2);
            }

            if(date < DateTime.Today)
            {
                res = false;
                this.datePicker.BorderBrush = Brushes.Red;
                this.datePicker.BorderThickness = new Thickness(2);
            }

            if(!CheckValue(time, "^([01]\\d|2[0-3]):[0-5]\\d:[0-5]\\d$"))
            { 
                res = false;
                this.timePicker.BorderBrush= Brushes.Red;
                this.timePicker.BorderThickness = new Thickness(2);
            }

            if(!CheckValue(tmpPriceAmpl, "^[0-9]*.?[0-9]*") || !double.TryParse(tmpPriceAmpl, out priceAmpl)){
                res = false;
                this.priceAmplification.BorderBrush = Brushes.Red;
                this.priceAmplification.BorderThickness = new Thickness(2);
            }

            if(title == string.Empty)
            {
                res = false;
                this.movieIDComboBox.BorderBrush = Brushes.Red;
                this.movieIDComboBox.BorderThickness = new Thickness(2);
            }

            if (!res)
            {
                MessageBox.Show("Ошибки форматирования полей ввода", "Проверьте введенные значения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int hallID = halls.Where(h => h.HallNumber == hallNumber).First().HallID;
            int movieID = movies.Where(m => m.Title == title).First().MovieID;

            List<Screenings> curDateHallAndTime = databaseScreenings.Where(s => s.Hall.HallID == hallID && s.Date == date).ToList();

            for(int i = 0; i < curDateHallAndTime.Count - 1; i++)
            {
                DateTime t;
                DateTime.TryParse(time, out t);
                if (Math.Abs((curDateHallAndTime[i].Time - t).TotalMinutes) < 
                    Round(curDateHallAndTime[i].Time.AddMinutes(curDateHallAndTime[i].Movie.Duration)).TimeOfDay.TotalMinutes ||
                    Math.Abs((curDateHallAndTime[i+1].Time - t).TotalMinutes) <
                    Round(curDateHallAndTime[i+1].Time.AddMinutes(curDateHallAndTime[i+1].Movie.Duration)).TimeOfDay.TotalMinutes)
                {
                    MessageBox.Show("Данное время уже занято другим сеансом", "Проверьте введенные значения", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.datePicker.BorderBrush = Brushes.Red;
                    this.datePicker.BorderThickness = new Thickness(2);
                    return;
                }
            }            

            Screenings newScreening = new Screenings(id, movies.Where(m => m.MovieID == movieID).First(), halls.Where(h => h.HallID == hallID).First(), date, DateTime.Parse(time), new List<Seat>(), priceAmpl);

            RemoveSelectionAdd();
            connection.InsertScreenings([newScreening]);
            this.idTextBox.Text = (id + 1).ToString();
            ClearFieldsAdd();
            MessageBox.Show("Новая запись добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            UpdateTable();
            this.Tabs.Focus();
        }

        private DateTime Round(DateTime time)
        {
            return time.AddHours(1).Date.AddHours(time.Hour + 1);
        }


        private void RandomFillScreening(object sender, MouseButtonEventArgs e)
        {
            if(MessageBox.Show("При случайной генерации все существующие сеансы текущей даты будут удалены!\nВы согласны?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            
            RandomScreeningGenWindow win = new RandomScreeningGenWindow();
            win.ShowDialog();

            DateTime res = win.result;
            if(res == DateTime.Today.AddDays(-1))
            {
                MessageBox.Show("Сеансы не сгенерированы!", "Дата не была выбрана", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            connection.DeleteSpecificDateScreenings(res);
            List<Screenings> generatedScreenings = GenerateScreenings(res);

            List<Screenings> tmp = databaseScreenings.ToList();
            tmp.AddRange(generatedScreenings);
            databaseScreenings = new ObservableCollection<Screenings>(tmp);
            connection.InsertScreenings(generatedScreenings);

            UpdateTable();
            MessageBox.Show("Сеансы сгенерированы!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private List<Screenings> GenerateScreenings(DateTime date)
        {
            Random rand = new Random();
            List<Screenings> genScreen = new List<Screenings>();
            int id = connection.GetLastRecordIdFromSpecificTable("ScreeningID", "screenings");          

            foreach(Halls hall in halls)
            {
                DateTime start = date.AddHours(9);

                while(start < date.AddDays(1))
                {
                    Movie curMovie = movies[rand.Next(movies.Count)];
                    genScreen.Add(new Screenings(id, curMovie, hall, date, start, new List<Seat>(), 1));
                    id++;
                    start = Round(start.AddMinutes(curMovie.Duration));
                }
            }

            return genScreen;
                       
        }

        #endregion

        #region EditRegion

        private void SetTextFields(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void EditRecords(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region DeleteRegion

        private void DeleteRecord(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

using Org.BouncyCastle.Bcpg.OpenPgp;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CinemaTicketSeller.UserControls
{
    /// <summary>
    /// Логика взаимодействия для MovieUserControl.xaml
    /// </summary>
    public partial class MovieUserControl : UserControl
    {
        AdminConnection connection;

        ObservableCollection<Movie> databaseMovies = new ObservableCollection<Movie>();
        bool containsThisID = false;

        public MovieUserControl(AdminConnection connection)
        {            
            InitializeComponent();
            this.connection = connection;
            databaseMovies = new ObservableCollection<Movie>(connection.GetListOfMovies());
            this.idTextBox.Text = connection.GetLastRecordIdFromSpecificTable("MovieID", "movies").ToString();
            this.Table.ItemsSource = databaseMovies;
            SetupComboBox();

            this.addMovie.MouseLeftButtonDown += AddNewMovieClick;

            this.idTextBoxEdit.TextChanged += SetTextFields;
            this.ApplyChanges.MouseLeftButtonDown += EditRecordClick;
            this.DeleteMovie.MouseLeftButtonDown += DeleteRecordClick;
        }

        private void SetupComboBox()
        {
            string[] ageRatings = { "0+", "6+", "12+", "16+", "18+" };

            this.ageRatingComboBox.ItemsSource = ageRatings;
            this.ageRatingComboBoxEdit.ItemsSource = ageRatings;
        }

        private void UpdateTable()
        {
            this.Table.ItemsSource = databaseMovies.ToList();
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
            this.titleTextBox.BorderThickness = new Thickness(0);
            this.ageRatingComboBox.BorderThickness = new Thickness(0);
            this.durationTextBox.BorderThickness = new Thickness(0);
        }

        private void ClearFieldsAdd()
        {
            this.titleTextBox.Text = string.Empty;
            this.ageRatingComboBox.Text = string.Empty;
            this.durationTextBox.Text = string.Empty;
        }

        private void AddNewMovieClick(object sender, MouseButtonEventArgs e)
        {
            int id = Convert.ToInt32(this.idTextBox.Text);
            string title = this.titleTextBox.Text;
            string ageRating = this.ageRatingComboBox.Text;
            string tmpDur = this.durationTextBox.Text;
            int duration = 0;

            bool res = true;

            if (!CheckValue(tmpDur, "^[1-9][0-9]*$") || !int.TryParse(tmpDur, out duration))
            {
                res = false;
                this.durationTextBox.BorderBrush = Brushes.Red;
                this.durationTextBox.BorderThickness = new Thickness(2);
            }

            if(ageRating == string.Empty)
            {
                res = false;
                this.ageRatingComboBox.BorderBrush = Brushes.Red;
                this.ageRatingComboBox.BorderThickness = new Thickness(2);
                MessageBox.Show("Необходимо выбрать возрастное ограничение!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            if (!res)
            {
                MessageBox.Show("Ошибки форматирования полей ввода", "Проверьте введенные значения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RemoveSelectionAdd();
            Movie newMovie = new Movie(id, title, ageRating, duration);
            connection.InsertMovies([newMovie]);
            databaseMovies.Add(newMovie);
            this.idTextBox.Text = (id + 1).ToString();
            ClearFieldsAdd();
            MessageBox.Show("Новая запись добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            UpdateTable();
            this.Tabs.Focus();
        }

        #endregion


        #region EditRegion

        Movie movie = new Movie(0, "", "", 0);

        private void RemoveSelectionEdit()
        {
            this.TitleTextBoxEdit.BorderThickness = new Thickness(0);
            this.ageRatingComboBoxEdit.BorderThickness = new Thickness(0);
            this.durationTextBoxEdit.BorderThickness = new Thickness(0);
        }

        private void ClearFieldsEdit()
        {
            this.TitleTextBoxEdit.Text = string.Empty;
            this.ageRatingComboBoxEdit.Text = string.Empty;
            this.durationTextBoxEdit.Text = string.Empty;
        }

        private void SetTextFields(object sender, TextChangedEventArgs e)
        {
            containsThisID = false;
            string idText = this.idTextBoxEdit.Text;
            if (idText != "" && !Regex.IsMatch(idText, "^[[1-9][0-9]*$"))
            {
                idTextBoxEdit.Focus();
                idTextBoxEdit.SelectAll();
                return;
            }
            if (idText == "")
            {
                ClearFieldsEdit();
                return;
            }

            int id = Convert.ToInt32(idText);

            if (!databaseMovies.Any(m => m.MovieID == id))
            {
                ClearFieldsEdit();
                this.idTextBoxEdit.Focus();
                return;
            }
            containsThisID = true;
            LoadRecord(id);
        }

        private void LoadRecord(int id)
        {
            movie = databaseMovies.Where(m => m.MovieID == id).First();
            this.TitleTextBoxEdit.Text = movie.Title;
            this.ageRatingComboBoxEdit.Text = movie.AgeRating;
            this.durationTextBoxEdit.Text = movie.Duration.ToString();
        }

        private bool CheckIfThereAnyChanges(string title, string age, int dur)
        {
            return movie.Title == title && movie.AgeRating == age && movie.Duration == dur;
        }

        private void EditRecordClick(object sender, MouseButtonEventArgs e)
        {
            if (!IfContainsThisID()) return;

            int id = movie.MovieID;
            string title= this.TitleTextBoxEdit.Text;
            string ageRating = this.ageRatingComboBoxEdit.Text;
            string tmpDur = this.durationTextBoxEdit.Text;

            bool res = true;

            int duration = 0;

            if (!CheckValue(tmpDur, "^[1-9][0-9]*$") || !int.TryParse(tmpDur, out duration))
            {
                res = false;
                this.durationTextBoxEdit.BorderBrush = Brushes.Red;
                this.durationTextBoxEdit.BorderThickness = new Thickness(2);
            }

            if(ageRating == string.Empty)
            {
                res = false;
                this.ageRatingComboBoxEdit.BorderBrush = Brushes.Red;
                this.ageRatingComboBoxEdit.BorderThickness = new Thickness(2);
            }

            if (!res)
            {
                MessageBox.Show("Ошибки форматирования полей ввода!", "Проверьте введенные значения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (CheckIfThereAnyChanges(title, ageRating, duration))
            {
                MessageBox.Show("Изменений не было. Данные не будут обновлены", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            RemoveSelectionEdit();
            Movie upd = new Movie(id, title, ageRating, duration);
            movie = upd;
            connection.UpdateMoviesRecords([upd], [id]);

            for (int i = 0; i < databaseMovies.Count; i++)
                if (databaseMovies[i].MovieID == id)
                {
                    databaseMovies[i] = upd;
                    break;
                }

            UpdateTable();
            MessageBox.Show("Запись изменена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion


        #region DeleteRegion

        private void DeleteEnityFromArray(int id)
        {
            ObservableCollection<Movie> newList = new ObservableCollection<Movie>();
            foreach (Movie m in databaseMovies)
                if (m.MovieID != id) newList.Add(m);
            databaseMovies = newList;
        }

        private void DeleteRecordClick(object sender, MouseButtonEventArgs e)
        {
            if (!IfContainsThisID()) return;

            if (MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;

            int id = Convert.ToInt32(this.idTextBoxEdit.Text);
            connection.DeleteRecords("movies", "MovieID", [id]);
            DeleteEnityFromArray(id);
            UpdateTable();
            this.idTextBoxEdit.Text = string.Empty;
            MessageBox.Show("Запись удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            ClearFieldsEdit();
            this.Tabs.Focus();
        }

        #endregion
    }
}

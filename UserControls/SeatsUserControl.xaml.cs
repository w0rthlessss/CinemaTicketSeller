using Google.Protobuf.WellKnownTypes;
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
    /// Логика взаимодействия для SeatsUserControl.xaml
    /// </summary>
    public partial class SeatsUserControl : UserControl
    {
        AdminConnection connection;

        ObservableCollection<Seat> databaseSeats = new ObservableCollection<Seat>();
        List<Halls> halls = new List<Halls>();

        public SeatsUserControl(AdminConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            databaseSeats = new ObservableCollection<Seat>(connection.GetListOfSeats());
            //this.idTextBox.Text = connection.GetLastRecordIdFromSpecificTable("SeatID", "seats").ToString();
            halls = connection.GetListOfHalls();
            this.Table.ItemsSource = databaseSeats;
           /* SetupTypeComboBox();

            this.addSeat.MouseLeftButtonDown += AddNewSeatClick;

            this.idTextBoxEdit.TextChanged += SetTextFields;
            this.ApplyChanges.MouseLeftButtonDown += EditRecordClick;
            this.DeleteSeat.MouseLeftButtonDown += DeleteRecordClick;*/

        }

       /* private void SetupTypeComboBox()
        {
            string[] types = { "regular", "couch" };
            this.typeComboBox.ItemsSource = types;
            this.typeEditComboBox.ItemsSource = types;

            int[] hall = halls.Select(h => h.HallNumber).ToArray();
            this.hallComboBox.ItemsSource = hall;
            this.hallComboBoxEdit.ItemsSource = hall;
        }*/

        private void UpdateTable()
        {
            this.Table.ItemsSource = databaseSeats.ToList();
        }

        /*private bool CheckValue(string value, string regex)
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
            this.hallComboBox.BorderThickness = new Thickness(0);
            this.rowNumberTextBox.BorderThickness = new Thickness(0);
            this.seatNumberTextBox.BorderThickness = new Thickness(0);
            this.typeComboBox.BorderThickness = new Thickness(0);
            this.priceTextBox.BorderThickness = new Thickness(0);
        }

        private void ClearFieldsAdd()
        {
            this.hallComboBox.Text = string.Empty;
            this.rowNumberTextBox.Text = string.Empty;
            this.seatNumberTextBox.Text = string.Empty;
            this.typeComboBox.Text = string.Empty;
            this.priceTextBox.Text= string.Empty;
        }

        private void AddNewSeatClick(object sender, MouseButtonEventArgs e)
        {
            int id = Convert.ToInt32(this.idTextBox.Text);
            string tmpHallNumber = this.hallComboBox.Text;
            string tmpRowNumber = this.rowNumberTextBox.Text;
            string tmpSeatNumber = this.seatNumberTextBox.Text;
            string type = this.typeComboBox.Text;
            string tmpPrice = this.priceTextBox.Text;

            int hallNumber = 0;
            int rowNumber = 0;  
            int seatNumber = 0;
            int price = 0;

            bool res = true;

            if(!CheckValue(tmpHallNumber, "^[1-9][0-9]*$") || !int.TryParse(tmpHallNumber, out hallNumber))
            {
                res = false;
                this.hallComboBox.BorderBrush = Brushes.Red;
                this.hallComboBox.BorderThickness = new Thickness(2);
            }

            int hallID = halls.Where(h => h.HallNumber == hallNumber).First().HallID;

            if(!CheckValue(tmpRowNumber, "^[1-9][0-9]*$") || !int.TryParse(tmpRowNumber, out rowNumber))
            {
                res = false;
                this.rowNumberTextBox.BorderBrush = Brushes.Red;
                this.rowNumberTextBox.BorderThickness = new Thickness(2);
            }

            if(!CheckValue(tmpSeatNumber, "^[1-9][0-9]*$") || !int.TryParse(tmpSeatNumber, out seatNumber))
            {
                res = false;
                this.seatNumberTextBox.BorderBrush= Brushes.Red;
                this.seatNumberTextBox.BorderThickness= new Thickness(2);
            }

            if(!CheckValue(tmpPrice, "^[1-9][0-9]*$") || !int.TryParse (tmpPrice, out price))
            {
                res = false;
                this.priceTextBox.BorderBrush= Brushes.Red;
                this.priceTextBox.BorderThickness = new Thickness(2);
            }

            if(type == string.Empty)
            {
                res = false;
                this.typeComboBox.BorderBrush = Brushes.Red;
                this.typeComboBox.BorderThickness= new Thickness(2);
            }

            if (!res)
            {
                MessageBox.Show("Ошибки форматирования полей ввода", "Проверьте введенные значения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (halls.Where(h => h.HallID == hallID).First().Capacity <= databaseSeats.Where(s => s.HallID == hallID).Count())
            {
                MessageBox.Show("В данный зал больше невозможно добавить место", "Место закончилось", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Seat newSeat = new Seat(id, hallID, rowNumber, seatNumber, type, price);

            if (databaseSeats.FirstOrDefault(s => s.HallID == hallID && s.RowNumber == rowNumber && s.SeatNumber == seatNumber) != null)
            {
                MessageBox.Show("Место с такой позицией уже существует", "Такое место уже есть", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RemoveSelectionAdd();           

            connection.InsertSeats([newSeat]);
            databaseSeats.Add(newSeat);
            this.idTextBox.Text = (id + 1).ToString();
            ClearFieldsAdd();
            MessageBox.Show("Новая запись добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            UpdateTable();
            this.Tabs.Focus();
        }

        #endregion


        #region EditRegion

        private void SetTextFields(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void EditRecordClick(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion


        #region DeleteRegion

        private void DeleteRecordClick(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
*/

    }
}

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
using System.Xml.Linq;

namespace CinemaTicketSeller.UserControls
{
    /// <summary>
    /// Логика взаимодействия для HallUserControl.xaml
    /// </summary>
    public partial class HallUserControl : UserControl
    {
        AdminConnection connection;

        ObservableCollection<Halls> databaseHalls = new ObservableCollection<Halls>();
        bool containsThisID = false;

        public HallUserControl(AdminConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            databaseHalls = new ObservableCollection<Halls>(connection.GetListOfHalls());
            //this.idTextBox.Text = connection.GetLastRecordIdFromSpecificTable("HallID", "hall").ToString();
            this.Table.ItemsSource = databaseHalls;

           /* this.addHall.MouseLeftButtonDown += AddNewHallClick;

            this.idTextBoxEdit.TextChanged += SetTextFields;
            this.ApplyChanges.MouseLeftButtonDown += EditRecordClick;
            this.DeleteHall.MouseLeftButtonDown += DeleteRecordClick;*/
        }

        private void UpdateTable()
        {
            this.Table.ItemsSource = databaseHalls.ToList();
        }

       /* private bool CheckValue(string value, string regex)
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


        /// <summary>
        /// Методы для обработки добавления новой сущности в таблицу
        /// </summary>
        #region AddRegion

        private void RemoveSelectionAdd()
        {
            this.hallNumberTextBox.BorderThickness = new Thickness(0);
            this.capacityTextBox.BorderThickness = new Thickness(0);
        }

        private void ClearFieldsAdd()
        {
            this.hallNumberTextBox.Text = string.Empty;
            this.capacityTextBox.Text = string.Empty;
        }

        private void AddNewHallClick(object sender, MouseButtonEventArgs e)
        {
            int id = Convert.ToInt32(this.idTextBox.Text);
            string tmpNumber = this.hallNumberTextBox.Text;
            string tmpCapacity = this.capacityTextBox.Text;
            bool res = true;

            int number = 0;
            int capacity = 0;

            if (!CheckValue(tmpNumber, "^[1-9][0-9]*$") || !int.TryParse(tmpNumber, out number))
            {                
                res = false;
                this.hallNumberTextBox.BorderBrush = Brushes.Red;
                this.hallNumberTextBox.BorderThickness = new Thickness(2);
            }

            if(!CheckValue(tmpCapacity, "^[1-9][0-9]*$") || !int.TryParse(tmpCapacity, out capacity))
            {
                res = false;
                this.capacityTextBox.BorderBrush = Brushes.Red;
                this.capacityTextBox.BorderThickness = new Thickness(2);
            }

            if (!res)
            {
                MessageBox.Show("Ошибки форматирования полей ввода", "Проверьте введенные значения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (databaseHalls.FirstOrDefault(h => h.HallNumber == number) != null)
            {
                MessageBox.Show("В базе данных уже содержится зал с таким номером!", "Проверьте введенные значения", MessageBoxButton.OK, MessageBoxImage.Error);
                this.hallNumberTextBox.BorderBrush = Brushes.Red;
                this.hallNumberTextBox.BorderThickness = new Thickness(2);
                return;
            }

            RemoveSelectionAdd();
            Halls newHall = new Halls(id, number, capacity);
            connection.InsertHalls([newHall]);
            databaseHalls.Add(newHall);
            this.idTextBox.Text = (id + 1).ToString();
            ClearFieldsAdd();
            MessageBox.Show("Новая запись добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            UpdateTable();
            this.Tabs.Focus();
        }

        #endregion


        /// <summary>
        /// Методы для обработки изменения сущности в таблице
        /// </summary>
        #region EditRegion

        Halls hall = new Halls(0,0,0);

        private void ClearFieldsEdit()
        {
            this.hallNumberTextBoxEdit.Text = string.Empty;
            this.capacityTextBoxEdit.Text= string.Empty;
        }

        private void RemoveSelectionEdit()
        {
            this.hallNumberTextBoxEdit.BorderThickness = new Thickness(0);
            this.capacityTextBoxEdit.BorderThickness = new Thickness(0);
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

            if (!databaseHalls.Any(h => h.HallID == id))
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
            hall = databaseHalls.Where(h => h.HallID == id).First();
            this.hallNumberTextBoxEdit.Text = hall.HallNumber.ToString();
            this.capacityTextBoxEdit.Text = hall.Capacity.ToString();
        }

        private bool CheckIfThereAnyChanges(int number, int capacity)
        {
            return hall.HallNumber == number && hall.Capacity == capacity;
        }

        private void EditRecordClick(object sender, MouseButtonEventArgs e)
        {
            if (!IfContainsThisID()) return;

            int id = hall.HallID;
            string tmpNumber = this.hallNumberTextBoxEdit.Text;
            string tmpCapacity = this.capacityTextBoxEdit.Text;
            bool res = true;

            int number = 0;
            int capacity = 0;

            if (!CheckValue(tmpNumber, "^[1-9][0-9]*$") || !int.TryParse(tmpNumber, out number))
            {
                res = false;
                this.hallNumberTextBoxEdit.BorderBrush = Brushes.Red;
                this.hallNumberTextBoxEdit.BorderThickness = new Thickness(2);
            }

            if (!CheckValue(tmpCapacity, "^[1-9][0-9]*$") || !int.TryParse(tmpCapacity, out capacity))
            {
                res = false;
                this.capacityTextBoxEdit.BorderBrush = Brushes.Red;
                this.capacityTextBoxEdit.BorderThickness = new Thickness(2);
            }

            if (!res)
            {
                MessageBox.Show("Ошибки форматирования полей ввода!", "Проверьте введенные значения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }            

            if (CheckIfThereAnyChanges(number, capacity))
            {
                MessageBox.Show("Изменений не было. Данные не будут обновлены", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            RemoveSelectionEdit();
            Halls upd = new Halls(id, number, capacity);
            hall = upd;
            connection.UpdateHallsRecords([upd], [id]);

            for (int i = 0; i < databaseHalls.Count; i++)
                if (databaseHalls[i].HallID == id)
                {
                    databaseHalls[i] = upd;
                    break;
                }

            UpdateTable();
            MessageBox.Show("Запись изменена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion


        /// <summary>
        /// Методы для обработки удаления сущности из таблицы
        /// </summary>
        #region DeleteRegion

        private void DeleteEnityFromArray(int id)
        {
            ObservableCollection<Halls> newList = new ObservableCollection<Halls>();
            foreach (Halls h in databaseHalls)
                if (h.HallID != id) newList.Add(h);
            databaseHalls = newList;
        }

        private void DeleteRecordClick(object sender, MouseButtonEventArgs e)
        {
            if (!IfContainsThisID()) return;

            if (MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;

            int id = Convert.ToInt32(this.idTextBoxEdit.Text);
            connection.DeleteRecords("hall", "HallID", [id]);
            DeleteEnityFromArray(id);
            UpdateTable();
            this.idTextBoxEdit.Text = string.Empty;
            MessageBox.Show("Запись удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            ClearFieldsEdit();
            this.Tabs.Focus();
        }

        #endregion*/



        

        

        
    }
}

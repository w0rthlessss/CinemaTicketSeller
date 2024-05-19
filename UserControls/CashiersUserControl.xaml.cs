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

namespace CinemaTicketSeller
{
    /// <summary>
    /// Логика взаимодействия для CashiersUserControl.xaml
    /// </summary>
    public partial class CashiersUserControl : UserControl
    {
        AdminConnection connection;

        ObservableCollection<Cashier> databaseCashiers = new ObservableCollection<Cashier>();
        //List<Cashier> newCashiers = new List<Cashier>();
        List<Cashier> updatedCashiers = new List<Cashier>();
        List<int> updatedIDs = new List<int>();
        Cashier currentUser;
        bool containsThisID = false;

        public CashiersUserControl(AdminConnection connection, Cashier currentUser)
        {
            InitializeComponent();

            this.idTextBox.Text = connection.GetLastRecordIdFromSpecificTable("CashierID", "cashiers").ToString();
            this.currentUser = currentUser;
            this.connection = connection;            
            databaseCashiers = new ObservableCollection<Cashier>(connection.GetListOfCashiers());
            this.Table.ItemsSource = databaseCashiers;
            UpdateTable();
            this.isAdminCheckBox.Checked += WarnGrantingAdminRights;
            this.isAdminCheckBoxEdit.Unchecked += CheckRemovingAdmin;
            this.isAdminCheckBoxEdit.Checked += WarnGrantingAdminRights;

            this.addCashier.MouseLeftButtonDown += AddNewRecordClick;

            this.idTextBoxEdit.TextChanged += SetTextFields;
            this.ApplyChanges.MouseLeftButtonDown += EditRecordClick;
            this.Delete.MouseLeftButtonDown += DeleteRecordClick;
        }

        private void UpdateTable()
        {
            this.Table.ItemsSource = databaseCashiers.ToList();

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
            this.nameTextBox.BorderThickness = new Thickness(0);
            this.surnameTextBox.BorderThickness = new Thickness(0);
            this.loginTextBox.BorderThickness = new Thickness(0);
            this.passwordTextBox.BorderThickness = new Thickness(0);
        }

        private void ClearFieldsAdd()
        {
            this.nameTextBox.Text = string.Empty;
            this.surnameTextBox.Text = string.Empty;
            this.loginTextBox.Text = string.Empty;
            this.passwordTextBox.Text = string.Empty;
            this.isAdminCheckBox.IsChecked = false;
        }

        private void AddNewRecordClick(object sender, MouseButtonEventArgs e)
        {
            int id = Convert.ToInt32(this.idTextBox.Text);
            string name = this.nameTextBox.Text;
            string surname = this.surnameTextBox.Text;
            string login = this.loginTextBox.Text;
            string password = this.passwordTextBox.Text;
            bool isAdmin = this.isAdminCheckBox.IsChecked ?? false;

            bool res = true;

            if(!CheckValue(name, "^[a-zA-Zа-яА-Я]+$"))
            {
                res = false;
                this.nameTextBox.BorderBrush = Brushes.Red;
                this.nameTextBox.BorderThickness = new Thickness(2);
            }
            if(!CheckValue(surname, "^[a-zA-Zа-яА-Я]+$"))
            {
                res = false;
                this.surnameTextBox.BorderBrush = Brushes.Red;
                this.surnameTextBox.BorderThickness = new Thickness(2);
            }
            if (!CheckValue(login, "^(?!.*\\s).+$"))
            {
                res = false;
                this.loginTextBox.BorderBrush = Brushes.Red;
                this.loginTextBox.BorderThickness = new Thickness(2);
            }
            if (!CheckValue(password, "^(?!.*\\s).+$"))
            {
                res = false;
                this.passwordTextBox.BorderBrush = Brushes.Red;
                this.passwordTextBox.BorderThickness = new Thickness(2);
            }

            if (!res)
            {
                MessageBox.Show("Ошибки форматирования полей ввода", "Проверьте введенные значения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RemoveSelectionAdd();
            Cashier newCashier = new Cashier(id, name, surname, login, password, isAdmin);
            connection.InsertCasheirs([newCashier]);
            databaseCashiers.Add(newCashier);
            this.idTextBox.Text = (id + 1).ToString();
            ClearFieldsAdd();
            MessageBox.Show("Новая запись добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            UpdateTable();
            this.Tabs.Focus();
        }

        #endregion

        #region EditRegion

        Cashier c = new Cashier(0, "", "", "", "", false);

        private void ClearFieldsEdit()
        {
            this.nameTextBoxEdit.Text = string.Empty;
            this.surnameTextBoxEdit.Text= string.Empty;
            this.loginTextBoxEdit.Text = string.Empty;
            this.passwordTextBoxEdit.Text = string.Empty;
            this.isAdminCheckBoxEdit.IsChecked = false;
        }

        private void RemoveSelectionEdit()
        {
            this.nameTextBoxEdit.BorderThickness = new Thickness(0);
            this.surnameTextBoxEdit.BorderThickness = new Thickness(0);
            this.loginTextBoxEdit.BorderThickness = new Thickness(0);
            this.passwordTextBoxEdit.BorderThickness = new Thickness(0);
        }

        private void SetTextFields(object sender, TextChangedEventArgs e)
        {
            containsThisID = false;
            string idText = this.idTextBoxEdit.Text;
            if(idText != "" && !Regex.IsMatch(idText, "^[[1-9][0-9]*$"))
            {
                idTextBoxEdit.Focus();
                idTextBoxEdit.SelectAll();
                return;
            }
            if(idText == "")
            {
                ClearFieldsEdit();
                return;
            }

            int id = Convert.ToInt32(idText);

            if(!databaseCashiers.Any(cashier => cashier.CashierID == id))
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
            c = databaseCashiers.Where(cas => cas.CashierID == id).First();
            this.nameTextBoxEdit.Text = c.FirstName;
            this.surnameTextBoxEdit.Text = c.LastName;
            this.loginTextBoxEdit.Text = c.Username;
            this.passwordTextBoxEdit.Text = c.Password;
            this.isAdminCheckBoxEdit.IsChecked = c.IsAdmin;
        }

        private void EditRecordClick(object sender, MouseButtonEventArgs e)
        {
            if (!IfContainsThisID()) return;

            int id = Convert.ToInt32(this.idTextBoxEdit.Text);
            string name = this.nameTextBoxEdit.Text;
            string surname = this.surnameTextBoxEdit.Text;
            string login = this.loginTextBoxEdit.Text;
            string password = this.passwordTextBoxEdit.Text;
            bool isAdmin = this.isAdminCheckBoxEdit.IsChecked ?? false;

            bool res = true;

            if(!CheckValue(name, "^[a-zA-Zа-яА-Я]+$"))
            {
                res = false;
                this.nameTextBoxEdit.BorderBrush = Brushes.Red;
                this.nameTextBoxEdit.BorderThickness = new Thickness(2);
            }

            if (!CheckValue(surname, "^[a-zA-Zа-яА-Я]+$"))
            {
                res = false;
                this.surnameTextBoxEdit.BorderBrush = Brushes.Red;
                this.surnameTextBoxEdit.BorderThickness = new Thickness(2);
            }

            if (!CheckValue(login, "^(?!.*\\s).+$"))
            {
                res = false;
                this.loginTextBoxEdit.BorderBrush = Brushes.Red;
                this.loginTextBoxEdit.BorderThickness = new Thickness(2);
            }
            if (!CheckValue(password, "^(?!.*\\s).+$"))
            {
                res = false;
                this.passwordTextBoxEdit.BorderBrush = Brushes.Red;
                this.passwordTextBoxEdit.BorderThickness = new Thickness(2);
            }

            if (!res)
            {
                MessageBox.Show("Ошибки форматирования полей ввода", "Проверьте введенные значения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (CheckIfThereAnyChanges(
                this.nameTextBoxEdit.Text,
                this.surnameTextBoxEdit.Text,
                this.loginTextBoxEdit.Text,
                this.passwordTextBoxEdit.Text,
                this.isAdminCheckBoxEdit.IsChecked ?? false))
            {
                MessageBox.Show("Изменений не было. Данные не будут обновление", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            RemoveSelectionEdit();
            Cashier upd = new Cashier(id, name, surname, login, password, isAdmin);
            connection.UpdateCashiersRecords([upd], [id]);

            for(int i =0; i < databaseCashiers.Count; i++)
                if (databaseCashiers[i].CashierID == id) databaseCashiers[i] = upd;

            UpdateTable();
            MessageBox.Show("Запись изменена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool CheckIfThereAnyChanges(string name, string surname, string login, string password, bool isAdmin)
        {
            return c.FirstName == name && c.LastName == surname && c.Username == login && c.Password == password && c.IsAdmin == isAdmin;
        }
        #endregion

        #region DeleteRegion

        private void DeleteEnityFromArray(int id)
        {
            ObservableCollection<Cashier> newList = new ObservableCollection<Cashier>();
            foreach (Cashier c in databaseCashiers)            
                if(c.CashierID != id) newList.Add(c);    
            databaseCashiers = newList;
        }

        private void DeleteRecordClick(object sender, MouseButtonEventArgs e)
        {
            if(!IfContainsThisID()) return;

            if (MessageBox.Show("Вы уверены, что хотите удалить эту запись?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;

            int id = Convert.ToInt32(this.idTextBoxEdit.Text);
            connection.DeleteRecords("cashiers", "CashierID", [id]);
            DeleteEnityFromArray(id);
            UpdateTable();
            this.idTextBoxEdit.Text = string.Empty;
            MessageBox.Show("Запись удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            ClearFieldsEdit();
            this.Tabs.Focus();
        }

        #endregion

        #region FieldFormatting               

        bool isUserChange = true;

        private void CheckRemovingAdmin(object sender, RoutedEventArgs e)
        {
            if (!isUserChange) return;
            if(databaseCashiers.Where(c => c.IsAdmin == true).Count() == 1 || databaseCashiers.Where(c => c.CashierID == Convert.ToInt32(this.idTextBoxEdit.Text)).First().CashierID == currentUser.CashierID)
            {
                MessageBox.Show("Вы не можете забрать права администратора у этого пользователя", "CinemaAdmin", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = false;
                return;
            }
            if (MessageBox.Show("Вы уверены, что хотите забрать права администратора у этого пользователя?", "CinemaAdmin", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                e.Handled = true;
            }
            else e.Handled = false;

        }

        private void WarnGrantingAdminRights(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите дать права администратора этому пользователю?", "CinemaAdmin", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
                isUserChange = false;
                this.isAdminCheckBoxEdit.IsChecked = false;
                isUserChange = true;
            }
        }

        #endregion
    }
}

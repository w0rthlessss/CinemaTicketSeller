using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CinemaTicketSeller
{
    /// <summary>
    /// Логика взаимодействия для AuthenticationWindow.xaml
    /// </summary>
    public partial class AuthenticationWindow : Window
    {
        public bool result = false;

        public AuthenticationWindow()
        {
            InitializeComponent();
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            this.Submit.Cursor = Cursors.Hand;            
            this.Submit.MouseEnter += HoverBtnEnterEvent;
            this.Submit.MouseLeave += HoverBtnLeaveEvent;
            this.Submit.MouseLeftButtonDown += ClickBtnEvent;
            this.Submit.MouseLeftButtonUp += ClickBtnEndEvent;
            this.Login.GotFocus += TextBoxClickEvent;
            this.Password.GotFocus += PasswordBoxClickEvent;
            this.Login.LostFocus += TextBoxClickEndEvent;
            this.Password.LostFocus += PasswordBoxClickEndEvent;
        }

        private void PasswordBoxClickEndEvent(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = (PasswordBox)sender;
            if (pb.Password != string.Empty) return;
            pb.Password = "        ";
        }

        private void PasswordBoxClickEvent(object sender, RoutedEventArgs e)
        {
            PasswordBox pb = (PasswordBox)sender;
            if(pb.Password == "        ") pb.Password = string.Empty;
        }

        private void TextBoxClickEndEvent(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text != string.Empty) return;
            tb.Text = "Введите Логин";
        }

        private void TextBoxClickEvent(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if(tb.Text == "Введите Логин")
                tb.Text = string.Empty;
        }

        private void ClickBtnEndEvent(object sender, MouseButtonEventArgs e)
        {
            this.Submit.Background = (Brush)new BrushConverter().ConvertFromString("#38A0E8")!;
        }

        private void HoverBtnLeaveEvent(object sender, MouseEventArgs e)
        {
            this.Submit.Background = (Brush)new BrushConverter().ConvertFromString("#38A0E8")!;
        }

        private void ClickBtnEvent(object sender, MouseButtonEventArgs e)
        {
            this.Submit.Background = (Brush)new BrushConverter().ConvertFromString("#3590CF")!;
            Confirmation();
        }

        private void HoverBtnEnterEvent(object sender, MouseEventArgs e)
        {
            this.Submit.Background = (Brush)new BrushConverter().ConvertFromString("#54B0F0")!;
        }

        private async void Confirmation()
        {
            this.ErrorMsg.Content = string.Empty;

            RequestConnection con = new RequestConnection();
            string username = this.Login.Text;
            string password = this.Password.Password;
            
            if(username == "Введите Логин" || string.IsNullOrWhiteSpace(password))
            {
                this.ErrorMsg.Content = "Введите и логин и пароль!";
                return;
            }

            try
            {
                Cashier cashier = await con.FindCashierAsync(username, password);
                if (cashier.IsNull())
                {
                    this.ErrorMsg.Content = "Неверный логин или пароль!";
                    return;
                }

                con.CloseConnection();

                this.Hide();
                MainWindow mw = new MainWindow(cashier);
                mw.Show();
                this.Close();
            }
            catch
            {
                MessageBox.Show("Что-то пошло не так при подключении к базе данных!", "Ошибка");
                this.Close();
            }            
            
        }

    }
}

using CinemaTicketSeller.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.Foundation.Collections;

namespace CinemaTicketSeller
{
    /// <summary>
    /// Логика взаимодействия для AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Window
    {

        public void InitializeButtonActions()
        {
            this.CashiersBorder.MouseLeftButtonDown += ShowMdiChild;
            this.HallsBorder.MouseLeftButtonDown += ShowMdiChild;
            this.MoviesBorder.MouseLeftButtonDown += ShowMdiChild;
            this.ScreeningsBorder.MouseLeftButtonDown += ShowMdiChild;
            this.SeatsBorder.MouseLeftButtonDown += ShowMdiChild;
            this.ReciptsBorder.MouseLeftButtonDown += ShowMdiChild;
            this.TicketsBorder.MouseLeftButtonDown += ShowMdiChild;
        }

        private void ShowMdiChild(object sender, MouseButtonEventArgs e)
        {
            Border currentButton = (Border)sender;    
            UserControl mdiChild;
            switch (currentButton.Name)
            {
                case "CashiersBorder":
                    mdiChild = new CashiersUserControl(connection, admin);
                    break;
                case "HallsBorder":
                    mdiChild = new HallUserControl(connection);
                    break;
                case "MoviesBorder":
                    mdiChild = new MovieUserControl(connection);
                    break;
                case "ScreeningsBorder":
                    mdiChild = new ScreeningUserControl(connection);
                    break;
                case "SeatsBorder":
                    mdiChild = new SeatsUserControl(connection);
                    break;
                case "TicketsBorder":
                    mdiChild = new TicketUserControl(connection);
                    break;
                case "ReciptsBorder":
                    mdiChild = new ReciptsUserControl(connection);
                    break;

                //не достигнет
                default:
                    mdiChild = new UserControl();
                    break;
            }

            this.ChildControl.Content = mdiChild;
        }

        AdminConnection connection;
        Cashier admin;
        public AdminPanel(Cashier admin)
        {
            InitializeComponent();
            this.admin = admin;
            connection = new AdminConnection();
            connection.OpenConnection();
            InitializeButtonActions();
            this.Closing += OnClosing;
        }

        private void OnClosing(object? sender, CancelEventArgs e)
        {
            connection.CloseConnection();
        }
    }
}

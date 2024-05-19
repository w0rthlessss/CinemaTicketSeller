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

        List<Seat> databaseSeats = new List<Seat>();
        List<Seat> newSeats = new List<Seat>();
        List<Seat> updatedSeats = new List<Seat>();
        List<int> updatedIDs = new List<int>();

        public SeatsUserControl(AdminConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            databaseSeats = connection.GetListOfSeats();
            this.Table.ItemsSource = databaseSeats;

        }
    }
}

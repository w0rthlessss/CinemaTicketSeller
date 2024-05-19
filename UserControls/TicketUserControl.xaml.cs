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
    /// Логика взаимодействия для TicketUserControl.xaml
    /// </summary>
    public partial class TicketUserControl : UserControl
    {
        AdminConnection connection;

        List<Ticket> databaseTickets = new List<Ticket>();
        List<Ticket> newTickets = new List<Ticket>();
        List<Ticket> updatedTickets = new List<Ticket>();
        List<int> updatedIDs = new List<int>();

        public TicketUserControl(AdminConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            databaseTickets = connection.GetListOfTickets();
            this.Table.ItemsSource = databaseTickets;

        }
    }
}

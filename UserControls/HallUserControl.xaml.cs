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
    /// Логика взаимодействия для HallUserControl.xaml
    /// </summary>
    public partial class HallUserControl : UserControl
    {
        AdminConnection connection;

        List<Halls> databaseHalls = new List<Halls>();
        List<Halls> newHalls = new List<Halls>();
        List<Halls> updatedHalls = new List<Halls>();
        List<int> updatedIDs = new List<int>();

        public HallUserControl(AdminConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            databaseHalls = connection.GetListOfHalls();
            this.Table.ItemsSource = databaseHalls;

        }
    }
}

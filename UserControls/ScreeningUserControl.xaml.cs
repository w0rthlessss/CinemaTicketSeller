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
    /// Логика взаимодействия для ScreeningUserControl.xaml
    /// </summary>
    public partial class ScreeningUserControl : UserControl
    {
        AdminConnection connection;

        List<Screenings> databaseScreenings = new List<Screenings>();
        List<Screenings> newScreenings = new List<Screenings>();
        List<Screenings> updatedScreenings = new List<Screenings>();
        List<int> updatedIDs = new List<int>();

        public ScreeningUserControl(AdminConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            databaseScreenings = connection.GetListOfScreenings();
            this.Table.ItemsSource = databaseScreenings;

        }
    }
}

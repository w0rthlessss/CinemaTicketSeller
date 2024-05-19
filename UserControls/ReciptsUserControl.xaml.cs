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
    /// Логика взаимодействия для ReciptsUserControl.xaml
    /// </summary>
    public partial class ReciptsUserControl : UserControl
    {
        AdminConnection connection;

        List<Recipt> databaseRecipts = new List<Recipt>();
        List<Recipt> newRecipts = new List<Recipt>();
        List<Recipt> updatedRecipts = new List<Recipt>();
        List<int> updatedIDs = new List<int>();

        public ReciptsUserControl(AdminConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            databaseRecipts = connection.GetListOfRecipts();
            this.Table.ItemsSource = databaseRecipts;

        }
    }
}

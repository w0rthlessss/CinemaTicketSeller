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
    /// Логика взаимодействия для MovieUserControl.xaml
    /// </summary>
    public partial class MovieUserControl : UserControl
    {
        AdminConnection connection;

        List<Movie> databaseMovies = new List<Movie>();
        List<Movie> newMovies = new List<Movie>();
        List<Movie> updatedMovies = new List<Movie>();
        List<int> updatedIDs = new List<int>();

        public MovieUserControl(AdminConnection connection)
        {
            InitializeComponent();
            this.connection = connection;
            databaseMovies = connection.GetListOfMovies();
            this.Table.ItemsSource = databaseMovies;

        }
    }
}

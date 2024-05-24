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
using System.Windows.Shapes;

namespace CinemaTicketSeller.UserControls
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class RandomScreeningGenWindow : Window
    {
        public DateTime result = DateTime.Today.AddDays(-1);
        DateTime selected;
        public RandomScreeningGenWindow()
        {
            InitializeComponent();
            this.apply.MouseLeftButtonDown += Apply;
            this.apply.Cursor = Cursors.Hand;
        }

        private void Apply(object sender, MouseButtonEventArgs e)
        {
            selected = this.datePicker.SelectedDate ?? result;

            if(selected < DateTime.Today)
            {
                MessageBox.Show("Вы не можете выбрать дату в прошлом!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            result = selected;
            this.Close();
        }
    }
}

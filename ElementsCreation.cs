using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CinemaTicketSeller
{
    internal static class ElementsCreation
    {
        public static void CreateScreeningsButtons(List<Screenings> databaseScreenings, ref List<Border> screeningsButtons)
        {      
            screeningsButtons.Clear();            

            Random random = new Random();
            for (int i = 0; i < databaseScreenings.Count; i++)
            {
                screeningsButtons.Add(new Border());
                screeningsButtons[i].Background = (Brush)(new BrushConverter().ConvertFromString("#38A0E8")!);
                screeningsButtons[i].Margin = new Thickness(3);
                DockPanel.SetDock(screeningsButtons[i], Dock.Top);
                DockPanel dockPanelMain = new DockPanel();
                DockPanel dockPaneChild = new DockPanel();
                dockPaneChild.LastChildFill = false;
                DockPanel.SetDock(dockPaneChild, Dock.Top);

                Label hall = new Label();
                hall.Content = $"Зал #{databaseScreenings[i].Hall.HallNumber}";
                hall.FontSize = 14;
                hall.Foreground = Brushes.White;
                DockPanel.SetDock(hall, Dock.Right);

                Label time = new Label();
                time.Content = $"{databaseScreenings[i].Time.ToString("HH:mm:ss")}";
                time.FontSize = 14;
                time.Foreground = Brushes.White;

                dockPaneChild.Children.Add(hall);
                dockPaneChild.Children.Add(time);

                Label title = new Label();
                title.Content = $"{databaseScreenings[i].Movie.Title}";
                title.FontSize = 14;
                title.Foreground = Brushes.White;

                dockPanelMain.Children.Add(dockPaneChild);
                dockPanelMain.Children.Add(title);
                screeningsButtons[i].Child = dockPanelMain;
                screeningsButtons[i].Cursor = Cursors.Hand;

            }
        }
                
        public static void CreateCalendarButtons(ref DateTime[] dates, ref List<Border> dateButtons)
        {
            DateTime today = DateTime.Today;            
            int difference = ((int)today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - (int)DayOfWeek.Monday);
            string[] days = ["ПН", "ВТ", "СР", "ЧТ", "ПТ", "СБ", "ВС"];

            for (int i = 0; i < 14; i++)
            {
                dateButtons.Add(new Border());
                dateButtons[i].Background = (Brush)(new BrushConverter().ConvertFromString("#595959")!);
                DockPanel dockPanel = new DockPanel();
                dates[i] = today.AddDays(-difference + i);

                Label day = new Label();
                day.Content = days[i % 7];
                day.Foreground = Brushes.White;
                day.HorizontalAlignment = HorizontalAlignment.Center;
                day.FontSize = 16;

                Label date = new Label();
                date.Content = dates[i].ToString("dd.MM");
                date.Foreground = Brushes.White;
                date.HorizontalAlignment = HorizontalAlignment.Center;
                date.FontSize = 12;

                DockPanel.SetDock(day, Dock.Top);
                dockPanel.Children.Add(day);
                DockPanel.SetDock(date, Dock.Bottom);
                dockPanel.Children.Add(date);
                dockPanel.VerticalAlignment = VerticalAlignment.Center;

                dateButtons[i].Child = dockPanel;
                dateButtons[i].Margin = new Thickness(5);

                if(i < ((int)DateTime.Today.DayOfWeek + 6) % 7)
                {
                    foreach (Label label in dockPanel.Children)
                        label.Foreground = Brushes.Gray;
                }

            }
        }

        public static void CreateSeatButtons(List<Seat> databaseSeats, ref List<Image> seatButtons)
        {
            seatButtons.Clear();

            for(int i = 0; i < databaseSeats.Count; i++)
            {
                seatButtons.Add(new Image());
                BitmapImage bitmapImage = new BitmapImage();                

                if (databaseSeats[i].Type == "regular")
                {
                    if (databaseSeats[i].IsOccupied)
                        bitmapImage = new BitmapImage(new Uri(@"/img/regSeatOccupied.png", UriKind.Relative));
                    else
                        bitmapImage = new BitmapImage(new Uri(@"/img/regSeat.png", UriKind.Relative));                    
                }  
                    
                else
                {                 
                    if (databaseSeats[i].IsOccupied)
                        bitmapImage = new BitmapImage(new Uri(@"/img/couchSingleOccupied.png", UriKind.Relative));
                    else
                        bitmapImage = new BitmapImage(new Uri(@"/img/couchSingle.png", UriKind.Relative));
                    
                }

                seatButtons[i].Margin = new Thickness(1);
                seatButtons[i].Source = bitmapImage;

                if (!databaseSeats[i].IsOccupied)
                    seatButtons[i].Cursor = Cursors.Hand;

            }
        }
    
        public static void ChangeSeatIcon(Seat seat, int indexOfSelectedSeat, ref List<Image> seatButtons, bool isSelected)
        {
            if (seat.Type == "regular")
            {
                if (isSelected)
                    seatButtons[indexOfSelectedSeat].Source = new BitmapImage(new Uri(@"/img/regSeatChosen.png", UriKind.Relative));
                else
                    seatButtons[indexOfSelectedSeat].Source = new BitmapImage(new Uri(@"/img/regSeat.png", UriKind.Relative));
            }
            else
            {
                if(isSelected)
                    seatButtons[indexOfSelectedSeat].Source = new BitmapImage(new Uri(@"/img/couchSingleChosen.png", UriKind.Relative));
                else
                    seatButtons[indexOfSelectedSeat].Source = new BitmapImage(new Uri(@"/img/couchSingle.png", UriKind.Relative));
            }
        }

        public static DockPanel CreateTicketPlaceInformation(int hallNumber, int price, Seat seat)
        {
            Label chosenSeat = new Label();
            Label seatPrice = new Label();

            chosenSeat.Content = $"Зал: {hallNumber}  Ряд: {seat.RowNumber}  Место: {seat.SeatNumber}";
            seatPrice.Content = $"{price}";

            chosenSeat.FontSize = 12;
            seatPrice.FontSize = 12;

            DockPanel.SetDock(seatPrice, Dock.Right);
            
            DockPanel dock = new DockPanel();
            dock.Children.Add(chosenSeat);
            dock.Children.Add(seatPrice);
            DockPanel.SetDock(dock, Dock.Top);

            return dock;
        }
    
        public static void SetSeatButtonToOccupied(Seat seat, int indexOfSelectedSeat, ref List<Image> seatButtons)
        {
            if (seat.Type == "regular")
            {
                seatButtons[indexOfSelectedSeat].Source = new BitmapImage(new Uri(@"/img/regSeatOccupied.png", UriKind.Relative));
            }
            else
            {               
               seatButtons[indexOfSelectedSeat].Source = new BitmapImage(new Uri(@"/img/couchSingleOccupied.png", UriKind.Relative));
            }

            seatButtons[indexOfSelectedSeat].Cursor = Cursors.Arrow;
        }
    }
}

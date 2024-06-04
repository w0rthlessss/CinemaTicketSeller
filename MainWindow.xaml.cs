using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CinemaTicketSeller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Border> dateButtons = new List<Border>();
        List<Border> screeningsButtons = new List<Border>();
        List<Image> seatButtons = new List<Image>();
        List<DockPanel> chosenSeats = new List<DockPanel>();

        int ticketsSum = 0;
        int currentScreeningIndex = 0;

        Border selectedDate = new Border();
        Border selectedScreening = new Border();
        DateTime[] dates = new DateTime[14];

        List<Screenings> databaseScreenings = new List<Screenings>();
        List<Seat> databaseSeats = new List<Seat>();
        List<Seat> selectedSeats = new List<Seat>();
        List<Ticket> tickets = new List<Ticket>();
        Recipt recipt = new Recipt(0, 0, new DateTime(), new DateTime(), new List<Ticket>(), 0);

        int calendarButtonIndex = 0;
        Cashier cashier;
        private DispatcherTimer timer;
        CashierConnection connection;

        public MainWindow(Cashier cashier)
        {
            InitializeComponent();

            this.cashier = cashier;
            connection = new CashierConnection();
            connection.OpenConnection();
            this.Closing += MainWindow_Closing;                       

            if (cashier.IsAdmin)
            {
                this.Logo.MouseLeftButtonDown += OpenAdminPanel;
                this.Logo.Cursor = Cursors.Hand;
                this.Logo.ToolTip = "Открыть панель администрирования";
            }

            ElementsCreation.CreateCalendarButtons(ref dates, ref dateButtons);
            DisplayCurrentWeekCalendarButtons();
            SelectDateEvent(dateButtons[((int)DateTime.Today.DayOfWeek + 6) % 7 ], null);
            SetupWeekArrows();
            DisplayCurrentDateScreeningsButtons(dateButtons[((int)DateTime.Today.DayOfWeek + 6) % 7], null);

            this.CurrentUserName.Content = $"{cashier.LastName} {cashier.FirstName}";
            timer = new DispatcherTimer();
            SetupDateTime();
            EnablePaymentButtons();
        }

        private void OpenAdminPanel(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            connection.CloseConnection();
            AdminPanel admin = new AdminPanel(cashier);
            admin.ShowDialog();
            this.Show();
            connection.OpenConnection();
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            if(selectedSeats.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Есть неоплаченные места. Вы точно хотите закрыть приложение?", "Внимание", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)                
                    connection.CloseConnection();
                                    
                else
                    e.Cancel = true;
                
            }
        }

        private void EnablePaymentButtons()
        {
            this.Card.Cursor = Cursors.Hand;
            this.Cash.Cursor = Cursors.Hand;

            this.Card.MouseLeftButtonDown += Pay;
            this.Cash.MouseLeftButtonDown += Pay;
      
        }

        private void Pay(object sender, MouseButtonEventArgs e)
        {
            Border button = (Border)sender;
            button.Background = (Brush)(new BrushConverter().ConvertFromString("#32C13B")!);
            button.Background = (Brush)(new BrushConverter().ConvertFromString("#39D642")!);

            if (selectedSeats.Count == 0) return;
            

            MessageBoxResult reply = MessageBox.Show("Оплата прошла?", this.Name, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (reply == MessageBoxResult.Yes)
            {
                Screenings currentScreening = databaseScreenings[screeningsButtons.IndexOf(selectedScreening)];
                DeselectSeats(currentScreening);

                //добавление чеков и билетов в бд
                CreateCurrentPaymentTickets(currentScreening);
                CreateRecipt();
                connection.AddTicketsToDatabase(tickets);
                connection.AddReciptsToDatabase(recipt);                

                selectedSeats.Clear();
                chosenSeats.Clear();
                tickets.Clear();
                ticketsSum = 0;
                ChangeTotalSum();
                this.PlaceInformation.Children.Clear();
            }

            
        }

        //Попробовать сделать асинхронное подключение в этой фунции

        private void CreateRecipt()
        {
            int id = connection.GetLastRecordIdFromSpecificTable("ReciptID", "recipts");

            recipt = new Recipt(id, cashier.CashierID, DateTime.Now, DateTime.Now, tickets, ticketsSum);
        }

        private void CreateCurrentPaymentTickets(Screenings currentScreening)
        {
            int id = connection.GetLastRecordIdFromSpecificTable("TicketID", "tickets");

            foreach (Seat seat in selectedSeats)
            {
                int price = Convert.ToInt32(Convert.ToDouble(seat.Price) * currentScreening.PriceAmplification);
                Ticket ticket = new Ticket(id, currentScreening.ScreenID, seat.SeatID, price);
                tickets.Add(ticket);
                id++;
            }
        }

        private void DeselectSeats(Screenings currentScreening)
        {            
            for (int i = 0; i < selectedSeats.Count; i++)
            {
                int seatIndex = databaseSeats.IndexOf(selectedSeats[i]);
                ElementsCreation.SetSeatButtonToOccupied(databaseSeats[seatIndex], seatIndex, ref seatButtons);
                seatButtons[seatIndex].MouseLeftButtonDown -= SelectSeatEvent;
                selectedSeats[i].IsOccupied = true;
                currentScreening.OccupiedSeats.Add(selectedSeats[i]);
            }
            string json = JsonConvert.SerializeObject(currentScreening.OccupiedSeats);
            connection.UpdateOccupiedSeats(json, currentScreening.ScreenID);
        }

        private void DisplayCurrentDateScreeningsButtons(object sender, MouseButtonEventArgs? e)
        {            
            this.Sessions.Children.Clear();
            int dateIndex = dateButtons.IndexOf((Border)sender);

            databaseScreenings.Clear();
            databaseScreenings = connection.GetCurrentDateScreeningsFromDatabase(dates[dateIndex]);

            ElementsCreation.CreateScreeningsButtons(databaseScreenings, ref screeningsButtons);
            for (int i = 0; i < screeningsButtons.Count; i++)
            {
                screeningsButtons[i].MouseLeftButtonDown += SelectScreeningEvent;
                this.Sessions.Children.Add(screeningsButtons[i]);
            }
                
        }

        private void SelectScreeningEvent(object sender, MouseButtonEventArgs e)
        {
            Border currentScreeningButton = (Border)sender;
            if (currentScreeningButton == selectedScreening) return;

            selectedScreening = currentScreeningButton;

            currentScreeningIndex = screeningsButtons.IndexOf(selectedScreening);
            Screenings currentScreening = databaseScreenings[currentScreeningIndex];
            currentScreeningIndex = databaseScreenings.IndexOf(currentScreening);

            this.PlaceInformation.Children.Clear();
            selectedSeats.Clear();
            chosenSeats.Clear();

            databaseSeats.Clear();
            databaseSeats = connection.GetSeatsForCurrnetHallAndSession(currentScreening);


            databaseScreenings[currentScreeningIndex].Hall.Seats = databaseSeats;

            FillSessionPanel(currentScreening);
            CreateSeatButtons(currentScreening);
            FillPaymentPanelMovieData(currentScreening);
        }

        private void FillPaymentPanelMovieData(Screenings currentScreening)
        {
            this.TicketDate.Content = $"Дата: {currentScreening.Date.ToString("dd.MM.yyyy")}";
            this.TicketTimeAndMovie.Content = currentScreening.Time.ToString("HH:mm") +'\t'+ currentScreening.Movie.Title;            
        }

        private void FillSessionPanel(Screenings currentScreening)
        {
            this.AgeRating.Content = currentScreening.Movie.AgeRating;
            this.MovieName.Content = currentScreening.Movie.Title;
            this.SessionDate.Content = currentScreening.Date.ToString("dd.MM");
            this.SessionTime.Content = currentScreening.Time.ToString("HH:mm");
            this.SessionHall.Content = $"Зал: {currentScreening.Hall.HallNumber}";
            this.CommonSeatPrice.Content = (currentScreening.Hall.Seats.First().Price * currentScreening.PriceAmplification).ToString();
            this.CouchSeatPrice.Content = (currentScreening.Hall.Seats.Last().Price * currentScreening.PriceAmplification).ToString();
        }

        private void CreateSeatButtons(Screenings currentScreening)
        {
            this.SeatGrid.Children.Clear();
            
            currentScreening.Hall.Seats = databaseSeats;
            ElementsCreation.CreateSeatButtons(databaseSeats, ref seatButtons);

            this.SeatGrid.Rows = currentScreening.Hall.Seats.OrderByDescending(h => h.RowNumber).First().RowNumber + 1;
            this.SeatGrid.Columns = currentScreening.Hall.Capacity / this.SeatGrid.Rows;

            for(int i = 0; i < SeatGrid.Rows; i++)
            {
                for(int j = 0; j < SeatGrid.Columns; j++)
                {
                    if (!databaseSeats[i * this.SeatGrid.Columns + j].IsOccupied)
                        seatButtons[i * this.SeatGrid.Columns + j].MouseLeftButtonDown += SelectSeatEvent;
                    seatButtons[i * this.SeatGrid.Columns + j].ToolTip = $"Ряд {i} Место {j}";
                    
                    this.SeatGrid.Children.Add(seatButtons[i * this.SeatGrid.Columns + j]);
                    Grid.SetColumn(seatButtons[i * this.SeatGrid.Columns + j], j);
                    Grid.SetRow(seatButtons[i * this.SeatGrid.Columns + j], i);
                    if(databaseSeats[i * this.SeatGrid.Columns + j].Type == "couch")
                    {
                        if(j % 2 == 0)
                            seatButtons[i * this.SeatGrid.Columns + j].HorizontalAlignment = HorizontalAlignment.Right;
                        else
                            seatButtons[i * this.SeatGrid.Columns + j].HorizontalAlignment = HorizontalAlignment.Left;

                    }
                }
            }
        }

        private void SelectSeatEvent(object sender, MouseButtonEventArgs e)
        {
            Image selectedSeat = (Image)sender;
            int seatIndex = seatButtons.IndexOf(selectedSeat);

            bool isSelected = !selectedSeats.Contains(databaseSeats[seatIndex]);
            int price = Convert.ToInt32(Convert.ToDouble(databaseSeats[seatIndex].Price) * databaseScreenings[screeningsButtons.IndexOf(selectedScreening)].PriceAmplification);

            ElementsCreation.ChangeSeatIcon(databaseSeats[seatIndex], seatIndex, ref seatButtons, isSelected);
            DockPanel ticketInfo = ElementsCreation.CreateTicketPlaceInformation(databaseScreenings[currentScreeningIndex].Hall.HallNumber, price, databaseSeats[seatIndex]);
            
            int prevTicketsCount = chosenSeats.Count;

            if (isSelected)
            {
                selectedSeats.Add(databaseSeats[seatIndex]);
                chosenSeats.Add(ticketInfo);
                ticketsSum += price;
            }

            else
            {
                chosenSeats.RemoveAt(selectedSeats.IndexOf(databaseSeats[seatIndex]));
                selectedSeats.Remove(databaseSeats[seatIndex]);
                ticketsSum -= price;
            }

            if(chosenSeats.Count != prevTicketsCount) DisplaySelectedTicketsInfo();

            ChangeTotalSum();
        }

        private void ChangeTotalSum()
        {
            this.SumLabel.Content = $"{ticketsSum} р.";
        }

        private void DisplaySelectedTicketsInfo()
        {
            this.PlaceInformation.Children.Clear();

            foreach(var seat in chosenSeats)
            {
                this.PlaceInformation.Children.Add(seat);
            }
        }

        private void SetupDateTime()
        {            
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Tick += OnTimerTick;
            this.CurrentDate.Content = DateTime.Now.ToString("dd.MM");
            this.CurrentTime.Content = DateTime.Now.ToString("HH:mm");
            timer.Start();
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            if(this.CurrentTime.Content.ToString() != DateTime.Now.ToString("HH:mm"))
                this.CurrentTime.Content = DateTime.Now.ToString("HH:mm");

            if(this.CurrentDate.Content.ToString() != DateTime.Now.ToString("dd:MM"))
                this.CurrentDate.Content = DateTime.Now.ToString("dd.MM");
        }

        private void SetupWeekArrows()
        {
            this.PreviousDate.MouseLeftButtonDown += SwitchWeek;
            this.NextDate.MouseLeftButtonDown += SwitchWeek;

            this.PreviousDate.Cursor = Cursors.Hand;
            this.NextDate.Cursor = Cursors.Hand;
        }

        private void SwitchWeek(object sender, MouseButtonEventArgs e)
        {
            Image arrow = (Image)sender;
            if (arrow.Name == "NextDate" && calendarButtonIndex == 7) return;
            else if (arrow.Name == "NextDate") calendarButtonIndex = 7;
            else if (calendarButtonIndex == 0) return;
            else calendarButtonIndex = 0;

            DisplayCurrentWeekCalendarButtons();
            if (calendarButtonIndex == 0)
            {
                SelectDateEvent(dateButtons[((int)DateTime.Today.DayOfWeek + 6) % 7], null);
            }
            else if (calendarButtonIndex == 7)
            {
                SelectDateEvent(dateButtons[calendarButtonIndex], null);
            }
        }

        private void DisplayCurrentWeekCalendarButtons()
        {
            this.Dates.Children.Clear();
            for (int i = calendarButtonIndex; i < calendarButtonIndex+7; i++)
            {
                if (i >= ((int)DateTime.Today.DayOfWeek + 6) % 7)
                {
                    dateButtons[i].MouseLeftButtonDown += SelectDateEvent;
                    dateButtons[i].MouseLeftButtonDown += DisplayCurrentDateScreeningsButtons;
                    dateButtons[i].Cursor = Cursors.Hand;
                }

                this.Dates.Children.Add(dateButtons[i]);
            }
                
            
        }

        private void SelectDateEvent(object sender, MouseButtonEventArgs? e)
        {
            Border clickedBorder = (Border)sender;

            if (clickedBorder == selectedDate) return;
            
            selectedDate.Background = (Brush)(new BrushConverter().ConvertFromString("#595959")!);
            selectedDate.BorderThickness = new Thickness(0);
            selectedDate = clickedBorder;

            selectedDate.Background = (Brush)(new BrushConverter().ConvertFromString("#38A0E8")!);
            selectedDate.BorderThickness = new Thickness(2.5);
            selectedDate.BorderBrush = Brushes.White;

            DisplayCurrentDateScreeningsButtons(dateButtons[dateButtons.IndexOf(clickedBorder)], null);


            this.PlaceInformation.Children.Clear();
            selectedSeats.Clear();
            chosenSeats.Clear();
            this.SeatGrid.Children.Clear();
            databaseSeats.Clear();
            ticketsSum = 0;
            ChangeTotalSum();

            this.AgeRating.Content = "";
            this.MovieName.Content = "";
            this.SessionDate.Content = "";
            this.SessionTime.Content = "";
            this.SessionHall.Content = "";
            this.CommonSeatPrice.Content = "";
            this.CouchSeatPrice.Content = "";

            this.TicketDate.Content = $"Дата: {dates[dateButtons.IndexOf(selectedDate)].ToString("dd.MM.yyyy")}";
            this.TicketTimeAndMovie.Content = "";
        }

    }
}
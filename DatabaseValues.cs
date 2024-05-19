using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketSeller
{
    public class Ticket
    {
        int ticketID;
        int screeningID;
        int seatID;
        int price;

        public int TicketID
        {
            get { return ticketID; }
            set { ticketID = value; }
        }

        public int Price
        {
            get { return price; }
            set { price = value; }
        }

        public int ScreeningID
        {
            get { return screeningID; }
            set { screeningID = value; }
        }

        public int SeatID
        {
            get { return seatID; }
            set { seatID = value; }
        }

        public Ticket(int id, int screeningID, int seatID, int price)
        {
            this.ticketID = id;
            this.screeningID = screeningID;
            this.seatID = seatID;
            this.price = price;
        }
    }

    public class Cashier
    {
        int cashierID;
        string firstName;
        string lastName;
        string username;
        string password;
        bool isAdmin;

        public int CashierID
        {
            get { return cashierID; }
            set { cashierID = value; }
        }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public bool IsAdmin
        {
            get { return isAdmin; }
            set { isAdmin = value; }
        }

        public bool IsNull()
        {
            return this.cashierID == 0;
        }

        public Cashier(int id, string firstName, string lastName, string username, string password, bool isAdmin)
        {
            this.cashierID = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.username = username;
            this.password = password;
            this.isAdmin = isAdmin;
        }
    }

    public class Movie
    {
        int movieID;
        string title;
        string ageRating;
        int duration;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public int Duration
        {
            get { return duration; }
            set { duration = value; }
        }
        public string AgeRating
        {
            get { return ageRating; }
            set { ageRating = value; }
        }

        public int MovieID
        {
            get { return movieID; }
            set { movieID = value; }
        }

        public Movie(int id, string title, string ageRating, int duration)
        {
            this.movieID = id;
            this.title = title;
            this.ageRating = ageRating;
            this.duration = duration;
        }

    }

    public class Screenings
    {
        int screenID;
        DateTime date;
        DateTime time;
        Movie movie;
        Halls hall;
        List<Seat> occupiedSeats;
        double priceAmplification;

        public int ScreenID
        {
            get { return screenID; }
            set { screenID = value; }
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }
        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
        public Halls Hall
        {
            get { return hall; }
            set { hall = value; }
        }

        public Movie Movie
        {
            get { return movie; }
            set { movie = value; }
        }

        public List<Seat> OccupiedSeats
        {
            get { return occupiedSeats; }
            set { occupiedSeats = value; }
        }

        public double PriceAmplification
        {
            get { return priceAmplification; }
            set { priceAmplification = value; }
        }

        public Screenings(int id, Movie movie, Halls hall, DateTime date, DateTime time, List<Seat> occupiedSeats, double priceAmplificaton)
        {
            this.screenID = id;
            this.date = date;
            this.time = time;
            this.movie = movie;
            this.hall = hall;
            this.occupiedSeats = occupiedSeats;
            this.priceAmplification = priceAmplificaton;
        }
    }

    public class Halls
    {
        /*
         * 1 зал 14х12
         * 2 зал 12х12
         * 3 зал 12х10
         */
        int hallID;
        int hallNumber;
        int capacity;
        List<Seat> seats;
        public int HallID
        {
            get { return this.hallID; }
            set { hallID = value; }
        }

        public int HallNumber
        {
            get { return hallNumber; }
            set { hallNumber = value; }
        }
        public int Capacity
        {
            get { return capacity; }
            set { capacity = value; }
        }

        public List<Seat> Seats
        {
            get { return seats; }
            set { seats = value; }
        }
        public Halls(int id, int hallNumber, int capacity)
        {
            this.hallID = id;
            this.hallNumber = hallNumber;
            this.capacity = capacity;
            this.seats = new List<Seat>(capacity);
        }
    }

    public class Seat
    {
        int seatID;
        int hallID;
        int rowNumber;
        int seatNumber;
        string type;
        bool isOccupied;
        int price;

        public int SeatID
        {
            get { return this.seatID; }
            set { seatID = value; }
        }
        public int HallID
        {
            get { return this.hallID; }
            set { hallID = value; }
        }

        public int RowNumber
        {
            get { return rowNumber; }
            set { rowNumber = value; }
        }
        public int SeatNumber
        {
            get { return seatNumber; }
            set { seatNumber = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public bool IsOccupied
        {
            get { return isOccupied; }
            set { isOccupied = value; }
        }
        public int Price
        {
            get { return price; }
            set { price = value; }
        }

        public Seat(int id, int hallId, int rowNumber, int seatNumber, string type, int price)
        {
            this.seatID = id;
            this.hallID = hallId;
            this.rowNumber = rowNumber;
            this.seatNumber = seatNumber;
            this.type = type;
            this.isOccupied = false;
            this.price = price;
        }
    }

    public class Recipt
    {
        int reciptID;
        int cashierID;
        DateTime date;
        DateTime time;
        List<Ticket> tickets;
        int sum;

        public int ReciptID
        {
            get { return reciptID; }
            set { reciptID = value; }
        }

        public int CashierID
        {
            get { return cashierID; }
            set { cashierID = value; }
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }

        public List<Ticket> Tickets
        {
            get { return tickets; }
            set { tickets = value; }
        }

        public int Sum
        {
            get { return sum; }
            set { sum = value; }
        }

        public Recipt(int reciptID, int cashierID, DateTime date, DateTime time, List<Ticket> tickets, int sum)
        {
            this.reciptID = reciptID;
            this.cashierID = cashierID;
            this.date = date;
            this.time = time;
            this.tickets = tickets;
            this.sum = sum;
        }
    }
}

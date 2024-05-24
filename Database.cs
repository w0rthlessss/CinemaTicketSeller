using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using System.Data;


namespace CinemaTicketSeller
{
    abstract public class DatabaseConnection
    {
        protected const string server = "localhost";
        protected const string port = "3306";
        protected const string database = "cinematicketseller";
        protected abstract string user { get;}
        protected abstract string password { get;}
        protected string connectionString = "";

        protected List<Seat> DeserializeOccupiedSeats(MySqlDataReader reader)
        {
            if (reader.IsDBNull(reader.GetOrdinal("OccupiedSeats")))
                return new List<Seat>();
            else
            {
                string json = reader.GetString(reader.GetOrdinal("OccupiedSeats"));
                List<Seat> occupiedSeats = JsonConvert.DeserializeObject<List<Seat>>(json)!;

                return occupiedSeats;
            }
        }
    }

    public class RequestConnection : DatabaseConnection
    {
        MySqlConnection connection;
        protected override string user { get => "RequestUsersData"; }
        protected override string password { get => "RequestUsersData1"; }

        public RequestConnection()
        { 
            this.connectionString = $"SERVER={server};PORT={port};DATABASE={database};UID={user};PASSWORD={password};";
            connection = new MySqlConnection();
            connection.ConnectionString = connectionString;
        }      
        
        protected async Task<byte[]> EncryptPassAsync(string pass)
        {
            string query = "select aes_encrypt(@pass, 'password')";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@pass", pass);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        long bytesRead = reader.GetBytes(0, 0, null, 0, 0);
                        byte[] blob = new byte[bytesRead];
                        reader.GetBytes(0, 0, blob, 0, (int)bytesRead);
                        return blob;
                    }
                    else return new byte[1];
                }                
            }
        }

        internal async Task<Cashier> FindCashierAsync(string username, string pass)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(pass)) 
                return new Cashier(0, string.Empty, string.Empty, string.Empty, string.Empty, false);

            using (connection)
            {
                try
                {
                    await connection.OpenAsync();
                    byte[] encryptedPass = await EncryptPassAsync(pass);

                    string query = "select * from cashiers where Username = @username and Password = @pass";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@pass", encryptedPass);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if(await reader.ReadAsync())
                            {
                                return new Cashier(
                                    reader.GetInt32(0), 
                                    reader.GetString(1),
                                    reader.GetString(2), 
                                    reader.GetString(3), 
                                    pass,
                                    reader.GetBoolean(5));
                                
                            }
                            else
                                return new Cashier(0, string.Empty, string.Empty, string.Empty, string.Empty, false);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Не удалось подключиться к базе данных!",
                    "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                    return new Cashier(0, string.Empty, string.Empty, string.Empty, string.Empty, false);
                }
            }

        }

        internal void CloseConnection()
        {
            connection.CloseAsync();
        }
    }

    public class CashierConnection : DatabaseConnection
    {
        MySqlConnection connection;
        protected override string user { get => "CinemaCashier"; }
        protected override string password { get => "CinemaCashier1"; }

        public CashierConnection()
        {
            this.connectionString = $"SERVER={server};PORT={port};DATABASE={database};UID={user};PASSWORD={password};";
            connection = new MySqlConnection();
            connection.ConnectionString = connectionString;
        }

        public void OpenConnection()
        {
            try
            {
                connection.Open();
            }
            catch {
                MessageBox.Show("Ошибка подключения к БД!");
            }
        }

        internal List<Seat> GetSeatsForCurrnetHallAndSession(Screenings currentScreening)
        {
            List<Seat> seats = new List<Seat>();

            string query = "select * from seats where HallID = @HallID";

            using(MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@HallID", currentScreening.Hall.HallID);

                using(var reader =  command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        seats.Add(new Seat
                            (reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetInt32(2),
                            reader.GetInt32(3),
                            reader.GetString(4),
                            reader.GetInt32(5)
                            ));
                    }
                }
            }

            foreach(Seat occSeat in currentScreening.OccupiedSeats)
            {
                var seatToUpdate = seats.FirstOrDefault(seat => seat.SeatID == occSeat.SeatID);
                if(seatToUpdate != null) seatToUpdate.IsOccupied = true;
            }
            
            return seats;
        }

        internal List<Screenings> GetCurrentDateScreeningsFromDatabase(DateTime date)
        {
            List<Screenings> currentDateScreenings = new List<Screenings>();
            string query = "select s.*, m.*, h.* " +
                            "from screenings s " +
                            "inner join movies m on s.MovieID = m.MovieID " +
                            "inner join hall h on s.HallID = h.HallID " +
                            "where s.Date = @date";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));
                using(var reader = command.ExecuteReader())
                {
                    
                    while (reader.Read())
                    {                                               
                        currentDateScreenings.Add(new Screenings(
                            reader.GetInt32(reader.GetOrdinal("ScreeningID")),
                            new Movie(reader.GetInt32(reader.GetOrdinal("MovieID")),
                                    reader.GetString(reader.GetOrdinal("Title")),
                                    reader.GetString(reader.GetOrdinal("AgeRating")),
                                    reader.GetInt32(reader.GetOrdinal("Duration"))),
                            new Halls(reader.GetInt32(reader.GetOrdinal("HallID")),
                                    reader.GetInt32(reader.GetOrdinal("HallNumber")),
                                    reader.GetInt32(reader.GetOrdinal("Capacity"))),
                            reader.GetDateTime(reader.GetOrdinal("Date")),
                            DateTime.Parse(reader.GetString(reader.GetOrdinal("Time"))),
                            DeserializeOccupiedSeats(reader), 
                            reader.GetDouble(reader.GetOrdinal("PriceAmplification"))));
                    }
                }
            }

            return currentDateScreenings;
        }

        internal int GetLastRecordIdFromSpecificTable(string columnName, string tableName)
        {
            string query =  $"select max({columnName}) from {tableName}";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                object id = command.ExecuteScalar();
                return (id.ToString() == string.Empty ? 1 : (int)id + 1);
            }
        }      

        internal void AddTicketsToDatabase(List<Ticket> tickets)
        {
            string query = "insert into tickets values (@TicketID, @screeningID, @seatID, @price)";
            foreach(Ticket ticket in tickets)
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    
                    command.Parameters.AddWithValue("@TicketID", ticket.TicketID);
                    command.Parameters.AddWithValue("@screeningID", ticket.ScreeningID);
                    command.Parameters.AddWithValue("@seatID", ticket.SeatID);
                    command.Parameters.AddWithValue("@price", ticket.Price);

                    command.ExecuteNonQuery();
                    
                }
            }
            
        }

        internal void AddReciptsToDatabase(Recipt recipt)
        {
            string query = "insert into recipts values (@id, @cashierID, @date, @time, @tickets, @sum)";

            string json = JsonConvert.SerializeObject(recipt.Tickets);                        

            using(MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", recipt.ReciptID);
                command.Parameters.AddWithValue("@cashierID", recipt.CashierID);
                command.Parameters.AddWithValue("@date", recipt.Date.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@time", recipt.Time.ToString("HH:mm:ss"));
                command.Parameters.AddWithValue("@tickets", json);
                command.Parameters.AddWithValue("@sum", recipt.Sum);

                command.ExecuteNonQuery();
            }
        }

        internal void UpdateOccupiedSeats(string json, int screeningID)
        {
            string query = "update screenings set OccupiedSeats = @seats where ScreeningID = @id";
            using(MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@seats", json);
                command.Parameters.AddWithValue("@id", screeningID);

                command.ExecuteNonQuery();
            }
        }

        internal void CloseConnection()
        {
            connection.CloseAsync();
        }
    }

    public class AdminConnection : DatabaseConnection
    {
        MySqlConnection connection;
        protected override string user { get => "CinemaAdmin"; }
        protected override string password { get => "CinemaAdmin1"; }

        public AdminConnection()
        {
            this.connectionString = $"SERVER={server};PORT={port};DATABASE={database};UID={user};PASSWORD={password};";
            connection = new MySqlConnection();
            connection.ConnectionString = connectionString;
        }

        public void OpenConnection()
        {
            try
            {
                connection.Open();
            }
            catch
            {
                MessageBox.Show("Ошибка подключения к БД!");
            }
        }

        internal void CloseConnection()
        {
            connection.CloseAsync();
        }


        protected byte[] EncryptPass(string pass)
        {
            string query = "select aes_encrypt(@pass, 'password')";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@pass", pass);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        long bytesRead = reader.GetBytes(0, 0, null, 0, 0);
                        byte[] blob = new byte[bytesRead];
                        reader.GetBytes(0, 0, blob, 0, (int)bytesRead);
                        return blob;
                    }
                    else return new byte[1];
                }
            }
        }

        protected string DecryptPassword(byte[] pass)
        {
            string query = "select aes_decrypt(@pass, 'password')";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@pass", pass);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Encoding.UTF8.GetString((byte[])reader[0]);
                    }
                    else return string.Empty;
                }
            }
        }

        internal int GetLastRecordIdFromSpecificTable(string columnName, string tableName)
        {
            string query = $"select max({columnName}) from {tableName}";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                object id = command.ExecuteScalar();
                return (id.ToString() == string.Empty ? 1 : (int)id + 1);
            }
        }

        /// <summary>
        /// Получение списков сущностей из БД
        /// </summary>
        #region GetListsOfEntities

        internal List<Cashier> GetListOfCashiers()
        {
            List<Cashier> listOfCashiers = new List<Cashier>();
            List<byte[]> passwords = new List<byte[]>();
            string query = "select * from cashiers";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listOfCashiers.Add(new Cashier(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            "pass",
                            reader.GetBoolean(5)));
                        passwords.Add((byte[])reader[4]);
                    }
                }
            }

            for(int i = 0; i < listOfCashiers.Count; i++)
            {
                listOfCashiers[i].Password = DecryptPassword(passwords[i]);
            }

            return listOfCashiers;
        }

        internal List<Halls> GetListOfHalls()
        {
            List<Halls> listOfHalls = new List<Halls>();
            string query = "select * from hall";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listOfHalls.Add(new Halls(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetInt32(2)));
                    }
                }
            }

            return listOfHalls;
        }

        internal List<Movie> GetListOfMovies()
        {
            List<Movie> listOfMovies = new List<Movie>();
            string query = "select * from movies";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listOfMovies.Add(new Movie(
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetInt32(3)));
                    }
                }
            }

            return listOfMovies;
        }

        internal List<Screenings> GetListOfScreenings()
        {
            List<Screenings> listOfScreenings = new List<Screenings>();
            string query = "select s.*, m.*, h.* " +
                            "from screenings s " +
                            "inner join movies m on s.MovieID = m.MovieID " +
                            "inner join hall h on s.HallID = h.HallID";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listOfScreenings.Add(new Screenings(
                            reader.GetInt32(reader.GetOrdinal("ScreeningID")),
                            new Movie(reader.GetInt32(reader.GetOrdinal("MovieID")),
                                    reader.GetString(reader.GetOrdinal("Title")),
                                    reader.GetString(reader.GetOrdinal("AgeRating")),
                                    reader.GetInt32(reader.GetOrdinal("Duration"))),
                            new Halls(reader.GetInt32(reader.GetOrdinal("HallID")),
                                    reader.GetInt32(reader.GetOrdinal("HallNumber")),
                                    reader.GetInt32(reader.GetOrdinal("Capacity"))),
                            reader.GetDateTime(reader.GetOrdinal("Date")),
                            DateTime.Parse(reader.GetString(reader.GetOrdinal("Time"))),
                            DeserializeOccupiedSeats(reader),
                            reader.GetDouble(reader.GetOrdinal("PriceAmplification"))));
                    }
                }
            }

            return listOfScreenings;
        }

        internal List<Seat> GetListOfSeats()
        {
            List<Seat> listOfSeats = new List<Seat>();
            string query = "select * from seats";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listOfSeats.Add(new Seat
                            (reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetInt32(2),
                            reader.GetInt32(3),
                            reader.GetString(4),
                            reader.GetInt32(5)
                            ));
                    }
                }
            }
            return listOfSeats;
        }

        internal List<Ticket> GetListOfTickets()
        {
            List<Ticket> listOfTickets = new List<Ticket>();
            string query = "select * from tickets";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listOfTickets.Add(new Ticket(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetInt32(2),
                            reader.GetInt32(3)));
                    }
                }
            }
            return listOfTickets;
        }

        internal List<Recipt> GetListOfRecipts()
        {
            List<Recipt> listOfRecipts = new List<Recipt>();
            string query = "select * from recipts";
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listOfRecipts.Add(new Recipt(
                            reader.GetInt32(0),
                            reader.GetInt32(1),
                            reader.GetDateTime(2),
                            DateTime.Parse(reader.GetString(3)),
                            DeserializeTicketsForRecipt(reader),
                            reader.GetInt32(5)));
                    }
                }
            }
            return listOfRecipts;
        }

        private List<Ticket> DeserializeTicketsForRecipt(MySqlDataReader reader)
        {
            if (reader.IsDBNull(reader.GetOrdinal("Tickets")))
                return new List<Ticket>();
            else
            {
                string json = reader.GetString(reader.GetOrdinal("Tickets"));
                List<Ticket> tickets = JsonConvert.DeserializeObject<List<Ticket>>(json)!;

                return tickets;
            }
        }

        #endregion
    

        /// <summary>
        /// Изменение существующих сущностей в БД
        /// </summary>
        /// <param name="updatedValues"></param> Список сущностей для изменения
        /// <param name="ids"></param> Список id сущностей для изменения
        #region UpdateEntities

        internal void UpdateCashiersRecords(List<Cashier> updatedValues, List<int> ids)
        {
            List<byte[]> passwords = new List<byte[]>();
            foreach(Cashier c in updatedValues)
                passwords.Add(EncryptPass(c.Password));
            
            try
            {
                string query = "update cashiers " +
                "set CashierID = @newId,  FirstName = @newName, LastName = @newLastName, Username = @newUsername, Password = @newPassword, Admin = @newAdmin " +
                "where CashierID = @id";
                for (int i = 0; i < updatedValues.Count; i++)
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@newId", updatedValues[i].CashierID);
                        command.Parameters.AddWithValue("@newName", updatedValues[i].FirstName);
                        command.Parameters.AddWithValue("@newLastName", updatedValues[i].LastName);
                        command.Parameters.AddWithValue("@newUsername", updatedValues[i].Username);
                        command.Parameters.AddWithValue("@newPassword", passwords[i]);
                        command.Parameters.AddWithValue("@newAdmin", updatedValues[i].IsAdmin);
                        command.Parameters.AddWithValue("@id", ids[i]);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при изменении данных в БД", "Ошибка");
                return;
            }

        }

        internal void UpdateHallsRecords(List<Halls> updatedValues, List<int> ids)
        {
            try
            {
                string query = "update hall " +
                "set HallID = @newId,  HallNumber = @newNumber, Capacity = @newCapacity " +
                "where HallID = @id";
                for (int i = 0; i < updatedValues.Count; i++)
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@newId", updatedValues[i].HallID);
                        command.Parameters.AddWithValue("@newNumber", updatedValues[i].HallNumber);
                        command.Parameters.AddWithValue("@newCapacity", updatedValues[i].Capacity);
                        command.Parameters.AddWithValue("@id", ids[i]);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при изменении данных в БД", "Ошибка");
                return;
            }

        }

        internal void UpdateMoviesRecords(List<Movie> updatedValues, List<int> ids)
        {
            try
            {
                string query = "update movies " +
                "set MovieID = @newId,  Title = @newTitle, AgeRating = @newAgeRating, Duration = @newDuration " +
                "where MovieID = @id";
                for (int i = 0; i < updatedValues.Count; i++)
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@newId", updatedValues[i].MovieID);
                        command.Parameters.AddWithValue("@newTitle", updatedValues[i].Title);
                        command.Parameters.AddWithValue("@newAgeRating", updatedValues[i].AgeRating);
                        command.Parameters.AddWithValue("@newDuration", updatedValues[i].Duration);

                        command.Parameters.AddWithValue("@id", ids[i]);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при изменении данных в БД", "Ошибка");
                return;
            }

        }

        internal void UpdateScreenignsRecords(List<Screenings> updatedValues, List<int> ids)
        {
            try
            {
                string query = "update screenings " +
                "set ScreeningID = @newId,  MovieID = @newMovieID, HallID = @newHallID, Date = @newDate, Time = @newTime, PriceAmplification = @newPriceAmpl, OccupiedSeats = @newOccupiedSeats " +
                "where ScreeningID = @id";
                for (int i = 0; i < updatedValues.Count; i++)
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@newId", updatedValues[i].ScreenID);
                        command.Parameters.AddWithValue("@newMovieID", updatedValues[i].Movie.MovieID);
                        command.Parameters.AddWithValue("@newHallID", updatedValues[i].Hall.HallID);
                        command.Parameters.AddWithValue("@newDate", updatedValues[i].Date.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@newTime", updatedValues[i].Time.ToString("HH:mm:ss"));
                        command.Parameters.AddWithValue("@newPriceAmpl", updatedValues[i].PriceAmplification);
                        command.Parameters.AddWithValue("@newOccupiedSeats", JsonConvert.SerializeObject(updatedValues[i].OccupiedSeats));
                        command.Parameters.AddWithValue("@id", ids[i]);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при изменении данных в БД", "Ошибка");
                return;
            }

        }

        internal void UpdateSeatsRecords(List<Seat> updatedValues, List<int> ids)
        {
            try
            {
                string query = "update seats " +
                "set SeatID = @newId,  HallID = @newHallID, RowNumber = @newRowNumber, SeatNumber = @newSeatNumber, Type = @newType, Price = @newPrice " +
                "where SeatID = @id";
                for (int i = 0; i < updatedValues.Count; i++)
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@newId", updatedValues[i].SeatID);
                        command.Parameters.AddWithValue("@newHallID", updatedValues[i].HallID);
                        command.Parameters.AddWithValue("@newRowNumber", updatedValues[i].RowNumber);
                        command.Parameters.AddWithValue("@newSeatNumber", updatedValues[i].SeatNumber);
                        command.Parameters.AddWithValue("@newType", updatedValues[i].Type);
                        command.Parameters.AddWithValue("@newPrice", updatedValues[i].Price);
                        command.Parameters.AddWithValue("@id", ids[i]);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при изменении данных в БД", "Ошибка");
                return;
            }

        }

        internal void UpdateTicketsRecords(List<Ticket> updatedValues, List<int> ids)
        {
            try
            {
                string query = "update tickets " +
                "set TicketID = @newId,  ScreeningID = @newScreeningID, SeatID = @newSeatID, Price = @newPrice " +
                "where TicketID = @id";
                for (int i = 0; i < updatedValues.Count; i++)
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@newId", updatedValues[i].TicketID);
                        command.Parameters.AddWithValue("@newScreeningID", updatedValues[i].ScreeningID);
                        command.Parameters.AddWithValue("@newSeatID", updatedValues[i].SeatID);
                        command.Parameters.AddWithValue("@newPrice", updatedValues[i].Price);
                        command.Parameters.AddWithValue("@id", ids[i]);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при изменении данных в БД", "Ошибка");
                return;
            }

        }

        internal void UpdateReciptsRecords(List<Recipt> updatedValues, List<int> ids)
        {
            try
            {
                string query = "update recipts " +
                "set ReciptID = @newId,  CashierID = @newCashier, Date = @newDate, Time = @newTime, Tickets = @newTickets, Sum = @newSum " +
                "where ReciptID = @id";
                for (int i = 0; i < updatedValues.Count; i++)
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@newId", updatedValues[i].ReciptID);
                        command.Parameters.AddWithValue("@newCashier", updatedValues[i].CashierID);
                        command.Parameters.AddWithValue("@newDate", updatedValues[i].Date.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@newTime", updatedValues[i].Time.ToString("HH:mm:ss"));
                        command.Parameters.AddWithValue("@newTickets", JsonConvert.SerializeObject(updatedValues[i].Tickets));
                        command.Parameters.AddWithValue("@newSum", updatedValues[i].Sum);
                        command.Parameters.AddWithValue("@id", ids[i]);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при изменении данных в БД", "Ошибка");
                return;
            }

        }
        #endregion


        /// <summary>
        /// Добавление новых сущностей в БД
        /// </summary>
        /// <param name="newValues"></param> Список новых сущностей для добавления
        #region InsertEntities
        internal void InsertCasheirs(List<Cashier> newValues)
        {
            List<byte[]> passwords = new List<byte[]>();
            foreach (Cashier c in newValues)
                passwords.Add(EncryptPass(c.Password));

            try
            {
                string query = "insert into cashiers " +
                "values (@id, @firstName, @lastName, @username, @password, @isAdmin)";
                for (int i = 0; i < newValues.Count; i++)
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", newValues[i].CashierID);
                        command.Parameters.AddWithValue("@firstName", newValues[i].FirstName);
                        command.Parameters.AddWithValue("@lastName", newValues[i].LastName);
                        command.Parameters.AddWithValue("@username", newValues[i].Username);
                        command.Parameters.AddWithValue("@password", passwords[i]);
                        command.Parameters.AddWithValue("@isAdmin", newValues[i].IsAdmin);

                        command.ExecuteNonQuery();
                    }

                }
            }
            catch
            {
                MessageBox.Show("Ошибка при добавлении данных в БД", "Ошибка");
                return;
            }
        }

        internal void InsertHalls(List<Halls> newValues)
        {
            try
            {
                string query = "insert into hall " +
                "values (@id, @hallNumber, @capacity)";
                for (int i = 0; i < newValues.Count; i++)
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", newValues[i].HallID);
                        command.Parameters.AddWithValue("@hallNumber", newValues[i].HallNumber);
                        command.Parameters.AddWithValue("@capacity", newValues[i].Capacity);

                        command.ExecuteNonQuery();
                    }

                }
            }
            catch
            {
                MessageBox.Show("Ошибка при добавлении данных в БД", "Ошибка");
                return;
            }
        }

        internal void InsertMovies(List<Movie> newValues)
        {
            try
            {
                string query = "insert into movies " +
                "values (@id, @title, @ageRating, @duration)";
                for (int i = 0; i < newValues.Count; i++)
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", newValues[i].MovieID);
                        command.Parameters.AddWithValue("@title", newValues[i].Title);
                        command.Parameters.AddWithValue("@ageRating", newValues[i].AgeRating);
                        command.Parameters.AddWithValue("@duration", newValues[i].Duration);

                        command.ExecuteNonQuery();
                    }

                }
            }
            catch
            {
                MessageBox.Show("Ошибка при добавлении данных в БД", "Ошибка");
                return;
            }
        }

        internal void InsertScreenings(List<Screenings> newValues)
        {
            try
            {
                string query = "insert into screenings " +
                "values (@screeningID, @movieID, @hallID, @date, @time, @priceAmpl, @occupiedSeats)";
                for (int i = 0; i < newValues.Count; i++)
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@screeningID", newValues[i].ScreenID);
                        command.Parameters.AddWithValue("@movieID", newValues[i].Movie.MovieID);
                        command.Parameters.AddWithValue("@hallID", newValues[i].Hall.HallID);
                        command.Parameters.AddWithValue("@date", newValues[i].Date.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@time", newValues[i].Time.ToString("HH:mm:ss"));
                        command.Parameters.AddWithValue("@priceAmpl", newValues[i].PriceAmplification);
                        if (newValues[i].OccupiedSeats.Count != 0 )
                            command.Parameters.AddWithValue("@occupiedSeats", JsonConvert.SerializeObject(newValues[i].OccupiedSeats));
                        else
                            command.Parameters.AddWithValue("@occupiedSeats", null);


                        command.ExecuteNonQuery();
                    }

                }
            }
            catch
            {
                MessageBox.Show("Ошибка при добавлении данных в БД", "Ошибка");
                return;
            }
        }

        internal void InsertSeats(List<Seat> newValues)
        {
            try
            {
                string query = "insert into seats " +
                "values (@seatID, @hallID, @rowNumber, @seatNumber, @type, @price)";
                for (int i = 0; i < newValues.Count; i++)
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@seatID", newValues[i].SeatID);
                        command.Parameters.AddWithValue("@hallID", newValues[i].HallID);
                        command.Parameters.AddWithValue("@rowNumber", newValues[i].RowNumber);
                        command.Parameters.AddWithValue("@seatNumber", newValues[i].SeatNumber);
                        command.Parameters.AddWithValue("@type", newValues[i].Type);
                        command.Parameters.AddWithValue("@price", newValues[i].Price);

                        command.ExecuteNonQuery();
                    }

                }
            }
            catch
            {
                MessageBox.Show("Ошибка при добавлении данных в БД", "Ошибка");
                return;
            }
        }

        internal void InsertTickets(List<Ticket> newValues)
        {
            try
            {
                string query = "insert into tickets " +
                "values (@ticketID, @screeningID, @seatID, @price)";
                for (int i = 0; i < newValues.Count; i++)
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ticketID", newValues[i].TicketID);
                        command.Parameters.AddWithValue("@screeningID", newValues[i].ScreeningID);
                        command.Parameters.AddWithValue("@seatID", newValues[i].SeatID);
                        command.Parameters.AddWithValue("@price", newValues[i].Price);

                        command.ExecuteNonQuery();
                    }

                }
            }
            catch
            {
                MessageBox.Show("Ошибка при добавлении данных в БД", "Ошибка");
                return;
            }
        }

        internal void InsertRecipts(List<Recipt> newValues)
        {
            try
            {
                string query = "insert into recipts " +
                "values (@reciptID, @cashierID, @date, @time, @tickets, @sum)";
                for (int i = 0; i < newValues.Count; i++)
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@reciptID", newValues[i].ReciptID);
                        command.Parameters.AddWithValue("@cashierID", newValues[i].CashierID);
                        command.Parameters.AddWithValue("@date", newValues[i].Date.ToString("yyyy-MM-dd"));
                        command.Parameters.AddWithValue("@time", newValues[i].Time.ToString("HH:mm:ss"));
                        command.Parameters.AddWithValue("@tickets", JsonConvert.SerializeObject(newValues[i].Tickets));
                        command.Parameters.AddWithValue("@sum", newValues[i].Sum);

                        command.ExecuteNonQuery();
                    }

                }
            }
            catch
            {
                MessageBox.Show("Ошибка при добавлении данных в БД", "Ошибка");
                return;
            }
        }

        #endregion

        #region RemoveEntities
        internal void DeleteRecords(string table, string column, List<int> ids)
        {
            try
            {
                string query = $"delete from {table} " +                
                $"where {column} = @id";
                for (int i = 0; i < ids.Count; i++)
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", ids[i]);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при удалении данных из БД", "Ошибка");
                return;
            }

        }

        internal void DeleteSpecificDateScreenings(DateTime date)
        {
            string query = "delete from screenings where Date = @date";
            using(MySqlCommand command = new MySqlCommand( query, connection))
            {
                command.Parameters.AddWithValue("@date", date.ToString("yyyy-MM-dd"));

                command.ExecuteNonQuery();
            }
        }
        #endregion
    }
}


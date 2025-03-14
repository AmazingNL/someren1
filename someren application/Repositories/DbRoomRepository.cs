using Microsoft.Data.SqlClient;
using someren_application.Models;

namespace someren_application.Repositories
{
    public class DbRoomRepository : IRoomRepository
    {
        private readonly string? _connectionString;
        List<Room> rooms =
        [
            new Room (1, "A", "A1-01", 1, "Lecturer"),
            new Room (2, "A", "A1-02", 1, "Lecturer"),
            new Room (3, "B", "B1-01", 8, "Student"),
            new Room (4, "B", "B1-02", 8, "Student"),
            new Room (5, "B", "B2-01", 8, "Student"),
            new Room (6, "B", "B2-02", 8, "Student"),
        ];

        public DbRoomRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SomerenConnection");
        }

        public void Add(Room room)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                string query = "INSERT INTO [room] (building, roomNumber, Capacity, roomType)" +  // Inserting SQL Query to the room table
                    " VALUES (@Building, @RoomNumber, @Capacity, @RoomType);" +
                    " SELECT SCOPE_IDENTITY();";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@building", room.Building); //  prevent SQL injection to occur by using SQL parameters
                    command.Parameters.AddWithValue("@roomNumber", room.RoomNumber);
                    command.Parameters.AddWithValue("@Capacity", room.Capacity);
                    command.Parameters.AddWithValue("@roomType", room.RoomType);
                    connection.Open();
                    room.RoomId = Convert.ToInt32(command.ExecuteNonQueryAsync()); // Check if the query actually inserted data
                }
                //Console.WriteLine($"Rows affected: {rowsAffected}");
            }
        }

        public void Delete(Room room)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [room] WHERE roomId = @roomId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@roomId", room.RoomId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<Room> GetAll()
        {
            List<Room> rooms = [];

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM [room]";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Room room = ReadUser(reader);
                        rooms.Add(room);
                    }
                }
            }
            return rooms;
        }

        private Room ReadUser(SqlDataReader reader)
        {
            // Retrieve data from room table
            int roomId = (int)reader["roomId"];
            string building = (string)reader["building"];
            string roomNumber = (string)reader["roomNumber"];
            int capacity = (int)reader["capacity"];
            string roomType = (string)reader["roomType"];
            // Return new User object
            return new Room(roomId, building, roomNumber, capacity, roomType);
        }

        public Room? GetById(int roomId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT roomId, building, roomNumber, capacity, roomType FROM [room] WHERE roomId = @RoomId"; // search for the room and return its record
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RoomId", roomId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) // If a record is found
                    {
                        return new Room(
                            (int)reader["roomId"],
                            (string)reader["building"],
                            (string)reader["roomNumber"],
                            (int)reader["capacity"],
                            (string)reader["roomType"]
                        );
                    }
                }
            }
            return null; // Return null if no room is found
        }

        public void Update(Room room)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [room] SET building = @Building, roomNumber = @RoomNumber, capacity = @Capacity, roomType = @RoomType WHERE roomId = @RoomId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RoomId", room.RoomId);
                command.Parameters.AddWithValue("@Building", room.Building);
                command.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);
                command.Parameters.AddWithValue("@Capacity", room.Capacity);
                command.Parameters.AddWithValue("@RoomType", room.RoomType);

                connection.Open();
                command.ExecuteNonQuery();

            }
        }
    }
}

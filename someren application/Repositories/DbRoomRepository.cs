using Microsoft.Data.SqlClient;
using someren_application.Models;
using System.Data;

namespace someren_application.Repositories
{
    public class DbRoomRepository : IRoomRepository
    {
        private readonly string? _connectionString;

        public DbRoomRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SomerenConnection");
        }

        List<Room> IRoomRepository.Filter(int capacity)
        {
            List<Room> rooms = [];

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT roomId, building, roomNumber, capacity, roomType FROM [room] WHERE capacity = @Capacity;";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Capacity", capacity);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Room room = ReadRoom(reader);
                            rooms.Add(room);
                        }
                    }
                }
                catch (SqlException ex)
                {

                    throw new Exception("Something went wrong in the database", ex);
                }
                catch (Exception ex)
                {

                    throw new Exception("Data not reading", ex);
                }
            }
            return rooms;
        }

        private bool IsRoomTypeAllowedInBuilding(Room room)
        {
            // Check if the roomType is allowed in the building
            // You can modify this logic to match your exact requirements.
            if ((room.Building == "A" && room.RoomType == "Lecturer" && room.Capacity == 1) || (room.Building == "A" && room.RoomType == "Student" && room.Capacity == 8)
                || (room.Building == "B" && room.RoomType == "Student" && room.Capacity == 8))
            {
                return true; 
            }

            return false; // If no restriction, return true (allow the combination)
        }

        private int IsStudentRoomFull(Room room)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM [room] WHERE roomNumber = @RoomNumber AND capacity = 8";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);

                    try
                    {
                        connection.Open();
                        int count = (int)command.ExecuteScalar();  // Executes the query and returns the count
                        return count;  // return number of roomNumber
                    }
                    catch (Exception)
                    {
                        throw new Exception($"error checking if room count");
                    }
                }
            }
        }

        private bool IsRoomNumberExist(Room room)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(*) FROM [room] WHERE roomNumber = @RoomNumber AND roomType = 'Lecturer'";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);

                    try
                    {
                        connection.Open();
                        int count = (int)command.ExecuteScalar();  // Executes the query and returns the count

                        return count > 0;  // If count > 0, it means the roomNumber exists
                    }
                    catch (Exception)
                    {
                        throw new Exception("Error checking if roomNumber exists");
                    }
                }
            }
        }


        void IRoomRepository.Add(Room room)
        {
            if (IsRoomNumberExist(room)) 
            {
                throw new Exception("Room name already exists!");
            }
            else if (!IsRoomTypeAllowedInBuilding(room))
            {
                throw new Exception("Lecturer can only stay in Building A");
            }
            else if(IsStudentRoomFull(room) == 8)
            {
                throw new Exception($"{room.RoomNumber} is full add to another room");
            }
            else
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
                    try
                    {
                        connection.Open(); // open the connection

                        int nrOfRowsAffected = command.ExecuteNonQuery();

                        if (nrOfRowsAffected != 1)
                        {
                            throw new Exception("Adding room failed!");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Something went wrong", ex);
                    }
                }
            }
        }

        void IRoomRepository.Delete(Room room)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [room] WHERE roomId = @RoomId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoomId", room.RoomId);

                    connection.Open();
                    int nrOfRowsAffected = command.ExecuteNonQuery();

                    if (nrOfRowsAffected == 0)
                    {
                        throw new Exception("No records deleted!");
                    }
                }
            }
        }


        List<Room> IRoomRepository.GetAll()
        {
            List<Room> rooms = [];

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM [room] ORDER BY roomNumber";
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Room room = ReadRoom(reader);
                            rooms.Add(room);
                        }
                    }
                }
                catch (SqlException ex)
                {

                    throw new Exception("Something went wrong in the database", ex);
                }
                catch (Exception ex)
                {

                    throw new Exception("Data not reading", ex);
                }

            }
            return rooms;
        }

        private Room ReadRoom(SqlDataReader reader)
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

        Room? IRoomRepository.GetById(int roomId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT roomId, building, roomNumber, capacity, roomType FROM [room] WHERE roomId = @RoomId"; // search for the room and return its record
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@RoomId", SqlDbType.Int).Value = roomId;

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

        void IRoomRepository.Update(Room room)
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
                try
                {
                    connection.Open();
                    int nrOfRowsAffected = command.ExecuteNonQuery();
                }
                catch (Exception)
                {

                    throw new Exception("No record updated!");
                }

            }
        }
    }
}

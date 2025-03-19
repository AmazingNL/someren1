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
                            Room room = ReadUser(reader);
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

        void IRoomRepository.Add(Room room)
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
                    try
                    {
                        connection.Open(); // open the connection

                        int nrOfRowsAffected = command.ExecuteNonQuery();

                        if (nrOfRowsAffected != 1)
                        {
                            throw new Exception("Adding user failed!");
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
                            Room room = ReadUser(reader);
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

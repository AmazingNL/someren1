using Microsoft.Data.SqlClient;
using someren_application.IRepositories;
using someren_application.Models;
using System.Collections.Generic;
using System.Data;

namespace someren_application.DbRepository
{
    public class DbStudentsRepository : IStudentsRepository
    {
        private readonly string? _connectionString;

        public DbStudentsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SomerenConnection");
        }


        List<Students> IStudentsRepository.GetAllStudent()
        {
            List<Students> students = new List<Students>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"SELECT S.studentId, S.studentNumber, S.firstName, S.lastName, S.phoneNumber," +
                    " S.studentClass, R.roomId, R.building, R.roomNumber, R.roomType, R.capacity " +
                    "FROM [student] AS S JOIN [room] AS R ON S.roomId = R.roomId ";
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add(ReadStudent(reader)); // Add the student to the list
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while fetching students with rooms.", ex);
                }
            }
            return students;
        }
            

        Students IStudentsRepository.GetStudentsById(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT S.studentId, S.studentNumber, S.firstName, S.lastName, " +
                    "S.phoneNumber, S.studentClass, S.roomId, R.roomId, R.building, R.roomNumber," +
                    " R.roomType, R.capacity " +
                    "FROM [student] AS S JOIN [room] AS R ON S.roomId = R.roomId" +
                    " WHERE S.[studentId] = @StudentId ";
                   // search for the room and return its record
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StudentId", studentId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) // If a record is found
                    {
                        return ReadStudent(reader);
                    }
                }
            }
            throw new Exception("student not found"); // Return null if no student is found
        }

        private Students ReadStudent(SqlDataReader reader)
        {
            // Retrieve data from room table
            List<Room> room = new List<Room> // Changed from Room to List<Room>
            {
                new Room
                {
                    RoomId = Convert.ToInt32(reader["roomId"]),
                    Building = (string)reader["building"],
                    RoomNumber = (string)reader["roomNumber"],
                    RoomType = (string)reader["roomType"],
                    Capacity = Convert.ToInt32(reader["capacity"])
                }
            };
            int studentId = Convert.ToInt32(reader["studentId"]);
            string studentNumber = (string)reader["studentNumber"];
            string firstName = (string)reader["firstName"];
            string lastName = (string)reader["lastName"];
            string phoneNumber = (string)reader["phoneNumber"];
            string studentClass = (string)reader["studentClass"];
            // Return new Room object
            return new Students(studentId, studentNumber, firstName, lastName, phoneNumber, studentClass, room);
        }

        //private int IsStudentRoomFull(Room room)
        //{
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        string query = "SELECT COUNT(*) FROM [room] WHERE roomNumber = @RoomNumber AND capacity = 8";

        //        using (SqlCommand command = new SqlCommand(query, connection))
        //        {
        //            command.Parameters.AddWithValue("@RoomNumber", room.RoomNumber);

        //            try
        //            {
        //                connection.Open();
        //                int count = (int)command.ExecuteScalar();  // Executes the query and returns the count
        //                return count;  // return number of roomNumber
        //            }
        //            catch (Exception)
        //            {
        //                throw new Exception($"error checking if room count");
        //            }
        //        }
        //    }
        //}

        //if (room == null)
        //{
        //    throw new Exception("Student must be assigned to a room.");
        //}

        //if (IsStudentRoomFull(room) == 8) // Pass the first room in the list
        //{
        //    throw new Exception("Room is full");
        //}

        void IStudentsRepository.Add(Students students)
        {
   


            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO [student] (studentNumber, firstName, lastName, phoneNumber, studentClass, roomId)" +
                    " VALUES (@StudentNumber, @FirstName, @LastName, @PhoneNumber, @StudentClass, @RoomId);" +
                    "SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    var room = students.Rooms.FirstOrDefault();
                    if (room == null)
                    {
                        throw new InvalidOperationException("No student selected.");
                    }

                    command.Parameters.AddWithValue("@StudentNumber", students.StudentNumber);
                    command.Parameters.AddWithValue("@FirstName", students.FirstName);
                    command.Parameters.AddWithValue("@LastName", students.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", students.PhoneNumber);
                    command.Parameters.AddWithValue("@StudentClass", students.StudentClass);
                    command.Parameters.AddWithValue("@RoomId", room.RoomId); // Pass the first room's RoomId

                    try
                    {
                        connection.Open(); // Open the connection
                        int nrOfRowsAffected = command.ExecuteNonQuery();

                        if (nrOfRowsAffected != 1)
                        {
                            throw new Exception("Adding student failed!");
                        }

                    }
                    catch (SqlException ex)
                    {
                        throw new Exception("Database error occurred while adding student.", ex);
                    }
                }
            }
        }



        void IStudentsRepository.Delete(Students students)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [student] WHERE studentId = @StudentId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentId", students.StudentId);
                    try
                    {
                        connection.Open();
                        int nrOfRowsAffected = command.ExecuteNonQuery();

                        if (nrOfRowsAffected == 0)
                        {
                            throw new Exception("No records deleted!");
                        }
                    }
                    catch (Exception)
                    {

                        throw new Exception("Something went wrong with deleting from database");
                    }

                }
            }
        }

        void IStudentsRepository.Edit(Students students)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [student] SET firstName = @FirstName, studentNumber = @StudentNumber, lastName = @LastName, phoneNumber = @PhoneNumber, studentClass = @StudentClass WHERE studentId = @StudentId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StudentId", students.StudentId);
                command.Parameters.AddWithValue("@StudentNumber", students.StudentNumber);
                command.Parameters.AddWithValue("@FirstName", students.FirstName);
                command.Parameters.AddWithValue("@LastName", students.LastName);
                command.Parameters.AddWithValue("@PhoneNumber", students.PhoneNumber);
                command.Parameters.AddWithValue("@StudentClass", students.StudentClass);

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

        public List<Students> GetStudentsInRooms(int roomId)
        {
            throw new NotImplementedException();
        }

      
    }
}

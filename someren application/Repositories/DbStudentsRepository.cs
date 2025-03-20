using Microsoft.Data.SqlClient;
using someren_application.Models;
using System.Collections.Generic;
using System.Data;

namespace someren_application.Repositories
{
    public class DbStudentsRepository : IStudentsRepository
    {
        private readonly string? _connectionString;

        public DbStudentsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SomerenConnection");
        }


        List<Students> IStudentsRepository.GetAllStudents()
        {
            List<Students> students = new List<Students>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM [student] ";
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read()) // If a record is found
                        {
                            students.Add(ReadUser(reader));
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
            return students;
        }

        Students? IStudentsRepository.GetStudentsById(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT studentId, studentNumber, firstName, lastName, phoneNumber, studentClass, roomId FROM [student] WHERE studentId = @StudentId"; // search for the room and return its record
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StudentId", studentId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) // If a record is found
                    {
                        return new Students(
                            (int)reader["studentId"],
                            (string)reader["studentNumber"],
                            (string)reader["firstName"],
                            (string)reader["lastName"],
                            (string)reader["phoneNumber"],
                            (string)reader["studentClass"],
                            (int)reader["roomId"]
                        );
                    }
                }
            }
            return null; // Return null if no student is found
        }

        private Students ReadUser(SqlDataReader reader)
        {
            // Retrieve data from room table
            int studentId = (int)reader["studentId"];
            string studentNumber = (string)reader["studentNumber"];
            string firstName = (string)reader["firstName"];
            string lastName = (string)reader["lastName"];
            string phoneNumber = (string)reader["phoneNumber"];
            string studentClass = (string)reader["studentClass"];
            int roomId = (int)reader["roomId"];
            // Return new User object
            return new Students(studentId, studentNumber, firstName, lastName, phoneNumber, studentClass, roomId);
        }

        public void Add(Students students)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO [student] (studentNumber, firstName, lastName, phoneNumber, studentClass, roomId)" +
                    " VALUES (@StudentNumber, @FirstName, @LastName, @PhoneNumber, @StudentClass, @RoomId);" +
                    "SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Parameters with correct casing
                    command.Parameters.AddWithValue("@StudentNumber", students.StudentNumber);
                    command.Parameters.AddWithValue("@FirstName", students.FirstName);
                    command.Parameters.AddWithValue("@LastName", students.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", students.PhoneNumber);
                    command.Parameters.AddWithValue("@StudentClass", students.StudentClass);
                    command.Parameters.AddWithValue("@RoomId", students.RoomId != 0 ? students.RoomId : (object)DBNull.Value);

                    try
                    {
                        connection.Open(); // Open the connection
                        int nrOfRowsAffected = command.ExecuteNonQuery();

                        //  Optional: Log success
                        Console.WriteLine($"Rows affected: {nrOfRowsAffected}");

                        if (nrOfRowsAffected != 1)
                        {
                            throw new Exception("Adding student failed!");
                        }
                    }
                    catch (SqlException ex)
                    {
                        // ✅ Log SQL-specific errors
                        Console.WriteLine($"SQL Error: {ex.Message}");
                        throw new Exception("Database error occurred while adding student.", ex);
                    }
                    catch (Exception ex)
                    {
                        // ✅ Log any other error
                        Console.WriteLine($"General Error: {ex.Message}");
                        throw new Exception("Something went wrong while adding student.", ex);
                    }
                }
            }
        }



        public void Delete(Students students)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [student] WHERE studentId = @StudentId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentId", students.StudentId);

                    connection.Open();
                    int nrOfRowsAffected = command.ExecuteNonQuery();

                    if (nrOfRowsAffected == 0)
                    {
                        throw new Exception("No records deleted!");
                    }
                }
            }
        }

        public void Edit(Students students)
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

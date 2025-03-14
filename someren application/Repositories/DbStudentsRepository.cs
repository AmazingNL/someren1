using Microsoft.Data.SqlClient;
using someren_application.Models;

namespace someren_application.Repositories
{
    public class DbStudentsRepository : IStudentsRepository
    {
        private readonly string? _connectionString;

        public DbStudentsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SomerenConnection");
        }

        public void Add(Students students)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                string query = "INSERT INTO [student] (studentNumber, firstName, lastName, phoneNumber, studentClass)" +  // Inserting SQL Query to the room table
                    " VALUES (@StudentNumber, @FirstName, @LastName, @phoneNumber, @StudentClass);" +
                    " SELECT SCOPE_IDENTITY();";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@studentNumber", students.StudentNumber);
                    command.Parameters.AddWithValue("@firstName", students.FirstName); //  prevent SQL injection to occur by using SQL parameters
                    command.Parameters.AddWithValue("@lastName", students.LastName);
                    command.Parameters.AddWithValue("@phoneNumber", students.PhoneNumber);
                    command.Parameters.AddWithValue("@studentClass", students.StudentClass);
                    connection.Open();
                    students.StudentId = Convert.ToInt32(command.ExecuteNonQueryAsync()); // Check if the query actually inserted data
                }
                //Console.WriteLine($"Rows affected: {rowsAffected}");
            }
        }

        public void Delete(Students students)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [students] WHERE studentId = @studentId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@studentId", students.StudentId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<Students> GetAll()
        {
            List<Students> students = [];

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM [student]";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Students students1 = ReadUser(reader);
                        students.Add(students1);
                    }
                }
            }
            return students;
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
            // Return new User object
            return new Students(studentId, studentNumber, firstName, lastName, phoneNumber, studentClass);
        }

        public Students? GetById(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT studentId, studentNumber, firstName, lastName, phoneNumber, studentClass FROM [students] WHERE studentId = @StudentId"; // search for the room and return its record
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
                            (string)reader["studentClass"]
                        );
                    }
                }
            }
            return null; // Return null if no room is found
        }

        public void Update(Students students)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [student] SET firstName = @FirstName, studentNumber = @StudentNumber, lastName = @LastName, phoneNumber = @PhoneNumber, studentClass = @StudentClass WHERE studentId = @StudentId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StudentId", students.StudentId);
                command.Parameters.AddWithValue("@StudentNumber", students.StudentNumber);
                command.Parameters.AddWithValue("@firstName", students.FirstName);
                command.Parameters.AddWithValue("@lastName", students.LastName);
                command.Parameters.AddWithValue("@phoneNumber", students.PhoneNumber);
                command.Parameters.AddWithValue("@studentClass", students.StudentClass);

                connection.Open();
                command.ExecuteNonQuery();

            }
        }
        
    }
}

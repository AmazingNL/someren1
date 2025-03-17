using Microsoft.Data.SqlClient;
using someren_application.Models;



namespace someren_application.Repositories
{
    public class LecturerDB : ILecturerRepository
    {
        private readonly string _connectionString;
        // Constructor gets connection string from appsettings.json
        public LecturerDB(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SomerenConnection");
        }
        public List<Lecturer> GetAll()
        {
            List<Lecturer> lecturers = new List<Lecturer>();


            //below code must be re-factored to a seperate private method
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "Select * FROM [lecturer]";
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    command.Connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Lecturer lecturer = new Lecturer(
                                reader["firstName"].ToString(),
                                reader["lastName"].ToString(),
                                reader["telephone"].ToString(),
                                Convert.ToInt32(reader["age"])
                            );

                            lecturers.Add(lecturer);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Something went wrong with the database", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception("Something went wrong reading data", ex);
                }
                //end method refactoring
            }
            return lecturers;


        }



        private Lecturer ReadLecturer(SqlDataReader reader)
        {
            // Retrieve lecturer-specific data from the reader
            string firstName = reader["firstName"].ToString();
            string lastName = reader["lastName"].ToString();
            string telephone = reader["telephone"].ToString();
            int age = Convert.ToInt32(reader["age"]);

            // Return new Lecturer object
            return new Lecturer(firstName, lastName, telephone, age);
        }


        //Lecturer? ILecturerRepository.GetById(int lecturerId)
        //{
        //    return null;
        //}
        void ILecturerRepository.Add(Lecturer lecturer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"INSERT INTO [lecturer] (firstName, lastName, telephone, age) 
                                 VALUES (@FirstName, @LastName, @Telephone, @Age)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FirstName", lecturer.FirstName);
                command.Parameters.AddWithValue("@LastName", lecturer.LastName);
                command.Parameters.AddWithValue("@Telephone", lecturer.Telephone);
                command.Parameters.AddWithValue("@Age", lecturer.Age);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        void ILecturerRepository.Update(Lecturer lecturer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"UPDATE [lecturer] 
                                 SET firstName = @FirstName, lastName = @LastName, 
                                     telephone = @Telephone, age = @Age 
                                 WHERE lecturerId = @LecturerId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FirstName", lecturer.FirstName);
                command.Parameters.AddWithValue("@LastName", lecturer.LastName);
                command.Parameters.AddWithValue("@Telephone", lecturer.Telephone);
                command.Parameters.AddWithValue("@Age", lecturer.Age);
                //command.Parameters.AddWithValue("@LecturerId", lecturer.LecturerId);

                connection.Open();
                command.ExecuteNonQuery();
            }

        }
        void ILecturerRepository.Delete(Lecturer lecturer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [lecturer] WHERE lecturerId = @LecturerId";
                SqlCommand command = new SqlCommand(query, connection);
                //command.Parameters.AddWithValue("@LecturerId", lecturer.LecturerId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}

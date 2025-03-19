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
        }
        void ILecturerRepository.Update(Lecturer lecturer)
        {
        }
        void ILecturerRepository.Delete(Lecturer lecturer)
        {
        }
    }
}

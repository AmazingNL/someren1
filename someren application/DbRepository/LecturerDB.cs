using Microsoft.Data.SqlClient;
using someren_application.Models;
using someren_application.Repositories;
using System.Data;



namespace someren_application.DbRepository
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
                            Lecturer lecturer = ReadLecturer(reader);   
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
            int lecturerID = Convert.ToInt32(reader["lecturerID"]);
            string firstName = reader["firstName"].ToString();
            string lastName = reader["lastName"].ToString();
            string phoneNumber = reader["phoneNumber"].ToString();
            int age = Convert.ToInt32(reader["age"]);
            int roomId = Convert.ToInt32(reader["roomId"]);


            // Return new Lecturer object
            return new Lecturer(lecturerID, firstName, lastName, phoneNumber, age, roomId);
            //public Lecturer(int lecturerID, string firstName, string lastName, string phoneNumber, int age, int roomId);

        }



        Lecturer? ILecturerRepository.GetById(int lecturerID)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT lecturerId, firstName, lastName, phoneNumber, age, roomId FROM [lecturer] WHERE lecturerId = @LecturerID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LecturerID", lecturerID);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) // Only one result expected
                            {
                                return ReadLecturer(reader); // Reusing your existing method for consistency
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
                }
            }

            // Return null if no lecturer is found
            return null;
        }





        void ILecturerRepository.Add(Lecturer lecturer)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO [lecturer] ( firstName, lastName, phoneNumber, age, roomId) " +
                "VALUES ( @FirstName, @LastName, @PhoneNumber, @Age ,@RoomID) " +
                "SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    //command.Parameters.AddWithValue("@LecturerID", lecturer.LecturerID);
                    command.Parameters.AddWithValue("@FirstName", lecturer.FirstName);
                    command.Parameters.AddWithValue("@LastName", lecturer.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", lecturer.PhoneNumber);
                    command.Parameters.AddWithValue("@Age", lecturer.Age);
                    command.Parameters.AddWithValue("@RoomId", lecturer.RoomID);

                    try
                    {
                        connection.Open();
                        //command.ExecuteNonQuery();
                        int nrOfRowsAffected = command.ExecuteNonQuery(); // Check if the query actually inserted data

                        if(nrOfRowsAffected != 1)
                        {
                            throw new Exception("No lecturer was inserted");
                        }


                    }
                    catch (Exception)
                    {

                        throw new Exception("Something went wrong with the database");
                    }

                    
                }

            }
        }

            void ILecturerRepository.Update(Lecturer lecturer)
        {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = @"UPDATE [lecturer] 
                                 SET firstName = @FirstName, lastName = @LastName, 
                                     phoneNumber = @PhoneNumber, age = @Age, roomId = @RoomID
                                 WHERE lecturerId = @LecturerId";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@FirstName", lecturer.FirstName);
                    command.Parameters.AddWithValue("@LastName", lecturer.LastName);
                    command.Parameters.AddWithValue("@PhoneNumber", lecturer.PhoneNumber);
                    command.Parameters.AddWithValue("@Age", lecturer.Age);
                    command.Parameters.AddWithValue("@RoomID", lecturer.RoomID);
                    command.Parameters.AddWithValue("@LecturerID", lecturer.LecturerID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

            }
            void ILecturerRepository.Delete(Lecturer lecturer)
        {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    string query = "DELETE FROM [lecturer] WHERE lecturerID = @LecturerID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@LecturerID", lecturer.LecturerID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

        } }

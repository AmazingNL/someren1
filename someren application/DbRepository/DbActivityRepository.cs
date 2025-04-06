using Microsoft.Data.SqlClient;
using someren_application.Models;
using someren_application.Repositories;
using System.Data;
using System.Diagnostics;

namespace someren_application.DbRepository
{
    public class DbActivityRepository : IActivityRepository
    {
        private readonly string? _connectionString;

        public DbActivityRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SomerenConnection");
        }
        List<ActivitiesModel> IActivityRepository.GetAll()
        {
            List<ActivitiesModel> activities = [];
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT [activityId], [activityName], [timeSlot] FROM [activity] ORDER BY activityName";
                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ActivitiesModel activity = ReadActivity(reader);
                            activities.Add(activity);
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
            return activities;
        }

        private ActivitiesModel ReadActivity(SqlDataReader reader)
        {
            // Retrieve data from room table
            int activityId = (int)reader["activityId"];
            string activityName = (string)reader["activityName"];
            DateTime timeSlot = (DateTime)reader["timeSlot"];
            // Return new User object
            return new ActivitiesModel(activityId, activityName, timeSlot);
        }

        ActivitiesModel? IActivityRepository.GetById(int activityId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT activityId, activityName, timeSlot FROM [activity] WHERE activityId = @ActivityId"; // search for the room and return its record
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.Add("@ActivityId", SqlDbType.Int).Value = activityId;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) // If a record is found
                    {
                        return new ActivitiesModel(
                            (int)reader["activityId"],
                            (string)reader["activityName"],
                            (DateTime)reader["timeSlot"]
                        );
                    }
                }
            }
            return null; // Return null if no room is found
        }
        void IActivityRepository.Add(ActivitiesModel activity)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                string query = "INSERT INTO [activity] (activityName, timeSlot)" +  // Inserting SQL Query to the room table
                    " VALUES (@ActivityName, @TimeSlot);" +
                    " SELECT SCOPE_IDENTITY();";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ActivityName", activity.ActivityName); //  prevent SQL injection to occur by using SQL parameters
                    command.Parameters.AddWithValue("@TimeSlot", activity.TimeSlot);
                    try
                    {
                        connection.Open(); // open the connection

                        int nrOfRowsAffected = command.ExecuteNonQuery();

                        if (nrOfRowsAffected != 1)
                        {
                            throw new Exception("Adding activity failed!");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Something went wrong", ex);
                    }
                }
            }
        }

        void IActivityRepository.Update(ActivitiesModel activity)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [activity] SET activityName = @ActivityName, timeSlot = @TimeSlot WHERE [activityId] = @ActivityId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ActivityId", activity.ActivityId);
                command.Parameters.AddWithValue("@ActivityName", activity.ActivityName); //  prevent SQL injection to occur by using SQL parameters
                command.Parameters.AddWithValue("@TimeSlot", activity.TimeSlot);
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
        void IActivityRepository.Delete(ActivitiesModel activity)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [activity] WHERE activityId = @ActivityId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ActivityId", activity.ActivityId);

                    connection.Open();
                    int nrOfRowsAffected = command.ExecuteNonQuery();

                    if (nrOfRowsAffected == 0)
                    {
                        throw new Exception("No records deleted!");
                    }
                }
            }
        }
        // need to add tables in the database
        List<Students> IActivityRepository.GetParticipants(int activityId)
        {
            List<Students> students = new List<Students>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT s.studentId, s.firstName, s.lastName FROM students s " +
                               "JOIN activityParticipants ap ON s.studentId = ap.StudentId " +
                               "WHERE ap.ActivityId = @ActivityId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ActivityId", activityId);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int studentId = (int)reader["studentId"];
                        string firstName = (string)reader["firstName"];
                        string lastName = (string)reader["lastName"];
                        students.Add(new Students(studentId, firstName, lastName));
                    }
                }
            }
            return students;
        }
        void IActivityRepository.AddParticipant(int activityId, int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO activityParticipants (activityId, studentId) VALUES (@ActivityId, @StudentId)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ActivityId", activityId);
                command.Parameters.AddWithValue("@StudentId", studentId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public void RemoveParticipant(int activityId, int studentId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM activityParticipants (activityId, studentId) VALUES (@ActivityId, @StudentId)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StudentId", studentId);
                command.Parameters.AddWithValue("@ActivityId", activityId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

    }
}

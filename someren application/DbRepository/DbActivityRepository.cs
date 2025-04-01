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
        List<Activities> IActivityRepository.GetAll()
        {
            List<Activities> activities = [];
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
                            Activities activity = ReadActivity(reader);
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

        private Activities ReadActivity(SqlDataReader reader)
        {
            // Retrieve data from room table
            int activityId = (int)reader["activityId"];
            string activityName = (string)reader["activityName"];
            DateTime timeSlot = (DateTime)reader["timeSlot"];
            // Return new User object
            return new Activities(activityId, activityName, timeSlot);
        }

        Activities? IActivityRepository.GetById(int activityId)
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
                        return new Activities(
                            (int)reader["activityId"],
                            (string)reader["activityName"],
                            (DateTime)reader["timeSlot"]
                        );
                    }
                }
            }
            return null; // Return null if no room is found
        }
        void IActivityRepository.Add(Activities activity)
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

        void IActivityRepository.Update(Activities activity)
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
        void IActivityRepository.Delete(Activities activity)
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
    }
}

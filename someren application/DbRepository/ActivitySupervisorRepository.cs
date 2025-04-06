using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using someren_application.Models;
using someren_application.Repositories;
using System.Diagnostics; //is it correct?

namespace someren_application.DbRepository
{
    public class ActivitySupervisorRepository : ISupervisionRepository
    {
        private readonly string? connectionString = "Server=tcp:den1.mssql7.gear.host,1433;Initial Catalog=dbproject242503;Persist Security Info=False;User ID=dbproject242503;Password=Gn2qn~?7Cx51;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;";


        public Activities getActivityById(int activityId)
        {
            Activities _activity = null;
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM activity WHERE activityId = @id", conn);
                cmd.Parameters.AddWithValue("@id", activityId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _activity = new Activities
                        {
                            ActivityId = (int)reader["activityId"],
                            ActivityName = reader["activityName"].ToString(),
                            TimeSlot = (DateTime)reader["timeSlot"]
                        };
                    }
                }
            }

            return _activity;

        }

        public List<Activities> GetAllActivities()
        {
            var activities = new List<Activities>();
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM activity", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        activities.Add(new Activities
                        {
                            ActivityId = (int)reader["activityId"],
                            ActivityName = reader["activityName"].ToString(),
                            TimeSlot = (DateTime)reader["timeSlot"]
                        });
                    }
                }
            }
            return activities;
        }


        public List<Lecturer> GetAllLecturers()
        {
            List<Lecturer> lecturers = new List<Lecturer>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT lecturerId, firstName, lastName FROM lecturer";  // Replace with your actual table name
                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    lecturers.Add(new Lecturer
                    {
                        LecturerID = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2)
                    });
                }
            }

            return lecturers;
        }

        public List<Lecturer> GetNonSupervisors(List<Lecturer> supervisors)
        {
            var list = new List<Lecturer>();

            List<Lecturer> lecturers = new List<Lecturer>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT lecturerId, firstName, lastName FROM lecturer";  // Replace with your actual table name
                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    lecturers.Add(new Lecturer
                    {
                        LecturerID = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2)
                    });
                }
            }

            for (int i = 0; i < lecturers.Count(); i++)
            {
                var exists = false;
                for (int j = 0; j < supervisors.Count() ; j++)
                {
                    if (lecturers[i].LecturerID == supervisors[j].LecturerID)
                    {
                        exists = true;
                        break;
                    }
                }

                if(!exists)
                {
                    list.Add(lecturers[i]);
                }
            }

            return list;

        }
        

        // Fetch supervisors for a specific activity
        public List<Lecturer> GetSupervisorsForActivity(int activityId)
        {
            var list = new List<Lecturer>();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"
                    SELECT s.superviceId, s.lecturerId, s.activityId, 
                           l.firstName, l.lastName, l.phoneNumber, l.age
                    FROM supervice s
                    JOIN lecturer l ON s.lecturerId = l.lecturerId
                    WHERE s.activityId = @activityId", conn);

                cmd.Parameters.AddWithValue("@activityId", activityId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Lecturer
                        {
                                LecturerID = (int)reader["lecturerId"],
                                FirstName = reader["firstname"].ToString(),
                                LastName = reader["lastName"].ToString(),
                                PhoneNumber = reader["phoneNumber"].ToString(),
                                Age = (int)reader["age"]
                            
                        });
                    }
                }
            }

            return list;
        }

        // Fetch non-supervisors for a specific activity
        public List<Lecturer> GetNonSupervisorsForActivity(int activityId)
        {
            var allLecturers = GetAllLecturers();
            var supervisors = GetSupervisorsForActivity(activityId);
            var nonSupervisors = allLecturers.Except(supervisors).ToList();
            return nonSupervisors;
        }

        // Add supervisor to activity
        public void AddSupervisorToActivity(int activityId, int lecturerId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO supervice (superviceId,activityId, lecturerId) VALUES (@SuperviceId, @ActivityId, @LecturerId)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ActivityId", activityId);
                command.Parameters.AddWithValue("@LecturerId", lecturerId);
                command.Parameters.AddWithValue("@SuperviceId", int.Parse(activityId.ToString()+lecturerId.ToString()));

                command.ExecuteNonQuery();
            }
        }

        // Remove supervisor from activity
        public void RemoveSupervisorFromActivity(int activityId, int lecturerId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM supervice WHERE activityId = @ActivityId AND lecturerId = @LecturerId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ActivityId", activityId);
                command.Parameters.AddWithValue("@LecturerId", lecturerId);

                command.ExecuteNonQuery();
            }
        }
    }

}

using Microsoft.Data.SqlClient;
using someren_application.IRepositories;
using someren_application.Models;
using someren_application.Repositories;
using System.Data;

namespace someren_application.DbRepository
{
    public class DbDrinksRepository : IDrinksRepository
    {
        private readonly string? _connectionString;

        public DbDrinksRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SomerenConnection");
        }

        void IDrinksRepository.Add(Drinks drink)
        {
            using SqlConnection connection = new(_connectionString);
            string? query = "INSERT INTO [drinks] (drinkName, isAlcoholic, vatRate, quantity)" +
                " VALUES (@DrinkName, @IsAlcoholic, @VatRate, @Quantity)" +
                "SELECT SCOPE_IDENTITY();";
            using SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@DrinkName", drink.DrinkName);
            command.Parameters.AddWithValue("@IsAlcoholic", drink.IsAlcoholic);
            command.Parameters.AddWithValue("@VatRate", drink.VatRate);
            command.Parameters.AddWithValue("@Quantity", drink.Quantity);

            try
            {
                connection.Open();
                int numOfRowsAffected = command.ExecuteNonQuery();

                if (numOfRowsAffected != 1)
                {
                    throw new Exception("No rows affected");
                }
            }
            catch (Exception)
            {

                throw new Exception("something went wrong with database");
            }
        }

        void IDrinksRepository.Delete(Drinks drink)
        {
            using SqlConnection connection = new(_connectionString);
            string query = "DELETE FROM [drinks] WHERE drinkId = @DrinkId;";

            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DrinkId", drink.DrinkId);

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

        List<Drinks> IDrinksRepository.Filter(string isAlcoholic)
        {
            List<Drinks> drinks = [];

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT drinkId, drinkName, isAlcoholic, vatRate, quantity FROM [drinks] WHERE isAlcoholic = @IsAlcoholic;";
                SqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@IsAlcoholic", isAlcoholic);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Drinks drink = new Drinks
                            {
                                DrinkId = Convert.ToInt32(reader["drinkId"]),
                                DrinkName = reader["drinkName"].ToString(),
                                IsAlcoholic = reader["isAlcoholic"].ToString(),
                                VatRate = Convert.ToInt32(reader["vatRate"]),
                                Quantity = Convert.ToInt32(reader["quantity"])
                            };
                            drinks.Add(drink);
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
            return drinks;
        }

        List<Drinks> IDrinksRepository.GetAllDrink()
        {
            List<Drinks> drinks = [];

            using (SqlConnection connection = new(_connectionString))
            {
                string query = "SELECT drinkId, drinkName, isAlcoholic, vatRate, quantity FROM [drinks];";
                SqlCommand command = new(query, connection);
                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            drinks.Add(ReadDrink(reader));
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
            return drinks;
        }

        private Drinks ReadDrink(SqlDataReader reader)
        {
            // Retrieve data from drink table
            int drinkId = (int)reader["drinkId"];
            string drinkName = (string)reader["drinkName"];
            string isAlcoholic = (string)reader["isAlcoholic"];
            int vatRate = (int)reader["vatRate"];
            int quantity = (int)reader["quantity"];
            // Return new drink object
            return new Drinks(drinkId, drinkName, isAlcoholic, vatRate, quantity);
        }

        Drinks? IDrinksRepository.GetById(int drinkId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT drinkId, drinkName, isAlcoholic, vatRate, quantity FROM [drinks] WHERE drinkId = @DrinkId"; // search for the drink and return its record
                SqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@DrinkId", drinkId);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // If a record is found
                        {
                            return ReadDrink(reader);
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }

            }
            return null; // Return null if no drink is found
        }

        void IDrinksRepository.Update(Drinks drinks)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [drinks] SET drinkName = @DrinkName, isAlcoholic = @IsAlcoholic, vatRate = @VatRate, quantity = @Quantity WHERE drinkId = @DrinkId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DrinkId", drinks.DrinkId);
                command.Parameters.AddWithValue("@DrinkName", drinks.DrinkName);
                command.Parameters.AddWithValue("@IsAlcoholic", drinks.IsAlcoholic);
                command.Parameters.AddWithValue("@VatRate", drinks.VatRate);
                command.Parameters.AddWithValue("@Quantity", drinks.Quantity);
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

using Microsoft.Data.SqlClient;
using someren_application.IRepositories;
using someren_application.Models;
using System.Data;
using System.Security.Claims;

namespace someren_application.DbRepository
{
    public class DbOrderDrinkRepository : IOrderDrinkRepository
    {
        private readonly string? _connectionString;

        public DbOrderDrinkRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("SomerenConnection");
        }

        public void DeleteOrder(OrderDrinks orderDrinks)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "DELETE FROM [orderDrinks] WHERE orderId = @OrderId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", orderDrinks.OrderId);

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

        public void UpdateOrder(OrderDrinks orderDrinks)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "UPDATE [orderDrinks] SET studentId = @StudentId, drinkId = @DrinkId, totalDrink = @TotalDrink WHERE orderId = @OrderId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@orderId", orderDrinks.OrderId);
                command.Parameters.AddWithValue("@totalDrink", orderDrinks.TotalDrink);
                command.Parameters.AddWithValue("@StudentId", orderDrinks.Students.FirstOrDefault());
                command.Parameters.AddWithValue("@DrinkId", orderDrinks.Drinks.FirstOrDefault());
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

        OrderDrinks IOrderDrinkRepository.GetOrdersById(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                SELECT 
                    O.orderId, O.totalDrink, S.studentId, S.studentNumber,
                    S.firstName, S.lastName, S.studentClass, S.phonenumber, S.roomId,
                    D.drinkId, D.drinkName, D.isAlcoholic, D.vatRate, D.quantity
                FROM [orderDrinks] AS O
                JOIN [student] AS S ON O.studentId = S.studentId
                JOIN [drinks] D ON O.drinkId = D.drinkId"; // search for the room and return its record
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) // If a record is found
                    {
                        return ReadOrder(reader);
                    }
                }
            }
            throw new Exception("Order not found");
        }

        void IOrderDrinkRepository.AddOrder(OrderDrinks orderDrinks)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO [orderDrinks] (studentId, drinkId, totalDrink)" +
                               " VALUES (@StudentId, @DrinkId, @TotalDrink);" +
                               " SELECT SCOPE_IDENTITY();";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Use FirstOrDefault to safely get the first item, or null if the list is empty
                    var drink = orderDrinks.Drinks.FirstOrDefault();
                    var student = orderDrinks.Students.FirstOrDefault();

                    // Check if the drink or student is null and throw an appropriate exception if necessary
                    if (drink == null)
                    {
                        throw new InvalidOperationException("No drink selected.");
                    }
                    if (student == null)
                    {
                        throw new InvalidOperationException("No student selected.");
                    }

                    command.Parameters.AddWithValue("@DrinkId", drink.DrinkId);
                    command.Parameters.AddWithValue("@StudentId", student.StudentId);
                    command.Parameters.AddWithValue("@TotalDrink", orderDrinks.TotalDrink);

                    try
                    {
                        connection.Open(); // Open the connection

                        int nrOfRowsAffected = command.ExecuteNonQuery();

                        if (nrOfRowsAffected != 1)
                        {
                            throw new Exception("Adding drink failed!");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Something went wrong", ex);
                    }
                }
            }
        }
        
    

        List<OrderDrinks> IOrderDrinkRepository.GetAllOrders()
        {
            var orders = new List<OrderDrinks>();

            using (SqlConnection connection = new(_connectionString))
            {
                string query = @"
                SELECT 
                    O.orderId, O.totalDrink, S.studentId, 
                    S.studentNumber,S.firstName, S.lastName, S.studentClass, S.phonenumber, S.roomId,
                    D.drinkId, D.drinkName, D.isAlcoholic, D.vatRate, D.quantity
                FROM [orderDrinks] AS O
                JOIN [student] AS S ON O.studentId = S.studentId
                JOIN [drinks] D ON O.drinkId = D.drinkId";

                SqlCommand command = new(query, connection);

                try
                {
                    connection.Open();
                    using SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {

                        orders.Add(ReadOrder(reader));
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Database error while retrieving orders", ex);
                }
            }

            return orders;
        }


        private OrderDrinks ReadOrder(SqlDataReader reader)
        {
            // Retrieve data from room table
            List<Students> student = new List<Students>
            {
                new Students
                {
                    StudentId = Convert.ToInt32(reader["studentId"]),
                    StudentNumber = (string)reader["studentNumber"],
                    FirstName = (string)reader["firstName"],
                    LastName = (string)reader["lastName"],
                    PhoneNumber = (string)reader["phoneNumber"],
                    StudentClass = (string)reader["studentClass"],
                    Rooms = new List<Room> // Fix the error by converting to a list
                    {
                        new Room
                        {
                            RoomId = Convert.ToInt32(reader["roomId"]),
                            //Building = (string)reader["building"],
                            //RoomNumber = (string)reader["roomNumber"],
                            //Capacity = (int)reader["capacity"],
                            //RoomType = (string)reader["roomType"]
                        }
                    }
                }
            };
            List<Drinks> drink = new List<Drinks>
            {
                new Drinks
                {
                    DrinkId = Convert.ToInt32(reader["drinkId"]),
                    DrinkName = (string)reader["drinkName"],
                    IsAlcoholic = (string)reader["isAlcoholic"],
                    VatRate = Convert.ToInt32(reader["vatRate"]),
                    Quantity = Convert.ToInt32(reader["quantity"])
                }
            };

            int orderId = Convert.ToInt32(reader["orderId"]);
            int totalDrink = Convert.ToInt32(reader["totalDrink"]);
            return new OrderDrinks(orderId, totalDrink, student, drink);
        }
    }

    
}

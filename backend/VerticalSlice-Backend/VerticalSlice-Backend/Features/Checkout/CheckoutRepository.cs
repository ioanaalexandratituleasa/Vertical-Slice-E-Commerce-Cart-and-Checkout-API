using Microsoft.Data.SqlClient;
using System.Data;
using VerticalSlice_Backend.Common;
using VerticalSlice_Backend.Features.Checkout.CheckoutDTOs;

namespace VerticalSlice_Backend.Features.Checkout
{
    public class CheckoutRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public CheckoutRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task PlaceOrderAsync(CheckoutCreateDTO data)
        {
            if (data.Items == null || !data.Items.Any())
                throw new Exception("The order does not contain any products");

            using var connection = _dbConnectionFactory.CreateConnection();
            var sqlConn = (SqlConnection)connection;

            if (sqlConn.State != ConnectionState.Open)
                await sqlConn.OpenAsync();

            using var transaction = sqlConn.BeginTransaction();

            try
            {
                const string orderSql = @"INSERT INTO OrderTable (DateOrder, Address, UserID) 
                                         OUTPUT INSERTED.OrderID 
                                         VALUES (GETDATE(), @Addr, @UserID)";

                using var cmdOrder = new SqlCommand(orderSql, sqlConn, transaction);
                cmdOrder.Parameters.Add("@Addr", SqlDbType.VarChar).Value = (object)data.Address ?? DBNull.Value;
                cmdOrder.Parameters.Add("@UserID", SqlDbType.Int).Value = 1;

                var result = await cmdOrder.ExecuteScalarAsync();
                if (result == null) throw new Exception("Couldn't generate Order's ID");
                int orderId = (int)result;

                foreach (var item in data.Items)
                {
                    const string itemSql = @"INSERT INTO OrderItems (OrderID, ProductID, TotalPrice, Quantity) 
                                           VALUES (@OID, @PID, @Price, @Qty)";

                    using var cmdItem = new SqlCommand(itemSql, sqlConn, transaction);
                    cmdItem.Parameters.AddWithValue("@OID", orderId);
                    cmdItem.Parameters.AddWithValue("@PID", item.ProductID);
                    cmdItem.Parameters.AddWithValue("@Price", item.TotalPrice * item.Quantity);
                    cmdItem.Parameters.AddWithValue("@Qty", item.Quantity);

                    await cmdItem.ExecuteNonQueryAsync();
                    const string stockSql = @"UPDATE Products 
                                             SET Stock = Stock - @Qty 
                                             WHERE ProductID = @PID AND Stock >= @Qty";

                    using var cmdStock = new SqlCommand(stockSql, sqlConn, transaction);
                    cmdStock.Parameters.AddWithValue("@Qty", item.Quantity);
                    cmdStock.Parameters.AddWithValue("@PID", item.ProductID);

                    int rowsAffected = await cmdStock.ExecuteNonQueryAsync();

                    if (rowsAffected == 0)
                    {
                        throw new Exception($"Out of stock or the product does not exist (ID: {item.ProductID})!");
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error processing the order " + ex.Message);
            }
        }
    }
}
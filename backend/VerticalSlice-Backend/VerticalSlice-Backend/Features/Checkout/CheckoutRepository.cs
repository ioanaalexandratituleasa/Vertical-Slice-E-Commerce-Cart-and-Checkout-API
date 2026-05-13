
using System.Data;
using Microsoft.Data.SqlClient;
using VerticalSlice_Backend.Common;

namespace VerticalSlice_Backend.Features.Checkout
{
    public class CheckoutRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public CheckoutRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task PlaceOrderAsync(CheckoutDTO data)
        {
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
                cmdOrder.Parameters.AddWithValue("@Addr", data.Address);
                cmdOrder.Parameters.AddWithValue("@UserID", 1);

                int orderId = (int)await cmdOrder.ExecuteScalarAsync();

                foreach (var item in data.Items)
                {
                    const string itemSql = @"INSERT INTO OrderItems (OrderID, ProductID, TotalPrice, Quantity) 
                                           VALUES (@OID, @PID, @Price, @Qty)";

                    using var cmdItem = new SqlCommand(itemSql, sqlConn, transaction);
                    cmdItem.Parameters.AddWithValue("@OID", orderId);
                    cmdItem.Parameters.AddWithValue("@PID", item.ProductID);
                    cmdItem.Parameters.AddWithValue("@Price", (decimal)item.Price * item.Quantity);
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
                        throw new Exception($"Stoc insuficient pentru produsul ID: {item.ProductID}!");
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
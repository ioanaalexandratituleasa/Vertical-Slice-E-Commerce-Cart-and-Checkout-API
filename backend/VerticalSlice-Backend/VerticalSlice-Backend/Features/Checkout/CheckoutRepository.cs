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
            await ((SqlConnection)connection).OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string orderSql = @"INSERT INTO OrderTable (DateOrder, Address, UserID) 
                                         VALUES (GETDATE(), @Addr, @UserID);
                                         SELECT CAST(SCOPE_IDENTITY() as int);";

                using var cmdOrder = connection.CreateCommand();
                cmdOrder.CommandText = orderSql;
                cmdOrder.Transaction = transaction; 
                var paramAddr = cmdOrder.CreateParameter();
                paramAddr.ParameterName = "@Addr";
                paramAddr.Value = (object)data.Address ?? DBNull.Value;
                cmdOrder.Parameters.Add(paramAddr);

                var paramUser = cmdOrder.CreateParameter();
                paramUser.ParameterName = "@UserID";
                paramUser.Value = data.UserID;
                cmdOrder.Parameters.Add(paramUser);

                var result = await ((SqlCommand)cmdOrder).ExecuteScalarAsync();
                if (result == null) throw new Exception("Database failed to generate OrderID.");
                int orderId = (int)result;

                foreach (var item in data.Items)
                {
                    const string itemSql = @"INSERT INTO OrderItems (OrderID, ProductID, TotalPrice, Quantity) 
                                           VALUES (@OID, @PID, @Price, @Qty)";

                    using var cmdItem = connection.CreateCommand();
                    cmdItem.CommandText = itemSql;
                    cmdItem.Transaction = transaction;

                    AddParam(cmdItem, "@OID", orderId);
                    AddParam(cmdItem, "@PID", item.ProductID);
                   
                    AddParam(cmdItem, "@Price", item.TotalPrice * item.Quantity);
                    AddParam(cmdItem, "@Qty", item.Quantity);

                    await ((SqlCommand)cmdItem).ExecuteNonQueryAsync();

                    const string stockSql = @"UPDATE Products 
                                             SET Stock = Stock - @Qty 
                                             WHERE ProductID = @PID AND Stock >= @Qty";

                    using var cmdStock = connection.CreateCommand();
                    cmdStock.CommandText = stockSql;
                    cmdStock.Transaction = transaction;

                    AddParam(cmdStock, "@Qty", item.Quantity);
                    AddParam(cmdStock, "@PID", item.ProductID);

                    int rowsAffected = await ((SqlCommand)cmdStock).ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                        throw new Exception($"Stock insufficient for Product ID: {item.ProductID}");
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error processing the order: " + ex.Message);
            }
        }

        private void AddParam(IDbCommand command, string name, object value)
        {
            var p = command.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            command.Parameters.Add(p);
        }
    }
}
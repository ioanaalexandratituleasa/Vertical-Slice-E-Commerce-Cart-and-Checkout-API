using System.Data;
using VerticalSlice_Backend.Common;
using Microsoft .Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

namespace VerticalSlice_Backend.Features.Products
{
    public class ProductsRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public ProductsRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        } 

        public async Task<ProductDetailDTO> GetProductByIdAsync(int ProductID)
        {
            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open();

            const string sql = @"Select ProductID, Title, DescriptionP, Price, ImageP, Stock
                                    From Products
                                    Where ProductID = @ProductID";
            using var command = connection.CreateCommand();
            command.CommandText = sql;

            var param = command.CreateParameter();
            param.ParameterName = "@ProductID";
            param.Value = ProductID;
            command.Parameters.Add(param);

            using var reader = await ((SqlCommand)command).ExecuteReaderAsync();

            if(await reader.ReadAsync())
            {
                return new ProductDetailDTO
                {
                    ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    DescriptionP = reader.IsDBNull(reader.GetOrdinal("DescriptionP"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("DescriptionP")),
                    Price = reader.GetDouble(reader.GetOrdinal("Price")),
                    ImageP = reader.IsDBNull(reader.GetOrdinal("ImageP"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("ImageP")),
                    Stock = reader.IsDBNull(reader.GetOrdinal("Stock"))
                             ? 0
                             : reader.GetInt32(reader.GetOrdinal("Stock"))
                };
            }
            return null;
        }

        public async Task<IEnumerable<ProductDetailDTO>> GetAllProductsAsync()
        {
            var products = new List<ProductDetailDTO>();
            using var connection = _dbConnectionFactory.CreateConnection();
            connection.Open(); 

            const string sql = @"SELECT ProductID, Title, DescriptionP, Price, ImageP, Stock 
                         FROM Products";

            using var command = connection.CreateCommand();
            command.CommandText = sql;

            using var reader = await ((SqlCommand)command).ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                products.Add(new ProductDetailDTO
                {
                    ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    DescriptionP = reader.IsDBNull(reader.GetOrdinal("DescriptionP"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("DescriptionP")),
                    Price = reader.GetDouble(reader.GetOrdinal("Price")),
                    ImageP = reader.IsDBNull(reader.GetOrdinal("ImageP"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("ImageP")),
                    Stock = reader.IsDBNull(reader.GetOrdinal("Stock"))
                             ? 0
                             : reader.GetInt32(reader.GetOrdinal("Stock"))
                });
            }

            return products;
        }

    }
}

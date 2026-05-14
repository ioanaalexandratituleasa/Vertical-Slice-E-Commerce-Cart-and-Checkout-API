using Microsoft.Data.SqlClient;
using VerticalSlice_Backend.Common;
using BCrypt.Net;

namespace VerticalSlice_Backend.Features.Identity
{
    public class IdentityRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;

        public IdentityRepository(DbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<RegisterResponseDTO> RegisterAsync(RegisterDTO data)
        {
            using var connection = (SqlConnection)_dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string checkSql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
            using var cmdCheck = new SqlCommand(checkSql, connection);
            cmdCheck.Parameters.AddWithValue("@Email", data.Email);
            int count = (int)await cmdCheck.ExecuteScalarAsync();

            if (count > 0)
                throw new Exception("Email already in use.");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(data.Password);

            const string insertSql = @"
                INSERT INTO Users (FName, LName, Email, Password)
                OUTPUT INSERTED.UserID
                VALUES(@FName, @LName, @Email, @Password)";

            using var cmdInsert = new SqlCommand(insertSql, connection);
            cmdInsert.Parameters.AddWithValue("@Fname", data.FName);
            cmdInsert.Parameters.AddWithValue("@LName", data.LName);
            cmdInsert.Parameters.AddWithValue("@Email", data.Email);
            cmdInsert.Parameters.AddWithValue("@Password", hashedPassword);

            int newUserID = (int)await cmdInsert.ExecuteScalarAsync();

            return new RegisterResponseDTO(newUserID, data.FName, data.Email);
        }
    }
}

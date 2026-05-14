using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VerticalSlice_Backend.Common;


namespace VerticalSlice_Backend.Features.Identity
{
    public class IdentityRepository:IIdentityRepository
    {
        private readonly DbConnectionFactory _dbConnectionFactory;
        private readonly IConfiguration _configuration;
        public IdentityRepository(DbConnectionFactory dbConnectionFactory, IConfiguration configuration)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _configuration = configuration;
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

        public async Task<LoginResponseDTO> LoginAsync(LoginDTO data)
        {
            using var connection = (SqlConnection)_dbConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            const string sql = @"
                   SELECT UserID, FName, Email, Password
                   FROM Users
                   Where Email = @Email";

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@Email", data.Email);

            using var reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                throw new Exception("Invalid email or password");

            int userID = reader.GetInt32(reader.GetOrdinal("UserID"));
            string fName = reader.GetString(reader.GetOrdinal("FName"));
            string email = reader.GetString(reader.GetOrdinal("Email"));
            string hash = reader.GetString(reader.GetOrdinal("Password"));

            bool isValid = BCrypt.Net.BCrypt.Verify(data.Password, hash);
            if (!isValid)
                throw new Exception("Invalid email or password");

            string token = GenerateToken(userID, fName, email);

            return new LoginResponseDTO(token, userID, fName, email);

        }

        private string GenerateToken(int userID, string fName, string email)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("userID", userID.ToString()),
                new Claim("fName", fName),
                new Claim(ClaimTypes.Email, email)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

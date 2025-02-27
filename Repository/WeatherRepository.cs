using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace TelegramBot_Api.Repository;

public class WeatherRepository
{
    private readonly string _connectionString;

    public WeatherRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task AddWeatherRequestAsync(long userId, string city, float temperature, string description, float wind, DateTime createdAt)
    {
        using var connection = CreateConnection();
        var sql = @"
            INSERT INTO WeatherHistory (UserId, City, Temperature, Description, Wind, RequestTime) 
            VALUES (@UserId, @City, @Temperature, @Description, @Wind, @RequestTime);";

        await connection.ExecuteAsync(sql, new { userId, city, temperature, description, wind, requestTime = createdAt });
    }
}

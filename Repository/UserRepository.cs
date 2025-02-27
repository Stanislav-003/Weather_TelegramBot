using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using TelegramBot_Api.Models;
using static Dapper.SqlMapper;

namespace TelegramBot_Api.Repository;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task AddUserOrUpdateAsync(long id, string username, string firstName, string lastName)
    {
        using var connection = CreateConnection();
        var sql = @"
            MERGE INTO Users AS target
            USING (SELECT @Id AS Id, @Username AS Username, @FirstName AS FirstName, @LastName AS LastName) AS source
            ON target.Id = source.Id
            WHEN MATCHED THEN
                UPDATE SET Username = source.Username, FirstName = source.FirstName, LastName = source.LastName
            WHEN NOT MATCHED THEN
                INSERT (Id, Username, FirstName, LastName) 
                VALUES (source.Id, source.Username, source.FirstName, source.LastName);";

        await connection.ExecuteAsync(sql, new { id, username, firstName, lastName });
    }

    public async Task<IEnumerable<WeatherHistory>> GetUserHistoryAsync(long userId)
    {
        using var connection = CreateConnection();
        var sql = "SELECT * FROM WeatherHistory WHERE UserId = @UserId ORDER BY RequestTime DESC";
        return await connection.QueryAsync<WeatherHistory>(sql, new { UserId = userId });
    }

    public async Task<User?> GetUserWithHistoryAsync(long userId)
    {
        using var connection = CreateConnection();

        const string query = @"
            SELECT u.Id, u.UserName, u.FirstName, u.LastName
            FROM Users u
            WHERE u.Id = @UserId;

            SELECT h.Id, h.City, h.Temperature, h.Description, h.Wind, h.RequestTime, h.UserId
            FROM WeatherHistory h
            WHERE h.UserId = @UserId
            ORDER BY h.RequestTime DESC;";

        using var multi = await connection.QueryMultipleAsync(query, new { UserId = userId });

        var user = await multi.ReadSingleOrDefaultAsync<User>();
        if (user != null)
        {
            user.WeatherHistory = await multi.ReadAsync<WeatherHistory>();
        }
        return user;
    }

    public async Task<User?> GetUserByIdAsync(long? userId)
    {
        using var connection = CreateConnection();

        const string query = @"
            SELECT * FROM Users WHERE Id = @UserId;";

        return await connection.QueryFirstOrDefaultAsync<User>(query, new { UserId = userId });
    }

    public async Task<List<User>?> GetAllUsersAsync()
    {
        using var connection = CreateConnection();

        const string query = @"
            SELECT * FROM Users;";

        return (await connection.QueryAsync<User>(query)).ToList();
    }


    //IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
    //        BEGIN
    //            CREATE TABLE Users(
    //                Id BIGINT PRIMARY KEY, -- Telegram ID
    //                Username NVARCHAR(100) NULL,
    //                FirstName NVARCHAR(100) NULL,
    //                LastName NVARCHAR(100) NULL
    //            );
    //        END";


    //IF NOT EXISTS(SELECT* FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WeatherHistory]') AND type in (N'U'))
    //        BEGIN
    //            CREATE TABLE WeatherHistory(
    //                Id INT IDENTITY(1,1) PRIMARY KEY,
    //                UserId BIGINT NOT NULL,
    //                City NVARCHAR(100) NOT NULL,
    //                Temperature FLOAT NOT NULL,
    //                Description NVARCHAR(50) NOT NULL,
    //                Wind FLOAT NOT NULL,
    //                RequestTime DATETIME DEFAULT GETDATE(),
    //                FOREIGN KEY(UserId) REFERENCES Users(Id) ON DELETE CASCADE
    //            );
    //        END";
}

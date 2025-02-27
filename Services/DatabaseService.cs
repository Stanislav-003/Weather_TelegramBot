using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace TelegramBot_Api.Services;

public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public void CreateTables()
    {
        using var connection = CreateConnection();

        var usersTable = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
            BEGIN
                CREATE TABLE Users (
                    Id BIGINT PRIMARY KEY, -- Telegram ID
                    Username NVARCHAR(100) NULL,
                    FirstName NVARCHAR(100) NULL,
                    LastName NVARCHAR(100) NULL
                );
            END";

        var weatherHistoryTable = @"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WeatherHistory]') AND type in (N'U'))
            BEGIN
                CREATE TABLE WeatherHistory (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    UserId BIGINT NOT NULL,
                    City NVARCHAR(100) NOT NULL,
                    Temperature FLOAT NOT NULL,
                    Description NVARCHAR(50) NOT NULL,
                    Wind FLOAT NOT NULL,
                    RequestTime DATETIME DEFAULT GETDATE(),
                    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
                );
            END";

        connection.ExecuteAsync(usersTable);
        connection.ExecuteAsync(weatherHistoryTable);
    }
}

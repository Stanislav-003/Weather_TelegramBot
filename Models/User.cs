namespace TelegramBot_Api.Models;

public class User
{
    public long Id { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public IEnumerable<WeatherHistory>? WeatherHistory { get; set; }
}

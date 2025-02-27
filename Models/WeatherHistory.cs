namespace TelegramBot_Api.Models;

public class WeatherHistory
{
    public long Id { get; set; }
    public string City { get; set; } = null!;
    public float Temperature { get; set; }
    public string Description { get; set; } = null!;
    public float Wind { get; set; }
    public DateTime RequestTime { get; set; }
    public long UserId { get; set; }
}

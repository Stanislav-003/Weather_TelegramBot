using Telegram.Bot;
using TelegramBot_Api.Commands;
using TelegramBot_Api.Configuration;
using TelegramBot_Api.Handlers;
using TelegramBot_Api.Repository;
using TelegramBot_Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Налаштування конфігурації для бота
var botConfigSection = builder.Configuration.GetSection("BotConfiguration");
builder.Services.Configure<BotConfiguration>(botConfigSection);
builder.Services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient<ITelegramBotClient>(
    httpClient => new TelegramBotClient(botConfigSection.Get<BotConfiguration>()!.BotToken, httpClient));
builder.Services.ConfigureTelegramBotMvc();

// Cервіс для роботи з погодою
builder.Services.Configure<WeatherConfig>(builder.Configuration.GetSection("WeatherConfig"));
builder.Services.AddHttpClient<WeatherService>();

// Реєструємо команди бота як singleton
builder.Services.AddSingleton<MenuCommand>();
builder.Services.AddSingleton<StartCommand>();
builder.Services.AddSingleton<WeatherCommand>();
builder.Services.AddSingleton<CommandHandler>();
builder.Services.AddSingleton<UpdateHandler>();

builder.Services.AddSingleton<DatabaseService>(); 
builder.Services.AddSingleton<TelegramBotService>();
builder.Services.AddSingleton<UserRepository>(); 

builder.Services.AddSingleton<WeatherRepository>();

builder.Services.AddSingleton<UserService>(); 
builder.Services.AddSingleton<WeatherNotificationService>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var dbService = app.Services.GetRequiredService<DatabaseService>();
dbService.CreateTables();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

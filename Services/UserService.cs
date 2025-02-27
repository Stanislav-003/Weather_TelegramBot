using TelegramBot_Api.Models;
using TelegramBot_Api.Repository;

namespace TelegramBot_Api.Services;

public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task AddUserOrUpdate(long id, string username, string firstName, string lastName)
    {
        await _userRepository.AddUserOrUpdateAsync(id, username, firstName, lastName);
    }

    public async Task<IEnumerable<WeatherHistory>> GetUserHistory(long userId)
    {
        return await _userRepository.GetUserHistoryAsync(userId);
    }

    public async Task<User?> GetUserWithHistory(long userId)
    {
        return await _userRepository.GetUserWithHistoryAsync(userId);
    }

    public async Task<User?> GetUserById(long? userId = null)
    {
        return await _userRepository.GetUserByIdAsync(userId);
    }

    public async Task<List<User>?> GetAllUsers()
    {
        return await _userRepository.GetAllUsersAsync();
    }
}

using BookManagement.Models.DTO;

namespace BookManagement.DataAccess.IServices;

public interface IUserService
{
    Task<bool> RegisterUserAsync(UserDto userDto);
    Task<string> AuthenticateUserAsync(string email, string password);
}

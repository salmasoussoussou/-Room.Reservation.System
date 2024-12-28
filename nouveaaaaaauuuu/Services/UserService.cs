using nouveaaaaaauuuu.models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace nouveaaaaaauuuu.Services
{
    public class UserService : IUserService
    {
        private readonly List<AppUser> _users = new List<AppUser>
        {
            new AppUser { Username = "testuser", Password = "password123" }
        };

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            return await Task.FromResult(_users.Any(u => u.Username == username && u.Password == password));
        }

        public async Task RegisterUserAsync(AppUser user)
        {
            if (_users.All(u => u.Username != user.Username))
            {
                _users.Add(user);
                await Task.CompletedTask;
            }
            else
            {
                throw new Exception("Username already exists.");
            }
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            return await Task.FromResult(user);
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            return await Task.FromResult(_users);
        }
    }
}

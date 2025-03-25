using Core.Entities;
using Core.Interfaces;
using System.Threading.Tasks;

namespace Core.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.FindAsync(u => u.Id == id);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            await _userRepository.DeleteAsync(user);
            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.FindAsync(u => u.Id == id);
            if (user == null) return false;
            await _userRepository.DeleteAsync(user);
            return true;
        }
    }
}

using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Core.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IPasswordHasher<User> _userpasswordHasher;

        public UserService(IRepository<User> userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
           _userpasswordHasher = passwordHasher;
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
            user.PasswordHash = _userpasswordHasher.HashPassword(user,user.PasswordHash);
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

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

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.FindAsync(u => u.Id == id);
        }
    }
}


using Core.Entities;

namespace Core.Interfaces
{

    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);
    }
}

using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Core.Services
{
    public class BranchAuthService : IBranchAuthService
    {
        private readonly IRepository<Branch> _branchRepository;
        private readonly IPasswordHasher<Branch> _passwordHasher;

        public BranchAuthService(IRepository<Branch> branchRepository, IPasswordHasher<Branch> passwordHasher)
        {
            _branchRepository = branchRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Branch> AuthenticateBranchAsync(string unp, string password)
        {
            var branch = await _branchRepository.FindAsync(b => b.UNP == unp);
            if (branch == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(branch, branch.PasswordHash, password);
            return result == PasswordVerificationResult.Success ? branch : null;
        }
    }
}
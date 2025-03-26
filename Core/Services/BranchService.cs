using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.Services
{
    public class BranchService : IBranchService
    {
        private readonly IRepository<Branch> _branchRepository;

        public BranchService(IRepository<Branch> branchRepository)
        {
            _branchRepository = branchRepository;
        }

        public async Task<IEnumerable<Branch>> GetAllBranchesAsync()
        {
            return await _branchRepository.GetAllAsync();
        }

        public async Task<Branch> GetBranchByIdAsync(int? id)
        {
            return await _branchRepository.FindAsync(b => b.Id == id);
        }

        public async Task<Branch> CreateBranchAsync(Branch branch)
        {
            await _branchRepository.AddAsync(branch);
            return branch;
        }

        public async Task<Branch> UpdateBranchAsync(Branch branch)
        {
            await _branchRepository.DeleteAsync(branch);
            return branch;
        }

        public async Task<bool> DeleteBranchAsync(int id)
        {
            var branch = await _branchRepository.FindAsync(b => b.Id == id);
            if (branch == null) return false;
            await _branchRepository.DeleteAsync(branch);
            return true;
        }
    }
}

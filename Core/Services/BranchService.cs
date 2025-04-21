using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTO;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Core.Services
{
    public class BranchService : IBranchService
    {
        private readonly IRepository<Branch> _branchRepository;
        private readonly IPasswordHasher<Branch> _branchpasswordHasher;

        public BranchService(IRepository<Branch> branchRepository, 
            IPasswordHasher<Branch> bramchpasswordhasher)
        {
            _branchRepository = branchRepository;
            _branchpasswordHasher = bramchpasswordhasher;
        }


        public async Task<Branch> CreateBranchAsync(BranchDto dto)
        {
            var entity = new Branch
            {
                Name = dto.Name,
                Shortname = dto.Shortname,
                UNP = dto.UNP,
                OKPO = dto.OKPO,
                OKYLP = dto.OKYLP,
                Region = dto.Region,
                Address = dto.Address,
                Email = dto.Email,
                GoverningName = dto.GoverningName,
                HeadName = dto.HeadName,
                Supervisor = dto.Supervisor,
                ChiefAccountant = dto.ChiefAccountant
            };
            entity.PasswordHash = _branchpasswordHasher.HashPassword(entity, dto.Password);
            return await _branchRepository.AddAsync(entity);
        }

        public async Task<bool> DeleteBranchAsync(int id)
        {
            var branch = await _branchRepository.FindAsync(b => b.Id == id);
            if (branch == null) return false;
            await _branchRepository.DeleteAsync(branch);
            return true;
        }

        public async Task<IEnumerable<BranchDto>> GetAllBranchesDtosAsync()
        {
            return (await _branchRepository.GetAllAsync()).Select(b => new BranchDto
            {
                Name = b.Name,
                Shortname = b.Shortname,
                UNP = b.UNP!,
                OKPO = b.OKPO,
                OKYLP = b.OKYLP,
                Region = b.Region,
                Address = b.Address,
                Email = b.Email,
                GoverningName = b.GoverningName,
                HeadName = b.HeadName,
                Supervisor = b.Supervisor,
                ChiefAccountant = b.ChiefAccountant,
                Password = "" // not returned
            });
        }

        public async Task<IEnumerable<Branch>> GetAllBranchesAsync()
        {
            return await _branchRepository.GetAllAsync();
        }

        public async Task<Branch> GetBranchByIdAsync(int? id)
        {
            return await _branchRepository.FindAsync(b => b.Id == id);
        }


        public async Task<Branch> UpdateBranchAsync(BranchProfileDto dto)
        {
            var branch = await _branchRepository.FindAsync(b=>b.Id==dto.Id) ?? throw new Exception("Филиал не найден.");
            branch.GoverningName = dto.GoverningName;
            branch.HeadName = dto.HeadName;
            branch.Name = dto.Name;
            branch.Shortname = dto.Shortname;
            branch.UNP = dto.UNP;
            branch.OKPO = dto.OKPO;
            branch.OKYLP = dto.OKYLP;
            branch.Region = dto.Region;
            branch.Address = dto.Address;
            branch.Email = dto.Email;
            branch.Supervisor = dto.Supervisor;
            branch.ChiefAccountant = dto.ChiefAccountant;

            return await _branchRepository.UpdateAsync(branch);
        }
    }
}

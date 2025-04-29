using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DTO;
using Core.Entities;
using Core.Interfaces;

namespace Core.Services
{
    public class RoleService:IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RoleService(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            return (await _unitOfWork.SystemRoles.GetAllAsync())
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    RoleName = r.RoleName
                });
        }
        public async Task<RoleDto> GetRoleByNameAsync(string name)
        {
            var role = await _unitOfWork.SystemRoles.FindAsync(r => r.RoleName == name);
            return new RoleDto
            {
                Id = role.Id,
                RoleName = role.RoleName
            };

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Core.Interfaces
{
    public interface IBranchAuthService
    {
        Task<Branch> AuthenticateBranchAsync(string unp, string password);
    }
}

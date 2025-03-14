using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.DTO.Employee;

namespace Core.Interfaces.Services
{
    public interface IAccountService
    {
       // public Task<bool> IsAuthenticatedLogin(string login, string password);
        //Task Register(PasswordEditDTO employeeDto);
        public string GetMD5HashData(string input);
       // Task<EmployeeDTO> GetUserByUsername(string username);
    }
}

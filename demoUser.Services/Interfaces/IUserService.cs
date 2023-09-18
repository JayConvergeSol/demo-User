using demoUser.Core.Entities;
using demoUser.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demoUser.Services.Interfaces
{
    public interface IUserService
    {
        //Task<User> GetByIdAsync(string id);
        Task<User> GetByEmailAsync(string email);
        Task AddAsync(User user);
        Task ResetPassword(string email, string password);
        Task ChangePassword(string email, string newPassword);
    }
}

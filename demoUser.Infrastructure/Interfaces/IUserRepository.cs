
using demoUser.Infrastructure.DTO;
using demoUser.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demoUser.Infrastructure.Interfaces
{
    public interface IUserRepository
    {
        //Task<User> GetByIdAsync(string id);
        Task<User> GetByEmail(string email);
        Task AddAsync(User user);
        Task ResetPassword(string email, string password);
        Task ChangePassword(string email, string newPassword);

    }
}

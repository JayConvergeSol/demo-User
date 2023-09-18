using demoUser.Core.Entities;
using demoUser.Infrastructure.DTO;
using demoUser.Infrastructure.Interfaces;
using demoUser.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demoUser.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task AddAsync(User user)
        {
            await _userRepository.AddAsync(user);
        }

        public async Task ChangePassword(string email, string newPassword)
        {
           await _userRepository.ChangePassword(email,newPassword);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmail(email);
        }

        public async Task ResetPassword(string email, string password)
        {
           await _userRepository.ResetPassword(email, password);
        }
    }
}

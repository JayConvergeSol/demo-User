using demoUser.Core.Entities;
using demoUser.Infrastructure.Interfaces;
using demoUser.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demoUser.Services.Services
{
    public class UserOtpService : IUserOtpService 
    {
        private readonly IUserOtpRepository _otpRepository;

        public UserOtpService(IUserOtpRepository otpRepository)
        {
            _otpRepository = otpRepository;
        }

        public async Task AddAsync(userOTP userOtp)
        {
           await _otpRepository.AddAsync(userOtp);
        }

        public async Task DeleteAsync(userOTP userOtp)
        {
            await _otpRepository.DeleteAsync(userOtp);
        }

        public async Task<userOTP> GetById(string id)
        {
            return await _otpRepository.GetById(id);
        }


        public Task<userOTP> GetByUserId(string userId)
        {
            return _otpRepository.GetByUserId(userId);
        }

        public Task<bool> IsValidOtp(User user)
        {
            return _otpRepository.IsValidOtp(user);
        }

        public async Task UpdateAsync(userOTP userOtp)
        {
            await _otpRepository.UpdateAsync(userOtp);
        }
    }
}

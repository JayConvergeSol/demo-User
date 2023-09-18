using demoUser.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demoUser.Services.Interfaces
{
    public interface IUserOtpService
    {
        Task<userOTP> GetById(string id);
        Task<userOTP> GetByUserId(string userId);
        Task AddAsync(userOTP userOtp);
        Task UpdateAsync(userOTP userOtp);
        Task DeleteAsync(userOTP userOtp);
        Task<bool> IsValidOtp(User user);
    }
}

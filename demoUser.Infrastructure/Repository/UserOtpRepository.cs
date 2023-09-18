using demoUser.Core.Entities;
using demoUser.Infrastructure.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demoUser.Infrastructure.Repository
{
    public class UserOtpRepository : IUserOtpRepository
    {
        private readonly IMongoCollection<userOTP> _userOtpCollection;

        public UserOtpRepository(IMongoDatabase database)
        {
            _userOtpCollection = database.GetCollection<userOTP>("UserOTP");
        }

        public Task<userOTP> GetById(string id)
        {
            return _userOtpCollection.Find(otp => otp.Id == id).SingleOrDefaultAsync();
        }

        public async Task<userOTP> GetByUserId(string userId)
        {
           var unsortedlist = _userOtpCollection.Find(otp => otp.UserId == userId).ToList();
            var sortedlist = unsortedlist.OrderByDescending(a => a.CreationTime).FirstOrDefault();
            return sortedlist;
        }

        public async Task AddAsync(userOTP userOtp)
        {
            await _userOtpCollection.InsertOneAsync(userOtp);
        }

        public async Task UpdateAsync(userOTP userOtp)
        {
            var filter = Builders<userOTP>.Filter.Eq(otp => otp.Id, userOtp.Id);
            await _userOtpCollection.ReplaceOneAsync(filter, userOtp);
        }

        public async Task DeleteAsync(userOTP userOtp)
        {
            var filter = Builders<userOTP>.Filter.Eq(otp => otp.Id, userOtp.Id);
            await _userOtpCollection.DeleteOneAsync(filter);
        }

        public async Task<bool> IsValidOtp(User user)
        {
            userOTP otpRow = await GetByUserId(user.Id);
            if (otpRow == null)
            {
                return false;
            }
            var chechExpiration = DateTime.Compare(DateTime.Now, Convert.ToDateTime(otpRow.ExpirationTime));
            if (chechExpiration < 0 && otpRow.IsUsed == false)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
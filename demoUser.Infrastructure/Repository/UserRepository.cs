using demoUser.Core.Entities;
using demoUser.Infrastructure.DTO;
using demoUser.Infrastructure.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demoUser.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _usersCollection;

        public UserRepository(IMongoDatabase database)
        {
            _usersCollection = database.GetCollection<User>("user");
        }

        public Task<User> GetById(string id)
        {
            return _usersCollection.Find(u => u.Id == id).SingleOrDefaultAsync();
        }

        public Task<User> GetByEmail(string email)
        {
            return _usersCollection.Find(u => u.Email == email).SingleOrDefaultAsync();  
        }

        public async Task AddAsync(User user)
        {
            await _usersCollection.InsertOneAsync(user);
        }

        public async Task ResetPassword(string email, string password)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email,email);
            var user = await GetByEmail(email);

            var update = Builders<User>.Update
           .Set(u => u.Email, user.Email)
           .Set(u => u.PasswordHash, password);

            await _usersCollection.UpdateOneAsync(filter, update);
        }

        public async Task ChangePassword(string email, string newPassword)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);

            var update = Builders<User>.Update
           .Set(u => u.Email, email)
           .Set(u => u.PasswordHash, newPassword);

            await _usersCollection.UpdateOneAsync(filter, update);
        }
    }
}

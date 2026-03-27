using Infrastructure.Ef.Authentication;
using Infrastructure.Ef.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Ef.User
{
    public class UserRepository : IUserRepository
    {
        private readonly ManagementHubContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UserRepository(ManagementHubContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public DbUser Create(string username, string password)
        {
            var user = new DbUser
            {
                Username = username,
                Password = _passwordHasher.HashPassword(password),
                Created_at = DateTime.UtcNow.AddHours(1.0),
                Updated_at = DateTime.UtcNow.AddHours(1.0),
                IsBanned = false
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public DbUser FetchByUsername(string username)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == username);
            if (user == null) throw new KeyNotFoundException($"User with username {username} has not been found");
            return user;
        }
    }
}

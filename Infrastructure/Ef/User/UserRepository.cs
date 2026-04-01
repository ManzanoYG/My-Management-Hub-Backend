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

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var userToUpdate = _context.Users.FirstOrDefault(u => u.Username == username);
            if (userToUpdate == null) throw new KeyNotFoundException($"User with username {username} has not benn found");

            if (!_passwordHasher.VerifyPassword(userToUpdate.Password, oldPassword))
            {
                return false;
            } else
            {
                userToUpdate.Password = _passwordHasher.HashPassword(newPassword);
                userToUpdate.Updated_at = DateTime.UtcNow;
                _context.SaveChanges();
            }
            return true;
        }

        public DbUser Create(string username, string password, string timeZone)
        {
            var user = new DbUser
            {
                Username = username,
                Password = _passwordHasher.HashPassword(password),
                TimeZone = timeZone,
                Created_at = DateTime.UtcNow,
                Updated_at = DateTime.UtcNow,
                IsBanned = false
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public bool Delete(string username)
        {
            var userToDelete = _context.Users.FirstOrDefault(u => u.Username == username);
            if(userToDelete == null) throw new KeyNotFoundException($"User with id {username} has not been found");

            _context.Users.Remove(userToDelete);
            _context.SaveChanges();
            return true;
        }

        public DbUser FetchByUsername(string username)
        {
            var user = _context.Users.FirstOrDefault(x => x.Username == username);
            if (user == null) throw new KeyNotFoundException($"User with username {username} has not been found");
            return user;
        }
    }
}

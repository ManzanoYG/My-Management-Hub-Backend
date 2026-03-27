using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Ef.Authentication
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int Cost = 12;
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: Cost);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            Console.WriteLine(password);
            Console.WriteLine(hashedPassword);

            if (string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrWhiteSpace(password)) return false;
            return BCrypt.Net.BCrypt.Verify(hashedPassword, password);
        }
    }
}

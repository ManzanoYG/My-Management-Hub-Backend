using Infrastructure.Ef.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Ef.User
{
    public interface IUserRepository
    {
        DbUser Create(string username, string password, string timeZone);
        DbUser FetchByUsername(string username);
        DbUser FetchById(Guid id);
        bool ChangePassword(Guid userId, string oldPassword, string newPassword);
        bool Delete(Guid userId);
    }
}

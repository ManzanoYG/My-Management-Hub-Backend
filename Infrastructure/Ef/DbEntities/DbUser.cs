using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Ef.DbEntities
{
    public class DbUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get;set; }
        public bool IsBanned { get; set; }
        public byte UserType { get; set; }
    }
}

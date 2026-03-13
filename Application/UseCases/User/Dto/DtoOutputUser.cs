using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.User.Dto
{
    public class DtoOutputUser
    {
        public string Username { get; set; }
        public DateTime Created_at { get; set; }
        public byte UserType { get; set; }
    }
}

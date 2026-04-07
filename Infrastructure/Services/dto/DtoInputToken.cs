using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.dto
{
    public class DtoInputToken
    {
        public Guid userID { get; set; }
        public string userType { get; set; }
    }
}

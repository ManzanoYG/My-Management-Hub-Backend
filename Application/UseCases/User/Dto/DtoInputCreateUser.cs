using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.User.Dto
{
    public class DtoInputCreateUser
    {
        [Required] public string UserName { get; set; }
        [Required] public string Password { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.User.Dto
{
    public class DtoInputDeleteUser
    {
        [Required] public string Username { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Authentication.Dtos
{
    public class DtoOutputUserLogin
    {
        public string UserName { get; set; }
        public byte UserType { get; set; }
    }
}

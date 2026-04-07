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
        public bool isLogged { get; set; }
        public Guid userId { get; set; }
        public string username { get; set; }
        public byte usertype { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Note.Dto
{
    public class DtoOutputCreateNote
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
    }
}

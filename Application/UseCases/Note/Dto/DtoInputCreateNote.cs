using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Note.Dto
{
    public class DtoInputCreateNote
    {
        [Required] public string Title { get; set; }
        [Required] public string Content { get; set; }
        public string Style { get; set; }
    }
}

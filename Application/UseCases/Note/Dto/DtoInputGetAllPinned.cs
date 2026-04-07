using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Note.Dto
{
    public class DtoInputGetAllPinned
    {
        [Required] public Guid userId { get; set; }
        [Range(1, int.MaxValue)] public int pageNumber { get; set; } = 1;
        [Range(1, 100)] public int pageSize { get; set; } = 20;
    }
}

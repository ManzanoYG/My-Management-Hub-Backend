using Application.UseCases.Note.Dto;
using Application.UseCases.Utils;
using AutoMapper;
using Infrastructure.Ef.Note;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Note
{
    public class UseCaseGetAllNotePinned : IUseCaseParameterizeQuery<List<DtoOutputGetAllPinned>, DtoInputGetAllPinned>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IMapper _mapper;

        public UseCaseGetAllNotePinned(IMapper mapper, INoteRepository noteRepository)
        {
            _mapper = mapper;
            _noteRepository = noteRepository;
        }

        public List<DtoOutputGetAllPinned> Execute(DtoInputGetAllPinned input)
        {
            var dbNotes = _noteRepository.GetAllPinned(input.userId, input.pageNumber, input.pageSize);
            return _mapper.Map<List<DtoOutputGetAllPinned>>(dbNotes);
        }
    }
}

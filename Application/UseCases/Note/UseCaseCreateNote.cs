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
    public class UseCaseCreateNote : IUseCaseParameterizeQuery<DtoOutputCreateNote, DtoInputCreateNote, Guid>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IMapper _mapper;

        public UseCaseCreateNote(IMapper mapper, INoteRepository noteRepository)
        {
            _mapper = mapper;
            _noteRepository = noteRepository;
        }

        public DtoOutputCreateNote Execute(DtoInputCreateNote input, Guid userId)
        {
            var dbNote = _noteRepository.Create(userId, input.Title, input.Content, input.Style);
            return _mapper.Map<DtoOutputCreateNote>(dbNote);
        }
    }
}

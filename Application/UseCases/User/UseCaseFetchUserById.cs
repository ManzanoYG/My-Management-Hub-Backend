using Application.UseCases.User.Dto;
using Application.UseCases.Utils;
using AutoMapper;
using Infrastructure.Ef.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.User
{
    public class UseCaseFetchUserById : IUseCaseParameterizeQuery<DtoOutputUser, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UseCaseFetchUserById(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public DtoOutputUser Execute(Guid id)
        {
            var dbUser = _userRepository.FetchById(id);
            return _mapper.Map<DtoOutputUser>(dbUser);
        }
    }
}

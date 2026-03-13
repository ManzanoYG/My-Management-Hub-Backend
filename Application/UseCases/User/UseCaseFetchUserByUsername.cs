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
    public class UseCaseFetchUserByUsername : IUseCaseParameterizeQuery<DtoOutputUser, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UseCaseFetchUserByUsername(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public DtoOutputUser Execute(string username)
        {
            var dbUser = _userRepository.FetchByUsername(username);
            return _mapper.Map<DtoOutputUser>(dbUser);
        }
    }
}

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
    public class UseCaseCreateUser : IUseCaseWriter<DtoOutputUser, DtoInputCreateUser>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UseCaseCreateUser(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        
        public DtoOutputUser Execute(DtoInputCreateUser input)
        {
            var dbUser = _userRepository.Create(input.UserName, input.Password);
            return _mapper.Map<DtoOutputUser>(dbUser);
        }
    }
}

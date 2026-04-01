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
    public class UseCaseDeleteUser : IUseCaseParameterizeQuery<DtoOutputDeleteUser, DtoInputDeleteUser>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UseCaseDeleteUser(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public DtoOutputDeleteUser Execute(DtoInputDeleteUser input)
        {
            var dbUser = _userRepository.Delete(input.Username);
            return _mapper.Map<DtoOutputDeleteUser>(new DtoOutputDeleteUser
            {
                Deleted = dbUser
            });
        }
    }
}

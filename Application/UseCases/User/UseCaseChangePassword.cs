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
    public class UseCaseChangePassword : IUseCaseParameterizeQuery<DtoOutputChangePassword, DtoInputChangePassword>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UseCaseChangePassword(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public DtoOutputChangePassword Execute(DtoInputChangePassword input)
        {
            var dbUser = _userRepository.ChangePassword(input.Username, input.OldPassword, input.NewPassword);

            return _mapper.Map<DtoOutputChangePassword>(new DtoOutputChangePassword
            {
                PasswordChanged = dbUser
            });
        }
    }
}

using Application.UseCases.Authentication.Dtos;
using AutoMapper;
using Infrastructure.Ef.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Authentication
{
    public class UseCaseLogin
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UseCaseLogin(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public DtoOutputUserLogin Execute(DtoInputLogin login)
        {
            Infrastructure.Ef.DbEntities.DbUser user;
            try
            {
                user = _userRepository.FetchByUsername(login.UserName);
            }
            catch (KeyNotFoundException)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            bool valid = BCrypt.Net.BCrypt.Verify(login.Password, user.Password);

            if (!valid)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            return _mapper.Map<DtoOutputUserLogin>(user);
        }
    }
}

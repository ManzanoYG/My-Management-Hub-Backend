using Application.UseCases.Authentication.Dtos;
using AutoMapper;
using Infrastructure.Ef.User;
using Infrastructure.Services;
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
        private readonly IAuditService _auditService;

        public UseCaseLogin(IUserRepository userRepository, IMapper mapper, IAuditService auditService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _auditService = auditService;
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
                _auditService.Log(login.UserName, AuditActions.UserLoginFailed, AuditEntities.User);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            bool valid = BCrypt.Net.BCrypt.Verify(login.Password, user.Password);

            if (!valid)
            {
                _auditService.Log(login.UserName, AuditActions.UserLoginFailed, AuditEntities.User);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            _auditService.Log(login.UserName, AuditActions.UserLogin, AuditEntities.Session);
            return _mapper.Map<DtoOutputUserLogin>(user);
        }
    }
}

using Application.UseCases.Authentication.Dtos;
using AutoMapper;
using Infrastructure.Ef.Authentication;
using Infrastructure.Ef.DbEntities;
using Infrastructure.Ef.User;
using Infrastructure.Services;

namespace Application.UseCases.Authentication
{
    public class UseCaseLogin
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuditService _auditService;

        public UseCaseLogin(IUserRepository userRepository, IMapper mapper, IAuditService auditService, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _auditService = auditService;
            _passwordHasher = passwordHasher;
        }

        public DtoOutputUserLogin Execute(DtoInputLogin login)
        {
            
            var user = _userRepository.FetchByUsername(login.username);
            if (user == null || string.IsNullOrEmpty(login.password))
            {
                _auditService.Log(login.username, AuditActions.UserLoginFailed, AuditEntities.User);
                return new DtoOutputUserLogin { isLogged = false };
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            else
            {
                bool valid = BCrypt.Net.BCrypt.Verify(login.password, user.Password);
                Console.WriteLine(login.password);
                Console.WriteLine(user.Password);
                if (!valid)
                {
                    _auditService.Log(login.username, AuditActions.UserLoginFailed, AuditEntities.User);
                    throw new UnauthorizedAccessException("Invalid credentials");
                }
                else
                {
                    _auditService.Log(login.username, AuditActions.UserLogin, AuditEntities.Session);
                }

            }
             return _mapper.Map<DtoOutputUserLogin>(new DtoOutputUserLogin
             {
                 isLogged = _passwordHasher.VerifyPassword(user.Password, login.password),
                 username = user.Username,
                 usertype = user.UserType
             });
        }
    }
}

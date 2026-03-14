using Application.UseCases.User.Dto;
using Application.UseCases.Utils;
using AutoMapper;
using Infrastructure.Ef.User;
using Infrastructure.Services;
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
        private readonly IAuditService _auditService;

        public UseCaseCreateUser(IUserRepository userRepository, IMapper mapper, IAuditService auditService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _auditService = auditService;
        }

        public DtoOutputUser Execute(DtoInputCreateUser input)
        {
            var dbUser = _userRepository.Create(input.UserName, input.Password);
            _auditService.Log(input.UserName, AuditActions.UserCreated, AuditEntities.User);
            return _mapper.Map<DtoOutputUser>(dbUser);
        }
    }
}

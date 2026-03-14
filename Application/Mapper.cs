using Application.UseCases.User.Dto;
using Application.UseCases.Authentication.Dtos;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Ef.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class Mapper : Profile
    {
        public Mapper() 
        {
            //User
            CreateMap<User, DtoOutputUser>();
            CreateMap<DbUser, DtoOutputUser>();
            CreateMap<DbUser, User>();
            CreateMap<DbUser, DtoOutputUserLogin>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username));
        }
    }
}

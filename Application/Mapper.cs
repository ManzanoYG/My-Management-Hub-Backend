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
using Application.UseCases.Note.Dto;

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
            CreateMap<bool, DtoOutputUserLogin>();
            CreateMap<bool, DtoOutputChangePassword>();
            CreateMap<bool, DtoOutputDeleteUser>();

            //Note
            CreateMap<Note, DtoOutputCreateNote>();
            CreateMap<DbNote, DtoOutputCreateNote>();
            CreateMap<DbNote, Note>();
            CreateMap<DbNote, DtoOutputGetAllPinned>();
        }
    }
}

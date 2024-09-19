using AutoMapper;
using BusinessObjects.Models;
using DTOs.Rank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateRank, Rank>();
            CreateMap<UpdateRank, Rank>();
        }

    }
}

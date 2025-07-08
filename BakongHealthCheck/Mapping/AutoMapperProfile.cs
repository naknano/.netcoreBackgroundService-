using AutoMapper;
using BakongHealthCheck.Dto;
using BakongHealthCheck.Dto.MBService;
using BakongHealthCheck.Entities;

namespace BakongHealthCheck.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // FROM SOURCE TO DESTINATION
            // BAKONG 
            CreateMap<MBService, RequestMBServiceDTO>().ReverseMap();
            CreateMap<RequestMBServiceDTO, MBService>().ReverseMap();
        }
    }
}

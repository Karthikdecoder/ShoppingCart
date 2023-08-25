using AutoMapper;
using ShoppingCartWeb.Models.Dto;

namespace ShoppingCartWeb
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<RegistrationDTO, RegistrationDTO>().ReverseMap();
        }
    }
}

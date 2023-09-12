using AutoMapper;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;

namespace ShoppingCartAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<Registration, RegistrationDTO>().ReverseMap();
            CreateMap<RoleMaster, RoleMasterDTO>().ReverseMap();
            CreateMap<CategoryMaster, CategoryMasterDTO>().ReverseMap();
			CreateMap<StateMaster, StateMasterDTO>().ReverseMap();
			CreateMap<CountryMaster, CountryMasterDTO>().ReverseMap();
			CreateMap<Menu, MenuDTO>().ReverseMap();
			CreateMap<MenuRoleMapping, MenuRoleMappingDTO>().ReverseMap();
		}
    }
}

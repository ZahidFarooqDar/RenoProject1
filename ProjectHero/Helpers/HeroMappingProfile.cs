using AutoMapper;
using ProjectHero.DomainModal;
using ProjectHero.ServiceModals;

namespace ProjectHero.Helpers
{
    public class HeroMappingProfile : Profile
    {
        public HeroMappingProfile() 
        {
            //ReverseMap() is like mapping Source with target and vice versa
            CreateMap<HeroDM, HeroSM>().ReverseMap()
                //This is Custom Mapping it will only update values when there exist any value
                //For Null Values the existing value will be replaced.
                .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != null))
                .ForMember(dest => dest.Image, opt => opt.Condition(src => src.Image != null))
                .ForMember(dest => dest.Power, opt => opt.Condition(src => src.Power != null)); 
            //CreateMap<HeroDM, HeroSM>();
            // Mapping when property names are different
            //CreateMap<HeroDM, HeroSM>()
            //    .ForMember(dest =>
            //    dest.FName,
            //    opt => opt.MapFrom(src => src.FirstName))
            //    .ForMember(dest =>
            //    dest.LName,
            //    opt => opt.MapFrom(src => src.LastName));
        }
    }
}

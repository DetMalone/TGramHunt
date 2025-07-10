using AutoMapper;
using TGramHunt.Contract;
using TGramHunt.Contract.ViewModels.EditProfile;
using TGramHunt.ViewModels;

namespace TGramHunt.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateProductViewModel, Product>()
                .ForMember(dest => dest.Tag, opt => opt.MapFrom(x => x.Tag.ToLower()));
            CreateMap<User, ProfileViewModel>();
        }
    }
}

using AutoMapper;
using Dating.API.DTO;
using Dating.API.Extensions;
using Dating.Data.Entities;
using Dating.Shared;

namespace Dating.API.Helpers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<AppUser, MemberDto>()
            .ForMember(d => d.PhotoUrl, o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain)!.Url))
            .ForMember(d => d.Age, o => o.MapFrom(s => s.DateOfBirth.CalculateAge()));
        CreateMap<Photo, PhotoDto>();
        CreateMap<MemberUpdateDto, AppUser>();
    }
}
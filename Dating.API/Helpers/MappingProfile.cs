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
        CreateMap<RegisterDto, AppUser>();
        CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
        CreateMap<Message, MessageDto>()
            .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.Photos.FirstOrDefault(x => x.IsMain)!.Url))
            .ForMember(d => d.RecipientPhotoUrl,
                o => o.MapFrom(s => s.Recipient.Photos.FirstOrDefault(x => x.IsMain)!.Url));
        CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue
            ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc)
            : null);
    }
}
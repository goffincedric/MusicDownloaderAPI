using System.Net;
using System.Security.Authentication;
using AutoMapper;
using MusicDownloader.Api.Models;
using MusicDownloader.Shared.Constants;
using MusicDownloader.Shared.Exceptions;

namespace MusicDownloader.Api.Profiles;

public class Exceptions : Profile
{
    public Exceptions()
    {
        CreateMap<MusicDownloaderException, ErrorDetails>()
            .IncludeBase<Exception, ErrorDetails>()
            .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => src.StatusCode))
            .ForMember(dest => dest.ErrorCode, opt => opt.MapFrom(src => src.ErrorCode));

        CreateMap<AuthenticationException, ErrorDetails>()
            .IncludeBase<Exception, ErrorDetails>()
            .ForMember(
                dest => dest.StatusCode,
                opt => opt.MapFrom(src => HttpStatusCode.Unauthorized)
            )
            .ForMember(dest => dest.ErrorCode, opt => opt.MapFrom(src => ErrorCodes.Unauthorized))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Unauthorized"));

        CreateMap<Exception, ErrorDetails>()
            .ForMember(
                dest => dest.StatusCode,
                opt => opt.MapFrom(src => HttpStatusCode.InternalServerError)
            )
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message));
    }
}

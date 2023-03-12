using System.Net;
using AutoMapper;
using MusicDownloader.Controllers.Models;
using MusicDownloader.Shared.Edxceptions;

namespace MusicDownloader.Controllers.Profiles;

public class Exceptions : Profile
{
    public Exceptions()
    {
        CreateMap<MusicDownloaderException, ErrorDetails>()
            .IncludeBase<Exception, ErrorDetails>()
            .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => src.StatusCode))
            .ForMember(dest => dest.ErrorCode, opt => opt.MapFrom(src => src.ErrorCode));
        
        // TODO: Set status codes for invalidArgumentException and other exceptions

        CreateMap<Exception, ErrorDetails>()
            .ForMember(dest => dest.StatusCode, opt => opt.MapFrom(src => HttpStatusCode.InternalServerError))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.Message));
    }
}
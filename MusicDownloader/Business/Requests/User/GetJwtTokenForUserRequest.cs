using MediatR;
using MusicDownloader.Business.Logic;
using MusicDownloader.Pocos.User;

namespace MusicDownloader.Business.Requests.User;

public class GetJwtTokenForUserRequest : IRequest<string>
{
    public ApiUser User { get; set; }
}

public class GetJwtTokenForUserRequestHandler(JwtTokenGenerator tokenGenerator)
    : IRequestHandler<GetJwtTokenForUserRequest, string>
{
    public Task<string> Handle(GetJwtTokenForUserRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(tokenGenerator.CreateToken(request.User));
    }
}
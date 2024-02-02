using MediatR;
using MusicDownloader.Business.Logic;
using MusicDownloader.Pocos.User;

namespace MusicDownloader.Business.Requests.User;

public record GetJwtTokenForUserRequest(ApiUser User) : IRequest<string>;

public class GetJwtTokenForUserRequestHandler(JwtTokenGenerator tokenGenerator)
    : IRequestHandler<GetJwtTokenForUserRequest, string>
{
    public Task<string> Handle(
        GetJwtTokenForUserRequest request,
        CancellationToken cancellationToken
    ) => Task.FromResult(tokenGenerator.CreateToken(request.User));
}

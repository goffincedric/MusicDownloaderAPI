using MediatR;
using MusicDownloader.Business.Logic;
using MusicDownloader.Pocos.User;

namespace MusicDownloader.Business.Requests.User;

public record GetJwtTokenForUserRequest(ApiUser User) : IRequest<string>;

public class GetJwtTokenForUserRequestHandler : IRequestHandler<GetJwtTokenForUserRequest, string>
{
    private readonly JwtTokenGenerator _tokenGenerator;

    public GetJwtTokenForUserRequestHandler(JwtTokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    public Task<string> Handle(
        GetJwtTokenForUserRequest request,
        CancellationToken cancellationToken
    ) => Task.FromResult(_tokenGenerator.CreateToken(request.User));
}

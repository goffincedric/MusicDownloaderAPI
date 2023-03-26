using MediatR;
using MusicDownloader.Business.Logic;
using MusicDownloader.Pocos.User;

namespace MusicDownloader.Business.Requests.User;

public class GetJwtTokenForUserRequest : IRequest<string>
{
    public ApiUser User { get; set; }
}

public class GetJwtTokenForUserRequestHandler : IRequestHandler<GetJwtTokenForUserRequest, string>
{
    private readonly JwtTokenGenerator _tokenGenerator;

    public GetJwtTokenForUserRequestHandler(JwtTokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    public Task<string> Handle(GetJwtTokenForUserRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_tokenGenerator.CreateToken(request.User));
    }
}
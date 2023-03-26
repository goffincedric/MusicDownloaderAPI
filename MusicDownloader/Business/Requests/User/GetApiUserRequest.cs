using System.Security.Authentication;
using MediatR;
using Microsoft.Extensions.Options;
using MusicDownloader.Pocos.User;
using MusicDownloader.Shared.Options;

namespace MusicDownloader.Business.Requests.User;

public class GetApiUserRequest : IRequest<ApiUser>
{
    public string UserToken { get; set; } = null!;
}

public class GetApiUserRequestHandler : IRequestHandler<GetApiUserRequest, ApiUser>
{
    private readonly ApiOptions _apiOptions;

    public GetApiUserRequestHandler(IOptions<ApiOptions> apiOptions)
    {
        _apiOptions = apiOptions.Value;
    }

    public Task<ApiUser> Handle(GetApiUserRequest request, CancellationToken cancellationToken)
    {
        var user = _apiOptions.Users.FirstOrDefault(user => string.Equals(user.Uuid, request.UserToken));
        if (user == null) throw new AuthenticationException();
        return Task.FromResult(user);
    }
}
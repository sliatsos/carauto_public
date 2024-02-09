using CarAuto.MAUI.Shared.Configuration;
using CarAuto.MAUI.Shared.ViewModels;
using CarAuto.Protos.User;
using Grpc.Net.Client;
using Microsoft.Maui.Controls;
using System.Text.Json.Nodes;
using static CarAuto.Protos.User.UserService;

namespace CarAuto.MAUI.Shared.Services;

public class LoginService : ILoginService
{
    private readonly GrpcChannel _channel;

    public LoginService(GrpcChannel channel)
    {
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
    }

    public async Task RegisterAsync(UserViewModel userInfo)
    {
        var client = new UserServiceClient(_channel);
        await client.CreateUserAsync(new CreateUserRequest
        {
            User = new User
            {
                FirstName = userInfo.FirstName,
                UserName = userInfo.UserName,
                LastName = userInfo.LastName,
                PhoneType = Protos.Enums.PhoneType.Mobile,
                Email = userInfo.Email,
                Password = userInfo.Password,
                PhoneNumber = userInfo.PhoneNumber,

            },
        });
    }

    public async Task<string> LoginAsync()
    {
        var authCode = await GetAuthCodeAsync();
        var url = $"{KeycloakConfiguration.OrganizationUrl}/auth/realms/{KeycloakConfiguration.Realm}/protocol/openid-connect/token";
        var httpClient = new HttpClient();
        var urlEncodedContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "client_id", "carauto-client" },
            { "client_secret", "XMovdkaeh3ibq2Fqu08EM3kqHGl45DT8" },
            { "code", authCode },
            { "redirect_uri", KeycloakConfiguration.Callback }
        });

        var result = await httpClient.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = urlEncodedContent,
        });

        var token = await result.Content.ReadAsStringAsync();
        var tokenJson = JsonNode.Parse(token);
        return tokenJson["access_token"].AsValue().GetValue<string>();
    }

    private string BuildAuthenticationUrl()
    {
        return $"{KeycloakConfiguration.OrganizationUrl}/auth/realms/{KeycloakConfiguration.Realm}/protocol/openid-connect/auth?response_type=code&client_id=carauto-client&redirect_uri={KeycloakConfiguration.Callback}";
    }

    private async Task<string> GetAuthCodeAsync()
    {
        var callbackUrl = new Uri(KeycloakConfiguration.Callback);
        var loginUrl = new Uri(BuildAuthenticationUrl());
        var authenticationResult = await WebAuthenticator.AuthenticateAsync(loginUrl, callbackUrl);
        return authenticationResult.Properties.First(e => e.Key == "code").Value;
    }
}

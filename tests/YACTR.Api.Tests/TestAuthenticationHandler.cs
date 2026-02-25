using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Api.Tests;

/// <summary>
/// Authenticates test users through the use of the <see cref="AuthenticationScheme"/>.
/// </summary>
/// <remarks>
/// Due to the nature of how the <see cref="DI.Authorization.UserPermissionsClaims.UserContextMiddleware"/> service
/// extracts and auto-creates valid JWT tokens in production, any valid <see cref="AuthenticationScheme"/> token/string
/// will have a user created automatically.
/// 
/// If you wish to set up the users ahead of time in testing, use the database context and/or provided helper functions.
/// </remarks>
public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string AuthenticationScheme = "TestScheme";
    public const string TokenSplitChars = "::";
    public static readonly User DEFAULT_TEST_USER = new()
    {
        Auth0UserId = $"test0|{Guid.Empty}",
        Email = "test@test.dev",
        Username = "test_user",
        PlatformPermissions = Enum.GetValues<Permission>()
    };

    /// <summary>
    /// Generates a token for use in the test version of the application.
    /// 
    /// Has format of "auth0Id::username::email"
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public static string GenerateAuthenticationToken(User user)
    {
        return string.Join(TokenSplitChars, new[] { user.Auth0UserId, user.Username, user.Email });
    }

    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    /// <summary>
    /// Test authentication scheme handler, allows us to use the Authorization header of the format
    /// defined in <see cref="GenerateAuthenticationToken"=>
    /// for the authorization and login as that user.
    /// </summary>
    /// <returns>AuthenticateResult based on validity of TestScheme authorization header</returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // If there's no authorization header in the test run; it's an unauthorized test.
        if (!Context.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            return Task.FromResult(AuthenticateResult.Fail("Unauthorized"));
        }

        var auth = AuthenticationHeaderValue.Parse(authorizationHeader!);

        if (auth.Scheme != AuthenticationScheme)
        {
            // Do not accept real Bearer tokens in test environment.
            return Task.FromResult(AuthenticateResult.Fail("Non-test requests should use the real JWT validation scheme"));
        }

        var userDetailsSplit = auth.Parameter!.Split(TokenSplitChars);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userDetailsSplit[0]),
            new Claim(ClaimTypes.Name, userDetailsSplit[1]),
            new Claim(ClaimTypes.Email, userDetailsSplit[2]),
        };

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

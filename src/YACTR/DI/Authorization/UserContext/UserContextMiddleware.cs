using System.Security.Claims;
using YACTR.Data.QueryExtensions;
using YACTR.Data.Repository.Interface;
using YACTR.Data.Model.Authentication;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace YACTR.DI.Authorization.UserContext;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserContextMiddleware> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public UserContextMiddleware(
        RequestDelegate next,
        ILogger<UserContextMiddleware> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var auth0Id = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!string.IsNullOrEmpty(auth0Id))
            {
                var email = context.User.FindFirst(ClaimTypes.Email)?.Value;
                var username = context.User.FindFirst(ClaimTypes.Name)?.Value;

                if (email != null)
                {
                    // This is a bit of a hack to access scoped services in middleware.
                    using var scope = _serviceScopeFactory.CreateScope();
                    
                    var userRepository = scope.ServiceProvider.GetRequiredService<IEntityRepository<User>>();
                    var clock = scope.ServiceProvider.GetRequiredService<IClock>();

                    var user = await userRepository
                        .BuildReadonlyQuery()
                        .WhereAuth0UserId(auth0Id)
                        .Include(u => u.OrganizationUsers)
                        .ThenInclude(ou => ou.Permissions)
                        .FirstOrDefaultAsync();

                    if (user == null)
                    {
                        _logger.LogInformation("First time login for user {UserAuth0NameIdentifier}. Creating local user.", auth0Id);
                        user = await userRepository.CreateAsync(new User()
                        {
                            Auth0UserId = auth0Id,
                            Email = email,
                            Username = username ?? email,
                            LastLogin = clock.GetCurrentInstant(),
                        });
                    }
                    else
                    {
                        user.LastLogin = clock.GetCurrentInstant();
                        user = await userRepository.UpdateAsync(user);
                    }

                    context.Items["CurrentUser"] = user;
                }
            }
        }

        await _next(context);
    }
} 
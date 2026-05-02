using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Infrastructure.Authorization.Permissions;

namespace YACTR.Infrastructure.Tests.Authorization.Permissions;

public class AuthorizationHandlerTests
{
    [Fact]
    public async Task AdminHandler_ShouldSucceed_WhenAdminPermissionClaimExists()
    {
        var requirement = new AdminPermissionRequiredAttribute(Permission.UsersRead);
        var context = CreateContext(requirement, new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(nameof(PermissionLevel.AdminPermission), Permission.UsersRead.ToString())
        ])));
        var handler = new AdminPermissionsAuthorizationHandler(Substitute.For<ILogger<AdminPermissionsAuthorizationHandler>>());

        await handler.HandleAsync(context);

        context.HasSucceeded.ShouldBeTrue();
        context.HasFailed.ShouldBeFalse();
    }

    [Fact]
    public async Task AdminHandler_ShouldFail_WhenAdminPermissionClaimMissing()
    {
        var requirement = new AdminPermissionRequiredAttribute(Permission.UsersRead);
        var context = CreateContext(requirement, new ClaimsPrincipal(new ClaimsIdentity()));
        var handler = new AdminPermissionsAuthorizationHandler(Substitute.For<ILogger<AdminPermissionsAuthorizationHandler>>());

        await handler.HandleAsync(context);

        context.HasSucceeded.ShouldBeFalse();
        context.HasFailed.ShouldBeTrue();
    }

    [Fact]
    public async Task AdminHandler_ShouldFail_WhenPrincipalHasNoIdentity()
    {
        var requirement = new AdminPermissionRequiredAttribute(Permission.UsersRead);
        var context = CreateContext(requirement, new ClaimsPrincipal());
        var handler = new AdminPermissionsAuthorizationHandler(Substitute.For<ILogger<AdminPermissionsAuthorizationHandler>>());

        await handler.HandleAsync(context);

        context.HasSucceeded.ShouldBeFalse();
        context.HasFailed.ShouldBeTrue();
    }

    [Fact]
    public async Task PlatformHandler_ShouldSucceed_WhenPlatformPermissionClaimExists()
    {
        var requirement = new PlatformPermissionRequiredAttribute(Permission.AreasRead);
        var context = CreateContext(requirement, new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(nameof(PermissionLevel.PlatformPermission), Permission.AreasRead.ToString())
        ])));
        var handler = new PlatformPermissionsAuthorizationHandler(Substitute.For<ILogger<PlatformPermissionsAuthorizationHandler>>());

        await handler.HandleAsync(context);

        context.HasSucceeded.ShouldBeTrue();
        context.HasFailed.ShouldBeFalse();
    }

    [Fact]
    public async Task PlatformHandler_ShouldSucceed_WhenAdminPermissionClaimExists()
    {
        var requirement = new PlatformPermissionRequiredAttribute(Permission.AreasRead);
        var context = CreateContext(requirement, new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(nameof(PermissionLevel.AdminPermission), Permission.AreasRead.ToString())
        ])));
        var handler = new PlatformPermissionsAuthorizationHandler(Substitute.For<ILogger<PlatformPermissionsAuthorizationHandler>>());

        await handler.HandleAsync(context);

        context.HasSucceeded.ShouldBeTrue();
        context.HasFailed.ShouldBeFalse();
    }

    [Fact]
    public async Task PlatformHandler_ShouldFail_WhenNoRelevantClaimExists()
    {
        var requirement = new PlatformPermissionRequiredAttribute(Permission.AreasRead);
        var context = CreateContext(requirement, new ClaimsPrincipal(new ClaimsIdentity()));
        var handler = new PlatformPermissionsAuthorizationHandler(Substitute.For<ILogger<PlatformPermissionsAuthorizationHandler>>());

        await handler.HandleAsync(context);

        context.HasSucceeded.ShouldBeFalse();
        context.HasFailed.ShouldBeTrue();
    }

    [Fact]
    public async Task PlatformHandler_ShouldFail_WhenPrincipalHasNoIdentity()
    {
        var requirement = new PlatformPermissionRequiredAttribute(Permission.AreasRead);
        var context = CreateContext(requirement, new ClaimsPrincipal());
        var handler = new PlatformPermissionsAuthorizationHandler(Substitute.For<ILogger<PlatformPermissionsAuthorizationHandler>>());

        await handler.HandleAsync(context);

        context.HasSucceeded.ShouldBeFalse();
        context.HasFailed.ShouldBeTrue();
    }

    private static AuthorizationHandlerContext CreateContext(
        IAuthorizationRequirement requirement,
        ClaimsPrincipal principal) => new([requirement], principal, resource: null);
}

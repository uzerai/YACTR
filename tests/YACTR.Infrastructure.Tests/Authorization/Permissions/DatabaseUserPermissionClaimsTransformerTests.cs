using System.Security.Claims;
using System.Linq.Expressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Query;
using NodaTime;
using NSubstitute;
using Shouldly;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Authorization;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Organizations;
using YACTR.Infrastructure.Authorization.Permissions;

namespace YACTR.Infrastructure.Tests.Authorization.Permissions;

public class DatabaseUserPermissionClaimsTransformerTests
{
    [Fact]
    public async Task TransformAsync_ShouldReturnCachedPrincipal_WhenCacheHitContainsPrincipal()
    {
        var repository = Substitute.For<IEntityRepository<User>>();
        var clock = Substitute.For<IClock>();
        var cache = Substitute.For<IMemoryCache>();
        var logger = Substitute.For<ILogger<DatabaseUserPermissionClaimsTransformer>>();
        var transformer = new DatabaseUserPermissionClaimsTransformer(repository, clock, cache, logger);
        var principal = BuildPrincipal("auth0|cached");
        var cachedPrincipal = new ClaimsPrincipal(new ClaimsIdentity([new Claim("cached", "true")]));

        SetCacheTryGetValue(cache, hit: true, cachedValue: cachedPrincipal);

        var result = await transformer.TransformAsync(principal);

        result.ShouldBe(cachedPrincipal);
        await repository.DidNotReceiveWithAnyArgs().CreateAsync(default!, TestContext.Current.CancellationToken);
        repository.DidNotReceive().BuildReadonlyQuery();
    }

    [Fact]
    public async Task TransformAsync_ShouldUpdateLastLoginAndSave_WhenCacheMissAndUserExists()
    {
        var existingUser = BuildUser(auth0UserId: "auth0|existing", email: "existing@yactr.dev", username: "existing");
        var fixedInstant = Instant.FromUnixTimeSeconds(1_700_000_000);

        var repository = Substitute.For<IEntityRepository<User>>();
        repository.BuildReadonlyQuery().Returns(ToAsyncQueryable(new List<User> { existingUser }));

        var clock = Substitute.For<IClock>();
        clock.GetCurrentInstant().Returns(fixedInstant);

        var cache = Substitute.For<IMemoryCache>();
        SetCacheTryGetValue(cache, hit: false, cachedValue: null);

        var logger = Substitute.For<ILogger<DatabaseUserPermissionClaimsTransformer>>();
        var transformer = new DatabaseUserPermissionClaimsTransformer(repository, clock, cache, logger);
        var principal = BuildPrincipal("auth0|existing", email: "existing@yactr.dev", name: "existing");

        var result = await transformer.TransformAsync(principal);

        existingUser.LastLogin.ShouldBe(fixedInstant);
        await repository.Received(1).SaveAsync(Arg.Any<CancellationToken>());
        result.Identities.Any(x => x.AuthenticationType == nameof(YactrAuthenticationType.Platform)).ShouldBeTrue();
    }

    [Fact]
    public async Task TransformAsync_ShouldContinueToRepository_WhenCacheHitContainsNull()
    {
        var existingUser = BuildUser(auth0UserId: "auth0|null-cache", email: "nullcache@yactr.dev", username: "nullcache");

        var repository = Substitute.For<IEntityRepository<User>>();
        repository.BuildReadonlyQuery().Returns(ToAsyncQueryable(new List<User> { existingUser }));

        var clock = Substitute.For<IClock>();
        clock.GetCurrentInstant().Returns(Instant.FromUnixTimeSeconds(1_700_000_100));

        var cache = Substitute.For<IMemoryCache>();
        SetCacheTryGetValue(cache, hit: true, cachedValue: null);

        var logger = Substitute.For<ILogger<DatabaseUserPermissionClaimsTransformer>>();
        var transformer = new DatabaseUserPermissionClaimsTransformer(repository, clock, cache, logger);
        var principal = BuildPrincipal("auth0|null-cache", email: "nullcache@yactr.dev", name: "nullcache");

        var result = await transformer.TransformAsync(principal);

        repository.Received(1).BuildReadonlyQuery();
        result.Identities.Any(x => x.AuthenticationType == nameof(YactrAuthenticationType.Platform)).ShouldBeTrue();
    }

    [Fact]
    public async Task TransformAsync_ShouldCreateUser_WhenCacheMissAndUserDoesNotExist()
    {
        var fixedInstant = Instant.FromUnixTimeSeconds(1_700_000_200);

        var repository = Substitute.For<IEntityRepository<User>>();
        repository.BuildReadonlyQuery().Returns(ToAsyncQueryable(new List<User>()));
        repository.CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        var clock = Substitute.For<IClock>();
        clock.GetCurrentInstant().Returns(fixedInstant);

        var cache = Substitute.For<IMemoryCache>();
        SetCacheTryGetValue(cache, hit: false, cachedValue: null);

        var logger = Substitute.For<ILogger<DatabaseUserPermissionClaimsTransformer>>();
        var transformer = new DatabaseUserPermissionClaimsTransformer(repository, clock, cache, logger);
        var principal = BuildPrincipal("auth0|new", email: "new@yactr.dev", name: "new-user");

        var result = await transformer.TransformAsync(principal);

        await repository.Received(1).CreateAsync(
            Arg.Is<User>(u =>
                u.Auth0UserId == "auth0|new"
                && u.Email == "new@yactr.dev"
                && u.Username == "new-user"
                && u.LastLogin == fixedInstant),
            Arg.Any<CancellationToken>());

        result.Identities.Any(x => x.AuthenticationType == nameof(YactrAuthenticationType.Platform)).ShouldBeTrue();
    }

    private static ClaimsPrincipal BuildPrincipal(string nameIdentifier, string email = "user@yactr.dev", string name = "test-user") =>
        new(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, nameIdentifier),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, name)
        ], authenticationType: "TestAuth"));

    private static User BuildUser(string auth0UserId, string email, string username) =>
        new()
        {
            Id = Guid.NewGuid(),
            Auth0UserId = auth0UserId,
            Email = email,
            Username = username,
            CreatedAt = Instant.FromUnixTimeSeconds(1_699_999_000),
            UpdatedAt = Instant.FromUnixTimeSeconds(1_699_999_000),
            LastLogin = Instant.FromUnixTimeSeconds(1_699_999_500),
            PlatformPermissions = [Permission.AreasRead],
            AdminPermissions = [],
            OrganizationUsers =
            [
                new OrganizationUser
                {
                    OrganizationId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    Permissions = [Permission.UsersRead]
                }
            ]
        };

    private static void SetCacheTryGetValue(IMemoryCache cache, bool hit, object? cachedValue)
    {
        object? value = null;
        cache.TryGetValue(Arg.Any<object>(), out value)
            .Returns(callInfo =>
            {
                callInfo[1] = cachedValue;
                return hit;
            });
    }

    private static IQueryable<User> ToAsyncQueryable(IEnumerable<User> users) => new TestAsyncEnumerable<User>(users);

    private sealed class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable) { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    private sealed class TestAsyncQueryProvider<TEntity>(IQueryProvider inner) : IAsyncQueryProvider
    {
        public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);

        public object Execute(Expression expression) => inner.Execute(expression)!;

        public TResult Execute<TResult>(Expression expression) => inner.Execute<TResult>(expression);

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments().First();
            var executionResult = typeof(IQueryProvider)
                .GetMethods()
                .Single(method => method.Name == nameof(IQueryProvider.Execute) && method.IsGenericMethod)
                .MakeGenericMethod(expectedResultType)
                .Invoke(inner, [expression]);

            return (TResult)typeof(Task)
                .GetMethods()
                .Single(method => method.Name == nameof(Task.FromResult) && method.IsGenericMethod)
                .MakeGenericMethod(expectedResultType)
                .Invoke(null, [executionResult])!;
        }
    }

    private sealed class TestAsyncEnumerator<T>(IEnumerator<T> inner) : IAsyncEnumerator<T>
    {
        public T Current => inner.Current;

        public ValueTask<bool> MoveNextAsync() => ValueTask.FromResult(inner.MoveNext());

        public ValueTask DisposeAsync()
        {
            inner.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}

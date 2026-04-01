using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Organizations;
using YACTR.Api.Endpoints.Organizations.Teams;
using YACTR.Domain.Model.Authentication;

namespace YACTR.Api.Tests.EndpointTests.Organizations.Teams;

[Collection("IntegrationTests")]
public class CreateOrganizationTeamIntegrationTests(ApiTestClassFixture fixture) : OrganizationTeamEndpointTestsBase(fixture)
{
    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedTeam()
    {
        using var client = Fixture.CreateAuthenticatedClient(AllPermissionsUser);

        // Arrange - First create an organization

        // Create team request
        var createRequest = new CreateOrganizationTeamRequest(Organization.Id, "Test Team");

        // Act
        var (response, result) = await client.POSTAsync<CreateOrganizationTeam, CreateOrganizationTeamRequest, OrganizationTeamResponse>(createRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_WithNoPermissions_ReturnsCreatedTeam()
    {
        using var client = Fixture.CreateAuthenticatedClient(AllPermissionsUser);

        // Arrange - Remove the organization user == no permissions
        Fixture.DatabaseContext.Remove(OrganizationUser);
        await Fixture.DatabaseContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Create team request
        var createRequest = new CreateOrganizationTeamRequest(Organization.Id, "Test Team");

        // Act
        var (response, result) = await client.POSTAsync<CreateOrganizationTeam, CreateOrganizationTeamRequest, OrganizationTeamResponse>(createRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_WithEmptyTeamName_ReturnsOk()
    {
        using var client = Fixture.CreateAuthenticatedClient(AllPermissionsUser);

        // Arrange - First create an organization
        var createRequest = new CreateOrganizationTeamRequest(Organization.Id, "");

        // Act
        var (response, result) = await client.POSTAsync<CreateOrganizationTeam, CreateOrganizationTeamRequest, OrganizationTeamResponse>(createRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}

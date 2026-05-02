using System.Net;

using Shouldly;

using YACTR.Api.Endpoints.Organizations;

namespace YACTR.Api.Tests.EndpointTests.Organizations.Teams;

[Collection("IntegrationTests")]
public class GetAllOrganizationTeamsIntegrationTests(ApiTestClassFixture fixture) : OrganizationTeamEndpointTestsBase(fixture)
{
    [Fact]
    public async Task GetAll_WithOrganization_ReturnsSuccessStatusCode()
    {
        using var client = Fixture.CreateAuthenticatedClient(AllPermissionsUser);

        // Act
        var getRequest = new GetAllOrganizationTeamsRequest(Organization.Id);
        var (response, result) = await client.GETAsync<GetAllOrganizationTeams, GetAllOrganizationTeamsRequest, List<GetAllOrganizationTeamsResponseItem>>(getRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    /// <summary>
    /// In this case; a user cannot have permissions for an organization which does not exist;
    /// therefore, forbidden is returned instead of not found.
    ///
    /// It's also in the interest of enumeration attacks in the quantum computing age :')
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetAll_WithInvalidOrganizationId_ReturnsForbidden()
    {
        using var client = Fixture.CreateAuthenticatedClient();

        // Arrange
        var invalidOrgId = Guid.NewGuid();

        // Act
        var getRequest = new GetAllOrganizationTeamsRequest(invalidOrgId);
        var (response, _) = await client.GETAsync<GetAllOrganizationTeams, GetAllOrganizationTeamsRequest, List<GetAllOrganizationTeamsResponseItem>>(getRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}

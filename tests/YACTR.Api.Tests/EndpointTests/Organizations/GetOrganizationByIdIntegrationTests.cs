using System.Net;

using FastEndpoints.Testing;

using Shouldly;

using YACTR.Api.Endpoints.Organizations;

namespace YACTR.Api.Tests.EndpointTests.Organizations;

[Collection("IntegrationTests")]
public class GetOrganizationByIdIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task GetById_WithValidId_ReturnsOrganization()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange - First create an organization
        var createRequest = new CreateOrganizationRequestData("Test Organization for Get");
        var (createResponse, createdOrg) = await client.POSTAsync<CreateOrganization, CreateOrganizationRequestData, CreateOrganizationResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act
        var getRequest = new GetOrganizationByIdRequest(createdOrg.Id);
        var (response, result) = await client.GETAsync<GetOrganizationById, GetOrganizationByIdRequest, GetOrganizationByIdResponse>(getRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdOrg.Id);
        result.Name.ShouldBe("Test Organization for Get");
    }

    [Fact]
    public async Task Get_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var invalidId = Guid.NewGuid();

        // Act
        var getRequest = new GetOrganizationByIdRequest(invalidId);
        var (response, _) = await client.GETAsync<GetOrganizationById, GetOrganizationByIdRequest, GetOrganizationByIdResponse>(getRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_WithoutAuthentication_ReturnsUnauthorized()
    {
        var invalidId = Guid.NewGuid();

        // Act
        var (response, _) = await fixture.CreateClient().GETAsync<GetOrganizationById, GetOrganizationByIdRequest, GetOrganizationByIdResponse>(new GetOrganizationByIdRequest(invalidId));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}

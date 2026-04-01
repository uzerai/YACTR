using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Organizations;
using YACTR.Domain.Model.Organizations;

namespace YACTR.Api.Tests.EndpointTests.Organizations;

[Collection("IntegrationTests")]
public class CreateOrganizationIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedOrganization()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var createRequest = new CreateOrganizationRequestData("Integration Test Org");

        // Act
        var (response, result) = await client.POSTAsync<CreateOrganization, CreateOrganizationRequestData, Organization>(createRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Integration Test Org");
    }

    [Fact]
    public async Task Create_WithEmptyName_IsAllowed()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var createRequest = new CreateOrganizationRequestData("");

        // Act
        var (response, result) = await client.POSTAsync<CreateOrganization, CreateOrganizationRequestData, Organization>(createRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.ShouldNotBeNull();
        result.Name.ShouldBe("");
    }

    [Fact]
    public async Task Create_WithoutAuthentication_ReturnsUnauthorized()
    {
        var createRequest = new CreateOrganizationRequestData("Test Org");

        // Act
        var (response, _) = await fixture.CreateClient().POSTAsync<CreateOrganization, CreateOrganizationRequestData, Organization>(createRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}

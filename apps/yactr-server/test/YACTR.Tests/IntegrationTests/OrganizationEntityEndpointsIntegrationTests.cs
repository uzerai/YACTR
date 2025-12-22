using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Data.Model.Organizations;
using YACTR.Endpoints.Organizations;

namespace YACTR.Tests.Endpoints;

[Collection("IntegrationTests")]
public class OrganizationEntityEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
{
    protected override async ValueTask SetupAsync()
    {
        await base.SetupAsync();
    }

    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Act
        var (response, result) = await client.GETAsync<GetAllOrganizations, EmptyRequest, List<Organization>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAll_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var (response, _) = await fixture.CreateClient().GETAsync<GetAllOrganizations, EmptyRequest, List<Organization>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

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

    [Fact]
    public async Task GetById_WithValidId_ReturnsOrganization()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Arrange - First create an organization
        var createRequest = new CreateOrganizationRequestData("Test Organization for Get");
        var (createResponse, createdOrg) = await client.POSTAsync<CreateOrganization, CreateOrganizationRequestData, Organization>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act
        var getRequest = new GetOrganizationByIdRequest(createdOrg.Id);
        var (response, result) = await client.GETAsync<GetOrganizationById, GetOrganizationByIdRequest, Organization>(getRequest);

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
        var (response, _) = await client.GETAsync<GetOrganizationById, GetOrganizationByIdRequest, Organization>(getRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_WithoutAuthentication_ReturnsUnauthorized()
    {
        var invalidId = Guid.NewGuid();

        // Act
        var (response, _) = await fixture.CreateClient().GETAsync<GetOrganizationById, GetOrganizationByIdRequest, Organization>(new GetOrganizationByIdRequest(invalidId));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}

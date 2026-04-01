using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Organizations;
using YACTR.Api.Pagination;
using YACTR.Domain.Model.Organizations;

namespace YACTR.Api.Tests.EndpointTests.Organizations;

[Collection("IntegrationTests")]
public class GetAllOrganizationsIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Act
        var (response, result) = await client.GETAsync<GetAllOrganizations, GetAllOrganizationsRequest, PaginatedResponse<OrganizationResponse>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsRequestedPageAndTotalCount()
    {
        using var client = fixture.CreateAuthenticatedClient();

        var (baselineResponse, baselineResult) = await client.GETAsync<GetAllOrganizations, GetAllOrganizationsRequest, PaginatedResponse<OrganizationResponse>>(new()
        {
            Page = 1,
            PageSize = 1
        });
        baselineResponse.IsSuccessStatusCode.ShouldBeTrue();
        baselineResult.ShouldNotBeNull();

        for (var i = 0; i < 3; i++)
        {
            var createRequest = new CreateOrganizationRequestData($"Pagination Test Org {Guid.NewGuid()}");
            var (createResponse, _) = await client.POSTAsync<CreateOrganization, CreateOrganizationRequestData, Organization>(createRequest);
            createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        var (response, result) = await client.GETAsync<GetAllOrganizations, GetAllOrganizationsRequest, PaginatedResponse<OrganizationResponse>>(new()
        {
            Page = 2,
            PageSize = 2
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(baselineResult.TotalCount + 3);
        var expectedPageCount = Math.Clamp(result.TotalCount - 2, 0, 2);
        result.Items.Count.ShouldBe(expectedPageCount);
    }

    [Fact]
    public async Task GetAll_WithPageSizeBelowMinimum_ClampsToMinimum()
    {
        using var client = fixture.CreateAuthenticatedClient();

        var createRequest = new CreateOrganizationRequestData($"Clamp Test Org {Guid.NewGuid()}");
        var (createResponse, _) = await client.POSTAsync<CreateOrganization, CreateOrganizationRequestData, Organization>(createRequest);
        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        var (baselineResponse, baselineResult) = await client.GETAsync<GetAllOrganizations, GetAllOrganizationsRequest, PaginatedResponse<OrganizationResponse>>(new()
        {
            Page = 1,
            PageSize = 1
        });
        baselineResponse.IsSuccessStatusCode.ShouldBeTrue();
        baselineResult.ShouldNotBeNull();

        var (response, result) = await client.GETAsync<GetAllOrganizations, GetAllOrganizationsRequest, PaginatedResponse<OrganizationResponse>>(new()
        {
            Page = 1,
            PageSize = 0
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(baselineResult.TotalCount);
        result.Items.Count.ShouldBe(1);
        result.Items.Single().Id.ShouldBe(baselineResult.Items.Single().Id);
    }

    [Fact]
    public async Task GetAll_WithDifferentPages_ReturnsDifferentItems()
    {
        using var client = fixture.CreateAuthenticatedClient();

        for (var i = 0; i < 2; i++)
        {
            var createRequest = new CreateOrganizationRequestData($"Page Diff Org {Guid.NewGuid()}");
            var (createResponse, _) = await client.POSTAsync<CreateOrganization, CreateOrganizationRequestData, Organization>(createRequest);
            createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        var (pageOneResponse, pageOneResult) = await client.GETAsync<GetAllOrganizations, GetAllOrganizationsRequest, PaginatedResponse<OrganizationResponse>>(new()
        {
            Page = 1,
            PageSize = 1
        });

        var (pageTwoResponse, pageTwoResult) = await client.GETAsync<GetAllOrganizations, GetAllOrganizationsRequest, PaginatedResponse<OrganizationResponse>>(new()
        {
            Page = 2,
            PageSize = 1
        });

        pageOneResponse.IsSuccessStatusCode.ShouldBeTrue();
        pageTwoResponse.IsSuccessStatusCode.ShouldBeTrue();
        pageOneResult.ShouldNotBeNull();
        pageTwoResult.ShouldNotBeNull();
        pageOneResult.TotalCount.ShouldBe(pageTwoResult.TotalCount);
        pageOneResult.Items.Count.ShouldBe(1);
        pageTwoResult.Items.Count.ShouldBe(1);
        pageOneResult.Items.Single().Id.ShouldNotBe(pageTwoResult.Items.Single().Id);
    }

    [Fact]
    public async Task GetAll_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var (response, _) = await fixture.CreateClient().GETAsync<GetAllOrganizations, GetAllOrganizationsRequest, PaginatedResponse<OrganizationResponse>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}

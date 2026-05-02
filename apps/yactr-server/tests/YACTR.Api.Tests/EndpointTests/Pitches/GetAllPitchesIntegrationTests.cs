using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Pitches;
using YACTR.Api.Pagination;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Tests.EndpointTests.Pitches;

[Collection("IntegrationTests")]
public class GetAllPitchesIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Act
        var (response, result) = await client.GETAsync<GetAllPitches, GetAllPitchesRequest, PaginatedResponse<GetAllPitchesResponseItem>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsRequestedPageAndTotalCount()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (_, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();

        var (baselineResponse, baselineResult) = await client.GETAsync<GetAllPitches, GetAllPitchesRequest, PaginatedResponse<GetAllPitchesResponseItem>>(new()
        {
            Page = 1,
            PageSize = 1
        });
        baselineResponse.IsSuccessStatusCode.ShouldBeTrue();
        baselineResult.ShouldNotBeNull();

        for (var i = 0; i < 3; i++)
        {
            var createRequest = new CreatePitchRequest(
                sector.Id,
                route.Id,
                $"Pagination Pitch {i}",
                ClimbingType.Sport,
                "Pagination test pitch",
                "5.9",
                i
            );
            var (createResponse, _) = await client.POSTAsync<CreatePitch, CreatePitchRequest, CreatePitchResponse>(createRequest);
            createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        var (response, result) = await client.GETAsync<GetAllPitches, GetAllPitchesRequest, PaginatedResponse<GetAllPitchesResponseItem>>(new()
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
        var (_, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();

        var createRequest = new CreatePitchRequest(
            sector.Id,
            route.Id,
            $"Clamp Pitch {Guid.NewGuid()}",
            ClimbingType.Sport,
            "Clamp test pitch",
            "5.8",
            0
        );
        var (createResponse, _) = await client.POSTAsync<CreatePitch, CreatePitchRequest, CreatePitchResponse>(createRequest);
        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        var (baselineResponse, baselineResult) = await client.GETAsync<GetAllPitches, GetAllPitchesRequest, PaginatedResponse<GetAllPitchesResponseItem>>(new()
        {
            Page = 1,
            PageSize = 1
        });
        baselineResponse.IsSuccessStatusCode.ShouldBeTrue();
        baselineResult.ShouldNotBeNull();

        var (response, result) = await client.GETAsync<GetAllPitches, GetAllPitchesRequest, PaginatedResponse<GetAllPitchesResponseItem>>(new()
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
        var (_, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();

        for (var i = 0; i < 2; i++)
        {
            var createRequest = new CreatePitchRequest(
                sector.Id,
                route.Id,
                $"Page Diff Pitch {i}",
                ClimbingType.Sport,
                "Page diff test pitch",
                "5.8",
                i
            );
            var (createResponse, _) = await client.POSTAsync<CreatePitch, CreatePitchRequest, CreatePitchResponse>(createRequest);
            createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        var (pageOneResponse, pageOneResult) = await client.GETAsync<GetAllPitches, GetAllPitchesRequest, PaginatedResponse<GetAllPitchesResponseItem>>(new()
        {
            Page = 1,
            PageSize = 1
        });

        var (pageTwoResponse, pageTwoResult) = await client.GETAsync<GetAllPitches, GetAllPitchesRequest, PaginatedResponse<GetAllPitchesResponseItem>>(new()
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
}

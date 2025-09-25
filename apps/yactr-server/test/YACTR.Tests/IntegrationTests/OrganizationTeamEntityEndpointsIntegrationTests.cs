// using System.Net;
// using FastEndpoints;
// using FastEndpoints.Testing;
// using Shouldly;
// using YACTR.Data.Model.Authentication;
// using YACTR.Data.Model.Authorization.Permissions;
// using YACTR.Data.Model.Organizations;
// using YACTR.Endpoints.Organizations;
// using YACTR.Endpoints.Organizations.Teams;

// namespace YACTR.Tests.Endpoints;

// [Collection("IntegrationTests")]
// public class OrganizationTeamEntityEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
// {

//     public Organization organization = null!;
//   public OrganizationUser organizationUser = null!;

//     public User AllPermissionsUser = new()
//   {
//     Username = "test_user_with_all_permissions",
//     Email = "test_user@test.dev",
//     Auth0UserId = $"test|{Guid.NewGuid()}",
//     PlatformPermissions = Enum.GetValues<Permission>()
//   };

//   protected override async ValueTask SetupAsync()
//   {
//     await base.SetupAsync();

//     User user = await fixture.GetEntityRepository<User>()
//     .CreateAsync(AllPermissionsUser, TestContext.Current.CancellationToken);

//     organization = await fixture.GetEntityRepository<Organization>()
//       .CreateAsync(new()
//       {
//         Name = "Test Organization",
//       });


//     OrganizationUser orgUser = await fixture.GetRepository<OrganizationUser>()
//       .CreateAsync(new()
//       {
//         UserId = user.Id,
//         OrganizationId = organization.Id,
//       });
//   }


//     [Fact]
//     public async Task GetAll_WithValidOrganizationId_ReturnsSuccessStatusCode()
//     {
//         using var client = fixture.CreateAuthenticatedClient(AllPermissionsUser);

//         // Arrange - First create an organization
//         var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Teams");
//         var (orgResponse, organization) = await client.POSTAsync<CreateOrganization, CreateOrganizationRequestData, Organization>(createOrgRequest);
//         orgResponse.IsSuccessStatusCode.ShouldBeTrue();

//         // Act
//         var getRequest = new GetAllOrganizationTeamsRequest(organization.Id);
//         var (response, result) = await client.GETAsync<GetAllOrganizationTeams, GetAllOrganizationTeamsRequest, List<OrganizationTeam>>(getRequest);

//         // Assert
//         response.IsSuccessStatusCode.ShouldBeTrue();
//         result.ShouldNotBeNull();
//     }

//     /// <summary>
//     /// In this case; a user cannot have permissions for an organization which does not exist;
//     /// therefore, forbidden is returned instead of not found.
//     /// 
//     /// It's also in the interest of enumeration attacks in the quantum computing age :')
//     /// </summary>
//     /// <returns></returns>
//     [Fact]
//     public async Task GetAll_WithInvalidOrganizationId_ReturnsForbidden()
//     {
//         using var client = fixture.CreateAuthenticatedClient();

//         // Arrange
//         var invalidOrgId = Guid.NewGuid();

//         // Act
//         var getRequest = new GetAllOrganizationTeamsRequest(invalidOrgId);
//         var (response, _) = await client.GETAsync<GetAllOrganizationTeams, GetAllOrganizationTeamsRequest, List<OrganizationTeam>>(getRequest);

//         // Assert
//         response.IsSuccessStatusCode.ShouldBeFalse();
//         response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
//     }

//     [Fact]
//     public async Task Create_WithValidData_ReturnsCreatedTeam()
//     {
//         using var client = fixture.CreateAuthenticatedClient(AllPermissionsUser);

//         // Arrange - First create an organization
//         var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Team Creation");
//         var (orgResponse, organization) = await client.POSTAsync<CreateOrganization, CreateOrganizationRequestData, Organization>(createOrgRequest);
//         orgResponse.IsSuccessStatusCode.ShouldBeTrue();

//         // Create team request
//         var createRequest = new CreateOrganizationTeamRequest(organization.Id, "Test Team");

//         // Act
//         var (response, result) = await client.POSTAsync<CreateOrganizationTeam, CreateOrganizationTeamRequest, OrganizationTeam>(createRequest);

//         // Assert
//         response.IsSuccessStatusCode.ShouldBeTrue();
//         result.ShouldNotBeNull();
//         result.Name.ShouldBe("Test Team");
//         result.OrganizationId.ShouldBe(organization.Id);
//     }

//     [Fact]
//     public async Task Create_WithEmptyTeamName_ReturnsOk()
//     {
//         using var client = fixture.CreateAuthenticatedClient(AllPermissionsUser);

//         // Arrange - First create an organization
//         var createOrgRequest = new CreateOrganizationRequestData("Test Organization for Empty Team");
//         var (orgResponse, organization) = await client.POSTAsync<CreateOrganization, CreateOrganizationRequestData, Organization>(createOrgRequest);
//         orgResponse.IsSuccessStatusCode.ShouldBeTrue();

//         var createRequest = new CreateOrganizationTeamRequest(organization.Id, "");

//         // Act
//         var (response, result) = await client.POSTAsync<CreateOrganizationTeam, CreateOrganizationTeamRequest, OrganizationTeam>(createRequest);

//         // Assert
//         response.StatusCode.ShouldBe(HttpStatusCode.OK);
//         result.ShouldNotBeNull();
//         result.Name.ShouldBe("");
//     }
// }
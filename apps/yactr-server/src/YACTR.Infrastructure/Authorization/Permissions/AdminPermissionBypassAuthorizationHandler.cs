// using Microsoft.AspNetCore.Authorization;
// using YACTR.Domain.Model.Authorization.Permissions;
//
// namespace YACTR.Infrastructure.Authorization.Permissions;
//
// public abstract class AdminPermissionBypassAuthorizationHandler<TRequirement> : AuthorizationHandler<TRequirement> where TRequirement : IAuthorizationRequirement, IAuthorizationRequirementData
// {
//     protected bool HasAdminPermissionInstead(AuthorizationHandlerContext context, TRequirement requirementData)
//     {
//         try
//         {
//             var permission = requirementData.Permission;
//
//             if (context.User.HasClaim(nameof(PermissionLevel.AdminPermission), permission.ToString()))
//             {
//                 return true;
//             }
//         } catch { /* Do nothing, requirement does not have permission property value */ }
//
//         return false;
//     }
// }
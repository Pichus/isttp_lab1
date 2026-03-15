using Microsoft.AspNetCore.Authorization;

using StudentParliamentSystem.Core.Aggregates.Role;

namespace StudentParliamentSystem.Api.Configurations;

public static class AuthorizationPolicyConfig
{
    public static void AddAuthorizationPolicies(this AuthorizationOptions authorizationOptions)
    {
        authorizationOptions.AddPolicy(AuthorizationPolicyNameConstants.CanAccessAdminPanel,
            policy => policy.RequireRole(RoleNameConstants.SuperAdminRoleName));
    }
}

public static class AuthorizationPolicyNameConstants
{
    public const string CanAccessAdminPanel = "CanAccessAdminPanel";
}
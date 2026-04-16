using Microsoft.AspNetCore.Authorization;

using StudentParliamentSystem.Core.Aggregates.Role;

namespace StudentParliamentSystem.Api.Configurations;

public static class AuthorizationPolicyConfig
{

    private static readonly string[] DepartmentRoles =
    [
        nameof(RoleName.SuperAdmin),
        nameof(RoleName.StudentParliamentMember),
        nameof(RoleName.HeadOfCulturalDepartment),
        nameof(RoleName.HeadOfScienceDepartment),
        nameof(RoleName.HeadOfCoworkingDepartment),
        nameof(RoleName.HeadOfInformationDepartment),
        nameof(RoleName.HeadOfDepartment),
        nameof(RoleName.CulturalDepartmentMember),
        nameof(RoleName.ScienceDepartmentMember),
        nameof(RoleName.CoworkingDepartmentMember),
        nameof(RoleName.InformationDepartmentMember),
    ];

    public static void AddAuthorizationPolicies(this AuthorizationOptions authorizationOptions)
    {

        authorizationOptions.AddPolicy(AuthorizationPolicyNameConstants.CanAccessAdminPanel,
            policy => policy.RequireRole(nameof(RoleName.SuperAdmin)));


        authorizationOptions.AddPolicy(AuthorizationPolicyNameConstants.CanAccessAdminPanelBase,
            policy => policy.RequireRole(DepartmentRoles));


        authorizationOptions.AddPolicy(AuthorizationPolicyNameConstants.CanManageDepartment,
            policy => policy.RequireRole(DepartmentRoles));


        authorizationOptions.AddPolicy(AuthorizationPolicyNameConstants.CanManageCoworkingBookings,
            policy => policy.RequireRole(nameof(RoleName.CoworkingDepartmentMember), nameof(RoleName.SuperAdmin)));
    }
}

public static class AuthorizationPolicyNameConstants
{

    public const string CanAccessAdminPanel = "CanAccessAdminPanel";


    public const string CanAccessAdminPanelBase = "CanAccessAdminPanelBase";


    public const string CanManageDepartment = "CanManageDepartment";


    public const string CanManageCoworkingBookings = "CanManageCoworkingBookings";
}
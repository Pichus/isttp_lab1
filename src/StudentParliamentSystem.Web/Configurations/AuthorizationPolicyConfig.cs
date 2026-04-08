using Microsoft.AspNetCore.Authorization;

using StudentParliamentSystem.Core.Aggregates.Role;

namespace StudentParliamentSystem.Api.Configurations;

public static class AuthorizationPolicyConfig
{
    // All roles that can access the admin panel (parliament members + heads + superadmin)
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
        // Full admin panel access (SuperAdmin only: Users, full Departments list)
        authorizationOptions.AddPolicy(AuthorizationPolicyNameConstants.CanAccessAdminPanel,
            policy => policy.RequireRole(nameof(RoleName.SuperAdmin)));

        // Admin panel entry (any parliament/department role OR superadmin)
        authorizationOptions.AddPolicy(AuthorizationPolicyNameConstants.CanAccessAdminPanelBase,
            policy => policy.RequireRole(DepartmentRoles));

        // Manage a specific department (enforced additionally in controller by dept ownership)
        authorizationOptions.AddPolicy(AuthorizationPolicyNameConstants.CanManageDepartment,
            policy => policy.RequireRole(DepartmentRoles));

        // Manage coworking bookings (Coworking dept members or SuperAdmin)
        authorizationOptions.AddPolicy(AuthorizationPolicyNameConstants.CanManageCoworkingBookings,
            policy => policy.RequireRole(nameof(RoleName.CoworkingDepartmentMember), nameof(RoleName.SuperAdmin)));
    }
}

public static class AuthorizationPolicyNameConstants
{
    /// <summary>Full admin panel — SuperAdmin only (Users, all Departments).</summary>
    public const string CanAccessAdminPanel = "CanAccessAdminPanel";

    /// <summary>Admin panel entry point — any parliament/department member or SuperAdmin.</summary>
    public const string CanAccessAdminPanelBase = "CanAccessAdminPanelBase";

    /// <summary>Manage a specific department — any department role; controller enforces ownership.</summary>
    public const string CanManageDepartment = "CanManageDepartment";

    /// <summary>Manage coworking bookings — Coworking department members or SuperAdmin.</summary>
    public const string CanManageCoworkingBookings = "CanManageCoworkingBookings";
}
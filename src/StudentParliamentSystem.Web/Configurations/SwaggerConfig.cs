namespace StudentParliamentSystem.Api.Configurations;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddOpenApiDocument(document =>
        {
            // to enable dropdowns for nullable enums in swagger ui
            document.SchemaSettings.AllowReferencesWithProperties = true;
            document.SchemaSettings.GenerateEnumMappingDescription = true;

            // document.AddSecurity("Bearer", [], new OpenApiSecurityScheme
            // {
            //     Type = OpenApiSecuritySchemeType.Http,
            //     Scheme = JwtBearerDefaults.AuthenticationScheme,
            //     BearerFormat = "JWT",
            //     Description = "Type into the textbox: {your JWT token}."
            // });
            //
            // document.OperationProcessors.Add(
            //     new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
        });

        return services;
    }
}
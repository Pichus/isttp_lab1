namespace StudentParliamentSystem.Api.Configurations;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddOpenApiDocument(document =>
        {

            document.SchemaSettings.AllowReferencesWithProperties = true;
            document.SchemaSettings.GenerateEnumMappingDescription = true;











        });

        return services;
    }
}
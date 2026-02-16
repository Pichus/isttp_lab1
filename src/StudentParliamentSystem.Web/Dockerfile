FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
#EXPOSE 8081
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY src/StudentParliamentSystem.Api/*.csproj src/StudentParliamentSystem.Api/
COPY src/StudentParliamentSystem.Infrastructure/*.csproj src/StudentParliamentSystem.Infrastructure/
#COPY src/StudentParliamentSystem.UseCases/*.csproj src/StudentParliamentSystem.UseCases/
COPY src/StudentParliamentSystem.Core/*.csproj src/StudentParliamentSystem.Core/

RUN dotnet restore src/StudentParliamentSystem.Api/StudentParliamentSystem.Api.csproj

COPY . .
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
WORKDIR /src/src/AnonymousStudentReviews.Api
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "StudentParliamentSystem.Api.dll"]
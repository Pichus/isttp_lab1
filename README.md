
# Student parliament system


## Run Locally

Clone the project

```bash
  git clone https://github.com/Pichus/isttp_lab1.git
```

Go to the project directory

```bash
  cd isttp_lab1
```

Ensure you have docker and docker compose installed

Run the project using docker compose

```bash
  docker compose up -d
```


[//]: # (## Running Tests)

[//]: # ()
[//]: # (To run all tests, run the following command)

[//]: # ()
[//]: # (```bash)

[//]: # (  dotnet test)

[//]: # (```)

[//]: # ()
[//]: # (To run unit tests, run the following command)

[//]: # ()
[//]: # (```bash)

[//]: # (  dotnet test tests/TaskManager.UnitTests)

[//]: # (```)

[//]: # ()
[//]: # (To run integration tests, run the following command. Ensure you have docker installed and running.)

[//]: # (&#40;In progress&#41;)

[//]: # ()
[//]: # (```bash)

[//]: # (  dotnet test tests/TaskManager.IntegrationTests)

[//]: # (```)

## Development

Add migrations

For the main application database:
```bash
    dotnet ef migrations add MigrationName --project src/StudentParliamentSystem.Infrastructure  --startup-project src/StudentParliamentSystem.Web --context ApplicationDatabaseContext -o ./Data/Migrations
```

For the identity database:
```bash
    dotnet ef migrations add MigrationName --project src/StudentParliamentSystem.Infrastructure.Identity --startup-project src/StudentParliamentSystem.Web --context IdentityDatabaseContext -o ./Data/Migrations
```

Apply migrations

```txt
    Since docker is used, migrations are applied automatically on startup
    when the project is run in Development environment for better developer experience
```

To apply them manually:

For the main application database:
```bash
    dotnet ef database update --project src/StudentParliamentSystem.Infrastructure  --startup-project src/StudentParliamentSystem.Web
```

For the identity database:
```bash
    dotnet ef database update --project src/StudentParliamentSystem.Infrastructure.Identity --startup-project src/StudentParliamentSystem.Web --context IdentityDatabaseContext
```

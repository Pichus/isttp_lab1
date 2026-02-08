
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

```bash
    dotnet ef migrations add MigrationName --project src/StudentParliamentSystem.Infrastructure  --startup-project src/StudentParliamentSystem.Api -o ./Data/Migrations
```

Apply migrations

```txt
    Since docker is used, migrations are applied automatically on startup
    when the project is run in Development environment for better developer experience
```

```bash
    dotnet ef database update --project src/StudentParliamentSystem.Infrastructure  --startup-project src/StudentParliamentSystem.Api
```


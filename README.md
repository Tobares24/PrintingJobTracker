# Printing Job Tracker

A Blazor Server application built with .NET 8 and Clean Architecture that allows users to track printing jobs and monitor their status throughout the production process.

## Project Overview

Printing Job Tracker demonstrates a modular and scalable architecture that includes:

- **Blazor Server** for the interactive UI
- **Entity Framework Core** for data access with **Code First** approach
- **SQL Server** as the database provider
- **Clean Architecture** principles for maintainability and separation of concerns
- **Automatic database seeding** for initial data population

## Project Structure
```
PrintingJobTracker/
│
├── PrintingJobTracker.Api/              # Presentation layer (Blazor Server)
│
├── PrintingJobTracker.Application/      # Application logic, services, interfaces
│
├── PrintingJobTracker.Domain/           # Entities and domain models
│
├── PrintingJobTracker.Infrastructure/   # Persistence, repositories, and configuration
│
└── README.md
```

## Configuration

### Environment Variables

The app reads environment variables and configuration settings from `appsettings.json`.
```json
{
  "ConnectionStrings": {
    "SQL_SERVER_CONNECTION_STRING": "Server=localhost;Database=PrintingJobTracker;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "ApplicationSettings": {
    "TAMANO_SQL_POOL": 10,
    "INIT_SEED_CLIENTS": 1,
    "INIT_SEED_JOBS": 1,
    "INIT_SEED_JOBSTATUS": 1
  }
}
```

You can override these values by defining them as environment variables in your hosting provider or `.env` file.

### Database Seeding

The application includes automatic database seeding controlled by the configuration settings above:

- `INIT_SEED_CLIENTS`: Set to `1` to seed initial client data
- `INIT_SEED_JOBS`: Set to `1` to seed initial printing jobs
- `INIT_SEED_JOBSTATUS`: Set to `1` to seed job status options

Set any value to `0` to disable seeding for that entity.

## Database Setup

The system uses **Code First** approach with Entity Framework Core. Migrations are automatically applied at startup using the `ApplicationBuilderExtensions`.

However, if you need to manually create or update migrations, you can run the following commands:
```bash
# Create a new migration
dotnet ef --verbose migrations add InitialCreate -p .\PrintingJobTracker.Infrastructure\ -s .\PrintingJobTracker.Api\

# Apply the migrations to the database
dotnet ef --verbose database update -p .\PrintingJobTracker.Infrastructure\ -s .\PrintingJobTracker.Api\
```

**Note:** If the database connection fails, check or modify the connection string in your `appsettings.json` or environment variables.

## Running the Application

1. Open the solution in Visual Studio 2022 or VS Code.
2. Ensure SQL Server is running and accessible.
3. Start the application using:
```bash
dotnet run --project .\PrintingJobTracker.Api\
```

4. Navigate to `https://localhost:7152` (or your configured host URL).

The application will automatically:
- Create the database if it doesn't exist
- Apply pending migrations
- Seed initial data based on configuration settings

## Technologies Used

- .NET 8
- Blazor Server
- Entity Framework Core (Code First)
- SQL Server
- SignalR
- Dependency Injection
- Clean Architecture
- Database Seeding

## Author

**Mauricio Steven Tobares**  
Full Stack Developer

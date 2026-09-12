# Medical Appointment System API

The Medical Appointment System API is a backend service for managing patients,
doctors, departments, medicines, appointments, and appointment-related email
notifications. It exposes HTTP endpoints that can be consumed by a web or
mobile frontend.

## Who Needs This System?

- **Patients** can register, log in, request appointments, and view their appointment details.
- **Clinic administrators or staff** can manage appointments and access patient, doctor, department, and medicine information.
- **Doctors and clinical staff** can use appointment and prescription-related data to support patient visits.
- **Frontend developers** can integrate the API with an Angular or other client application. The current CORS policy allows `http://localhost:4200`.

## Main Functionality

### Authentication

- Register a user with `POST /api/signup`.
- Log in with `POST /api/login` using a mobile number and password.
- Users have a role, with `Patient` as the database default role.

### Appointment Management

- Create an appointment with `POST /api/save-appointment`.
- Create an appointment with patient and department details using `POST /api/appointments`.
- Update an appointment with `PUT /api/appointments/{id}`.
- Delete an appointment with `DELETE /api/appointments/{id}`.
- Retrieve a paginated appointment list with `GET /api/appointments`.
- Retrieve one appointment with `GET /api/appointments/{id}`.
- Filter appointment lists using `pageNumber`, `pageSize`, `role`, and `contactNumber` query parameters.
- Send an appointment email after a successful save or update when a patient email is available.

### Reference Data

- `GET /api/get-all-patients` returns patients.
- `GET /api/get-all-doctors` returns doctors.
- `GET /api/get-all-department` returns departments.
- `GET /api/get-all-medicines` returns medicines.

### Email

- `POST /api/Email` sends a general email using the configured SMTP service.
- Appointment confirmation emails are sent by the appointment workflow when an email address is available.

## Architecture

The solution follows a Clean Architecture-style structure with CQRS and
MediatR:

```text
MedicalAppointmentSystem.API           HTTP controllers and application startup
Core.Application                        Commands, queries, interfaces, and services
Core.Domain                             Domain entities and core models
MedicalAppointmentSystem.Infrastructure Repositories, database context, and persistence
Shared.DTOs                             Request and response data transfer objects
```

The typical request flow is:

```text
HTTP request -> API controller -> MediatR command/query -> repository
             -> SQL Server/stored procedure -> DTO response
```

## Technologies Used

- **C# and .NET 9**
- **ASP.NET Core Web API** for HTTP endpoints and dependency injection
- **MediatR** for command and query dispatching
- **CQRS** to separate write commands from read queries
- **Clean Architecture-style layering** for separation of responsibilities
- **Dapper** and SQL Server stored procedures for database operations
- **Entity Framework Core 9** for the configured database context and design-time support
- **Swagger/OpenAPI** through Swashbuckle for API exploration in Development
- **SMTP/Gmail** for email delivery
- **Docker** for containerized execution
- **SQL Server** for application data

## Database

The checked-in [DatabaseSetup.sql](DatabaseSetup.sql) script creates the main
tables, relationships, stored procedures, and database objects required by the
application. It includes tables for:

- Departments
- Patients
- Doctors
- Users
- Medicines
- Appointments
- Prescription details

### Database Setup

1. Install SQL Server and ensure the SQL Server service is running.
2. Create or select the `MedicalAppointmentSystemDB` database.
3. Run [DatabaseSetup.sql](DatabaseSetup.sql) against that database.
4. Confirm that the connection string in `MedicalAppointmentSystem.API/appsettings.json` points to your SQL Server instance.

The default local connection string uses Windows authentication:

```text
Server=localhost;Database=MedicalAppointmentSystemDB;Trusted_Connection=True;TrustServerCertificate=True;
```

For another environment, use configuration or environment variables instead of
committing environment-specific connection details.

## Configuration

The application reads the database connection and SMTP settings from
`MedicalAppointmentSystem.API/appsettings.json`.

Before running the application, configure:

- `ConnectionStrings:DefaultConnection`
- `Smtp:Host`
- `Smtp:Port`
- `Smtp:EnableSsl`
- `Smtp:UserName`
- `Smtp:Password`

SMTP passwords and other secrets must be supplied through User Secrets,
environment variables, or a secret manager. Do not commit real credentials to
source control.

## Run Locally

### Prerequisites

- .NET 9 SDK
- SQL Server
- A configured database using [DatabaseSetup.sql](DatabaseSetup.sql)

Clone the repository and start the API:

```bash
git clone https://github.com/muharib979/MedicalAppointmentSystemAPI.git
cd MedicalAppointmentSystemAPI/MedicalAppointmentSystem.API
dotnet restore
dotnet run
```

When running in Development, Swagger is available at:

```text
https://localhost:<port>/swagger
```

The exact port is shown by `dotnet run` and can also be configured in
`MedicalAppointmentSystem.API/Properties/launchSettings.json`.

## Run with Docker

Build and run the API from the repository root:

```bash
docker build -t medical-appointment-system-api .
docker run --rm -p 8080:8080 medical-appointment-system-api
```

The container listens on port `8080`. The Dockerfile sets the environment to
Development so Swagger is enabled. The container still needs access to a SQL
Server instance and must receive its database and SMTP configuration through
the runtime environment.

## Current Implementation Notes

- The API configures authorization middleware, but the current login endpoint returns user information and does not issue a JWT or other access token.
- Appointment visibility is currently controlled by the `role` and `contactNumber` query parameters sent to the appointment-list endpoint. A production deployment should enforce authorization server-side.
- The existing CORS policy allows only `http://localhost:4200`; update it for the deployed frontend origin.
- The SQL setup script is the reproducible database setup path. An external database import link may also exist in older project notes, but it should not be required for a local setup.

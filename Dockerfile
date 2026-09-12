# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY *.sln ./
COPY Core.Application/Core.Application.csproj ./Core.Application/
COPY Core.Domain/Core.Domain.csproj ./Core.Domain/
COPY MedicalAppointmentSystem.Infrastructure/MedicalAppointmentSystem.Infrastructure.csproj ./MedicalAppointmentSystem.Infrastructure/
COPY Shared.DTOs/Shared.DTOs.csproj ./Shared.DTOs/
COPY MedicalAppointmentSystem.API/MedicalAppointmentSystem.API.csproj ./MedicalAppointmentSystem.API/

# Restore dependencies
RUN dotnet restore MedicalAppointmentSystem.sln

# Copy the rest of the files
COPY . .

# Publish solution (release mode)
RUN dotnet publish MedicalAppointmentSystem.sln -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /app/publish ./

# Set environment variable for HTTP
# Set environment variable for HTTP + Swagger
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

# Expose port
EXPOSE 8080

# Start the app
ENTRYPOINT ["dotnet", "MedicalAppointmentSystem.API.dll"]

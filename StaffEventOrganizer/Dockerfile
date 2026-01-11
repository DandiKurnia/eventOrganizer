# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY Models/Models.csproj ./Models/
COPY StaffEventOrganizer/StaffEventOrganizer.csproj ./StaffEventOrganizer/

RUN dotnet restore StaffEventOrganizer/StaffEventOrganizer.csproj

# Copy all source code
COPY Models/ ./Models/
COPY StaffEventOrganizer/ ./StaffEventOrganizer/

# Build and publish
WORKDIR /src/StaffEventOrganizer
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install ICU libraries for globalization support
RUN apt-get update && apt-get install -y libicu-dev && rm -rf /var/lib/apt/lists/*

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "StaffEventOrganizer.dll"]

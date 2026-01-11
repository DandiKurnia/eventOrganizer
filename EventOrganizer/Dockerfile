# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY Models/Models.csproj ./Models/
COPY EventOrganizer/EventOrganizer.csproj ./EventOrganizer/

RUN dotnet restore EventOrganizer/EventOrganizer.csproj

# Copy all source code
COPY Models/ ./Models/
COPY EventOrganizer/ ./EventOrganizer/

# Build and publish
WORKDIR /src/EventOrganizer
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install ICU libraries for globalization support
RUN apt-get update && apt-get install -y libicu-dev && rm -rf /var/lib/apt/lists/*

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "EventOrganizer.dll"]

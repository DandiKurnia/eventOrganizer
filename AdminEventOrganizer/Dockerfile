# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY Models/Models.csproj ./Models/
COPY AdminEventOrganizer/AdminEventOrganizer.csproj ./AdminEventOrganizer/

RUN dotnet restore AdminEventOrganizer/AdminEventOrganizer.csproj

# Copy all source code
COPY Models/ ./Models/
COPY AdminEventOrganizer/ ./AdminEventOrganizer/

# Build and publish
WORKDIR /src/AdminEventOrganizer
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install ICU libraries for globalization support
RUN apt-get update && apt-get install -y libicu-dev && rm -rf /var/lib/apt/lists/*

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AdminEventOrganizer.dll"]

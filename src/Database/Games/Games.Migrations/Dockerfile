FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy projects
COPY ["Games.Domain/Games.Domain.csproj", "Games.Domain/"]
COPY ["Games.Infrastructure/Games.Infrastructure.csproj", "Games.Infrastructure/"]
COPY ["Games.Migrations/Games.Migrations.csproj", "Games.Migrations/"]

# Restore dependencies
RUN dotnet restore "Games.Migrations/Games.Migrations.csproj"

# Copy source code
COPY . .

# Build
RUN dotnet build "Games.Migrations/Games.Migrations.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "Games.Migrations/Games.Migrations.csproj" -c Release -o /app/publish

# Final image
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/publish .

# Set production environment
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Games.Migrations.dll"] 
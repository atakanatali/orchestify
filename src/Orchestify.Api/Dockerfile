# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["Orchestify.Api/Orchestify.Api.csproj", "Orchestify.Api/"]
COPY ["Orchestify.Application/Orchestify.Application.csproj", "Orchestify.Application/"]
COPY ["Orchestify.Domain/Orchestify.Domain.csproj", "Orchestify.Domain/"]
COPY ["Orchestify.Infrastructure/Orchestify.Infrastructure.csproj", "Orchestify.Infrastructure/"]
COPY ["Orchestify.Contracts/Orchestify.Contracts.csproj", "Orchestify.Contracts/"]
COPY ["Orchestify.Shared/Orchestify.Shared.csproj", "Orchestify.Shared/"]
COPY ["Directory.Packages.props", "./"]
COPY ["Directory.Build.props", "./"]

RUN dotnet restore "Orchestify.Api/Orchestify.Api.csproj"

# Copy all source files
COPY . .

# Build the project
WORKDIR "/src/Orchestify.Api"
RUN dotnet build "Orchestify.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Orchestify.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Orchestify.Api.dll"]

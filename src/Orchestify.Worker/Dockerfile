# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["Orchestify.Worker/Orchestify.Worker.csproj", "Orchestify.Worker/"]
COPY ["Orchestify.Application/Orchestify.Application.csproj", "Orchestify.Application/"]
COPY ["Orchestify.Domain/Orchestify.Domain.csproj", "Orchestify.Domain/"]
COPY ["Orchestify.Infrastructure/Orchestify.Infrastructure.csproj", "Orchestify.Infrastructure/"]
COPY ["Orchestify.Contracts/Orchestify.Contracts.csproj", "Orchestify.Contracts/"]
COPY ["Orchestify.Shared/Orchestify.Shared.csproj", "Orchestify.Shared/"]
COPY ["Directory.Packages.props", "./"]
COPY ["Directory.Build.props", "./"]

RUN dotnet restore "Orchestify.Worker/Orchestify.Worker.csproj"

# Copy all source files
COPY . .

# Build the project
WORKDIR "/src/Orchestify.Worker"
RUN dotnet build "Orchestify.Worker.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Orchestify.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Orchestify.Worker.dll"]

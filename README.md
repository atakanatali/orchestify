# Orchestify

A distributed task orchestration system built with .NET 8.0, featuring structured logging, background processing, and resilient messaging.

## Overview

Orchestify is a microservices-based application designed to manage and orchestrate complex workflows. It provides a robust foundation for building distributed systems with proper observability, error handling, and correlation tracking.

## Architecture

The solution follows Clean Architecture principles with the following layers:

- **Orchestify.Api** - ASP.NET Core Web API for HTTP endpoints
- **Orchestify.Worker** - Background service for async processing
- **Orchestify.Application** - Application business logic and use cases
- **Orchestify.Domain** - Core domain entities and business rules
- **Orchestify.Infrastructure** - External concerns (data access, logging, messaging)
- **Orchestify.Contracts** - Shared DTOs and interfaces
- **Orchestify.Shared** - Cross-cutting utilities (constants, results, logging abstractions)

## Features

### Structured Logging (ORC-05)

- **Serilog Integration**: Production-ready structured logging with Serilog
- **Elasticsearch Sink**: Persistent log storage with automatic index management
- **Console Sink**: Local debugging with colored console output
- **Log Enrichment**: Automatic enrichment with:
  - Correlation ID (request tracing)
  - Service name
  - Environment
  - Machine name
  - Process ID
  - Thread ID
- **Exception Serialization**: Full exception details including:
  - Exception type
  - Exception message
  - Stack trace
  - Inner exceptions (recursive)
  - Target site
  - HResult
  - Custom exception data

### Log Indices

Logs are stored in Elasticsearch under the following patterns:

- `logs-orchestify-api-{date}` - API service logs
- `logs-orchestify-worker-{date}` - Worker service logs

Query all logs with `logs-*` pattern in Kibana.

## Quick Start

### Prerequisites

- Docker and Docker Compose
- .NET 8.0 SDK

### Running with Docker Compose

```bash
# Start all infrastructure services and applications
cd infra
docker compose up -d

# View logs
docker compose logs -f orchestify-api
docker compose logs -f orchestify-worker

# Stop all services
docker compose down
```

### Accessing Services

| Service | URL | Credentials |
|---------|-----|-------------|
| API | http://localhost:5000 | - |
| Kibana | http://localhost:5601 | - |
| Elasticsearch | http://localhost:9200 | - |
| RabbitMQ Management | http://localhost:15672 | orchestify/orchestify_password |

### Health Check

```bash
curl http://localhost:5000/health
```

## Configuration

### Serilog Configuration

Configure Serilog via `appsettings.json`:

```json
{
  "Serilog": {
    "ElasticsearchUrl": "http://localhost:9200",
    "ServiceName": "orchestify-api",
    "Environment": "Development",
    "MinimumLogLevel": "Debug",
    "EnableConsoleSink": true,
    "EnableElasticsearchSink": true
  }
}
```

### Environment Variables

Override settings with environment variables (used in Docker):

- `Serilog__ElasticsearchUrl` - Elasticsearch connection URL
- `Serilog__ServiceName` - Service identifier
- `Serilog__Environment` - Environment name (Development/Production)
- `ConnectionStrings__Postgres` - PostgreSQL connection string
- `ConnectionStrings__Redis` - Redis connection string
- `RabbitMQ__Host` - RabbitMQ host
- `RabbitMQ__Username` - RabbitMQ username
- `RabbitMQ__Password` - RabbitMQ password

## Logging Usage

### Using ILogService

```csharp
public class MyService
{
    private readonly ILogService _logService;

    public MyService(ILogService logService)
    {
        _logService = logService;
    }

    public void DoWork()
    {
        _logService.Info("ORC_WORK_START", "Starting work");

        try
        {
            // ... do work ...
            _logService.Info("ORC_WORK_COMPLETE", "Work completed successfully");
        }
        catch (Exception ex)
        {
            _logService.Error(ex, "ORC_WORK_ERROR", "Work failed");
        }
    }
}
```

### Log Levels

- `Info(code, message, data)` - Informational messages
- `Warn(code, message, data)` - Warning messages
- `Error(exception, code, message, data)` - Error with exception details
- `Debug(code, message, data)` - Debug-level diagnostics

## Project Structure

```
orchestify/
├── src/
│   ├── Orchestify.Api/          # Web API
│   ├── Orchestify.Worker/       # Background worker
│   ├── Orchestify.Application/  # Application layer
│   ├── Orchestify.Domain/       # Domain layer
│   ├── Orchestify.Infrastructure/  # Infrastructure
│   ├── Orchestify.Contracts/    # Shared contracts
│   └── Orchestify.Shared/       # Shared utilities
├── tests/
├── infra/
│   └── docker-compose.yml       # Infrastructure services
└── scripts/
```

## Development

### Building

```bash
dotnet build
```

### Running Locally

```bash
# API
cd src/Orchestify.Api
dotnet run

# Worker
cd src/Orchestify.Worker
dotnet run
```

## License

MIT License
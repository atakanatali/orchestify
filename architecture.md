# Orchestify Architecture

## System Architecture

Orchestify follows Clean Architecture principles with clear separation of concerns and dependency inversion.

## Layers

### Presentation Layer

**Orchestify.Api**
- ASP.NET Core Web API
- HTTP endpoints for client requests
- Middleware pipeline (correlation ID, exception handling)
- Health check endpoints
- Dependency injection configuration

**Orchestify.Worker**
- Background service implementation
- Long-running task processing
- Message consumption from queues
- Scheduled job execution

### Application Layer

**Orchestify.Application**
- Use cases and business workflows
- CQRS command/query handlers (e.g., `CreateWorkspaceHandler`, `ListWorkspacesHandler`)
- Application services
- DTOs for data transfer
- Validation logic

### Domain Layer

**Orchestify.Domain**
- Core business entities (e.g., `WorkspaceEntity`)
- Aggregates and value objects
- Domain events
- Business rules
- Domain-specific interfaces

### Infrastructure Layer

**Orchestify.Infrastructure**
- Data access (EF Core with PostgreSQL)
- External service integrations
- Messaging (MassTransit with RabbitMQ)
- Logging (Serilog with Elasticsearch)
- Caching (Redis)
- Repository implementations

### Cross-Cutting Concerns

**Orchestify.Shared**
- Logging abstractions (`ILogService`)
- Correlation tracking (`ICorrelationIdProvider`)
- Result pattern (`ServiceResult<T>`)
- Error definitions (`ServiceError`)
- Constants and configuration

**Orchestify.Contracts**
- API contracts and DTOs
- Service interfaces
- Event definitions

## Structured Logging Architecture (ORC-05)

### Design Principles

1. **Separation of Concerns**: Logging abstraction in Shared, implementation in Infrastructure
2. **Structured Data**: All logs are structured with codes and metadata
3. **Correlation Tracking**: Every log includes correlation ID for request tracing
4. **Exception Serialization**: Full exception details captured in structured format

### Log Service Abstraction

```csharp
public interface ILogService
{
    void Info(string code, string message, IDictionary<string, object?>? data = null);
    void Warn(string code, string message, IDictionary<string, object?>? data = null);
    void Error(Exception exception, string code, string message, IDictionary<string, object?>? data = null);
    void Debug(string code, string message, IDictionary<string, object?>? data = null);
}
```

### Serilog Implementation

**Orchestify.Infrastructure.Logging.SerilogLogService**

- Implements `ILogService` using Serilog
- Provides automatic enrichment:
  - Service name
  - Environment
  - Machine name
  - Process ID
  - Thread ID
  - Correlation ID (when available)
- Serializes exceptions to structured objects
- Handles nested exceptions recursively

### Log Sinks

**Console Sink**
- Colored output for local development
- Template: `[{Timestamp}] [{ServiceName}] [{CorrelationId}] [{Code}] {Message}`

**Elasticsearch Sink**
- Persistent log storage
- Daily indices: `logs-orchestify-{service}-{date}`
- Automatic index template creation
- Retry logic for startup scenarios
- HTTP compression enabled

### Log Enrichment

```csharp
// Automatic enrichment applied to all logs
LogContext.PushProperty("ServiceName", serviceName);
LogContext.PushProperty("Environment", environment);
LogContext.PushProperty("MachineName", machineName);
LogContext.PushProperty("ProcessId", processId);
LogContext.PushProperty("ThreadId", threadId);
LogContext.PushProperty("CorrelationId", correlationId); // when available
```

### Exception Serialization

Error logs include:

```json
{
  "Code": "ORC_CORE_ERROR",
  "Message": "An error occurred",
  "ExceptionType": "System.InvalidOperationException",
  "ExceptionMessage": "Invalid operation",
  "ExceptionStackTrace": "...",
  "ExceptionDetails": {
    "Type": "System.InvalidOperationException",
    "Message": "Invalid operation",
    "StackTrace": "...",
    "TargetSite": "...",
    "HResult": -2146233079,
    "InnerException": { ... }
  }
}
```

## Infrastructure Services

### Docker Compose Stack

| Service | Purpose | Port |
|---------|---------|------|
| elasticsearch | Log storage | 9200, 9300 |
| kibana | Log visualization | 5601 |
| postgres | Primary database | 5432 |
| redis | Caching layer | 6379 |
| rabbitmq | Message broker | 5672, 15672 |
| orchestify-api | Web API | 5000 |
| orchestify-worker | Background worker | - |

### Health Checks

All services include health checks:

- **Elasticsearch**: Cluster health verification
- **Kibana**: Status endpoint verification
- **PostgreSQL**: Connection readiness
- **Redis**: PING response
- **RabbitMQ**: Diagnostics ping
- **API/Worker**: HTTP health endpoint

## Middleware Pipeline

### API Middleware Order (Critical)

1. **CorrelationIdMiddleware** - Generates/extracts correlation IDs
2. **GlobalExceptionMiddleware** - Catches unhandled exceptions

### Correlation Tracking

- `X-Correlation-Id` header for cross-service tracing
- `X-Request-Id` header for request identification
- Stored in `HttpContext.Items` for service access
- Propagated to all log entries

## Dependency Flow

```
┌─────────────────────────────────────────────────────────────┐
│                      Orchestify.Api                         │
│                     Orchestify.Worker                       │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                  Orchestify.Application                     │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    Orchestify.Domain                        │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                Orchestify.Infrastructure                    │
│  - Data Access    - Logging    - Messaging    - Caching     │
└─────────────────────────────────────────────────────────────┘

                        ▲       ▲
                        │       │
            ┌───────────┘       └───────────┐
            │                                   │
┌─────────────────────┐           ┌─────────────────────┐
│ Orchestify.Shared   │           │ Orchestify.Contracts │
│ - Logging           │           │ - DTOs               │
│ - Correlation       │           │ - Interfaces         │
│ - Results           │           │                     │
└─────────────────────┘           └─────────────────────┘
```

## Package Management

### Central Package Management

- `Directory.Packages.props` - All package versions
- `Directory.Build.props` - Global MSBuild properties
- Consistent versions across all projects

### Key Dependencies

- **.NET 8.0** - Target framework
- **Serilog** - Structured logging
- **Serilog.Sinks.Elasticsearch** - Elasticsearch integration
- **Entity Framework Core** - ORM
- **Npgsql** - PostgreSQL provider
- **MassTransit** - Messaging framework
- **StackExchange.Redis** - Redis client
- **xUnit** - Testing framework

## Development Workflow

1. Implement domain entities and business rules
2. Create application services/use cases
3. Implement infrastructure (repositories, external services)
4. Add API endpoints or worker consumers
5. Write unit tests for business logic
6. Write integration tests for full workflows
7. Update documentation

## Testing Strategy

- **Unit Tests** - Business logic, rules, services
- **Integration Tests** - Database, messaging, external services
- **Testcontainers** - Isolated test environments
- **xUnit** - Test framework
- **Moq** - Mocking framework
- **FluentAssertions** - Assertion library

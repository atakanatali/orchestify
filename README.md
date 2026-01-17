# Orchestify

[![CI](https://github.com/atakanatali/orchestify/actions/workflows/ci.yml/badge.svg)](https://github.com/atakanatali/orchestify/actions/workflows/ci.yml)
[![Unit Tests](https://github.com/atakanatali/orchestify/actions/workflows/tests.yml/badge.svg)](https://github.com/atakanatali/orchestify/actions/workflows/tests.yml)

AI-powered code orchestration platform for managing automated development workflows.

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- Docker & Docker Compose
- PostgreSQL 16+ (or use Docker)
- Redis 7+ (or use Docker)
- n8n (included in Docker setup)

### Development with Docker

```bash
# Start all services (including n8n)
docker-compose -f docker-compose.yml -f docker-compose.n8n.yml up -d

# Apply migrations
dotnet ef database update --project src/Orchestify.Infrastructure --startup-project src/Orchestify.Api
```

### Development without Docker

1. Update connection strings in `appsettings.Development.json`
2. Run the API:
```bash
cd src/Orchestify.Api
dotnet run
```

3. Run the Worker:
```bash
cd src/Orchestify.Worker
dotnet run
```

4. (Optional) Run n8n separately if not using Docker.

## API Endpoints

### Workspaces
- `GET /api/workspaces` - List workspaces
- `POST /api/workspaces` - Create workspace
- `GET /api/workspaces/{id}` - Get workspace
- `PUT /api/workspaces/{id}` - Update workspace
- `DELETE /api/workspaces/{id}` - Delete workspace
- `GET /api/workspaces/discovery` - Discover workspaces in local directory

### Boards
- `GET /api/workspaces/{workspaceId}/boards` - List boards
- `POST /api/workspaces/{workspaceId}/boards` - Create board
- `GET /api/workspaces/{workspaceId}/boards/{id}` - Get board
- `PUT /api/workspaces/{workspaceId}/boards/{id}` - Update board
- `DELETE /api/workspaces/{workspaceId}/boards/{id}` - Delete board

### Tasks
- `GET /api/boards/{boardId}/tasks` - List tasks
- `POST /api/boards/{boardId}/tasks` - Create task  
- `GET /api/boards/{boardId}/tasks/{id}` - Get task
- `PUT /api/boards/{boardId}/tasks/{id}` - Update task
- `DELETE /api/boards/{boardId}/tasks/{id}` - Delete task
- `PATCH /api/boards/{boardId}/tasks/{id}/move` - Move task
- `POST /api/boards/{boardId}/tasks/{id}/run` - Run task

### Messages
- `GET /api/tasks/{taskId}/messages` - List task messages
- `POST /api/tasks/{taskId}/messages` - Send a message to the task/AI agent

### Attempts
- `GET /api/tasks/{taskId}/attempts` - List attempts
- `GET /api/tasks/{taskId}/attempts/{id}` - Get attempt
- `POST /api/tasks/{taskId}/attempts/{id}/cancel` - Cancel attempt
- `GET /api/tasks/{taskId}/attempts/{id}/steps` - List run steps

### System
- `GET /api/health` - Health check
- `GET /api/health/detailed` - Detailed health
- `GET /api/dashboard/stats` - Dashboard statistics
- `GET /api/settings` - List settings
- `PUT /api/settings/{key}` - Upsert setting

### Git & Build
- `GET /api/workspaces/{id}/git/branch` - Get current branch
- `POST /api/workspaces/{id}/git/pull` - Pull changes
- `POST /api/workspaces/{id}/git/checkout` - Checkout branch
- `POST /api/workspaces/{id}/build` - Build project
- `POST /api/workspaces/{id}/build/restore` - Restore deps
- `POST /api/workspaces/{id}/build/test` - Run tests

### Real-time
- `GET /api/attempts/{id}/stream` - SSE log stream
- SignalR Hub: `/hubs/execution`

## Architecture

```
┌─────────────────────────────────────────────────────────┐
│                        API Layer                         │
│  Controllers → MediatR Handlers → Database              │
├─────────────────────────────────────────────────────────┤
│                    Application Layer                     │
│  Commands/Queries, Validators, Pipeline Behaviors       │
├─────────────────────────────────────────────────────────┤
│                 Infrastructure Layer                     │
│  EF Core, Queue Service, Git Service, Agent Service     │
├─────────────────────────────────────────────────────────┤
│                     Domain Layer                         │
│  Entities, Enums, Value Objects                         │
└─────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────┐
│                    Worker Service                        │
│  AttemptProcessor → StepPipeline → StepExecutors        │
└─────────────────────────────────────────────────────────┘
```

## Features
- **AI-Powered Orchestration**: Integrated AI agent for managing development tasks via chat.
- **Workflow Automation**: Embedded n8n for visually designing complex automation flows.
- **Real-time Monitoring**: Stream logs and execution status directly to the web dashboard.
- **Workspace Discovery**: Automatically find and import existing projects.

## License

MIT
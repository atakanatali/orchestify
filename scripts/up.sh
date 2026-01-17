#!/bin/bash

set -e

# Load configurations
if [ -f .env.llm ]; then
    export $(grep -v '^#' .env.llm | xargs)
else
    echo "⚠️  Warning: .env.llm not found. Using default values."
    export ACTIVE_MODEL="qwen2.5-coder:7b-instruct-q4_K_M"
fi

echo "🚀 Starting Orchestify Full Stack..."

# 0. Infrastructure Prep
mkdir -p repos

# 1. Hardware Check
echo "📊 Checking System Resources..."
OS_TYPE=$(uname)

if [ "$OS_TYPE" == "Darwin" ]; then
    # macOS Memory Check
    FREE_PAGES=$(vm_stat | grep "Pages free" | awk '{print $3}' | sed 's/\.//')
    INACTIVE_PAGES=$(vm_stat | grep "Pages inactive" | awk '{print $3}' | sed 's/\.//')
    FREE_RAM_GB=$(((FREE_PAGES + INACTIVE_PAGES) * 4096 / 1024 / 1024 / 1024))
else
    # Linux/WSL2 Memory Check
    FREE_RAM_GB=$(free -g | awk '/^Mem:/{print $4}')
fi

echo "   Available RAM: ${FREE_RAM_GB}GB"
if [ "$FREE_RAM_GB" -lt 2 ]; then
    echo "⚠️  Low memory detected. Proceeding with caution..."
fi

# 2. Start Containers
echo "🐳 Starting Docker Containers (orchestify)..."
docker-compose up -d --build

# 3. Wait for Postgres
echo "🐘 Waiting for PostgreSQL to be ready..."
until docker exec orchestify-postgres pg_isready -U orchestify > /dev/null 2>&1; do
  echo -n "."
  sleep 2
done
echo " Ready!"

echo "🏗️ Applying Database Migrations..."
if command -v dotnet &> /dev/null; then
    dotnet ef database update --project src/Orchestify.Infrastructure --startup-project src/Orchestify.Api
else
    echo "⚠️  dotnet SDK not found. Skipping migrations."
fi

# 4.5 Wait for Elasticsearch
echo "🔍 Waiting for Elasticsearch to be ready..."
until curl -sf http://localhost:9200/_cluster/health > /dev/null 2>&1; do
  echo -n "."
  sleep 3
done
echo " Ready!"

# 5. Model Management
echo "📥 Checking LLM Models..."
# Ensure Ollama is ready to accept commands
until curl -s http://localhost:11434/api/tags > /dev/null; do
  echo -n "."
  sleep 2
done

echo "   Ensuring active model: $ACTIVE_MODEL"
docker exec -it orchestify-ollama ollama pull $ACTIVE_MODEL

if [ ! -z "$WARM_MODELS" ]; then
    for model in ${WARM_MODELS//,/ }; do
        echo "   Ensuring warm model: $model"
        docker exec -it orchestify-ollama ollama pull $model
    done
fi

echo ""
echo "✨ Orchestify is up and running!"
echo "------------------------------------------------"
echo "Web UI:         http://localhost:3000"
echo "API:            http://localhost:5001"
echo "n8n:            http://localhost:5678"
echo "Kibana:         http://localhost:5601"
echo "Elasticsearch:  http://localhost:9200"
echo "RabbitMQ:       http://localhost:15672 (admin UI)"
echo "Ollama:         http://localhost:11434"
echo "Postgres:       localhost:5432"
echo "Redis:          localhost:6379"
echo "------------------------------------------------"
echo "Active AI Model: $ACTIVE_MODEL"

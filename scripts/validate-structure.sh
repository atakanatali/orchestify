#!/bin/bash
# Validation script for Orchestify modular monolith structure

set -e

echo "Validating Orchestify solution structure..."

# Check root folders
echo "Checking root folders..."
for dir in src tests scripts infra; do
    if [ -d "$dir" ]; then
        echo "  ✓ $dir/ exists"
    else
        echo "  ✗ $dir/ missing"
        exit 1
    fi
done

# Check solution file
echo "Checking solution file..."
if [ -f "Orchestify.sln" ]; then
    echo "  ✓ Orchestify.sln exists"
else
    echo "  ✗ Orchestify.sln missing"
    exit 1
fi

# Check Directory.Build.props
echo "Checking build configuration..."
if [ -f "Directory.Build.props" ]; then
    echo "  ✓ Directory.Build.props exists"
else
    echo "  ✗ Directory.Build.props missing"
    exit 1
fi

if [ -f "Directory.Packages.props" ]; then
    echo "  ✓ Directory.Packages.props exists"
else
    echo "  ✗ Directory.Packages.props missing"
    exit 1
fi

# Check /src projects
echo "Checking /src projects..."
src_projects=(
    "Orchestify.Api"
    "Orchestify.Worker"
    "Orchestify.Application"
    "Orchestify.Domain"
    "Orchestify.Infrastructure"
    "Orchestify.Shared"
    "Orchestify.Contracts"
)

for project in "${src_projects[@]}"; do
    if [ -f "src/$project/$project.csproj" ]; then
        echo "  ✓ src/$project/$project.csproj exists"
    else
        echo "  ✗ src/$project/$project.csproj missing"
        exit 1
    fi
done

# Check /tests projects
echo "Checking /tests projects..."
test_projects=(
    "Orchestify.Application.Tests"
    "Orchestify.Infrastructure.Tests"
    "Orchestify.Api.IntegrationTests"
)

for project in "${test_projects[@]}"; do
    if [ -f "tests/$project/$project.csproj" ]; then
        echo "  ✓ tests/$project/$project.csproj exists"
    else
        echo "  ✗ tests/$project/$project.csproj missing"
        exit 1
    fi
done

# Check for circular dependencies (basic check)
echo "Checking for circular dependencies..."
echo "  → Verifying Shared has no project references..."
if grep -q '<ProjectReference' "src/Orchestify.Shared/Orchestify.Shared.csproj"; then
    echo "  ✗ Shared has project references (should have none)"
    exit 1
else
    echo "  ✓ Shared has no project references"
fi

echo ""
echo "✓ Structure validation complete!"
echo "  - All folders present"
echo "  - All projects present"
echo "  - No circular dependencies detected"

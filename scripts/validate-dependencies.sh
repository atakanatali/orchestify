#!/bin/bash
# Validate project reference rules for modular monolith

set -e

echo "Validating project reference rules..."

# Define expected references (format: project -> allowed_references)
declare -A expected_refs=(
    ["Orchestify.Api"]="Orchestify.Application|Orchestify.Shared|Orchestify.Contracts"
    ["Orchestify.Worker"]="Orchestify.Application|Orchestify.Shared"
    ["Orchestify.Application"]="Orchestify.Domain|Orchestify.Shared|Orchestify.Contracts"
    ["Orchestify.Infrastructure"]="Orchestify.Domain|Orchestify.Shared"
    ["Orchestify.Domain"]="Orchestify.Shared"
    ["Orchestify.Contracts"]=""
    ["Orchestify.Shared"]=""
)

check_project_refs() {
    local project=$1
    local csproj_file="src/$project/$project.csproj"
    local test_csproj_file="tests/$project/$project.csproj"

    # Use test file if it's a test project
    if [ -f "$test_csproj_file" ]; then
        csproj_file="$test_csproj_file"
    fi

    if [ ! -f "$csproj_file" ]; then
        return
    fi

    # Extract actual project references
    local actual_refs=$(grep -oP '<ProjectReference Include="\.\.[^"]*\K[^/]+(?=/[^"]+\.csproj")' "$csproj_file" 2>/dev/null | tr '\n' '|' | sed 's/|$//')

    # Get expected refs
    local expected="${expected_refs[$project]}"

    echo "  Checking $project:"

    # If no refs expected
    if [ -z "$expected" ]; then
        if [ -z "$actual_refs" ]; then
            echo "    ✓ No references (as expected)"
        else
            echo "    ✗ Expected no references, but found: $actual_refs"
            return 1
        fi
    else
        # Check each actual reference is in expected list
        if [ -n "$actual_refs" ]; then
            IFS='|' read -ra ACTUAL_ARRAY <<< "$actual_refs"
            for ref in "${ACTUAL_ARRAY[@]}"; do
                if [[ ! "$expected" == *"$ref"* ]]; then
                    echo "    ✗ Unexpected reference: $ref"
                    return 1
                fi
            done
            echo "    ✓ All references valid: $actual_refs"
        else
            echo "    ✓ No references present"
        fi
    fi
}

# Check all src projects
for project in "${!expected_refs[@]}"; do
    check_project_refs "$project" || exit 1
done

echo ""
echo "✓ Project reference validation complete!"
echo "  - All references follow Clean Architecture rules"
echo "  - No circular dependencies detected"

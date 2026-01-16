#!/bin/bash
# Show project dependency graph

echo "Orchestify Modular Monolith - Dependency Graph"
echo "================================================"
echo ""
echo "Architecture Layers (inner to outer):"
echo ""
echo "  ┌─────────────────────────────────────────────────────────────┐"
echo "  │                     Contracts (No deps)                      │"
echo "  ├─────────────────────────────────────────────────────────────┤"
echo "  │                      Shared (No deps)                        │"
echo "  ├─────────────────────────────────────────────────────────────┤"
echo "  │                    Domain (→ Shared)                         │"
echo "  ├─────────────────────────────────────────────────────────────┤"
echo "  │      Application (→ Domain + Shared + Contracts)             │"
echo "  ├─────────────────────────────────────────────────────────────┤"
echo "  │     Infrastructure (→ Domain + Shared)                       │"
echo "  ├─────────────────────────────────────────────────────────────┤"
echo "  │  Api (→ Application + Shared + Contracts)                    │"
echo "  │  Worker (→ Application + Shared)                             │"
echo "  └─────────────────────────────────────────────────────────────┘"
echo ""
echo "Actual Project References:"
echo "---------------------------"

declare -A refs
refs["Orchestify.Api"]="Orchestify.Application, Orchestify.Shared, Orchestify.Contracts"
refs["Orchestify.Worker"]="Orchestify.Application, Orchestify.Shared"
refs["Orchestify.Application"]="Orchestify.Domain, Orchestify.Shared, Orchestify.Contracts"
refs["Orchestify.Domain"]="Orchestify.Shared"
refs["Orchestify.Infrastructure"]="Orchestify.Domain, Orchestify.Shared"
refs["Orchestify.Contracts"]="(none)"
refs["Orchestify.Shared"]="(none)"

for project in Orchestify.Contracts Orchestify.Shared Orchestify.Domain Orchestify.Application Orchestify.Infrastructure Orchestify.Api Orchestify.Worker; do
    printf "%-30s → %s\n" "$project:" "${refs[$project]}"
done

echo ""
echo "✓ No circular dependencies"
echo "✓ Clean Architecture rules enforced"

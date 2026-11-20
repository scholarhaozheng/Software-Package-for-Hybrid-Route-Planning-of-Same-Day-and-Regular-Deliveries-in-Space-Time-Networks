# Build and run

Install the .NET 10 SDK and PowerShell 7. From the repository root:

```powershell
pwsh -File examples/run.ps1
pwsh -File examples/verify.ps1
```

The first command runs the space-time route and dynamic-update examples; the second checks their fixtures and invalid-input cases. Both use the standard library, include synthetic JSON input and carry their own `.csproj` files. Individual `dotnet run` commands are in the [example guide](../examples/README.md).

The research C# source uses Windows/.NET Framework and Gurobi. It is a source collection without a standalone project. Initialization and execution interfaces are defined in the [algorithm specifications](../pseudocode/README.md); the [implementation map](COVERAGE.md) identifies the corresponding C# routines. The example projects above provide runnable entry points.

Test results are recorded in [validation results](../validation/RESULTS.md).

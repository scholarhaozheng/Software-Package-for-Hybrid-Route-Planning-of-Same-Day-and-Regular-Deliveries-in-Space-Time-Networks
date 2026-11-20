# C# examples

Two C# examples cover time-space route feasibility and periodic order updates.
Both use synthetic inputs and the .NET standard library.

| Example | What it computes | Dependencies | Entry point |
| --- | --- | --- | --- |
| [Time-space routes](01-space-time/README.md) | Vehicle layers, time windows, service order and capacity | .NET 10 SDK | `01-space-time/SpaceTime.csproj` |
| [Dynamic update](02-dynamic-update/README.md) | Committed operations, onboard state and new-order acceptance | .NET 10 SDK | `02-dynamic-update/DynamicUpdate.csproj` |

From the repository root:

```powershell
pwsh -File examples/run.ps1
```

All projects target `net10.0`; no NuGet packages are required. The common project compiles
[`ArcTime`](../ClassName/ArcTime.cs) and [`ArcType`](../ClassName/ArcType.cs) value objects.
Records define locations and service events.

Each project copies `input.json` to the build output. To use another file, pass its
path after `--` when invoking `dotnet run`. Assertions check the expected case
results, so replacement inputs must satisfy those checks. Invalid inputs and failed
assertions produce a nonzero exit code.

The dynamic scheduler enumerates feasible suffixes for the small input case.
The [paper map](../docs/PAPER_MAP.md) links these concepts to the paper's methods.

See [validation results](validation/RESULTS.md) for the environment and measured
outputs, and [verify.ps1](verify.ps1) for the fixture and invalid-input checks.

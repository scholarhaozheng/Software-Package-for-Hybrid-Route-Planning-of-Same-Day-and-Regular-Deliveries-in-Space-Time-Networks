# Source index

C# modules are located at the repository root and in `ClassName/`. The table links them to the algorithm specifications. Implementation details are listed in [COVERAGE.md](COVERAGE.md) and [technical notes](TECHNICAL_NOTES.md).

| Component | C# module | Method entry |
| --- | --- | --- |
| IDs, points, arcs and branch records | `Point.cs`, `Arc.cs`, `ClassName/`, comparers | [Data structures](../pseudocode/DataStructures.pseudo) |
| Network catalog | `RoadNetwork.cs` | [Network](../pseudocode/RoadNetwork.pseudo), [all arc families](../specs/NETWORK_RULES.md) |
| Initial pool | Calls in `Program.cs`; `A_B_generator.cs` | [Arc model and initialization](../pseudocode/ArcModel.pseudo) |
| Route coefficients | `A_B_generator.cs` | [Column extraction](../pseudocode/A_B_generator.pseudo) |
| Master | `GeneratingMasterProblem.cs` | [Restricted master](../pseudocode/GeneratingMasterProblem.pseudo) |
| Pricing | `GeneratingSubProblem.cs`, `SubProblemGeneratorVeh.cs`, `SubProblemGeneratorPas.cs` | [Dispatch](../pseudocode/GeneratingSubProblem.pseudo) and separate pricing modules |
| Branching/controller | `Program.cs`, `BB_node.cs`, `BB_info.cs` | [Controller](../pseudocode/Program.pseudo), [branching](../pseudocode/Branching.pseudo) |
| Direct arc model and updates | `vrp_time_space.cs`, `OrderManager.cs`, `DynConstrGenerater.cs` | [Dynamic update](../pseudocode/DynamicUpdate.pseudo) |
| Companion model/results | `GeneratingMasterProblemQ.cs`, `Generating_TS_RS_results.cs` | [Pool selection](../pseudocode/GeneratingMasterProblemQ.pseudo), [results](../pseudocode/Results.pseudo) |

## Initialization and execution

`Program.cs` calls `vrp_time_space_ini` and `vrp_time_space_vis_pas` to construct the initial route pool. These classes are not included in this source collection; their procedures are specified in [ArcModel.pseudo](../pseudocode/ArcModel.pseudo). `A_B_generator.cs` converts the resulting paths into route costs and incidence coefficients.

`Program.Main` also calls the batch driver `String_beads0427`, which is absent from the source collection. The [batch specification](../pseudocode/BatchDrivers.pseudo) defines case selection, execution and output records. The [examples](../examples/README.md) provide project files and synthetic inputs for their two applications.

The [module index](../specs/SOURCE_MODULES.csv) lists C# paths and their associated specifications. A blank repository path identifies an initialization interface available as pseudocode.

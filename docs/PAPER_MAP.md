# Paper, source and algorithm map

**Integrated Routing of Trucks and Robots for Multimodal Urban Express Logistics**, by Hao Zheng, Han Zheng and George Giannopoulos. [DOI: 10.1177/03611981261468762](https://doi.org/10.1177/03611981261468762).

The tables map the paper's equations 1--40 and periodic rescheduling framework to the C# modules and algorithm specifications.

## Equations 1--40

Unqualified C# filenames refer to repository-root files. The [source map](SOURCE_MAP.md) describes the modules; the [coverage map](COVERAGE.md) distinguishes implemented behavior from the specification.

| Equation | Paper role | Implementation reference | Specification |
| --- | --- | --- | --- |
| 1 | Multilayer graph | `RoadNetwork.GetArc`; [network](../pseudocode/RoadNetwork.pseudo) | [RoadNetwork](../pseudocode/RoadNetwork.pseudo) |
| 2 | LT vertex set | `RoadNetwork.cs`; network module | [RoadNetwork](../pseudocode/RoadNetwork.pseudo) |
| 3 | ADR vertex set | `RoadNetwork.cs`; network module | [RoadNetwork](../pseudocode/RoadNetwork.pseudo) |
| 4 | Package vertex set | `RoadNetwork.cs`; network module | [RoadNetwork](../pseudocode/RoadNetwork.pseudo) |
| 5 | Weighted vehicle/package cost | `vrp_time_space.cs`; [arc model](../pseudocode/ArcModel.pseudo) | [ArcModel](../pseudocode/ArcModel.pseudo) |
| 6 | Vehicle source flow | `vrp_time_space.cs`; arc model | [ArcModel](../pseudocode/ArcModel.pseudo) |
| 7 | Vehicle sink flow | `vrp_time_space.cs`; arc model | [ArcModel](../pseudocode/ArcModel.pseudo) |
| 8 | Vehicle internal balance | `vrp_time_space.cs`; arc model | [ArcModel](../pseudocode/ArcModel.pseudo) |
| 9 | Package source flow | `vrp_time_space.cs`; arc model | [ArcModel](../pseudocode/ArcModel.pseudo) |
| 10 | Package sink flow | `vrp_time_space.cs`; arc model | [ArcModel](../pseudocode/ArcModel.pseudo) |
| 11 | Transfer balance | `vrp_time_space.cs`, `Constr5_Flow` | [ArcModel](../pseudocode/ArcModel.pseudo) |
| 12 | Package internal balance | `vrp_time_space.cs`; arc model | [ArcModel](../pseudocode/ArcModel.pseudo) |
| 13 | Product coupling/capacity | `vrp_time_space.cs`; arc model | [ArcModel](../pseudocode/ArcModel.pseudo) |
| 14 | Maximum duty time | `vrp_time_space.cs`; arc model | [ArcModel](../pseudocode/ArcModel.pseudo) |
| 15 | Vehicle binary domain | `vrp_time_space.cs`; arc model | [ArcModel](../pseudocode/ArcModel.pseudo) |
| 16 | Package binary domain | `vrp_time_space.cs`; arc model | [ArcModel](../pseudocode/ArcModel.pseudo) |
| 17 | Supported arc family $A_r^\Phi$ | `RoadNetwork.cs` and coupling filters | [RoadNetwork](../pseudocode/RoadNetwork.pseudo) |
| 18 | Split-route master cost | `GeneratingMasterProblem.cs`; [master](../pseudocode/GeneratingMasterProblem.pseudo) | [GeneratingMasterProblem](../pseudocode/GeneratingMasterProblem.pseudo) |
| 19 | Package-route coverage | Same master | [GeneratingMasterProblem](../pseudocode/GeneratingMasterProblem.pseudo) |
| 20 | Physical vehicle assignment | C# master uses service-based vehicle coverage; see [difference](TECHNICAL_NOTES.md) | [GeneratingMasterProblem](../pseudocode/GeneratingMasterProblem.pseudo) |
| 21 | Route-indexed nonlinear capacity | Same master | [GeneratingMasterProblem](../pseudocode/GeneratingMasterProblem.pseudo) |
| 22 | Restricted-master objective | Same master | [GeneratingMasterProblem](../pseudocode/GeneratingMasterProblem.pseudo) |
| 23 | Restricted package coverage | Same master | [GeneratingMasterProblem](../pseudocode/GeneratingMasterProblem.pseudo) |
| 24 | Restricted vehicle assignment | C# master uses service-based vehicle coverage; see [difference](TECHNICAL_NOTES.md) | [GeneratingMasterProblem](../pseudocode/GeneratingMasterProblem.pseudo) |
| 25 | First $w$ product inequality | Same master | [GeneratingMasterProblem](../pseudocode/GeneratingMasterProblem.pseudo) |
| 26 | Second $w$ product inequality | Same master | [GeneratingMasterProblem](../pseudocode/GeneratingMasterProblem.pseudo) |
| 27 | Third $w$ product inequality | Same master | [GeneratingMasterProblem](../pseudocode/GeneratingMasterProblem.pseudo) |
| 28 | Route-indexed linearized capacity | Same master | [GeneratingMasterProblem](../pseudocode/GeneratingMasterProblem.pseudo) |
| 29 | Vehicle-column LP domain | Same master | [GeneratingMasterProblem](../pseudocode/GeneratingMasterProblem.pseudo) |
| 30 | Package-column LP domain | Same master | [GeneratingMasterProblem](../pseudocode/GeneratingMasterProblem.pseudo) |
| 31 | Coupling-variable LP domain | Same master | [GeneratingMasterProblem](../pseudocode/GeneratingMasterProblem.pseudo) |
| 32 | Vehicle reduced-cost objective | `SubProblemGeneratorVeh.cs`; [vehicle pricing](../pseudocode/SubProblemGeneratorVeh.pseudo) | [SubProblemGeneratorVeh](../pseudocode/SubProblemGeneratorVeh.pseudo) |
| 33 | Vehicle-pricing source | Same vehicle pricing | [SubProblemGeneratorVeh](../pseudocode/SubProblemGeneratorVeh.pseudo) |
| 34 | Vehicle-pricing sink | Same vehicle pricing | [SubProblemGeneratorVeh](../pseudocode/SubProblemGeneratorVeh.pseudo) |
| 35 | Vehicle-pricing balance | Same vehicle pricing | [SubProblemGeneratorVeh](../pseudocode/SubProblemGeneratorVeh.pseudo) |
| 36 | Vehicle-pricing duty limit | Same vehicle pricing | [SubProblemGeneratorVeh](../pseudocode/SubProblemGeneratorVeh.pseudo) |
| 37 | Separate package reduced cost | `SubProblemGeneratorPas.cs`; [package pricing](../pseudocode/SubProblemGeneratorPas.pseudo) | [SubProblemGeneratorPas](../pseudocode/SubProblemGeneratorPas.pseudo) |
| 38 | Package-pricing source | Same package pricing | [SubProblemGeneratorPas](../pseudocode/SubProblemGeneratorPas.pseudo) |
| 39 | Package-pricing sink | Same package pricing | [SubProblemGeneratorPas](../pseudocode/SubProblemGeneratorPas.pseudo) |
| 40 | Package-pricing internal balance | Same package pricing | [SubProblemGeneratorPas](../pseudocode/SubProblemGeneratorPas.pseudo) |

The model is explained in [MODEL.md](MODEL.md), with implementation differences in [TECHNICAL_NOTES.md](TECHNICAL_NOTES.md). Equations 29--31 specify the restricted master's LP domains. Source/sink sums include the complete connector families and their time indices; the multilayer graph retains each vehicle and package layer's vertex and arc identities.

## Paper algorithm and dynamic sections

| Paper component | Implementation reference | Specification |
| --- | --- | --- |
| Methodology: assumptions and service setting | Input schemas and network restrictions | [Input](../pseudocode/Input.pseudo), [technical notes](TECHNICAL_NOTES.md) |
| Periodic order assignment/rescheduling | `Program.cs`, `OrderManager.cs`, `DynConstrGenerater.cs` | [Dynamic update](../pseudocode/DynamicUpdate.pseudo), [assignment](../pseudocode/OrderManager.pseudo), [committed constraints](../pseudocode/DynConstrGenerater.pseudo) |
| Initial route construction | Initializer calls in `Program.cs` and coefficient extraction in `A_B_generator.cs` | [Arc models](../pseudocode/ArcModel.pseudo), [column extraction](../pseudocode/A_B_generator.pseudo) |
| Decomposition controller | `Program.cs`, `GeneratingSubProblem.cs` | [Controller](../pseudocode/Program.pseudo), [pricing orchestration](../pseudocode/GeneratingSubProblem.pseudo) |
| Branch-and-bound-style integrality | `GeneratingMasterProblem.cs`, `Program.Generating_new_node` | [Branching](../pseudocode/Branching.pseudo) |
| Numerical experiments | Paper section 4, scenario inputs and figure/table data | [Batch execution and reporting](../pseudocode/BatchDrivers.pseudo) |

The mapping also covers the unnumbered product definition of $w$, the P1 summary, support-state quantifiers and the arc-cost/time-window table. Initialization calls appear in `Program.cs` at lines 851–876; their interfaces are specified in [ArcModel.pseudo](../pseudocode/ArcModel.pseudo).

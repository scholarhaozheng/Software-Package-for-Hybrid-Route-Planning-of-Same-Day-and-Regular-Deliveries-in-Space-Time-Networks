# Algorithm modules

The modules describe the paper's separate vehicle/package route method, from network construction to periodic rescheduling. Start with [notation](../specs/PSEUDOCODE.md) and [interfaces](../specs/INTERFACES.md), then follow the procedure calls from [Program](Program.pseudo).

`ASSUMPTIONS` identify design choices left open by the paper, including seed construction, reference-route pricing contexts and branch selection. Solver results carry their scope and termination status. The [technical notes](../docs/TECHNICAL_NOTES.md) compare these procedures with the C# implementation.

## Paper workflow

| Module | Role |
| --- | --- |
| [DataStructures](DataStructures.pseudo) | IDs, points, windows, arcs, columns, branch records and coefficient views |
| [Input](Input.pseudo) | Coordinate/window/attribute files and chronological arrivals |
| [RoadNetwork](RoadNetwork.pseudo) | Layered catalogs, timed arcs, physical/virtual variants and route checks |
| [ArcModel](ArcModel.pseudo) | Static arc model, initial and virtual-package solutions |
| [A_B_generator](A_B_generator.pseudo) | Route extraction, service/arc incidence, costs and pool updates |
| [GeneratingMasterProblem](GeneratingMasterProblem.pseudo) | Linearized split-route master and dual extraction |
| [GeneratingMasterProblemQ](GeneratingMasterProblemQ.pseudo) | Optional nonlinear route-pool solve |
| [SubProblemGeneratorVeh](SubProblemGeneratorVeh.pseudo) | Separate vehicle pricing |
| [SubProblemGeneratorPas](SubProblemGeneratorPas.pseudo) | Separate package pricing and supporting routes |
| [GeneratingSubProblem](GeneratingSubProblem.pseudo) | Pricing dispatch and candidate collection |
| [Branching](Branching.pseudo) | Arc marginals, child decisions and synchronized pool filtering |
| [Program](Program.pseudo) | Initialization, column loop, search and mode dispatch |
| [OrderManager](OrderManager.pseudo) | New-order eligibility screening |
| [DynConstrGenerater](DynConstrGenerater.pseudo) | Executed actions, ongoing travel and accepted service |
| [DynamicUpdate](DynamicUpdate.pseudo) | Decision epochs and residual-route interface |
| [Results](Results.pseudo) | Path formatting, transfer/sharing metrics and search records |
| [BatchDrivers](BatchDrivers.pseudo) | Case configurations, variants and initialization probes |


The [coverage map](../docs/COVERAGE.md) identifies the code and pseudocode for each method component. The [paper map](../docs/PAPER_MAP.md) links all 40 equations to these modules.

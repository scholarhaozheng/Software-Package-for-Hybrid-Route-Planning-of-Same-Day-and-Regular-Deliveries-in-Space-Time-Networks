# Algorithm interfaces

The paper uses separate vehicle routes and complete package paths. A package path may be stored as linked pickup and delivery fragments, but one route-selection decision selects the complete, transfer-consistent path. Vehicle ownership and service incidence are separate coefficients.

## Input and route records

| Record | Required information |
| --- | --- |
| Order | Stable ID, origin/destination, windows, submission time, service category, transfer/sharing permissions and rejection policy |
| Vehicle | Stable physical ID, type, source/return states, availability, capacity, duty limit and allowed network |
| Arc | Original record identity, layer/owner, full tail and head states/times, family, duration, cost and service role |
| Network | Arc catalog, incoming/outgoing adjacency, source/sink connectors, transfer matching, supported-carriage family and cost convention |
| Split route | Kind, complete path, physical owner or parent order, original cost, service/ownership/arc coefficients, linked fragments and artificial status |
| Branch decision | Entity identity, immutable original-arc key and required use/non-use |
| Physical plan | Selected vehicle/package routes, assignments, transfers, timetable, validated cost and epoch state |
| Dynamic state | Executed prefix, in-progress commitments, positions, onboard and accepted orders, pending arrivals and previous feasible plan |

Exact field definitions and result containers are in [DataStructures.pseudo](../pseudocode/DataStructures.pseudo). Preserve full point IDs, service decorations and time in endpoint keys. Each point role has a distinct identity even at shared coordinates. Freeze keys after insertion and keep route costs consistent with the declared paper catalog.

## Module contracts

| Call | Inputs → outputs | Required consistency |
| --- | --- | --- |
| `LoadCase` | File bundle/configuration → identified orders, fleet and arrivals | Unique IDs, aligned windows and chronological event access |
| `BuildNetwork` | Input and configuration → paper arc catalog | Declared family rules, complete states and consistent costs |
| `CreateSeedPool` | Network and input → initial route pool and seed result | Physical route validity; tagged artificial objects |
| `ExtractSplitColumns` | Selected arcs → complete routes and coefficients | All forward/transposed A/B views describe the same paths |
| `BuildSplitMaster` | Pool, capacity, node and domain → model/maps | Parent-order coverage, physical-vehicle ownership and all route-indexed coupling rows |
| `SolveSplitMaster` | Same pool/model context → master result | Read weights/duals only when available; retain pool and row signatures |
| `SolveSeparateVehicleModel` | Vehicle network, dual context and inherited branches → pricing result | Original reduced-cost expression, duty limit and branch-compatible path |
| `SolveSeparatePackageModel` | Package network, duals and branches → candidate and optional support routes | Pricing and support objectives/statuses remain separate |
| `PriceSplitFamilies` | Network, duals, pool and node → pricing batch | Every required pricing context has a recorded outcome |
| `InsertPricingResults` | Batch and current pool → admission result | Validate complete paths, rebuild coefficients and detect duplicate identities |
| `GenerateChildren` | Parent and branch decision → two child pools | Inherited filtering agrees with the restrictions used by future pricing |
| `SolveIntegerPool` | Current pool/node/input → candidate physical plan | Validate original arc, capacity, transfer and service conditions |
| `RunDynamicEpochs` | Validated plan, arrivals and epochs → updated plan/history | Preserve executed/in-progress work; commit only a validated replacement |

`MasterResult` separates solver status, primal availability, restricted objective, weights, row duals and residuals. `PricingBatch` retains each original pricing outcome and its context. `AdmissionResult` distinguishes newly inserted routes, duplicates and validation failures. The controller retains a validated incumbent separately from restricted-model objectives and unresolved nodes.

## Source dictionaries

The C# code uses `ADic`/`ADic2` for service-to-route and route-to-service views, `BDic`/`BDic2` for endpoint incidence, `CostDic` for costs, `VarYDic` for arc variables, `VarZDic` for assignments and `VarThetaDic` for route weights. Physical-vehicle ownership and service incidence use distinct coefficients.

An absent objective or bound is unknown. A solver bound belongs to the model and route universe that produced it. Operated-route exports exclude artificial initialization objects. Unserved-package routes carry the paper's rejection penalty. The [implementation map](../docs/COVERAGE.md) relates the code to these interfaces, and [technical notes](../docs/TECHNICAL_NOTES.md) explain source differences and stopping scope.

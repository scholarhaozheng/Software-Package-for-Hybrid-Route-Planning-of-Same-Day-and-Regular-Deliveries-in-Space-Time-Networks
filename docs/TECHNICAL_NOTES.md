# Technical notes

## C# implementation

The C# implementation uses separate vehicle and package route pools with route-indexed coupling variables and constraints. The table identifies differences from the paper and pseudocode. Module and equation references are in the [coverage map](COVERAGE.md), [source index](SOURCE_MAP.md) and [paper map](PAPER_MAP.md).

| Component | C# behavior | Technical consequence |
| --- | --- | --- |
| `Program` root stopping branch | Assigns `soluLB = 104` after a companion-master solve | This value acts as a fixed cutoff; it has no lower-bound derivation |
| Column loop | Can stop when consecutive `CostDic` value sequences match | Equal costs do not establish identical columns or complete pricing |
| `GeneratingMasterProblem` | Continuous Theta/W; exact 0/1 comparisons; route-indexed coupling; class capacity fixed at 4 | The specification uses explicit integrality tolerances and vehicle-specific capacities |
| Master coverage and paired sides | Vehicle-route coverage is service-based; pasDown aliases the corresponding pasUp Theta | The paper's vehicle-assignment row instead uses physical ownership |
| Linearized capacity row | Uses `capacity * B[pair,l]` on the RHS | Equation 28 also multiplies this term by the vehicle-route selection variable |
| Product inequalities | Two vehicle-service product rows are added only when the service-to-route A entry exists | The specification includes all three product rows, including zero service-incidence cases |
| `GeneratingMasterProblemQ` | Continuous Theta and coverage inequalities `>= 1` in a quadratic companion | A feasible operational plan requires integral route selection and physical validation |
| Vehicle-pricing dual context | Constructs and solves a pricing model for each current vehicle-route ID | Sharing a model across route IDs and aggregating their coupling duals changes the pricing objective |
| Vehicle branch list | `veh_BB_info_subpro` is stored but not translated into vehicle pricing constraints | Child pool filtering does not also constrain future generated vehicle routes |
| Package pricing | A pricing solve is followed by a support-route solve with changed objective; returned objective is used by dispatch | The stop flag compares different objectives; the specification stores the pricing and support results separately |
| Dynamic handoff | Starts from `myModel_sta` and its Y/Z maps | The final split-master Theta mixture is not reconstructed as the dynamic starting plan |
| Dynamic commitments | Arc choices with arrival time at or before implementation time are fixed | In-progress movement needs an explicit commitment interface, illustrated in the dynamic example |
| Dynamic ordering | Processing stops at the first future submission | Arrival files must be chronological |
| Coordinate cost helper | Truncated Euclidean distance supplies both duration and cost | Road-based travel times require a different input cost/duration function |
| Fixed-route input | Static/dynamic arc builder reads `file220501_12/FixedRouteTransit_*.txt` | Resolve these files alongside the selected case |
| Reporting and wrappers | Some paths assume an incumbent, exact selected values and nonempty route endpoints | Check status and path structure before using exported metrics |

The specification applies the paper's ownership and capacity rows, duty limits and inherited branches. It also defines solver-status exits and physical-plan validation. These requirements are stated explicitly in the master, pricing and controller modules.

The [network catalog](../specs/NETWORK_RULES.md) lists all 13 vehicle and 15 package arc families. The C# builder groups records into seven categories; its `tsopdown` and `tsopup` construction branches are empty. Transfers use matching pickup-end/delivery-start states and flow coupling. The paper's package-ending arc starts at the delivery-arrival state, whereas the C# delivery fragment ends after handling. The paper also assigns zero cost to carried-through package handling, while the C# catalog uses duration-based costs for these records. Endpoint and cost conventions must match the chosen model when constructing or validating routes.

## Solve scope and stopping

Adding a vehicle route changes the coupling variables and rows. Each pricing result therefore records its route-indexed dual context, and the master is rebuilt after the pool changes. A pass without improving candidates terminates that decomposition pass. Its objective applies to the current route pool; a global bound requires a separate justification. Integer route selections undergo arc, capacity and transfer validation before becoming operational plans.

The branching specification filters inherited columns and constrains future pricing. A failed pricing, pool-repair or branch step leaves an unresolved node. Time and iteration limits return the best validated plan with the termination reason.

## Dynamic state and experiments

The periodic-update specification carries executed and in-progress commitments, accepted orders, vehicle positions and onboard packages into the next epoch. It reconstructs the selected physical plan before this handoff and activates an update only after validating the replacement. An unsuccessful update keeps the previous feasible plan in operation.

Experiment records must identify the source version, solver configuration, input files, TS/RS flags and figure data. A case name alone is insufficient: different TS/RS flags define different service conditions. The two example applications use synthetic inputs to test route feasibility and dynamic updates; their results are separate from the paper's experiments.

See [validation results](../validation/RESULTS.md) for build commands, executed checks and example outputs.

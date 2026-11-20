# Paper model and method interfaces

The model represents vehicle and package movements on separate space-time layers, selects their routes through a coupled master problem, and updates routing at decision epochs. The [paper map](PAPER_MAP.md) links equations 1--40 to the C# modules and [algorithm specification](../pseudocode/Program.pseudo).

The [coverage map](COVERAGE.md) distinguishes the C# implementation from the [pseudocode specification](../pseudocode/README.md), including initialization, solver interfaces and plan validation.

## Orders, states and arcs

At decision epoch $e$, $R_e$ contains the available orders and $K$ the physical vehicles. The catalogs $A_{e,k}$ and $A_{e,r}$ contain legal vehicle and package arcs. An arc retains its full index $((i,t),(j,t'))$, including point type, complete point identifier, service state and time. Arrival, departure and passing states remain distinct even at the same geographic coordinate.

Source/end connectors, travel, handling, waiting and transfer arcs express their respective time windows and costs. The [network rule catalog](../specs/NETWORK_RULES.md) lists all 13 vehicle and 15 package arc families with endpoints, windows and costs, and maps them to `RoadNetwork.cs`. It specifies the cost conventions of both the paper and the C# builder.

## Arc-flow model: equations 5--17

The objective is

$$
\min Z=\eta^K\sum_{k\in K}\sum_{((i,t),(j,t'))\in A_{e,k}}
c^k_{i,t,j,t'}y^k_{i,t,j,t'}
+\eta^R\sum_{r\in R_e}\sum_{((i,t),(j,t'))\in A_{e,r}}
c^r_{i,t,j,t'}y^r_{i,t,j,t'}.
$$

The vehicle catalog includes a zero-cost unused-vehicle arc. The package catalog includes an unserved arc with penalty $\pi_r$, so route costs also account for unserved demand.

| Constraint family | Mathematical role | Implementation entry |
| --- | --- | --- |
| Vehicle source/sink, 6--7 | One unit enters and leaves each vehicle layer through the source/sink connector families | `vrp_time_space.cs`; initial seed models |
| Vehicle internal flow, 8 | Incoming flow equals outgoing flow at internal vehicle states | Same arc models |
| Package source/sink, 9--10 | One unit enters and leaves each package layer | Same arc models |
| Transfer balance, 11 | Package inflow and outflow agree at the transfer state and time | `Constr5_Flow` and pickup/delivery-side endpoints |
| Package internal flow, 12 | Incoming flow equals outgoing flow at other package states | Same arc models |
| Coupling and capacity, 13 | Package movement assigned to a vehicle requires its matching movement and respects onboard capacity | Arc support and service-assignment constraints |
| Duty time, 14 | Total selected vehicle arc duration is at most its allowed duty time | Vehicle arc constraints |
| Binary domains, 15--16 | Vehicle and package arc decisions are binary | Y variables |
| Supported arc subset, 17 | Travel, handling, waiting and carried-through package movements require carriage support | Network family membership and coupling filters |

[ArcModel.pseudo](../pseudocode/ArcModel.pseudo) specifies the flow constraints, initialization calls and dynamic-model inputs. [NETWORK_RULES.md](../specs/NETWORK_RULES.md) defines the supported arc subset and state transitions.

## Split-route master: equations 18--31

A vehicle column $l\in L_k$ belongs to physical vehicle $k$; a package column $q\in Q$ carries package coverage $d_{q,r}$ and arc-incidence coefficients $b_{q,i,t,j,t'}$. The master minimizes

$$\eta^K\sum_{l\in L}c_lx_l+\eta^R\sum_{q\in Q}c_qx_q,$$

with package coverage and physical-vehicle ownership rows

$$
\sum_{q\in Q}d_{q,r}x_q=1\quad(r\in R_e),\qquad
\sum_{l\in L}g_{l,k}x_l=1\quad(k\in K).
$$

Here $g_{l,k}=1$ exactly when $l\in L_k$, including a vehicle's unused route. For a supported package arc, the nonlinear master couples the vehicle's service indicator to the selected package incidence. The linearized master introduces

$$
w^r_{l,i,t,j,t'}=d_{l,r}x_l
\left(\sum_qb_{q,i,t,j,t'}d_{q,r}x_q\right)
$$

and imposes all three product inequalities:

$$
\begin{aligned}
\sum_qb_{q,i,t,j,t'}d_{q,r}x_q-w^r_{l,i,t,j,t'}&\ge0,\\
d_{l,r}x_l-w^r_{l,i,t,j,t'}&\ge0,\\
w^r_{l,i,t,j,t'}-d_{l,r}x_l-\sum_qb_{q,i,t,j,t'}d_{q,r}x_q&\ge-1.
\end{aligned}
$$

For each vehicle route and supported state transition, capacity limits the sum of the corresponding package $w$ values by $Cp_l$ times that vehicle route's selected arc incidence. Endpoint matching follows the full origin/destination, order and arrival/departure/passing indices in equation 28. During restricted-master LP solves, $x_l,x_q,w\in[0,1]$.

[A_B_generator.pseudo](../pseudocode/A_B_generator.pseudo) specifies route costs, coverage and arc-incidence extraction. [GeneratingMasterProblem.pseudo](../pseudocode/GeneratingMasterProblem.pseudo) specifies the route-selection master, dual dictionaries and branch aggregates. Adding vehicle routes also changes the route-indexed coupling rows. The C# master's service-based vehicle coverage is compared with the paper's physical-owner equality in the [technical notes](TECHNICAL_NOTES.md).

## Pricing, branching and stopping: equations 32--40

The master passes service-coverage and route-indexed coupling duals to separate vehicle and package pricing routines. Vehicle pricing modifies the vehicle arc costs with the relevant coupling and capacity duals and subtracts the vehicle-coverage term. Its feasible set preserves source/sink flow, internal balance and the duty limit. Package pricing modifies package arc costs with coverage and coupling duals and preserves its source/sink and internal-flow conditions.

The paper adds a route when its reduced cost is negative and repeats the restricted-master solve until no negative route is found. The following specifications define objective construction, solve order, candidate extraction and termination:

1. [GeneratingSubProblem.pseudo](../pseudocode/GeneratingSubProblem.pseudo) coordinates vehicle and package pricing and candidate admission.
2. [SubProblemGeneratorVeh.pseudo](../pseudocode/SubProblemGeneratorVeh.pseudo) defines vehicle pricing and its route/dual inputs.
3. [SubProblemGeneratorPas.pseudo](../pseudocode/SubProblemGeneratorPas.pseudo) defines package pricing and the subsequent supporting-vehicle solve.
4. [Branching.pseudo](../pseudocode/Branching.pseudo) defines branch selection, inherited restrictions and child-node handling; [Program.pseudo](../pseudocode/Program.pseudo) specifies the controller and stopping conditions.

Pricing results carry the route-indexed dual context, solver status and candidate objective. Package pricing and the optional supporting-vehicle solve have separate objectives. The [technical notes](TECHNICAL_NOTES.md) describe the C# behavior and the scope of the stopping tests.

## Initialization and dynamic epochs

Initialization constructs a feasible route pool before `A_B_generator` extracts its coefficients. [ArcModel.pseudo](../pseudocode/ArcModel.pseudo) specifies the initial and companion package-path models. Physical routes pass feasibility checks before export; artificial seeding objects remain internal to initialization. The [source index](SOURCE_MAP.md#initialization-and-execution) lists the C# initialization dependencies.

At each decision epoch, the periodic framework updates the available orders, vehicle state and committed decisions before solving the next routing problem. [DynamicUpdate.pseudo](../pseudocode/DynamicUpdate.pseudo) defines that sequence; [OrderManager.pseudo](../pseudocode/OrderManager.pseudo) specifies assignment eligibility; [DynConstrGenerater.pseudo](../pseudocode/DynConstrGenerater.pseudo) specifies committed actions and service obligations. The technical notes compare the specified plan handoff with the C# dynamic loop.

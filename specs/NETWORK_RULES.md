# Space-time network rules

This specification lists the paper's arc families and their mapping to `RoadNetwork.cs`. Use `paper_catalog` for the paper definitions and `local_catalog` for C# records. Each catalog identifies its rule set, time grid, cost convention and input data.

## State and time conventions

The full vertex identity is `(layer owner, point type, complete point id, time)`. The endpoint-pair key used by fixed capacity rows omits the layer owner but retains all other fields. Coordinates determine distance; state identity also includes point type, ID and time.

| State | Meaning | C# point decoration |
| --- | --- | --- |
| $o^{p,a}_{e,\Theta,r_0}$ | Arrive at order $r_0$'s pickup location before handling | `OpasUp{r0}` |
| $o^{p,p}_{e,\Theta,r_0}$ | Pass/wait through that pickup location without pickup | `MpasUp{r0}` |
| $o^{p,d}_{e,\Theta,r_0}$ | Depart that pickup location after handling | `pasUp{r0}` |
| $d^{p,a}_{e,\Theta,r_0}$ | Arrive at order $r_0$'s delivery location | `pasDown{r0}` |
| $d^{p,p}_{e,\Theta,r_0}$ | Pass/wait through that delivery location | `MpasDown{r0}` |
| $d^{p,d}_{e,\Theta,r_0}$ | Depart that delivery location after handling | `DpasDown{r0}` |
| $o_k^v,d_k^v$ | Vehicle's recorded origin and return locations | Vehicle input points, retaining their point types and ids |
| $vo,vd$ | Virtual source and sink | Source/end records for the relevant layer |
| $ts_r$ | Package transfer state | Represented in the C# flow coupling through pickup-end/delivery-start state-time identities |
| $l^{dc},l^{et}_{st}$ | Distribution center and express terminals | Input spatial points; their identities remain distinct from service states |

Here $\Theta$ is the vehicle or package layer owner. The physical location of all three states for one pickup/delivery point is the same. In the C# source representation, `pasUp_r` and `pasDown_r` also name **layer owners** for $r^+$ and $r^-$. A layer owner and a point named `pasUp{r0}` are different fields.

The paper uses a one-minute grid. $I_k,J_k$ are vehicle departure/return windows; $I_r,J_r$ are package pickup/delivery windows. $t^o$ is handling duration and $t^{to}$ transfer duration. In the tables, $r_0$ denotes the order whose location is being visited; its pickup/delivery window applies to handling at that location.

Each allowed arc has both endpoints on the chosen grid, within the epoch horizon and in its layer's vertex catalog. Travel uses the configured travel-time function. Handling and transfer connectors advance time as shown; zero-duration connectors preserve a declared service-state order. If half-transfer durations do not lie on the grid, the input must declare a compatible grid or discretization policy before generating those arcs.

## Vehicle arc families

The IDs match the 13 vehicle families in the paper's space-time arc table. Families 11 and 12 each have two endpoint forms. Outside the listed feasible window the paper gives infinite cost; a sparse implementation omits the infeasible arc.

| ID / family | Endpoints $((i,t),(j,t'))$ | Feasible departure time | Cost | Role |
| --- | --- | --- | --- | --- |
| 1 / starting | $(vo,t)\to(o_k^v,t)$ | $t\in I_k$ | $0$ | Enter the vehicle's own operating layer |
| 2 / ending | $(d_k^v,t)\to(vd,t)$ | $t\in J_k$ | $0$ | Complete the vehicle route at its recorded return location |
| 3 / travel | $(i,t)\to(j,t')$ | $t\in[I_k^l,J_k^u]$; arrival on grid/in horizon | $c_k^{tr}(t'-t)$ | Move between permitted spatial states |
| 4 / loading | $(o^{p,a}_{e,k,r_0},t)\to(o^{p,d}_{e,k,r_0},t+t^o)$ | $t\in I_{r_0}\cap[I_k^l,J_k^u]$ | $c_k^o t^o$ | Perform pickup handling |
| 5 / unloading | $(d^{p,a}_{e,k,r_0},t)\to(d^{p,d}_{e,k,r_0},t+t^o)$ | $t\in J_{r_0}\cap[I_k^l,J_k^u]$ | $c_k^o t^o$ | Perform delivery handling |
| 6 / dwelling | $(i,t)\to(i,t')$ at a valid location | $t\in[I_k^l,J_k^u]$, $t'>t$ | $c_k^{dw}(t'-t)$ | Wait at a retained state |
| 7 / wait load I | $(o^{p,a}_{e,k,r_0},t)\to(o^{p,p}_{e,k,r_0},t+t^o)$ | $t\in I_{r_0}\cap[I_k^l,J_k^u]$ | $c_k^w t^o$ | Traverse a pickup location without loading |
| 8 / wait load II | $(o^{p,p}_{e,k,r_0},t)\to(o^{p,d}_{e,k,r_0},t)$ | $t\in I_{r_0}\cap[I_k^l,J_k^u]$ | $0$ | Finish the pickup passing transition |
| 9 / wait unload I | $(d^{p,a}_{e,k,r_0},t)\to(d^{p,p}_{e,k,r_0},t+t^o)$ | $t\in J_{r_0}\cap[I_k^l,J_k^u]$ | $c_k^w t^o$ | Traverse a delivery location without unloading |
| 10 / wait unload II | $(d^{p,p}_{e,k,r_0},t)\to(d^{p,d}_{e,k,r_0},t)$ | $t\in J_{r_0}\cap[I_k^l,J_k^u]$ | $0$ | Finish the delivery passing transition |
| 11(1) / transfer I | $(o^{p,d}_{e,k,r},t)\to(ts_r,t)$ | $t\in I_r\cap[I_k^l,J_k^u]$ | $0$ | Enter a transfer state from a pickup-departure state |
| 11(2) / transfer I | $(i,t)\to(ts_r,t+t^{to}/2)$, $i\in\{l^{dc},l^{et}_{st}\}$ | $t\in I_r\cap[I_k^l,J_k^u]$ | $\tfrac12 c_r^{tr}t^{to}$ | First half of terminal handover |
| 12(1) / transfer II | $(ts_r,t)\to(o^{p,d}_{e,k,r},t)$ | $t\in I_r\cap[I_k^l,J_k^u]$ | $0$ | Leave a transfer state for a pickup-departure state |
| 12(2) / transfer II | $(ts_r,t)\to(i,t+t^{to}/2)$, $i\in\{l^{dc},l^{et}_{st}\}$ | $t\in I_r\cap[I_k^l,J_k^u]$ | $\tfrac12 c_r^{tr}t^{to}$ | Second half of terminal handover |
| 13 / unused | $(vo,t)\to(vd,t)$ | $t\in[I_k^l,J_k^u]$ | $0$ | Select an unused-vehicle route |

For ADR travel, departure states are the pickup/delivery departure states and $o_k^v$; arrival states are the pickup/delivery arrival states and $d_k^v$. LT travel additionally permits express terminals. Vehicle type, operating zone, location reachability and input arcs determine which pairs exist. The paper uses $c_r^{tr}$ for transfer costs in both the vehicle and package tables.

## Package arc families

These are the 15 package families in the same arc table. Their costs contribute to the package part of the objective, including the penalty on family 13.

| ID / family | Endpoints | Feasible departure time | Cost | Role |
| --- | --- | --- | --- | --- |
| 1 / starting | $(vo,t)\to(o^{p,a}_{e,r,r_0},t)$ | $t\in I_r$ | $0$ | Enter a package layer at a permitted pickup state |
| 2 / ending | $(d^{p,a}_{e,r,r_0},t)\to(vd,t)$ | $t\in J_r$ | $0$ | Exit the package layer at the table's stated delivery-arrival state |
| 3 / travel | $(i,t)\to(j,t')$ at permitted service/terminal states | $t\in[I_r^l,J_r^u]$; arrival on grid/in horizon | $c_r^{tr}(t'-t)$ | Package movement supported by vehicle carriage |
| 4 / loaded | $(o^{p,a}_{e,r,r},t)\to(o^{p,d}_{e,r,r},t+t^o)$ | $t\in I_r$ | $c_r^o t^o$ | Handle the package at its own pickup |
| 5 / unloaded | $(d^{p,a}_{e,r,r},t)\to(d^{p,d}_{e,r,r},t+t^o)$ | $t\in J_r$ | $c_r^o t^o$ | Handle the package at its own delivery |
| 6 / dwelling | $(i,t)\to(i,t')$ at permitted service/terminal states | $t\in[I_r^l,J_r^u]$, $t'>t$ | $c_r^{tr}(t'-t)$ | Retain package flow while waiting |
| 7 / wait pickup I | $(o^{p,a}_{e,r,r_0},t)\to(o^{p,p}_{e,r,r_0},t+t^o)$ | $t\in[I_r^l,J_r^u]$ | $c_r^o t^o$ | Pass through a pickup operation |
| 8 / wait pickup II | $(o^{p,p}_{e,r,r_0},t)\to(o^{p,d}_{e,r,r_0},t)$ | $t\in[I_r^l,J_r^u]$ | $0$ | Complete pickup passing |
| 9 / wait dropoff I | $(d^{p,a}_{e,r,r_0},t)\to(d^{p,p}_{e,r,r_0},t+t^o)$ | $t\in[I_r^l,J_r^u]$ | $c_r^o t^o$ | Pass through a delivery operation |
| 10 / wait dropoff II | $(d^{p,p}_{e,r,r_0},t)\to(d^{p,d}_{e,r,r_0},t)$ | $t\in[I_r^l,J_r^u]$ | $0$ | Complete delivery passing |
| 11(1) / transfer I | $(o^{p,d}_{e,r,r},t)\to(ts_r,t)$ | $t\in[I_r^l,J_r^u]$ | $0$ | Enter a pickup-side transfer state |
| 11(2) / transfer I | $(i,t)\to(ts_r,t+t^{to}/2)$, $i\in\{l^{dc},l^{et}_{st}\}$ | $t\in[I_r^l,J_r^u]$ | $\tfrac12 c_r^{tr}t^{to}$ | First half of terminal transfer |
| 12(1) / transfer II | $(ts_r,t)\to(o^{p,d}_{e,r,r},t)$ | $t\in[I_r^l,J_r^u]$ | $0$ | Leave a pickup-side transfer state |
| 12(2) / transfer II | $(ts_r,t)\to(i,t+t^{to}/2)$, $i\in\{l^{dc},l^{et}_{st}\}$ | $t\in[I_r^l,J_r^u]$ | $\tfrac12 c_r^{tr}t^{to}$ | Second half of terminal transfer |
| 13 / unserved | $(vo,t)\to(vd,t)$ | $t\in[I_r^l,J_r^u]$ | $\pi_r$ | Record an unserved package in the paper objective |
| 14 / carried through pickup | $(o^{p,a}_{e,r,r_0},t)\to(o^{p,d}_{e,r,r_0},t+t^o)$ | $t\in[I_r^l,J_r^u]$ | $0$ | Remain carried while another pickup is handled |
| 15 / carried through dropoff | $(d^{p,a}_{e,r,r_0},t)\to(d^{p,d}_{e,r,r_0},t+t^o)$ | $t\in[I_r^l,J_r^u]$ | $0$ | Remain carried while another delivery is handled |

Package travel and dwelling use origin/destination arrival and departure states plus express terminals. Own-order handling and carried-through handling are distinct family records even when endpoint coordinates coincide. The paper places the package ending endpoint at arrival state $d^{p,a}$. The C# endpoint follows unloading; see [technical notes](../docs/TECHNICAL_NOTES.md).

The supported carriage set $A_r^\Phi$ comprises families 3--10, 14 and 15. Virtual start/end, transfer and unserved connectors lie outside this supported subset.

## Mapping to the C# builder

The repository-root `RoadNetwork.cs` declares seven string categories at lines 60--66. Its active construction is organized by `pasUp`, `pasDown` and `veh` layers, rather than one branch per numbered paper family.

| C# category / source location | Paper roles | Construction |
| --- | --- | --- |
| `travel`: pickup 173, delivery 664, vehicle 1058 | Vehicle/package family 3 | Entity-specific state-pair filters, reachability bounds and travel duration |
| `dwell`: pickup 244, delivery 721, vehicle 1516 | Family 6 and service-state passing transitions 7--10; some carried-through transitions 14--15 | Direct and `M`-state transitions share one category; inspect endpoints and owner to identify the role |
| `start`: pickup 469, delivery 918, vehicle 1989 | Family 1; also the delivery fragment's possible transfer entry | Pickup-side initial connector and delivery-side transfer entry have different filters |
| `end`: pickup 504, delivery 883, vehicle 1922 | Family 2; also the pickup fragment's possible transfer exit | Pickup-end/delivery-start records export matching point/time transfer states |
| `oper`: pickup 595, delivery 997, vehicle 2027 | Own-order/vehicle handling, families 4--5 | Uses `opTime`; vehicle and package cases use different endpoint filters |
| `tsopdown`, `tsopup`: vehicle 2100, 2104 | Names related to transfer families 11--12 | Both declared construction branches are empty in this C# source |
| No separate `unused`/`unserved` category | Family 13 | Idle, virtual and unserved policies must be identified from actual path/solver records; a category-name match is unavailable |

`GeneratingTravellingCost` at lines 99--105 truncates Euclidean distance to an integer and uses it as both travel time and cost. `GeneratingTimeVertex` at lines 107--123 sets the arrival time to departure time plus duration and assigns duration as the arc value. Individual branches adjust special records. The pseudocode uses the family costs listed above; importing a C# catalog requires checking its cost convention.

Transfer balance in `vrp_time_space.Constr5_Flow` connects pickup-end and delivery-start states. The `tsop*` branches are empty.

## Generation interface

```text
GeneratePaperCatalog(case, grid, familyRules)
    input: physical locations; layer owners; I/J windows; travel times;
           vehicle type/zone; handling/transfer times; rate and penalty costs
    output: arc records with owner, full endpoints, family id, duration and cost

    FOR each vehicle and each rule in the vehicle table
        Enumerate endpoint pairs allowed by the rule and layer restrictions.
        Intersect the rule's departure-time window with the epoch/grid.
        FOR each permitted departure time
            Compute arrival time using travel time, handling time or the rule offset.
            Retain zero-time transitions only in the declared service-state order.
            Check both endpoint states and grid membership.
            Emit a separate record with the rule's duration and cost.
    FOR each package and each rule in the package table
        Apply the same procedure with its own-order and carried-through distinctions.
    Tag supported carriage families 3--10 and 14--15.
    Return the complete catalogs and their rule/cost fingerprint.

ImportLocalCatalog(case, RoadNetwork.GetArc output)
    input: original dictionaries keyed by entity type/id, category, ArcID and ArcTime
    output: original records plus a descriptive paper-role mapping

    Preserve owner, complete point ids, times, record multiplicity and actual cost.
    Classify a role using category, endpoint states and owner relation.
    Preserve a multiple/unknown mapping when one C# category spans paper roles.
    Build pickup-end/delivery-start transfer-state identities from recorded endpoints.
    Return actual catalogs, classification and the C# cost fingerprint.
```

The [network procedure](../pseudocode/RoadNetwork.pseudo) defines paper-catalog generation and the source import interface. [Data structures](../pseudocode/DataStructures.pseudo) define the complete keys and coefficient records.

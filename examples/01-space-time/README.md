# Time-space network and route feasibility

```powershell
dotnet run --project examples/01-space-time/SpaceTime.csproj
```

The input lists ten directed arcs in truck and robot layers. A vertex contains its
location, service state and integer time. Arcs advance time and perform travel,
waiting, pickup or delivery. Selecting an arc requires the exact previous endpoint
and the vehicle's layer. A service visit consumes time; a delivery must finish by
its deadline.

`Network.Validate(Route)` returns feasibility, a reason, route cost and peak load.
It checks continuity, pickup before delivery, one pickup per order, empty final load
and the prescribed sink. Shared orders use their input quantity; an exclusive order
uses the vehicle's entire capacity while onboard. Costs are the sum of selected arc
costs. The two vehicle layers in this fixture are independent.

Expected output includes:

```text
truck1: feasible; arcs=6; cost=9.00; peakLoad=1
robot1: feasible; arcs=4; cost=3.00; peakLoad=1
PASS: 10 directed time-space arcs; 2 feasible routes; 4 infeasible variants; original ArcTime/ArcType checks.
```

The negative variants remove available capacity, break the route source, tighten a
delivery deadline and change the vehicle layer. The supplied file is synthetic and
uses integer time units and arbitrary nonnegative costs.

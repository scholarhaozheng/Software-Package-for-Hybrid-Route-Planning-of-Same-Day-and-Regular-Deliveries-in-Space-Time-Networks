# Dynamic orders and a frozen route prefix

```powershell
dotnet run --project examples/02-dynamic-update/DynamicUpdate.csproj
```

The input has one vehicle, four locations and a complete directed travel table.
Order `r1` is already accepted. At time 3 its pickup at A is complete, so the vehicle
is at A carrying `r1`. The update receives feasible `r2` and infeasible `r3`.

The procedure is:

1. Freeze every step that started before the update time. An in-progress step is
   committed through its end; the new suffix starts after that end.
2. Derive the current location, time, picked orders and delivered orders from this
   prefix.
3. Consider newly released orders in input order. Enumerate precedence-feasible
   suffixes for the accepted set plus the candidate. Enforce capacity, service
   duration, release times, delivery deadlines and the return horizon.
4. Accept a candidate with a feasible suffix; otherwise retain the current accepted
   set and suffix. Minimize remaining travel cost, breaking ties by return time.
5. Combine the unchanged prefix and the new suffix; check the entire route again.

`Planner.Solve(location, time, orders, picked, delivered)` returns a suffix `Plan`
or `null`. `Planner.CheckPlan` independently checks the final sequence.

Expected: two frozen steps, onboard `r1`, acceptance of `r2`, rejection of `r3`,
remaining travel cost 4 and return time 10. The program also checks commitment of an
arc in progress. Existing accepted orders remain mandatory.

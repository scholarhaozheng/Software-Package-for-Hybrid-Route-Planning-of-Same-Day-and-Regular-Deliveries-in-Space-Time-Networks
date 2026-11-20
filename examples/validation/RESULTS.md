# Example validation

Run date: 2026-11-21. Environment: Windows 10 x64, .NET SDK 10.0.300,
.NET runtime 10.0.8. Inputs: synthetic cases supplied with the examples.

| Check | Result |
| --- | --- |
| Time-space project | Passed; truck cost 9, robot cost 3; ten arcs, two feasible routes and four rejected variants |
| Dynamic project | Passed; two steps frozen at time 3; `r2` accepted, `r3` rejected; suffix travel cost 4; return at time 10 |
| Process-level invalid inputs | Non-advancing arc and duplicate order both returned exit code 1 |

Outputs:

- [Time-space stdout](space-time-output.txt)
- [Dynamic stdout](dynamic-output.txt)
- [Process-check results](check-results.json)
- [Machine-readable summary](summary.json)

Run the four process-level checks with:

```powershell
pwsh -File examples/verify.ps1
```

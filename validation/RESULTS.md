# Validation results

Test date: **2026-11-21**.

| Test | Result |
| --- | --- |
| C# source comparison with baseline `866b7446` | All 29 files match |
| Equation references and module links | Equations 1–40 have module entries |
| README and documentation links | Local file, image and heading links resolve |
| Space-time route example | Passed; truck cost 9, robot cost 3 |
| Dynamic order example | Passed; `r2` accepted, `r3` rejected, return at time 10 |
| Non-advancing arc input | Rejected with exit code 1 |
| Duplicate order input | Rejected with exit code 1 |

Both valid cases exit with code 0. Console output and process results are available in the [example test records](../examples/validation/RESULTS.md).

The checks cover the synthetic examples, source integrity and documentation consistency.

## Run the checks

With Python 3, .NET 10 and PowerShell 7 installed, run from the repository root:

```powershell
python validation/check_repository.py
pwsh -File examples/verify.ps1
```

The repository check verifies source integrity, project and module references, pseudocode headers, equation mappings and documentation links. Recorded results are in [repository-checks.json](repository-checks.json); pass `--write` to update this file. See [BUILD.md](../docs/BUILD.md) for the example entry points.

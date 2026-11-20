[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$exampleRoot = $PSScriptRoot
$temporaryInputs = Join-Path $exampleRoot 'validation/tmp'
New-Item -ItemType Directory -Force -Path $temporaryInputs | Out-Null
$results = [System.Collections.Generic.List[object]]::new()
function Invoke-Check([string]$name, [string]$project, [string]$inputPath, [int]$expected) {
    $arguments = @('run', '--project', (Join-Path $exampleRoot $project))
    if ($inputPath) { $arguments += @('--', $inputPath) }
    $output = & dotnet @arguments 2>&1
    $actual = $LASTEXITCODE
    $results.Add([pscustomobject]@{check=$name;expectedExit=$expected;actualExit=$actual;passed=($actual -eq $expected)})
    $output | Write-Output
    if ($actual -ne $expected) { throw "$name expected exit $expected but received $actual" }
}
Invoke-Check 'space-time fixture' '01-space-time/SpaceTime.csproj' '' 0
Invoke-Check 'dynamic fixture' '02-dynamic-update/DynamicUpdate.csproj' '' 0
$network = Get-Content (Join-Path $exampleRoot '01-space-time/input.json') -Raw | ConvertFrom-Json
$network.arcs[0].to.time = 0
$networkPath = Join-Path $temporaryInputs 'backward-time.json'
$network | ConvertTo-Json -Depth 20 | Set-Content $networkPath -Encoding utf8
Invoke-Check 'non-advancing arc rejected' '01-space-time/SpaceTime.csproj' $networkPath 1
$dynamic = Get-Content (Join-Path $exampleRoot '02-dynamic-update/input.json') -Raw | ConvertFrom-Json
$dynamic.incoming[0].id = $dynamic.accepted[0].id
$dynamicPath = Join-Path $temporaryInputs 'duplicate-order.json'
$dynamic | ConvertTo-Json -Depth 20 | Set-Content $dynamicPath -Encoding utf8
Invoke-Check 'duplicate order rejected' '02-dynamic-update/DynamicUpdate.csproj' $dynamicPath 1
$results | ConvertTo-Json -Depth 4 | Set-Content (Join-Path $exampleRoot 'validation/check-results.json') -Encoding utf8
Write-Output "PASS: $($results.Count) process-level checks."

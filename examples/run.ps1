[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$exampleRoot = $PSScriptRoot
foreach ($project in @('01-space-time/SpaceTime.csproj', '02-dynamic-update/DynamicUpdate.csproj')) {
    & dotnet run --project (Join-Path $exampleRoot $project)
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}

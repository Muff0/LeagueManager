param(
    [string]$MigrationName = "AutoMigration",
    [string]$Project = "Data/Data.csproj",
    [string]$StartupProject = "LeagueCoreService/LeagueCoreService.csproj",
    [ValidateSet("LeagueContext", "QueueContext")]
    [string]$Context = "LeagueContext"
)

$Path = switch ($Context) {
    "LeagueContext" { "Migrations/League" }
    "QueueContext"  { "Migrations/Queue" }
}

dotnet ef migrations add $MigrationName --project $Project --startup-project $StartupProject --context $Context --output-dir $Path
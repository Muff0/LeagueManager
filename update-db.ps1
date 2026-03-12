
param(
    [string]$MigrationName = "AutoMigration",
    [string]$LeagueProject = "Data/Data.csproj",
    [string]$LeagueStartupProject = "LeagueCoreService/LeagueCoreService.csproj" 
)

# Create migration
dotnet ef migrations add $MigrationName --project $LeagueProject --startup-project $LeagueStartupProject --context LeagueContext --output-dir Migrations/League



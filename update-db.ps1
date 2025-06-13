
param(
    [string]$MigrationName = "AutoMigration",
    [string]$LeagueProject = "Data/Data.csproj",
    [string]$LeagueStartupProject = "LeagueManager/LeagueManager.csproj" 
)

# Create migration
dotnet ef migrations add $MigrationName --project $LeagueProject --startup-project $LeagueStartupProject --context LeagueContext

# Apply migration
dotnet ef database update --project $LeagueProject --startup-project $LeagueStartupProject --context LeagueContext
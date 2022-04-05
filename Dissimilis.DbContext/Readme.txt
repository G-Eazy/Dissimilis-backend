# Entity FramWork help
    get-help entityframework


# Install/Update ef tools
    dotnet tool install --global dotnet-ef --version 5.0.2
    dotnet tool update --global dotnet-ef --version 5.0.2


# Migration help
## Add another migration
    dotnet ef migrations add InitialMigration --project Dissimilis.DbContext --startup-project Dissimilis.WebAPI --context DissimilisDbContext


## Remove last migration
    Remove-Migration -Context DissimilisDbContext
    Remove-Migration -Force
    Remove-Migration -Verbose


## Update db urt run the project
    Update-Database -context DissimilisDbContext
    Update-Database -Migration Test


## Look at differences between migrations
    Script-Migration -From Test2 -To Test3    








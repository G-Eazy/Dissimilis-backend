# Entity FramWork help
    get-help entityframework


# Install/Update ef tools
    dotnet tool install --global dotnet-ef --version 3.1.2
    dotnet tool update --global dotnet-ef --version 3.1.3


# Migration help
## Add another migration
    Add-Migration InitialCreate -context DissimilisDbContext
    dotnet ef migrations add AddIptModels --project Dissimilis.WebAPI. --startup-project Dissimilis.WebAPI--context DissimlisDbContext


## Remove last migration
    Remove-Migration -Context DissimilisDbContext
    Remove-Migration -Force
    Remove-Migration -Verbose


## Update db urt run the project
    Update-Database -context DissimilisDbContext
    Update-Database -Migration Test


## Look at differences between migrations
    Script-Migration -From Test2 -To Test3    








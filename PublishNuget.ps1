&dotnet build

$folderPath = "C:\Repos\EventSourcingCQRS\src\EventSourcingCQRS\bin\Debug"
$latestFile = Get-ChildItem $folderPath | Sort-Object LastWriteTime -Descending | Select-Object -First 1
Write-Host "The most recently changed file is: $($latestFile.Name)"
.\nuget.exe add ($latestFile.FullName) -source \\z8\NugetPackages

$folderPath = "C:\Repos\EventSourcingCQRS\src\EventSourcingCQRS.EventStore.MongoDb\bin\Debug"
$latestFile = Get-ChildItem $folderPath | Sort-Object LastWriteTime -Descending | Select-Object -First 1
Write-Host "The most recently changed file is: $($latestFile.Name)"
.\nuget.exe add ($latestFile.FullName) -source \\z8\NugetPackages

$folderPath = "C:\Repos\EventSourcingCQRS\src\EventSourcingCQRS.EventStore.SqlServer\bin\Debug"
$latestFile = Get-ChildItem $folderPath | Sort-Object LastWriteTime -Descending | Select-Object -First 1
Write-Host "The most recently changed file is: $($latestFile.Name)"
.\nuget.exe add ($latestFile.FullName) -source \\z8\NugetPackages

$folderPath = "C:\Repos\EventSourcingCQRS\src\EventSourcingCQRS.EventStore.AzureTableStorage\bin\Debug"
$latestFile = Get-ChildItem $folderPath | Sort-Object LastWriteTime -Descending | Select-Object -First 1
Write-Host "The most recently changed file is: $($latestFile.Name)"
.\nuget.exe add ($latestFile.FullName) -source \\z8\NugetPackages

$folderPath = "C:\Repos\EventSourcingCQRS\src\EventSourcingCQRS.EventStore.CosmosDb\bin\Debug"
$latestFile = Get-ChildItem $folderPath | Sort-Object LastWriteTime -Descending | Select-Object -First 1
Write-Host "The most recently changed file is: $($latestFile.Name)"
.\nuget.exe add ($latestFile.FullName) -source \\z8\NugetPackages/
# Set the NuGet feed URL and your API key
$nugetFeedUrl = "http://localhost:8196/v3/index.json"
$apiKey = "YourApiKey"

# Specify the folder containing the NuGet package files
$packageFolder = "LocalPublish"

# Get a list of all .nupkg files in the specified folder
$packageFiles = Get-ChildItem -Path $packageFolder -Filter "*.nupkg"

# Loop through each package file and push it to the NuGet feed
foreach ($packageFile in $packageFiles) {
    $packageName = $packageFile.Name
    
    $publishCommand = "dotnet nuget push -s `"$nugetFeedUrl`" `"$($packageFile.FullName)`""
    Invoke-Expression $publishCommand
    
}
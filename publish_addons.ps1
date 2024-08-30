# build modulartistic
$base_dir = $PSScriptRoot
Set-Location -Path $base_dir

# Build AddOns
$AddOnBaseDir = "$PSScriptRoot\AddOns"
$AddOnOutputDir = "$PSScriptRoot\publish\addons"

# Create the output directory if it doesn't exist
if (-Not (Test-Path -Path $AddOnOutputDir)) {
    New-Item -ItemType Directory -Path $AddOnOutputDir
}

# Get all addon directories
$AddonDirs = Get-ChildItem -Path $AddOnBaseDir -Directory -Filter "Modulartistic.AddOns.*"

foreach ($dir in $AddonDirs) {
    $csprojFile = Get-ChildItem -Path $dir.FullName -Filter "*.csproj" | Select-Object -First 1

    if ($csprojFile) {
        # Build
        Write-Host "Publishing $($csprojFile.Name)..."
        dotnet publish $csprojFile.FullName -c Release -o "$($dir.FullName)\publish"

        # Check if the publish was successful
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Failed to publish $($csprojFile.Name)" -ForegroundColor Red
            exit 1
        }

        # Move to publish output directory
        Move-Item -Path "$($dir.FullName)\publish\*" -Destination "$AddOnOutputDir"
    } else {
        Write-Host "No .csproj file found in $($dir.FullName)" -ForegroundColor Yellow
    }
}

Write-Host "All addons have been published and moved to $AddOnOutputDir"


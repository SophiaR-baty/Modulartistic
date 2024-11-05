# Clear any existing output in the build location before building
$base_dir = $PSScriptRoot
Set-Location -Path $base_dir

# Build AddOns
$AddOnBaseDir = "$PSScriptRoot\AddOns"
$AddOnOutputDir = "$PSScriptRoot\publish\addons"

# Delete contents of the output directory before building
if (Test-Path -Path $AddOnOutputDir) {
    Write-Host "Cleaning output directory"
	Remove-Item -Path "$AddOnOutputDir\*" -Recurse -Force -ErrorAction Stop
} else {
    # Create the output directory if it doesn't exist
    Write-Host "Creating output directory"
	New-Item -ItemType Directory -Path $AddOnOutputDir
}

# Get all addon directories
$AddonDirs = Get-ChildItem -Path $AddOnBaseDir -Directory -Filter "Modulartistic.AddOns.*"

foreach ($dir in $AddonDirs) {
    $csprojFile = Get-ChildItem -Path $dir.FullName -Filter "*.csproj" | Select-Object -First 1

    if ($csprojFile) {
        # Build with error output only
        Write-Host "Publishing $($csprojFile.Name)..."
        dotnet publish $csprojFile.FullName -c Release -o "$($dir.FullName)\publish" *> $null 2>&1

        # Check if the publish was successful
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Failed to publish $($csprojFile.Name)" -ForegroundColor Red
            exit 1
        }

        # Copy files to publish output directory with overwrite, then remove from source
        Copy-Item -Path "$($dir.FullName)\publish\*" -Destination "$AddOnOutputDir" -Recurse -Force -ErrorAction Stop
        Remove-Item -Path "$($dir.FullName)\publish\*" -Recurse -Force -ErrorAction Stop
    } else {
        Write-Host "No .csproj file found in $($dir.FullName)" -ForegroundColor Yellow
    }
}

Write-Host "All addons have been published and moved to $AddOnOutputDir"

# If a console argument is provided, copy the build result to the specified location
if ($args.Count -gt 0) {
    $destinationPath = $args[0]
    
	Write-Host "Copying addons to $destinationPath"
	
	if (-Not (Test-Path -Path $destinationPath)) {
        New-Item -ItemType Directory -Path $destinationPath
    }
    
    Copy-Item -Path "$AddOnOutputDir\*" -Destination $destinationPath -Recurse -Force -ErrorAction Stop
    Write-Host "Build results copied to $destinationPath"
}

# Set the base directory
$base_dir = $PSScriptRoot
Set-Location -Path $base_dir

# Define publish directories
$WindowsOutput = "$base_dir\publish\windows"
$WindowsSingleFileOutput = "$base_dir\publish\windows_singlefile"
$LinuxOutput = "$base_dir\publish\linux"
$LinuxSingleFileOutput = "$base_dir\publish\linux_singlefile"

# Suppress regular output, show only errors during dotnet restore
dotnet restore *> $null 2>&1

# Clear output directories before building
Write-Host "Cleaning output directories"
Remove-Item -Path "$WindowsOutput\*" -Recurse -Force -ErrorAction Ignore
Remove-Item -Path "$WindowsSingleFileOutput\*" -Recurse -Force -ErrorAction Ignore
Remove-Item -Path "$LinuxOutput\*" -Recurse -Force -ErrorAction Ignore
Remove-Item -Path "$LinuxSingleFileOutput\*" -Recurse -Force -ErrorAction Ignore


# Build and publish for Windows
Write-Host "Building Modulartistic for windows"
dotnet publish "$base_dir\Modulartistic\Modulartistic.csproj" -c Release -r win-x64 -p:DebugSymbols=false -o $WindowsOutput *> $null 2>&1

Write-Host "Building Modulartistic for windows as single file"
dotnet publish "$base_dir\Modulartistic\Modulartistic.csproj" -c Release -r win-x64 -p:PublishSingleFile=true -p:DebugSymbols=false -o $WindowsSingleFileOutput *> $null 2>&1

# Build and publish for Linux
Write-Host "Building Modulartistic for linux"
dotnet publish "$base_dir\Modulartistic\Modulartistic.csproj" -c Release -r linux-x64 -p:DebugSymbols=false -o $LinuxOutput *> $null 2>&1

Write-Host "Building Modulartistic for linux as single file"
dotnet publish "$base_dir\Modulartistic\Modulartistic.csproj" -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true -p:DebugSymbols=false -o $LinuxSingleFileOutput *> $null 2>&1

# Check if output path and version arguments are provided
if ($args.Count -ge 2) {
    $destinationPath = $args[0]
    $version = $args[1]

    # Determine the source directory based on the specified version
    switch ($version) {
        'windows' { $sourceDir = $WindowsOutput }
        'windows_singlefile' { $sourceDir = $WindowsSingleFileOutput }
        'linux' { $sourceDir = $LinuxOutput }
        'linux_singlefile' { $sourceDir = $LinuxSingleFileOutput }
        default {
            Write-Host "Invalid version specified. Please use one of: windows, windows_singlefile, linux, linux_singlefile" -ForegroundColor Red
            exit 1
        }
    }

    # Check if the destination directory exists; if not, create it
    if (-Not (Test-Path -Path $destinationPath)) {
        New-Item -ItemType Directory -Path $destinationPath -ErrorAction Stop
    }

    # Copy the specified version to the destination directory, showing only errors
    Copy-Item -Path "$sourceDir\*" -Destination $destinationPath -Recurse -Force -ErrorAction Stop
} else {
    Write-Host "Please provide both a destination path and a version to copy (e.g., windows, windows_singlefile, linux, linux_singlefile)." -ForegroundColor Yellow
    exit 1
}

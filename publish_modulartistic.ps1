# build modulartistic
$base_dir = $PSScriptRoot
Set-Location -Path $base_dir

dotnet restore

# Build and publish for Windows
dotnet publish "$base_dir\Modulartistic\Modulartistic.csproj" -c Release -r win-x64 -p:DebugSymbols=false -o "$base_dir\publish\windows"
dotnet publish "$base_dir\Modulartistic\Modulartistic.csproj" -c Release -r win-x64 -p:PublishSingleFile=true -p:DebugSymbols=false -o "$base_dir\publish\windows_singlefile"

# Build and publish for Linux
dotnet publish "$base_dir\Modulartistic\Modulartistic.csproj" -c Release -r linux-x64 -p:DebugSymbols=false -o "$base_dir\publish\linux"
dotnet publish "$base_dir\Modulartistic\Modulartistic.csproj" -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true -p:DebugSymbols=false -o "$base_dir\publish\linux_singlefile"

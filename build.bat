@echo off
setlocal

rem Get the directory of the batch file
set "base_dir=%~dp0"

rem Change to the base directory
cd /d "%base_dir%"

rem restore everything
dotnet restore

rem build for windows
dotnet publish Modulartistic\Modulartistic.csproj -c Release -r win-x64 -p:DebugSymbols=false -o ./publish/windows
dotnet publish Modulartistic\Modulartistic.csproj -c Release -r win-x64 -p:PublishSingleFile=true -p:DebugSymbols=false -o ./publish/windows_singlefile

rem build for linux
dotnet publish Modulartistic\Modulartistic.csproj -c Release -r linux-x64 -p:DebugSymbols=false -o ./publish/linux 
dotnet publish Modulartistic\Modulartistic.csproj -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true -p:DebugSymbols=false -o ./publish/linux_singlefile

rem Change to the base directory
cd /d "%base_dir%"

for /d %%d in (Modulartistic.AddOns.*) do (
  echo build in project in directory %%d
  cd "%%d"
  dotnet publish -c Release --self-contained -p:DebugSymbols=false -p:GenerateDepsFile=false -o "../publish/addons/%%d"
  cd /d "%base_dir%" 
)

pause

endlocal
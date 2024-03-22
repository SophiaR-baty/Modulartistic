@echo off

set "source_file=%~1"
set "destination_dir=%~2"

copy "%source_file%" "%destination_dir%\" > nul

exit /b 0

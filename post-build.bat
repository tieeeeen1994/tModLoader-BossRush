@echo off
setlocal

REM Define source directories
set "source1=ExampleBossRush"
set "source2=BossRushAPI"

REM Define destination directory
set "destination=.."

REM Copy directories and force overwrite
xcopy /E /I /Y "%source1%" "%destination%\%source1%"
xcopy /E /I /Y "%source2%" "%destination%\%source2%"

echo Copy completed.

endlocal
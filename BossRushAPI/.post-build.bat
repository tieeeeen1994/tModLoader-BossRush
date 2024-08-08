@echo off
setlocal

REM Check if the build configuration is passed as an argument
if "%1"=="" (
    echo No build configuration provided.
    exit /b 1
)

set "buildConfig=%1"

REM Output the build configuration for debugging
echo Build configuration: %buildConfig%

REM Define source and destination directories
set "source=BossRushAPI"
set "destination=..\.."

REM Check if the build configuration is Release
if /I "%buildConfig%"=="Release" (
    echo Copying directories...
    xcopy /E /I /Y "..\%source%" "%destination%\%source%"
) else (
    echo Deleting directories...
    if exist "%destination%\%source%" (
        rmdir /S /Q "%destination%\%source%"
    )
)

endlocal

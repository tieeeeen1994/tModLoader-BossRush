@echo off
setlocal

REM Check if the build configuration is passed as an argument
if "%1"=="" (
    exit /b 1
)

set "buildConfig=%1"

REM Define source and destination directories
set "source=ExampleBossRush"
set "destination=..\.."

REM Check if the build configuration is Release
if /I "%buildConfig%"=="Release" (
    xcopy /E /I /Y "..\%source%" "%destination%\%source%"
) else (
    if exist "%destination%\%source%" (
        rmdir /S /Q "%destination%\%source%"
    )
)

endlocal
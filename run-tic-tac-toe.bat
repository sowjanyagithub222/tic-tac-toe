@echo off
setlocal
title Tic Tac Toe - Build and Run

cd /d "%~dp0"

echo ==========================================
echo       TIC TAC TOE - BUILD AND RUN
echo ==========================================

REM API project path
set "API=TicTacToe.Api\TicTacToe.Api\TicTacToe.Api.csproj"

REM Angular project folder
set "UI=tic-tac-toe-ui"

if not exist "%API%" (
    echo ERROR: API project not found: %API%
    pause
    exit /b 1
)

if not exist "%UI%\package.json" (
    echo ERROR: Angular package.json not found in %UI%
    echo Update the UI folder path in this BAT file.
    pause
    exit /b 1
)

echo.
echo [1/5] Restoring .NET API...
dotnet restore "%API%"
if errorlevel 1 goto error

echo.
echo [2/5] Building .NET API...
dotnet build "%API%" --no-restore
if errorlevel 1 goto error

echo.
echo [3/5] Checking Angular dependencies...
if not exist "%UI%\node_modules" (
    cd /d "%~dp0%UI%"
    call npm install
    if errorlevel 1 goto error
)

echo.
echo [4/5] Building Angular...
cd /d "%~dp0%UI%"
call npm run build
if errorlevel 1 goto error

echo.
echo [5/5] Starting both applications...

REM Start .NET API using HTTPS profile on port 5088
cd /d "%~dp0"
start "TicTacToe API HTTPS" cmd /k "cd /d ""%~dp0TicTacToe.Api\TicTacToe.Api"" && dotnet run --launch-profile https"

REM Start Angular
start "TicTacToe Angular" cmd /k "cd /d ""%~dp0%UI%"" && npm start"

echo.
echo Build successful. Both applications are starting.
echo Angular: http://localhost:4200
echo API HTTPS: https://localhost:5088
echo Check the API window for startup errors.
exit /b 0

:error
echo.
echo BUILD FAILED. Please check the error above.
pause
exit /b 1
```
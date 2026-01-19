@echo off
REM ============================================
REM LayerTabs - Yak Package Builder (Windows)
REM ============================================

echo.
echo === LayerTabs Yak Package Builder ===
echo.

REM Check for Yak
where yak >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Yak not found. Install from:
    echo         https://developer.rhino3d.com/guides/yak/
    echo.
    echo Or run in Rhino: _PackageManager then search "yak"
    pause
    exit /b 1
)

REM Set variables
set VERSION=1.0.0
set OUTPUT_DIR=dist

REM Create output directory
if not exist %OUTPUT_DIR% mkdir %OUTPUT_DIR%

REM Build Rhino 7
echo.
echo [1/4] Building for Rhino 7...
dotnet build src\LayerTabs.csproj -c Release -f net48
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Rhino 7 build failed
    pause
    exit /b 1
)

REM Create Rhino 7 package folder
set PKG7=%OUTPUT_DIR%\rh7
if exist %PKG7% rmdir /s /q %PKG7%
mkdir %PKG7%

copy bin\Release\7\LayerTabs.dll %PKG7%\
copy bin\Release\7\LayerTabs.rhp %PKG7%\ 2>nul
copy bin\Release\7\Newtonsoft.Json.dll %PKG7%\
copy manifest.yml %PKG7%\
copy resources\icon.png %PKG7%\ 2>nul

REM Build Rhino 7 yak package
echo [2/4] Creating Rhino 7 yak package...
cd %PKG7%
yak build --platform win
move *.yak ..\layertabs-%VERSION%-rh7-win.yak
cd ..\..

REM Build Rhino 8
echo.
echo [3/4] Building for Rhino 8...
dotnet build src\LayerTabs.csproj -c Release -f net7.0
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Rhino 8 build failed
    pause
    exit /b 1
)

REM Create Rhino 8 package folder
set PKG8=%OUTPUT_DIR%\rh8
if exist %PKG8% rmdir /s /q %PKG8%
mkdir %PKG8%

copy bin\Release\8\LayerTabs.dll %PKG8%\
copy bin\Release\8\LayerTabs.rhp %PKG8%\ 2>nul
copy bin\Release\8\Newtonsoft.Json.dll %PKG8%\
copy manifest.yml %PKG8%\
copy resources\icon.png %PKG8%\ 2>nul

REM Build Rhino 8 yak packages
echo [4/4] Creating Rhino 8 yak packages...
cd %PKG8%
yak build --platform win
move *.yak ..\layertabs-%VERSION%-rh8-win.yak
yak build --platform mac
move *.yak ..\layertabs-%VERSION%-rh8-mac.yak
cd ..\..

REM Cleanup
rmdir /s /q %OUTPUT_DIR%\rh7
rmdir /s /q %OUTPUT_DIR%\rh8

echo.
echo === Build Complete! ===
echo.
echo Output files in %OUTPUT_DIR%\:
dir /b %OUTPUT_DIR%\*.yak
echo.
echo To install: Double-click the .yak file
echo To publish: yak push dist\layertabs-%VERSION%-rh8-win.yak
echo.
pause

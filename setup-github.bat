@echo off
REM ============================================
REM LayerTabs - GitHub Setup (Windows)
REM ============================================

echo.
echo ========================================
echo    LayerTabs GitHub Setup
echo ========================================
echo.

REM Check git
where git >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Git is not installed
    echo Download from: https://git-scm.com/download/win
    pause
    exit /b 1
)

REM Get GitHub username
set /p GITHUB_USER="Enter your GitHub username: "
if "%GITHUB_USER%"=="" (
    echo [ERROR] Username is empty
    pause
    exit /b 1
)

echo.
echo Repository: https://github.com/%GITHUB_USER%/LayerTabs
echo.
echo IMPORTANT: First create an empty repository on GitHub:
echo   1. Go to https://github.com/new
echo   2. Repository name: LayerTabs
echo   3. Do NOT add README (keep it empty)
echo   4. Click "Create repository"
echo.
set /p CONFIRM="Press Enter when ready..."

echo.
echo [1/4] Initializing git...
if exist .git (
    echo      Already initialized - skip
) else (
    git init
)

echo [2/4] Adding files...
git add .

echo [3/4] Creating commit...
git commit -m "Initial commit - LayerTabs v1.0.0"

echo [4/4] Pushing to GitHub...
git remote remove origin 2>nul
git remote add origin https://github.com/%GITHUB_USER%/LayerTabs.git
git branch -M main
git push -u origin main

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Push failed
    echo Check your GitHub authentication
    pause
    exit /b 1
)

echo.
echo ========================================
echo    DONE!
echo ========================================
echo.
echo Repository: https://github.com/%GITHUB_USER%/LayerTabs
echo Actions:    https://github.com/%GITHUB_USER%/LayerTabs/actions
echo.
echo GitHub Actions will start building automatically (2-3 min)
echo After completion, download from Actions - Artifacts
echo.
pause

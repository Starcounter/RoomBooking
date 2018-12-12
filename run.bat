@echo off

setlocal EnableDelayedExpansion

if "%Configuration%"=="" set Configuration=Debug

:: Determine the app name.
:: The root directory is assumed to be named <App>.Mapper.
set root=%~dp0
set root=%root:~0,-1%
for %%d in (%root%) do set AppName=%%~nd

pushd %~dp0
dotnet cake --targets=Run%AppName%I --configuration="%Configuration%"
popd

endlocal

exit /b %errorlevel%

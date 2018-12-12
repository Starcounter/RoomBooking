@echo off

setlocal EnableDelayedExpansion

if "%Configuration%"=="" set Configuration=Debug

pushd %~dp0
dotnet cake --targets=Build --configuration="%Configuration%"
popd

endlocal

exit /b %errorlevel%

@echo off

setlocal EnableDelayedExpansion

if "%vsbase%"=="" (
    :: Ensure we're using the latest MSBuild.
    for /f "usebackq tokens=*" %%d in (`"C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe" -property installationPath -format value`) do set vsbase=%%d
)
call "%vsbase%\Common7\Tools\VsDevCmd.bat" >NUL

if "%Configuration%"=="" set Configuration=Debug

pushd %~dp0
dotnet cake --targets=Build --configuration="%Configuration%"
popd

endlocal

exit /b %errorlevel%

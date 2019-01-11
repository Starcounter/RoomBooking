@echo off

setlocal EnableDelayedExpansion

if "%vsbase%"=="" (
    :: Ensure we're using the latest MSBuild.
    for /f "usebackq tokens=*" %%d in (`"C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe" -property installationPath -format value`) do set vsbase=%%d
)
call "%vsbase%\Common7\Tools\VsDevCmd.bat" >NUL

if "%Configuration%"=="" set Configuration=Debug

:: Determine the app name.
:: The root directory is assumed to be the App name
set root=%~dp0
set root=%root:~0,-1%
for %%d in (%root%) do set AppName=%%~nxd

pushd %~dp0
dotnet cake --targets=Pack%AppName%I --configuration="%Configuration%" --starpackArtifactsPath="%StarpackOutputDir%"
popd

endlocal

exit /b %errorlevel%

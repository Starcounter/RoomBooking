@echo off

if "%Configuration%"=="" SET Configuration=Debug

if "%StarpackOutputDir%"=="" SET StarpackOutputDir=%~dp0src\RoomBooking

pushd %~dp0src\RoomBooking
starpack.exe --pack RoomBooking.csproj --config=%Configuration% --output=%StarpackOutputDir% || goto end

:end
popd
exit /b %errorlevel%
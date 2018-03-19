@ECHO OFF

IF "%CONFIGURATION%"=="" SET CONFIGURATION=Debug

star %* --resourcedir="%~dp0src\RoomBooking\wwwroot" "%~dp0src/RoomBooking/bin/%CONFIGURATION%/RoomBooking.exe"
@echo off

for /f "usebackq tokens=*" %%i in (`vswhere -property installationPath`) do (
  set InstallDir=%%i
)

if exist "%InstallDir%\Common7\Tools\vsdevcmd.bat" (
  CALL "%InstallDir%\Common7\Tools\vsdevcmd.bat" %*
  cd luajit/src
  CALL msvcbuild.bat
)

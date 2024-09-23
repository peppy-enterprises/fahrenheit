:: Fahrenheit `extern` build, Stage 1 : LuaJIT build
:: 24/9/24 00:28 fkelava

@echo off

:: https://github.com/microsoft/vswhere/wiki/Find-VC#batch
:: Obtain the Visual Studio installation path for the machine.
for /f "usebackq tokens=*" %%i in (`vswhere -property installationPath`) do (
  set InstallDir=%%i
)

:: https://github.com/microsoft/vswhere/wiki/Find-VC#batch
:: Convert the shell into a Visual Studio Developer Command Prompt.
if exist "%InstallDir%\Common7\Tools\vsdevcmd.bat" (
  CALL "%InstallDir%\Common7\Tools\vsdevcmd.bat" %*
  :: https://luajit.org/install.html
  :: > Open a "Visual Studio Command Prompt" (x86, x64 or ARM64), 
  :: > cd to the directory with the source code and run these commands:
  :: >     cd src
  :: >     msvcbuild
  cd luajit/src
  CALL msvcbuild.bat
)

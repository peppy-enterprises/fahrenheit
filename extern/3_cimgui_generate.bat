:: Fahrenheit `extern` build, Stage 3 : cimgui "generator"
:: 24/9/24 00:28 fkelava

:: Slightly modified from https://github.com/cimgui/cimgui/blob/docking_inter/generator/generator.bat
:: according to https://github.com/cimgui/cimgui/issues/267
:: and https://github.com/cimgui/cimgui/issues/232#issuecomment-1497059497

@echo off

:: LuaJIT must be on the path set up in 2_luajit_copy.bat
set PATH=%PATH%;%~dp0\luajit-bin;

:: https://github.com/microsoft/vswhere/wiki/Find-VC#batch
:: Obtain the Visual Studio installation path for the machine.
for /f "usebackq tokens=*" %%i in (`vswhere -property installationPath`) do (
  set InstallDir=%%i
)

:: https://github.com/microsoft/vswhere/wiki/Find-VC#batch
:: Convert the shell into a Visual Studio Developer Command Prompt.
if exist "%InstallDir%\Common7\Tools\vsdevcmd.bat" (
  CALL "%InstallDir%\Common7\Tools\vsdevcmd.bat" %*
  cd cimgui/generator
  luajit ./generator.lua cl "internal noimstrv" dx11 win32 %*
  cmd /k
)
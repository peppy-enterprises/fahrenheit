:: Fahrenheit `extern` build, Stage 4(a) : cimgui build, Debug
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
  :: See CMakeLists.txt, CMakePresets.txt.
  cmake --preset "x86-debug"
  cmake --build --preset "x86-debug"
)
:: Fahrenheit `extern` build, Stage 3 : cimgui "generator"
:: 24/9/24 00:28 fkelava

:: Slightly modified from https://github.com/cimgui/cimgui/blob/docking_inter/generator/generator.bat
:: according to https://github.com/cimgui/cimgui/issues/267
:: and https://github.com/cimgui/cimgui/issues/232#issuecomment-1497059497

:: LuaJIT must be on the path set up in 2_luajit_copy.bat
set PATH=%PATH%;%~dp0\luajit-bin;

cd cimgui/generator
luajit ./generator.lua cl "internal noimstrv" dx11 win32 %*
cmd /k

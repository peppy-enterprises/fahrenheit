:: Fahrenheit `extern` build, Stage 7 : ImGui.NET code generator run & copy
:: 24/9/24 00:28 fkelava

@echo off
@set NETTFM=net7.0

@set SRCDIR=..\artifacts\build\CodeGenerator\Debug\%NETTFM%
@set DESTDIR=%~dp0fh-imgui-net\src\ImGui.NET\Generated

:: Clean any existing generated output, then remake the generated directory.
rd /s /q %DESTDIR%
mkdir %DESTDIR%
:: Then inform the code generator to place its output in the destination directory.
cd %SRCDIR%
dotnet CodeGenerator.dll %DESTDIR%
 

:: Fahrenheit `extern` build, Stage 5 : cimgui copy
:: 24/9/24 00:28 fkelava

:: Prepares the JSON files generated in Step 3 and DLL generated in Step 4a/4b for C# binding generation.

robocopy cimgui-bin ImGui.NET\deps\cimgui\win-x86 cimgui.dll
robocopy cimgui/generator/output ImGui.NET\src\CodeGenerator\definitions\cimgui definitions.json
robocopy cimgui/generator/output ImGui.NET\src\CodeGenerator\definitions\cimgui impl_definitions.json
robocopy cimgui/generator/output ImGui.NET\src\CodeGenerator\definitions\cimgui structs_and_enums.json

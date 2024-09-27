:: Fahrenheit `extern` build, Stage 6 : ImGui.NET code generator build
:: 24/9/24 00:28 fkelava

@echo off 

cd fh-imgui-net\src\CodeGenerator
:: There is no point in building this in Release. Performance is irrelevant
:: but the stack traces are way too useful to forgo.
dotnet build CodeGenerator.csproj -c Debug
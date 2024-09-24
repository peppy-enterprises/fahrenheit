# Fahrenheit external dependencies

Fahrenheit relies on the [ImGui.NET](https://github.com/ImGuiNET/ImGui.NET) project to obtain C# ImGui bindings, 
which is itself only a thin wrapper over [cimgui](https://github.com/cimgui/cimgui).

However, the default `cimgui` that `ImGui.NET` provides is unsuitable for use in Fahrenheit because it does not
wrap any of the bindings/backends that ImGui provides- we need DX11 and Win32 for use with Final Fantasy X/X-2 HD.
`ImGui.NET` itself also needs modifications to fit the Fahrenheit build system and accept binding/backend input.

Therefore, Fahrenheit compiles its own version of `cimgui` and `ImGui.NET`. The batch files in this directory
assist you with the process of preparing these dependencies. They are designed to be run in the specified order, 
except 4a/4b, which are mutually exclusive choices.

In order:
- `1_luajit_build.bat`: builds [LuaJIT](https://luajit.org/), a direct dependency of `cimgui`.
- `2_luajit_copy.bat`: installs the LuaJIT binaries in a folder `luajit-bin` under this folder.
- `3_cimgui_generate.bat`: runs the `cimgui` "generator", which produces C source for `cimgui` and JSON files for `ImGui.NET`.
- `4a_cimgui_build_dbg.bat`: builds a modified `cimgui` in Debug mode.
- `4b_cimgui_build_rel.bat`: builds a modified `cimgui` in Release mode.
- `5_cimgui_copy.bat`: places the JSONs generated in Step 3 and `cimgui` DLL generated in steps 4a/4b in their proper place.
- `6_codegen_build.bat`: builds the `ImGui.NET` code-generator, required to customize our `ImGui.NET`.
- `7_codegen_copy.bat`: performs `ImGui.NET` code generation and copies the generated code to its proper place.

At this point you may fix up the generated code in `ImGui.NET`, build it, and use it as a normal dependency. Happy hunting!

_Note_: The `cimgui` and `luajit` submodules can be safely kept up to date with upstream- `ImGui.NET` may not.

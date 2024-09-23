:: set your PATH if necessary for LuaJIT or Lua5.1 or luajit with: (for example)
set PATH=%PATH%;%~dp0\luajit-bin;

:: https://github.com/cimgui/cimgui/issues/232#issuecomment-1497059497
cd cimgui/generator

::process  files
:: arg[1] compiler name gcc, clang or cl
:: arg[2] options as words in one string: internal for imgui_internal generation, freetype for freetype generation, comments for comments generation, nochar to skip char* function version, noimstrv to skip imstrv
:: examples: "" "internal" "internal freetype comments"
:: arg[3..n] name of implementations to generate and/or CFLAGS (e.g. -DIMGUI_USER_CONFIG or -DIMGUI_USE_WCHAR32)
luajit ./generator.lua cl "internal noimstrv" dx11 win32 %*

::leave console open
cmd /k

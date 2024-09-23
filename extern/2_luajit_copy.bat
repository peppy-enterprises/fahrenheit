:: Fahrenheit `extern` build, Stage 2 : LuaJIT install
:: 24/9/24 00:28 fkelava

:: https://luajit.org/install.html:
:: > Copy luajit.exe and lua51.dll (built in the src directory) 
::   to a newly created directory (any location is ok). 
:: > Add lua and lua\jit directories below it and copy all Lua files
::   from the src\jit directory of the distribution to the latter directory.
mkdir "luajit-bin/lua/jit"

robocopy luajit\src luajit-bin luajit.exe
robocopy luajit\src luajit-bin lua51.dll
robocopy luajit\src\jit luajit-bin\lua\jit\ /e

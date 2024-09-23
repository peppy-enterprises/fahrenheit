mkdir "luajit-bin/lua/jit"

robocopy luajit\src luajit-bin luajit.exe
robocopy luajit\src luajit-bin lua51.dll
robocopy luajit\src\jit luajit-bin\lua\jit\ /e

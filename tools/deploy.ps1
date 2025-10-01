# [fkelava 28/6/25 15:27]
# Adds tools to 'deploy'.
# If you simply want to use tools or run them from VS, this script is not required.

msbuild ..\Fahrenheit.slnx /t:Fahrenheit_Tools_ModManager:Publish /p:Configuration=Release
msbuild ..\Fahrenheit.slnx /t:Fahrenheit_Tools_Atelier:Publish /p:Configuration=Release
msbuild ..\Fahrenheit.slnx /t:Fahrenheit_Tools_DEdit:Publish /p:Configuration=Release
msbuild ..\Fahrenheit.slnx /t:Fahrenheit_Tools_Inspector:Publish /p:Configuration=Release
msbuild ..\Fahrenheit.slnx /t:Fahrenheit_Tools_STEP:Publish /p:Configuration=Release

# [fkelava 28/6/25 15:27]
# Adds tools to 'localdeploy', if you need them there.
# You must provide $SolutionDir yourself. A TRAILING SLASH IS REQUIRED.
# If you simply want to use tools or run them from VS, this script is not required.

$SolutionDir = "ABSOLUTE_PATH_OF_FOLDER_YOU_CLONED_FAHRENHEIT_TO_WITH_TRAILING_SLASH"
dotnet.exe publish ".\step" --no-build -c "Release" -p:SolutionDir=$($SolutionDir) /p:PublishProfileFullPath="$($SolutionDir)tools\deploy.pubxml"
dotnet.exe publish ".\h2cs" --no-build -c "Release" -p:SolutionDir=$($SolutionDir) /p:PublishProfileFullPath="$($SolutionDir)tools\deploy.pubxml"
dotnet.exe publish ".\modmgr" --no-build -c "Release" -p:SolutionDir=$($SolutionDir) /p:PublishProfileFullPath="$($SolutionDir)tools\deploy.pubxml"
dotnet.exe publish ".\dedit" --no-build -c "Release" -p:SolutionDir=$($SolutionDir) /p:PublishProfileFullPath="$($SolutionDir)tools\deploy.pubxml"

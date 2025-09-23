# Substantially copied from TerraFX.Interop.Windows.
# Original copyright © Tanner Gooding and Contributors - see THIRD-PARTY-NOTICES in repository root!

# ---
# THIS SCRIPT REQUIRES POWERSHELL 7 TO EXECUTE
# ---

function Generate() {
  $generationDir = Join-Path -Path $RepoRoot -ChildPath "generation"
  $generateRspFiles = Get-ChildItem -Path "$generationDir" -Recurse -Filter "generate.rsp"

  $generateRspFiles | ForEach-Object -Parallel {
    Push-Location -Path $_.DirectoryName
    & ClangSharpPInvokeGenerator "@generate.rsp"
    Pop-Location
  }
}

try {
  $RepoRoot = Join-Path -Path $PSScriptRoot -ChildPath ".."
  Generate
}
catch {
  Write-Host -Object $_
  Write-Host -Object $_.Exception
  Write-Host -Object $_.ScriptStackTrace
  exit 1
}
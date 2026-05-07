# Substantially copied from TerraFX.Interop.Windows.
# Original copyright © Tanner Gooding and Contributors - see THIRD-PARTY-NOTICES in repository root!

# ---
# THIS SCRIPT REQUIRES POWERSHELL 7 TO EXECUTE
# ---

function Generate() {
  $generateRspFiles = Get-ChildItem -Path $PSScriptRoot -Recurse -Filter "sgen_f*.rsp"

  $generateRspFiles | ForEach-Object {
    Push-Location -Path $_.DirectoryName
    & ClangSharpPInvokeGenerator "@$($_.Name)"
    Pop-Location
  }
}

try {
  Generate
}
catch {
  Write-Host -Object $_
  Write-Host -Object $_.Exception
  Write-Host -Object $_.ScriptStackTrace
  exit 1
}
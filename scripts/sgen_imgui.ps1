<# [fkelava 06/03/26 13:33]
    This script is used to run Dear Bindings over the version
    of ImGui pinned in `external/imgui`, then copy over all relevant
    source to `src/imgui`, where a project stands ready to build a static library.
    
    REQUIRES Python 3.10+ or later and 'ply' package!
    
    Parts of this script were adapted from TerraFX.Interop.Windows,
    © Tanner Gooding and Contributors.
    
    See THIRD-PARTY-NOTICES.
#>

Set-StrictMode -Version 2.0
$ErrorActionPreference = "Stop"

function Create-Directory([string[]] $Path) {
    if (!(Test-Path -Path $Path)) {
        New-Item -Path $Path -Force -ItemType "Directory" | Out-Null
    }
}

try {
    <# [fkelava 06/03/26 14:09]
        https://github.com/LuaJIT/LuaJIT/blob/659a61693aa3b87661864ad0f12eee14c865cd7f/src/msvcbuild.bat#L15
    #>
    if (-not $env:INCLUDE) {
        Write-Host -Object "This script must be run from a Visual Studio Developer PowerShell."
        exit 0
    }
   
    # Find the Dear Bindings folder
    $Root              = Join-Path -Path $PSScriptRoot -ChildPath ".."
    $RootImGui         = Join-Path -Path $Root         -ChildPath "external" "imgui"
    $RootImGuiBackends = Join-Path -Path $Root         -ChildPath "external" "imgui" "backends"
    $RootDearBindings  = Join-Path -Path $Root         -ChildPath "external" "dear_bindings"
    
    $RootFhImGui         = Join-Path -Path $Root -ChildPath "src" "imgui"
    $RootFhImGuiSource   = Join-Path -Path $Root -ChildPath "src" "imgui" "src"
    $RootFhImGuiIncludes = Join-Path -Path $Root -ChildPath "src" "imgui" "inc"
    
    $ImGuiH       = Join-Path -Path $RootImGui         -ChildPath "imgui.h"
    $ImConfigH    = Join-Path -Path $RootImGui         -ChildPath "imconfig.h"
    $InternalH    = Join-Path -Path $RootImGui         -ChildPath "imgui_internal.h"
    $ImplDX11H    = Join-Path -Path $RootImGuiBackends -ChildPath "imgui_impl_dx11.h"
    $ImplDX11Cpp  = Join-Path -Path $RootImGuiBackends -ChildPath "imgui_impl_dx11.cpp"
    $ImplWin32H   = Join-Path -Path $RootImGuiBackends -ChildPath "imgui_impl_win32.h"
    $ImplWin32Cpp = Join-Path -Path $RootImGuiBackends -ChildPath "imgui_impl_win32.cpp"
    
    $AllBindingCpp  = Join-Path -Path $RootFhImGui -ChildPath "*.cpp"
    $AllBindingH    = Join-Path -Path $RootFhImGui -ChildPath "*.h"
    $AllBindingJson = Join-Path -Path $RootFhImGui -ChildPath "*.json"
    
    $AllImGuiCpp  = Join-Path -Path $RootImGui -ChildPath "*.cpp"
    $AllImGuiH    = Join-Path -Path $RootImGui -ChildPath "*.h"

    Get-ChildItem -Path $AllImGuiCpp | Copy-Item -Force -Destination $RootFhImGuiSource
    Get-ChildItem -Path $AllImGuiH   | Copy-Item -Force -Destination $RootFhImGuiIncludes
    
    Get-ChildItem -Path $ImplDX11Cpp  | Copy-Item -Force -Destination $RootFhImGuiSource
    Get-ChildItem -Path $ImplWin32Cpp | Copy-Item -Force -Destination $RootFhImGuiSource
    Get-ChildItem -Path $ImplDX11H    | Copy-Item -Force -Destination $RootFhImGuiIncludes
    Get-ChildItem -Path $ImplWin32H   | Copy-Item -Force -Destination $RootFhImGuiIncludes
    
    Push-Location $RootDearBindings
    
    python dear_bindings.py `
    -o (Join-Path $RootFhImGui "dcimgui") `
    $ImGuiH
    
    python dear_bindings.py `
    -o (Join-Path $RootFhImGui "dcimgui_internal") `
    --include $ImGuiH $InternalH
    
    python dear_bindings.py `
    -o (Join-Path $RootFhImGui "dcimgui_impl_dx11") `
    --backend `
    --include $ImGuiH `
    --imconfig-path $ImConfigH `
    $ImplDX11H
    
    python dear_bindings.py `
    -o (Join-Path $RootFhImGui "dcimgui_impl_win32") `
    --backend `
    --include $ImGuiH `
    --imconfig-path $ImConfigH `
    $ImplWin32H
    
    Get-ChildItem -Path $AllBindingCpp  -Recurse | Move-Item -Force -Destination $RootFhImGuiSource
    Get-ChildItem -Path $AllBindingH    -Recurse | Move-Item -Force -Destination $RootFhImGuiIncludes
    Get-ChildItem -Path $AllBindingJson -Recurse | Remove-Item

    Pop-Location
    
    ClangSharpPInvokeGenerator "@sgen_imgui.rsp"
    
    exit 0
}
catch {
    Write-Host -Object $_
    Write-Host -Object $_.Exception
    Write-Host -Object $_.ScriptStackTrace
    
    Pop-Location
    
    exit 1
}

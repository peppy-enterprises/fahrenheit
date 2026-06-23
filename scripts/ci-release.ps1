<# [fkelava 06/03/26 13:33]
    This script is used to generate release assets for Fahrenheit.
    
    We used to perform this fully on CI, but the code-signing solution
    in use sadly does not have the ability to integrate with it.
    
    Thus we still perform build checks on CI, but releases are created
    and signed locally using this script.
    
PREREQUISITES:
    - Git
    - GitHub CLI
    - Visual Studio 2026, or equivalent-version Build Tools with:
        - MSVC latest
        - .NET SDK latest
    - 7-Zip or equivalent
    
THIS SCRIPT MUST BE RUN AT A DEVELOPER POWERSHELL TERMINAL.
#>

<# [fkelava 06/03/26 13:47]
    Large parts of this script, as with many other build system details,
    were adapted from TerraFX.Interop.Windows, © Tanner Gooding and Contributors.
    
    See THIRD-PARTY-NOTICES.
#>

[CmdletBinding(PositionalBinding=$false)]
Param(
    [switch]                                                                         $help,
    [switch]                                                                         $dryRun,
    [Parameter(Mandatory)]                                                [string]   $certFingerprint,
    [Parameter(Mandatory)]                                                [string]   $nugetApiKey,
    [Parameter(Mandatory)]                                                [string]   $tag,
    [ValidateSet("debug", "release")]                                     [string]   $configuration = "release",
    [ValidateSet("quiet", "minimal", "normal", "detailed", "diagnostic")] [string]   $verbosity     = "normal",
    [Parameter(ValueFromRemainingArguments=$true)]                        [String[]] $properties
)

Set-StrictMode -Version 2.0
$ErrorActionPreference = "Stop"

function Help() {
    Write-Host -Object "Common settings:"
   
    Write-Host -Object "  -certFingerprint <value> SHA-1 thumbprint of certificate to sign binaries"
    Write-Host -Object "  -nugetApiKey <value>     NuGet API key for package publication"
    Write-Host -Object "  -tag <value>             Build tag/version ID"
    Write-Host -Object "  -configuration <value>   Build configuration (Debug, Release)"
    Write-Host -Object "  -verbosity <value>       MSBuild verbosity (q[uiet], m[inimal], n[ormal], d[etailed], and diag[nostic])"
    Write-Host -Object "  -help                    Print help and exit"
    Write-Host -Object ""
    Write-Host -Object "Command line arguments not listed above are passed through to MSBuild."
    Write-Host -Object "The above arguments can be shortened as much as to be unambiguous (e.g. -co for configuration)."
}

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
    
    if ($help) {
        Help
        exit 0
    }
   
    # Find the solution
    $Root         = Join-Path -Path $PSScriptRoot -ChildPath ".."
    $Solution     = Join-Path -Path $Root         -ChildPath "Fahrenheit.slnx"
    
    # Well known build directories
    $DirArtifactFlavor = $configuration -eq "debug" ? "dbg" : "rel"
    
    $DirArtifacts    = Join-Path -Path $Root         -ChildPath "artifacts"
    $DirLog          = Join-Path -Path $DirArtifacts -ChildPath "log"
    $DirPkg          = Join-Path -Path $DirArtifacts -ChildPath "pkg" $DirArtifactFlavor
    $DirDeploy       = Join-Path -Path $DirArtifacts -ChildPath "deploy" $DirArtifactFlavor
    $DirArtifactsBin = Join-Path -Path $DirDeploy    -ChildPath "bin"
    
    # Actual artifact and package locations
    $Artifacts    = Join-Path -Path $DirDeploy -ChildPath "**"
    $Packages     = Join-Path -Path $DirPkg    -ChildPath "*.nupkg"
    
    Create-Directory -Path $DirArtifacts
    Create-Directory -Path $DirLog
    
    $LogFileRestore = Join-Path -Path $DirLog -ChildPath "$($configuration)\restore.binlog"
    $LogFileBuild   = Join-Path -Path $DirLog -ChildPath "$($configuration)\build.binlog"
    
    # Build everything.
    msbuild /m /p:ContinuousIntegrationBuild=true /p:Configuration=$configuration /bl:$LogFileRestore /t:Restore $Solution
    msbuild /m /p:ContinuousIntegrationBuild=true /p:Configuration=$configuration /bl:$LogFileBuild              $Solution

    <# [fkelava 06/03/26 14:09]
        The NuGet packages are generated implicitly on successful build.
    #>
    
    # Sign all Fahrenheit artifacts.
    $ArtifactsToSign = Get-ChildItem -Path $DirArtifactsBin -Recurse -Include "fh*.dll", "fh*.exe"
    signtool sign /sha1 $certFingerprint /tr http://time.certum.pl /fd sha1 /td sha256 /v $ArtifactsToSign
    
    # ZIP up the signed release artifacts.
    7z a -tzip "fahrenheit_windows_x86_$($configuration)_$($tag).zip" $Artifacts
    
    if ($dryRun) {
        Write-Host "Dry run complete. Exiting."
        exit 0
    }
    
    # Attach the generated release ZIP to the appropriate GH tag.
    gh release upload $tag "fahrenheit_windows_x86_$($configuration)_$($tag).zip"
    
    # Push the NuGet packages.
    # They are currently not signed pending a support thread with Certum wrt. inability to sign SHA-256.
    dotnet nuget push $Packages --api-key $nugetApiKey --source https://api.nuget.org/v3/index.json
}
catch {
    Write-Host -Object $_
    Write-Host -Object $_.Exception
    Write-Host -Object $_.ScriptStackTrace
    exit 1
}

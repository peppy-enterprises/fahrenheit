// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core;

/// <summary>
///     The accessor for objects and helpers that compose the public Fahrenheit API.
/// </summary>
public static class FhApi {
    public static          FhModController       Mods          = new FhModController();
    public static readonly FhLocalizationManager Localization  = new FhLocalizationManager();
    public static readonly FhResourceLoader      Resources     = new FhResourceLoader();
    public static readonly FhImGuiHelper         ImGuiHelper   = new FhImGuiHelper();
    public static readonly FhInput               Input         = new FhInput();
}

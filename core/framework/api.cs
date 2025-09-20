// SPDX-License-Identifier: MIT

namespace Fahrenheit.Core;

/// <summary>
///     The accessor for objects and helpers that compose the public Fahrenheit API.
/// </summary>
public static class FhApi {
    public static          FhModController       ModController       = new FhModController();
    public static readonly FhLocalizationManager LocalizationManager = new FhLocalizationManager();
    public static readonly FhResourceLoader      ResourceLoader      = new FhResourceLoader();
    public static readonly FhImGuiHelper         ImGuiHelper         = new FhImGuiHelper();
    public static readonly FhInput               Input               = new FhInput();
}

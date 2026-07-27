// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

/* Jobs:
 * - FhGameLauncher: validate the Fahrenheit install (stage0/stage1/game exe
 *   all present) and launch FFX/FFX-2 through fhstage0.exe, optionally in
 *   debug mode.
 * - FhShell: open an arbitrary folder in the OS's file browser (Explorer on
 *   Windows, xdg-open elsewhere).
 */

namespace Fahrenheit.Tools.ModManager;

internal enum FhLaunchTarget {
    FFX,
    FFX2
}

internal readonly record struct FhLaunchResult(
    bool Success,
    string Message
);

internal static class FhGameLauncher {

    internal static FhLaunchResult launch(
        FhLaunchTarget target,
        string configured_game_directory,
        bool debug = false) {
        if (!OperatingSystem.IsWindows()) {
            return new(
                false,
                "Launching Final Fantasy X/X-2 is currently supported only on Windows.");
        }

        string game_directory;

        try {
            game_directory =
                FhModManagerSettingsStore.normalize_path(
                    configured_game_directory);
        }
        catch (Exception exception) {
            return new(
                false,
                "The configured game directory is invalid.\n\n"
                + exception.Message);
        }

        string game_executable = target switch {
            FhLaunchTarget.FFX  => "FFX.exe",
            FhLaunchTarget.FFX2 => "FFX-2.exe",
            _ => throw new ArgumentOutOfRangeException(nameof(target))
        };

        string fahrenheit_bin = Path.Join(
        game_directory,
        "fahrenheit",
        "bin");

        string stage0_path = Path.Join(
        fahrenheit_bin,
        "fhstage0.exe");

        string stage1_path = Path.Join(
        fahrenheit_bin,
        "fhstage1.dll");

        string game_path = Path.Join(
        game_directory,
        game_executable);

        List<string> missing_files = [];

        foreach (string required_path in new[] { stage0_path, stage1_path, game_path }) {
            if (!File.Exists(required_path)) {
                missing_files.Add(required_path);
            }
        }

        if (missing_files.Count > 0) {
            return new(
                false,
                "The Fahrenheit installation is incomplete.\n\n"
                + "Missing files:\n"
                + string.Join('\n', missing_files));
        }

        /*
         * Mirrors the manual install steps from the project README: run fhstage0.exe
         * with its own directory (fahrenheit/bin) as the working directory, and point
         * it at the game executable with a path relative to that directory - e.g.
         * `..\..\FFX.exe`. Passing an absolute path here would also work, but this
         * keeps the launcher's invocation identical to what a developer would type
         * by hand, which is what stage0/stage1 are tested against.
         */
        string relative_game_path = Path.GetRelativePath(
        fahrenheit_bin,
        game_path);

        ProcessStartInfo start_info = new() {
            FileName         = stage0_path,
            WorkingDirectory = fahrenheit_bin,
            UseShellExecute  = true
        };

        start_info.ArgumentList.Add(relative_game_path);

        // fhstage0.exe checks its own argument list for "--debug" (src/stage0/src/main.cpp)
        // and, if present, creates the game process suspended and waits for a keypress
        // before injecting Stage 1 - giving you a window to attach a debugger first.
        if (debug) {
            start_info.ArgumentList.Add("--debug");
        }

        try {
            Process? process = Process.Start(start_info);

            if (process == null) {
                return new(
                    false,
                    "Windows did not create the Fahrenheit Stage 0 process.");
            }

            return new(
                true,
                debug
                    ? $"Started {game_executable} through Fahrenheit in debug mode. "
                        + "Attach a debugger, then press any key in the Stage 0 console window."
                    : $"Started {game_executable} through Fahrenheit.");
        }
        catch (Exception exception) {
            return new(
                false,
                $"Fahrenheit could not start {game_executable}.\n\n"
                + exception.Message);
        }
    }
}

internal static class FhShell {
    // Opens `path` in the OS's file browser (Explorer on Windows, whatever
    // xdg-open resolves to on Linux - this project targets both RIDs).
    internal static bool try_open_folder(string path, out string error) {
        error = "";

        if (!Directory.Exists(path)) {
            error = $"The folder does not exist:\n{path}";
            return false;
        }

        try {
            ProcessStartInfo start_info = OperatingSystem.IsWindows()
                ? new ProcessStartInfo { FileName = path, UseShellExecute = true }
                : new ProcessStartInfo { FileName = "xdg-open" };

            if (!OperatingSystem.IsWindows()) {
                start_info.ArgumentList.Add(path);
            }

            Process.Start(start_info);

            return true;
        }
        catch (Exception exception) {
            error =
                "The folder could not be opened.\n\n"
                + exception.Message;

            return false;
        }
    }
}

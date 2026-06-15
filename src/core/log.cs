// SPDX-License-Identifier: LGPL-3.0-or-later
//
// This file is part of Fahrenheit, © 2023-2026 The Fahrenheit contributors.
// It is licensed to you under the GNU Lesser General Public License, version 3.0 or later. See COPYING, COPYING.LESSER.

namespace Fahrenheit;

public enum FhLogLevel {
    Trace   = 0,
    Debug   = 1,
    Info    = 2,
    Warning = 3,
    Error   = 4,
    Fatal   = 5,
    None    = 6
}

/// <summary>
///     Provides access to a log file. All entries are additionally echoed to the console.
/// </summary>
public class FhLogger {
#if DEBUG
    private const FhLogLevel MinLevel = FhLogLevel.Debug;
#else
    private const FhLogLevel MinLevel = FhLogLevel.Info;
#endif

    private readonly TextWriterTraceListener _console;
    private readonly TextWriterTraceListener _file;

    public FhLogger(string log_file_name) {
        string log_path = Path.Join(FhEnvironment.Finder.Logs.FullName, log_file_name);

        _console = new TextWriterTraceListener(Console.Out);
        _file    = new TextWriterTraceListener(File.Open(log_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite));
    }

    /// <summary>
    ///     Logs a given <paramref name="message"/> unconditionally,
    ///     without timestamp or caller information.
    /// </summary>
    internal void Log(string message) {
        _console.WriteLine(message);
        _file   .WriteLine(message);
        _file   .Flush();
    }

    /// <summary>
    ///     Logs a given <paramref name="message"/> at a given <paramref name="level"/>,
    ///     prepending a timestamp and caller information.
    /// </summary>
    public void Log(                   FhLogLevel level,
                                       string     message,
                    [CallerMemberName] string     member = "",
                    [CallerFilePath]   string     file   = "",
                    [CallerLineNumber] int        line   = 0) {
        if (level < MinLevel) return;
        string lstr = $"{TimeProvider.System.GetUtcNow():u} | [{level}] {Path.GetFileName(file)}:{line} ({member}): {message}";

        _console.WriteLine(lstr);
        _file   .WriteLine(lstr);
        _file   .Flush();
    }

    public void Debug(                   string message,
                      [CallerMemberName] string member = "",
                      [CallerFilePath]   string file   = "",
                      [CallerLineNumber] int    line   = 0) {
        Log(FhLogLevel.Debug, message, member, file, line);
    }

    public void Info(                   string message,
                     [CallerMemberName] string member = "",
                     [CallerFilePath]   string file   = "",
                     [CallerLineNumber] int    line   = 0) {
        Log(FhLogLevel.Info, message, member, file, line);
    }

    public void Warning(                   string message,
                        [CallerMemberName] string member = "",
                        [CallerFilePath]   string file   = "",
                        [CallerLineNumber] int    line   = 0) {
        Log(FhLogLevel.Warning, message, member, file, line);
    }

    public void Error(                   string message,
                      [CallerMemberName] string member = "",
                      [CallerFilePath]   string file   = "",
                      [CallerLineNumber] int    line   = 0) {
        Log(FhLogLevel.Error, message, member, file, line);
    }

    public void Fatal(                   string message,
                      [CallerMemberName] string member = "",
                      [CallerFilePath]   string file   = "",
                      [CallerLineNumber] int    line   = 0) {
        Log(FhLogLevel.Fatal, message, member, file, line);
    }
}

using System.Diagnostics;

namespace Fahrenheit.Core;

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
    private const LogLevel MinLevel = LogLevel.Debug;
#else
    private const FhLogLevel MinLevel = FhLogLevel.Info;
#endif

    private readonly TextWriterTraceListener _console;
    private readonly TextWriterTraceListener _file;

    public FhLogger(string log_file_name) {
        string log_path = Path.Join(FhInternal.PathFinder.Logs.Path, log_file_name);

        _console = new TextWriterTraceListener(Console.Out);
        _file    = new TextWriterTraceListener(File.Open(log_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite));
    }

    public void Log(                   FhLogLevel level,
                                       string   msg,
                    [CallerMemberName] string   mname = "",
                    [CallerFilePath]   string   fpath = "",
                    [CallerLineNumber] int      lnb   = 0) {
        if (level < MinLevel) return;
        string lstr = $"{DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)} | [{level}] {Path.GetFileName(fpath)}:{lnb.ToString()} ({mname}): {msg}";

        _console.WriteLine(lstr);
        _file   .WriteLine(lstr);
        _file   .Flush();
    }

    public void Debug(                   string msg,
                      [CallerMemberName] string mname = "",
                      [CallerFilePath]   string fpath = "",
                      [CallerLineNumber] int    lnb   = 0) {
        Log(FhLogLevel.Debug, msg, mname, fpath, lnb);
    }

    public void Info(                   string msg,
                     [CallerMemberName] string mname = "",
                     [CallerFilePath]   string fpath = "",
                     [CallerLineNumber] int    lnb   = 0) {
        Log(FhLogLevel.Info, msg, mname, fpath, lnb);
    }

    public void Warning(                   string msg,
                        [CallerMemberName] string mname = "",
                        [CallerFilePath]   string fpath = "",
                        [CallerLineNumber] int    lnb   = 0) {
        Log(FhLogLevel.Warning, msg, mname, fpath, lnb);
    }

    public void Error(                   string msg,
                      [CallerMemberName] string mname = "",
                      [CallerFilePath]   string fpath = "",
                      [CallerLineNumber] int    lnb   = 0) {
        Log(FhLogLevel.Error, msg, mname, fpath, lnb);
    }

    public void Fatal(                   string msg,
                      [CallerMemberName] string mname = "",
                      [CallerFilePath]   string fpath = "",
                      [CallerLineNumber] int    lnb   = 0) {
        Log(FhLogLevel.Fatal, msg, mname, fpath, lnb);
    }
}

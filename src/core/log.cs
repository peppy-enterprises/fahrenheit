using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Fahrenheit.Core;

public enum LogLevel {
    Trace   = 0,
    Debug   = 1,
    Info    = 2,
    Warning = 3,
    Error   = 4,
    Fatal   = 5,
    None    = 6
}

public static class FhLog {
#if DEBUG
    private const LogLevel MinLevel = LogLevel.Debug;
#else
    private const LogLevel MinLevel = LogLevel.Info;
#endif

    static FhLog() {
        Trace.AutoFlush = true;
        Trace.Listeners.Add(new ConsoleTraceListener());
        Trace.Listeners.Add(new TextWriterTraceListener(File.Open(Path.Join(FhRuntimeConst.Logs.LinkPath, $"{FhUtil.get_timestamp_string()}.log"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)));
    }

    public static void Log(                   LogLevel level,
                                              string   msg,
                           [CallerMemberName] string   mname = "",
                           [CallerFilePath]   string   fpath = "",
                           [CallerLineNumber] int      lnb   = 0) {
        if (level < MinLevel) return;

        Trace.WriteLine($"{DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)} | [{level}] {Path.GetFileName(fpath)}:{lnb.ToString()} ({mname}): {msg}");
    }

    public static void Debug(                   string msg,
                             [CallerMemberName] string mname = "",
                             [CallerFilePath]   string fpath = "",
                             [CallerLineNumber] int    lnb   = 0) {
        Log(LogLevel.Debug, msg, mname, fpath, lnb);
    }

    public static void Info(                   string msg,
                            [CallerMemberName] string mname = "",
                            [CallerFilePath]   string fpath = "",
                            [CallerLineNumber] int    lnb   = 0) {
        Log(LogLevel.Info, msg, mname, fpath, lnb);
    }

    public static void Warning(                   string msg,
                               [CallerMemberName] string mname = "",
                               [CallerFilePath]   string fpath = "",
                               [CallerLineNumber] int    lnb   = 0) {
        Log(LogLevel.Warning, msg, mname, fpath, lnb);
    }

    public static void Error(                   string msg,
                             [CallerMemberName] string mname = "",
                             [CallerFilePath]   string fpath = "",
                             [CallerLineNumber] int    lnb   = 0) {
        Log(LogLevel.Error, msg, mname, fpath, lnb);
    }

    public static void Fatal(                   string msg,
                             [CallerMemberName] string mname = "",
                             [CallerFilePath]   string fpath = "",
                             [CallerLineNumber] int    lnb   = 0) {
        Log(LogLevel.Fatal, msg, mname, fpath, lnb);
    }
}
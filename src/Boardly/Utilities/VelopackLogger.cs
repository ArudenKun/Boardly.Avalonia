using Serilog.Events;
using Velopack;
using Velopack.Logging;

namespace Boardly.Utilities;

public class VelopackLogger : IVelopackLogger
{
    public static VelopackLogger Instance { get; } = new();

    private VelopackLogger() { }

    public void Log(VelopackLogLevel logLevel, string? message, Exception? exception)
    {
        LogHelper.Log<VelopackApp>(
            MapToSerilogLogEventLevel(logLevel),
            exception,
            "{Message}",
            message
        );
    }

    private static LogEventLevel MapToSerilogLogEventLevel(VelopackLogLevel logLevel) =>
        logLevel switch
        {
            VelopackLogLevel.Trace => LogEventLevel.Verbose,
            VelopackLogLevel.Debug => LogEventLevel.Debug,
            VelopackLogLevel.Information => LogEventLevel.Information,
            VelopackLogLevel.Warning => LogEventLevel.Warning,
            VelopackLogLevel.Error => LogEventLevel.Error,
            VelopackLogLevel.Critical => LogEventLevel.Fatal,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null),
        };
}

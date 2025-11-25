using Boardly.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog.Events;

namespace Boardly.Options;

public sealed partial class LoggingOptions : ObservableObject
{
    [ObservableProperty]
    public partial LogEventLevel LogLevel { get; set; } =
        AppHelper.IsDebug ? LogEventLevel.Verbose : LogEventLevel.Information;

    [ObservableProperty]
    // ReSharper disable once InconsistentNaming
    public partial long SizeInMB { get; set; } = 0;

    [ObservableProperty]
    public partial long TimeLimitInDays { get; set; } = 30;
}

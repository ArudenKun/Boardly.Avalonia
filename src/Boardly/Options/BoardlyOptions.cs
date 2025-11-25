using Boardly.Utilities;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Boardly.Options;

[ObservableObject]
public sealed partial class BoardlyOptions : JsonFileBase
{
    public BoardlyOptions()
        : base(AppHelper.OptionsPath) { }

    [ObservableProperty]
    public partial LoggingOptions Logging { get; set; } = new();

    [ObservableProperty]
    // ReSharper disable once InconsistentNaming
    public partial UIOptions UI { get; set; } = new();
}

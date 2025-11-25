using Boardly.Extensions;
using Velopack.Locators;

namespace Boardly.Utilities;

public static class AppHelper
{
    public const string Name = nameof(Boardly);
    public const string BaseResourcePath = $"avares://{Name}";

    public static bool IsDebug
#if DEBUG
        => true;
#else
        => false;
#endif
    public static readonly string AppDir = AppDomain.CurrentDomain.BaseDirectory;

    public static readonly string RoamingDir = Environment.GetFolderPath(
        Environment.SpecialFolder.ApplicationData
    );

    public static readonly string DataDir =
        VelopackLocator.Current.IsPortable || File.Exists(AppDir.CombinePath("data")) || IsDebug
            ? AppDir.CombinePath("data")
            : RoamingDir.CombinePath(Name);

    public static readonly string LogsDir = DataDir.CombinePath("logs");

    public static readonly string OptionsPath = DataDir.CombinePath("options.json");
}

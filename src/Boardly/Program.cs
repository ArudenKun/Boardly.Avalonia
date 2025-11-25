using Avalonia;
using Boardly.Utilities;
using Velopack;

namespace Boardly;

public static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var builder = BuildAvaloniaApp();
        try
        {
            VelopackApp.Build().SetArgs(args).SetLogger(VelopackLogger.Instance).Run();
            builder.StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            LogHelper.Error<App>(e, "Unhandled Exception");
            throw;
        }
        finally
        {
            if (builder.Instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();
}

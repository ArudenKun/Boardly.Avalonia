using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Boardly.Utilities;
using Boardly.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ZLinq;

namespace Boardly;

public sealed class App : Application, IDisposable
{
    private static readonly Lazy<ServiceProvider> LazyServiceProvider = new(
        () =>
            new ServiceCollection()
                .AddViewModels()
                .AddServices()
                .AddScannedServices()
                .AddSerilog()
                .BuildServiceProvider(true),
        LazyThreadSafetyMode.ExecutionAndPublication
    );

    private static ServiceProvider ServiceProvider => LazyServiceProvider.Value;

    public static T? GetService<T>()
        where T : notnull => ServiceProvider.GetService<T>();

    public static object? GetService(Type type) => ServiceProvider.GetService(type);

    public static T GetRequiredService<T>()
        where T : notnull => ServiceProvider.GetRequiredService<T>();

    public static object GetRequiredService(Type type) => ServiceProvider.GetRequiredService(type);

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        LogHelper.Initialize();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit.
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow =
                DataTemplates[0].Build(GetRequiredService<MainWindowViewModel>()) as Window;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove = BindingPlugins
            .DataValidators.AsValueEnumerable()
            .OfType<DataAnnotationsValidationPlugin>()
            .ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    public void Dispose()
    {
        LogHelper.Cleanup();
        ServiceProvider.Dispose();
    }
}

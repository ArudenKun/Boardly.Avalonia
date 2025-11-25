using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Boardly.Dependency;
using Boardly.Options;
using Boardly.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using ServiceScan.SourceGenerator;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace Boardly;

public static partial class DependencyInjection
{
    [GenerateServiceRegistrations(
        AssignableTo = typeof(ViewModel),
        CustomHandler = nameof(AddViewModelsHandler)
    )]
    public static partial IServiceCollection AddViewModels(this IServiceCollection services);

    [GenerateServiceRegistrations(
        AssignableTo = typeof(ISingletonDependency),
        AsSelf = true,
        AsImplementedInterfaces = true,
        Lifetime = ServiceLifetime.Singleton
    )]
    [GenerateServiceRegistrations(
        AssignableTo = typeof(IScopedDependency),
        AsSelf = true,
        AsImplementedInterfaces = true,
        Lifetime = ServiceLifetime.Scoped
    )]
    [GenerateServiceRegistrations(
        AssignableTo = typeof(ITransientDependency),
        AsSelf = true,
        AsImplementedInterfaces = true,
        Lifetime = ServiceLifetime.Transient
    )]
    public static partial IServiceCollection AddScannedServices(this IServiceCollection services);

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton(sp => new LoggingLevelSwitch(
            sp.GetRequiredService<BoardlyOptions>().Logging.LogLevel
        ));
        services.AddSingleton<ISukiToastManager, SukiToastManager>();
        services.AddSingleton<ISukiDialogManager, SukiDialogManager>();
        return services;
    }

    public static IServiceCollection AddSerilog(this IServiceCollection services) =>
        services.AddLogging(builder => builder.ClearProviders().AddSerilog(dispose: true));

    private static void AddViewModelsHandler<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel
    >(this IServiceCollection services)
        where TViewModel : ViewModel
    {
        var viewModelType = typeof(TViewModel);
        var attribute =
            viewModelType.GetCustomAttribute<DependencyLifetimeAttribute>()
            ?? new DependencyLifetimeAttribute();
        var viewModelSd = ServiceDescriptor.Describe(
            viewModelType,
            viewModelType,
            attribute.ServiceLifetime
        );
        var viewModelBasesSd = EnumerateBaseTypes<ViewModel>(viewModelType)
            .Select(baseType =>
                ServiceDescriptor.Describe(
                    baseType,
                    sp => sp.GetRequiredService<TViewModel>(),
                    attribute.ServiceLifetime
                )
            )
            .ToArray();
        services.Add(viewModelSd);
        services.Add(viewModelBasesSd);
    }

    private static IEnumerable<Type> EnumerateBaseTypes<TRoot>(Type t)
    {
        ArgumentNullException.ThrowIfNull(t);

        var baseType = t.BaseType;
        while (baseType is not null && baseType != typeof(object))
        {
            yield return baseType;
            if (baseType == typeof(TRoot))
            {
                yield break;
            }

            baseType = baseType.BaseType;
        }
    }
}

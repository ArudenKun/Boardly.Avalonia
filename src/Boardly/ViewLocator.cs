using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Boardly.ViewModels;
using Boardly.Views;
using ServiceScan.SourceGenerator;

namespace Boardly;

public sealed partial class ViewLocator : IDataTemplate
{
    private static readonly Dictionary<Type, Func<Control>> ViewFactory = new();

    public ViewLocator()
    {
        AddViews(this);
    }

    public TView CreateView<TView>(ViewModel viewModel)
        where TView : Control => (TView)CreateView(viewModel);

    public Control CreateView(ViewModel viewModel)
    {
        var viewModelType = viewModel.GetType();

        if (!ViewFactory.TryGetValue(viewModelType, out var factory))
        {
            return CreateText($"Could not find view for {viewModelType.FullName}");
        }

        var view = factory();
        view.DataContext = viewModel;
        return view;
    }

    Control ITemplate<object?, Control?>.Build(object? data)
    {
        if (data is ViewModel viewModel)
        {
            return CreateView(viewModel);
        }

        return CreateText($"Could not find view for {data?.GetType().FullName}");
    }

    bool IDataTemplate.Match(object? data) => data is ViewModel;

    private static TextBlock CreateText(string text) => new() { Text = text };

    private void TryAdd(Type viewModelType, Func<Control> factory) =>
        ViewFactory.TryAdd(viewModelType, factory);

    [GenerateServiceRegistrations(
        AssignableTo = typeof(IView<>),
        CustomHandler = nameof(AddViewsHandler)
    )]
    private static partial void AddViews(ViewLocator viewLocator);

    private static void AddViewsHandler<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TView,
        TViewModel
    >(ViewLocator viewLocator)
        where TView : Control, IView<TViewModel>, new()
        where TViewModel : ViewModel => viewLocator.TryAdd(typeof(TViewModel), () => new TView());
}

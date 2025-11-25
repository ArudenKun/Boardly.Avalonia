using Avalonia.Interactivity;
using Boardly.ViewModels;
using SukiUI.Controls;

namespace Boardly.Views;

public abstract class SukiWindow<TViewModel> : SukiWindow, IView<TViewModel>
    where TViewModel : ViewModel
{
    public new TViewModel DataContext
    {
        get =>
            base.DataContext as TViewModel
            ?? throw new InvalidCastException(
                $"DataContext is null or not of the expected type '{typeof(TViewModel).FullName}'."
            );
        set => base.DataContext = value;
    }

    public TViewModel ViewModel => DataContext;

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ViewModel.OnLoaded();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        ViewModel.OnUnloaded();
    }
}

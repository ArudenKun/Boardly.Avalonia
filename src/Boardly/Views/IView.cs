using Boardly.ViewModels;

namespace Boardly.Views;

public interface IView<out TViewModel>
    where TViewModel : ViewModel
{
    TViewModel DataContext { get; }
    TViewModel ViewModel { get; }
}

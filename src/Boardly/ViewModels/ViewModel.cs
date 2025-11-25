using CommunityToolkit.Mvvm.ComponentModel;

namespace Boardly.ViewModels;

public abstract class ViewModel : ObservableRecipient, IDisposable
{
    public virtual void OnLoaded() { }

    public virtual void OnUnloaded() { }

    protected void OnAllPropertiesChanged() => OnPropertyChanged(string.Empty);

    #region Disposal

    ~ViewModel() => Dispose(false);

    private List<Action?>? _onDisposeActions;

    public void OnDispose(Action? callback = null)
    {
        _onDisposeActions ??= [];
        _onDisposeActions.Add(callback);
    }

    private bool _isDisposed;

    /// <inheritdoc cref="Dispose"/>>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        if (!disposing)
            return;

        if (_onDisposeActions is { Count: > 0 })
        {
            foreach (var disposeAction in _onDisposeActions.Distinct())
            {
                disposeAction?.Invoke();
            }
        }

        _isDisposed = true;
    }

    /// <inheritdoc />>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}

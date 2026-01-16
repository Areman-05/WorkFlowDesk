using CommunityToolkit.Mvvm.ComponentModel;

namespace WorkFlowDesk.ViewModel.Base;

public abstract class ViewModelBase : ObservableObject
{
    private bool _isLoading;
    private string? _errorMessage;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }
}

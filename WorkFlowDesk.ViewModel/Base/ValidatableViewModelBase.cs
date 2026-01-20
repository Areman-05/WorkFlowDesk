using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WorkFlowDesk.ViewModel.Base;

public abstract class ValidatableViewModelBase : ViewModelBase, INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> _errors = new();

    public bool HasErrors => _errors.Any();

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public System.Collections.IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return _errors.Values.SelectMany(e => e);

        return _errors.ContainsKey(propertyName) ? _errors[propertyName] : Enumerable.Empty<string>();
    }

    protected void AddError(string propertyName, string error)
    {
        if (!_errors.ContainsKey(propertyName))
            _errors[propertyName] = new List<string>();

        if (!_errors[propertyName].Contains(error))
        {
            _errors[propertyName].Add(error);
            OnErrorsChanged(propertyName);
        }
    }

    protected void RemoveError(string propertyName, string error)
    {
        if (_errors.ContainsKey(propertyName) && _errors[propertyName].Contains(error))
        {
            _errors[propertyName].Remove(error);
            if (_errors[propertyName].Count == 0)
                _errors.Remove(propertyName);

            OnErrorsChanged(propertyName);
        }
    }

    protected void ClearErrors(string propertyName)
    {
        if (_errors.ContainsKey(propertyName))
        {
            _errors.Remove(propertyName);
            OnErrorsChanged(propertyName);
        }
    }

    protected void ValidateProperty<T>(T value, string propertyName, Func<T, string?> validator)
    {
        ClearErrors(propertyName);
        var error = validator(value);
        if (!string.IsNullOrEmpty(error))
        {
            AddError(propertyName, error);
        }
    }

    protected virtual void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        OnPropertyChanged(nameof(HasErrors));
    }
}

using System.Windows;
using CommunityToolkit.Mvvm.Input;

namespace WorkFlowDesk.UI.Controls;

public partial class ModalDialog : Window
{
    public ModalDialog()
    {
        InitializeComponent();
    }

    public static bool? ShowDialog(string title, object content, IRelayCommand? acceptCommand = null, IRelayCommand? cancelCommand = null)
    {
        var dialog = new ModalDialog
        {
            Title = title,
            Owner = Application.Current.MainWindow
        };

        var viewModel = new ModalDialogViewModel
        {
            Title = title,
            Content = content,
            AcceptCommand = acceptCommand ?? new RelayCommand(() => dialog.DialogResult = true),
            CancelCommand = cancelCommand ?? new RelayCommand(() => dialog.DialogResult = false)
        };

        dialog.DataContext = viewModel;
        return dialog.ShowDialog();
    }
}

public class ModalDialogViewModel
{
    public string Title { get; set; } = string.Empty;
    public object? Content { get; set; }
    public IRelayCommand? AcceptCommand { get; set; }
    public IRelayCommand? CancelCommand { get; set; }
}

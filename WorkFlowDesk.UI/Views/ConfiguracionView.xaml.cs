using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using WorkFlowDesk.UI.Helpers;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class ConfiguracionView : UserControl
{
    private readonly ConfiguracionViewModel _viewModel;
    private bool _passwordActualVisible;

    public ConfiguracionView(ConfiguracionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        ViewConfirmationHelper.BindConfirmaciones(viewModel);

        viewModel.OperacionCompletada += (_, mensaje) =>
            NotificationService.ShowSuccess(mensaje);
        viewModel.ExplorarRutaSolicitada += (_, path) =>
            Process.Start(new ProcessStartInfo("explorer.exe", path) { UseShellExecute = true });
        viewModel.RestaurarBackupSolicitado += OnRestaurarBackupSolicitado;
    }

    private async void OnRestaurarBackupSolicitado(object? sender, EventArgs e)
    {
        var backupsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
        var dialog = new OpenFileDialog
        {
            Title = "Seleccionar backup",
            Filter = "Backup SQLite (*.db)|*.db|Todos los archivos (*.*)|*.*",
            InitialDirectory = Directory.Exists(backupsDir) ? backupsDir : AppDomain.CurrentDomain.BaseDirectory
        };

        if (dialog.ShowDialog() != true)
            return;

        _viewModel.BackupSeleccionado = dialog.FileName;
        await _viewModel.RestaurarBackupCommand.ExecuteAsync(null);
    }

    private void OnTogglePasswordActualVisibility(object sender, RoutedEventArgs e)
    {
        _passwordActualVisible = !_passwordActualVisible;

        if (_passwordActualVisible)
        {
            PasswordActualTextBox.Text = PasswordActualBox.Password;
            PasswordActualBox.Visibility = Visibility.Collapsed;
            PasswordActualTextBox.Visibility = Visibility.Visible;
            PasswordActualEye.Text = "\uE891";
        }
        else
        {
            PasswordActualBox.Password = PasswordActualTextBox.Text;
            PasswordActualTextBox.Visibility = Visibility.Collapsed;
            PasswordActualBox.Visibility = Visibility.Visible;
            PasswordActualEye.Text = "\uE890";
        }
    }
}

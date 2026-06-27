using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Controls;

public partial class GlobalSearchOverlay : UserControl
{
    public GlobalSearchOverlay()
    {
        InitializeComponent();
        IsVisibleChanged += OnIsVisibleChanged;
    }

    private MainViewModel? ViewModel => DataContext as MainViewModel;

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (IsVisible)
        {
            SearchBox.Focus();
            SearchBox.SelectAll();
        }
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            ViewModel?.CloseGlobalSearchCommand.Execute(null);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Down && ResultsList.Items.Count > 0)
        {
            ResultsList.Focus();
            if (ResultsList.SelectedIndex < 0)
                ResultsList.SelectedIndex = 0;
            e.Handled = true;
        }
    }

    private void OnOverlayBackgroundClick(object sender, MouseButtonEventArgs e)
    {
        ViewModel?.CloseGlobalSearchCommand.Execute(null);
    }

    private void OnDialogClick(object sender, MouseButtonEventArgs e) =>
        e.Handled = true;

    private void OnResultDoubleClick(object sender, MouseButtonEventArgs e) =>
        SelectCurrentResult();

    private void OnResultKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            SelectCurrentResult();
            e.Handled = true;
        }
    }

    private void SelectCurrentResult()
    {
        if (ResultsList.SelectedItem is GlobalSearchResult result)
            ViewModel?.SelectGlobalSearchResultCommand.Execute(result);
    }
}

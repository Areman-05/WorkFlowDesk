using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WorkFlowDesk.Common.Services;

namespace WorkFlowDesk.UI.Controls;

public partial class CityAutocompleteBox : UserControl
{
    private bool _suppressTextChange;
    private bool _isSelecting;

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(CityAutocompleteBox),
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public ObservableCollection<string> Suggestions { get; } = new();

    public CityAutocompleteBox()
    {
        InitializeComponent();
        SuggestionsList.ItemsSource = Suggestions;
    }

    private void OnInputTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_suppressTextChange)
            return;

        UpdateSuggestions(InputBox.Text);
    }

    private void UpdateSuggestions(string query)
    {
        Suggestions.Clear();
        foreach (var city in CityCatalog.Search(query))
            Suggestions.Add(city);

        SuggestionsPopup.IsOpen = Suggestions.Count > 0 && InputBox.IsFocused;
    }

    private void OnSuggestionSelected(object sender, SelectionChangedEventArgs e)
    {
        if (SuggestionsList.SelectedItem is not string city)
            return;

        _isSelecting = true;
        _suppressTextChange = true;
        Text = city;
        InputBox.Text = city;
        _suppressTextChange = false;
        SuggestionsPopup.IsOpen = false;
        SuggestionsList.SelectedItem = null;
        _isSelecting = false;
    }

    private void OnInputGotFocus(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(InputBox.Text))
            UpdateSuggestions(InputBox.Text);
    }

    private void OnInputLostFocus(object sender, RoutedEventArgs e)
    {
        if (_isSelecting)
            return;

        Dispatcher.BeginInvoke(() =>
        {
            if (!SuggestionsPopup.IsMouseOver && !SuggestionsList.IsMouseOver)
                SuggestionsPopup.IsOpen = false;
        }, System.Windows.Threading.DispatcherPriority.Input);
    }

    private void OnInputPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Down && Suggestions.Count > 0)
        {
            SuggestionsList.Focus();
            SuggestionsList.SelectedIndex = 0;
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            SuggestionsPopup.IsOpen = false;
            e.Handled = true;
        }
    }
}

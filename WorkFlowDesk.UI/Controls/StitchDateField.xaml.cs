using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WorkFlowDesk.UI.Controls;

public partial class StitchDateField : UserControl
{
    private static readonly string[] DateFormats = ["dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy"];
    private bool _isInternalUpdate;

    public static readonly DependencyProperty SelectedDateProperty =
        DependencyProperty.Register(
            nameof(SelectedDate),
            typeof(DateTime),
            typeof(StitchDateField),
            new FrameworkPropertyMetadata(
                DateTime.Today,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnSelectedDateChanged));

    public StitchDateField()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    public DateTime SelectedDate
    {
        get => (DateTime)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is StitchDateField field && field.IsLoaded)
            field.SyncFromSelectedDate();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        SyncFromSelectedDate();
    }

    private void SyncFromSelectedDate()
    {
        _isInternalUpdate = true;
        DateTextBox.Text = SelectedDate.ToString("dd/MM/yyyy", CultureInfo.CurrentCulture);
        Calendar.SelectedDate = SelectedDate;
        Calendar.DisplayDate = SelectedDate;
        _isInternalUpdate = false;
    }

    private void OnDateTextLostFocus(object sender, RoutedEventArgs e) => TryParseDateText();

    private void OnDateTextKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            TryParseDateText();
            e.Handled = true;
        }
    }

    private void TryParseDateText()
    {
        if (_isInternalUpdate)
            return;

        var text = DateTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            SyncFromSelectedDate();
            return;
        }

        if (DateTime.TryParseExact(
                text,
                DateFormats,
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out var parsed) ||
            DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsed))
        {
            SelectedDate = parsed.Date;
            SyncFromSelectedDate();
            return;
        }

        SyncFromSelectedDate();
    }

    private void OnCalendarButtonClick(object sender, RoutedEventArgs e)
    {
        Calendar.SelectedDate = SelectedDate;
        Calendar.DisplayDate = SelectedDate;
        CalendarPopup.IsOpen = true;
    }

    private void OnCalendarSelected(object sender, SelectionChangedEventArgs e)
    {
        if (_isInternalUpdate || Calendar.SelectedDate is not DateTime selected)
            return;

        SelectedDate = selected.Date;
        SyncFromSelectedDate();
        CalendarPopup.IsOpen = false;
    }
}

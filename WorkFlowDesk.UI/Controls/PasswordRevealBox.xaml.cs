using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WorkFlowDesk.UI.Controls;

public partial class PasswordRevealBox : UserControl
{
    private bool _syncing;

    public PasswordRevealBox()
    {
        InitializeComponent();
        HiddenBox.KeyDown += (_, e) => RaiseBubbleKeyDown(e);
        VisibleBox.KeyDown += (_, e) => RaiseBubbleKeyDown(e);
    }

    private void RaiseBubbleKeyDown(KeyEventArgs e)
    {
        var args = new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, e.Key)
        {
            RoutedEvent = KeyDownEvent
        };
        RaiseEvent(args);
    }

    public string Password
    {
        get => HiddenBox.Password;
        set
        {
            if (HiddenBox.Password == value && VisibleBox.Text == value)
                return;

            _syncing = true;
            HiddenBox.Password = value ?? string.Empty;
            VisibleBox.Text = value ?? string.Empty;
            _syncing = false;
        }
    }

    public void FocusPassword()
    {
        if (IsRevealed)
            VisibleBox.Focus();
        else
            HiddenBox.Focus();
    }

    private bool IsRevealed => VisibleBox.Visibility == Visibility.Visible;

    private void OnToggleClick(object sender, RoutedEventArgs e)
    {
        if (IsRevealed)
            HidePassword();
        else
            RevealPassword();
    }

    private void RevealPassword()
    {
        VisibleBox.Text = HiddenBox.Password;
        HiddenBox.Visibility = Visibility.Collapsed;
        VisibleBox.Visibility = Visibility.Visible;
        ToggleIcon.Text = "\uE890";
        ToggleButton.ToolTip = "Ocultar contraseña";
        VisibleBox.Focus();
        VisibleBox.CaretIndex = VisibleBox.Text.Length;
    }

    private void HidePassword()
    {
        HiddenBox.Password = VisibleBox.Text;
        VisibleBox.Visibility = Visibility.Collapsed;
        HiddenBox.Visibility = Visibility.Visible;
        ToggleIcon.Text = "\uE7B3";
        ToggleButton.ToolTip = "Mostrar contraseña";
        HiddenBox.Focus();
    }

    private void OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (_syncing || IsRevealed)
            return;

        _syncing = true;
        VisibleBox.Text = HiddenBox.Password;
        _syncing = false;
    }

    private void OnVisibleTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_syncing || !IsRevealed)
            return;

        _syncing = true;
        HiddenBox.Password = VisibleBox.Text;
        _syncing = false;
    }
}

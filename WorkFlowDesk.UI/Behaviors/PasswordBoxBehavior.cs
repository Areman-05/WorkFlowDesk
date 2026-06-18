using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WorkFlowDesk.UI.Behaviors;

/// <summary>Helper para binding de PasswordBox (propiedad adjunta Password).</summary>
public static class PasswordBoxHelper
{
    private static readonly DependencyProperty IsUpdatingProperty =
        DependencyProperty.RegisterAttached(
            "IsInternalUpdating",
            typeof(bool),
            typeof(PasswordBoxHelper),
            new PropertyMetadata(false));

    private static readonly DependencyProperty HasHandlersProperty =
        DependencyProperty.RegisterAttached(
            "HasHandlers",
            typeof(bool),
            typeof(PasswordBoxHelper),
            new PropertyMetadata(false));

    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.RegisterAttached(
            "Password",
            typeof(string),
            typeof(PasswordBoxHelper),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnPasswordPropertyChanged));

    public static string GetPassword(DependencyObject obj) => (string)obj.GetValue(PasswordProperty);

    public static void SetPassword(DependencyObject obj, string value) => obj.SetValue(PasswordProperty, value);

    private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not PasswordBox passwordBox)
            return;

        EnsureHandlers(passwordBox);

        if (GetIsUpdating(passwordBox))
            return;

        var newPassword = e.NewValue as string ?? string.Empty;
        if (passwordBox.Password == newPassword)
            return;

        passwordBox.PasswordChanged -= PasswordBox_PasswordChanged;
        passwordBox.Password = newPassword;
        passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
    }

    private static void EnsureHandlers(PasswordBox passwordBox)
    {
        if (GetHasHandlers(passwordBox))
            return;

        passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
        SetHasHandlers(passwordBox, true);
    }

    private static void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not PasswordBox passwordBox || GetIsUpdating(passwordBox))
            return;

        SetIsUpdating(passwordBox, true);
        try
        {
            passwordBox.SetCurrentValue(PasswordProperty, passwordBox.Password);
            BindingOperations.GetBindingExpression(passwordBox, PasswordProperty)?.UpdateSource();
        }
        finally
        {
            SetIsUpdating(passwordBox, false);
        }
    }

    private static bool GetIsUpdating(DependencyObject obj) => (bool)obj.GetValue(IsUpdatingProperty);

    private static void SetIsUpdating(DependencyObject obj, bool value) => obj.SetValue(IsUpdatingProperty, value);

    private static bool GetHasHandlers(DependencyObject obj) => (bool)obj.GetValue(HasHandlersProperty);

    private static void SetHasHandlers(DependencyObject obj, bool value) => obj.SetValue(HasHandlersProperty, value);
}

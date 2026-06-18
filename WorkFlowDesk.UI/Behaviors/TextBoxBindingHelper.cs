using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WorkFlowDesk.UI.Behaviors;

/// <summary>Binding seguro para TextBox con UpdateSourceTrigger=PropertyChanged (preserva el cursor).</summary>
public static class TextBoxBindingHelper
{
    private static readonly DependencyProperty IsUpdatingProperty =
        DependencyProperty.RegisterAttached(
            "IsInternalUpdating",
            typeof(bool),
            typeof(TextBoxBindingHelper),
            new PropertyMetadata(false));

    private static readonly DependencyProperty HasHandlersProperty =
        DependencyProperty.RegisterAttached(
            "HasHandlers",
            typeof(bool),
            typeof(TextBoxBindingHelper),
            new PropertyMetadata(false));

    public static readonly DependencyProperty BoundTextProperty =
        DependencyProperty.RegisterAttached(
            "BoundText",
            typeof(string),
            typeof(TextBoxBindingHelper),
            new FrameworkPropertyMetadata(
                string.Empty,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnBoundTextChanged));

    public static string GetBoundText(DependencyObject obj) => (string)obj.GetValue(BoundTextProperty);

    public static void SetBoundText(DependencyObject obj, string value) => obj.SetValue(BoundTextProperty, value);

    private static void OnBoundTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox)
            return;

        EnsureHandlers(textBox);

        if (GetIsUpdating(textBox))
            return;

        var newText = e.NewValue as string ?? string.Empty;
        if (textBox.Text == newText)
            return;

        var caretIndex = textBox.CaretIndex;
        var selectionLength = textBox.SelectionLength;

        textBox.TextChanged -= OnTextBoxTextChanged;
        textBox.Text = newText;
        textBox.CaretIndex = Math.Min(caretIndex, textBox.Text.Length);
        textBox.SelectionLength = Math.Min(
            selectionLength,
            Math.Max(0, textBox.Text.Length - textBox.CaretIndex));
        textBox.TextChanged += OnTextBoxTextChanged;
    }

    private static void EnsureHandlers(TextBox textBox)
    {
        if (GetHasHandlers(textBox))
            return;

        textBox.TextChanged += OnTextBoxTextChanged;
        SetHasHandlers(textBox, true);
    }

    private static void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox || GetIsUpdating(textBox))
            return;

        SetIsUpdating(textBox, true);
        try
        {
            textBox.SetCurrentValue(BoundTextProperty, textBox.Text);
            BindingOperations.GetBindingExpression(textBox, BoundTextProperty)?.UpdateSource();
        }
        finally
        {
            SetIsUpdating(textBox, false);
        }
    }

    private static bool GetIsUpdating(DependencyObject obj) => (bool)obj.GetValue(IsUpdatingProperty);

    private static void SetIsUpdating(DependencyObject obj, bool value) => obj.SetValue(IsUpdatingProperty, value);

    private static bool GetHasHandlers(DependencyObject obj) => (bool)obj.GetValue(HasHandlersProperty);

    private static void SetHasHandlers(DependencyObject obj, bool value) => obj.SetValue(HasHandlersProperty, value);
}

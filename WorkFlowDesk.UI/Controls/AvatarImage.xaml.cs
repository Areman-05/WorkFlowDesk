using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.UI.Helpers;

namespace WorkFlowDesk.UI.Controls;

public partial class AvatarImage : UserControl
{
    public static readonly DependencyProperty AvatarIndexProperty =
        DependencyProperty.Register(
            nameof(AvatarIndex),
            typeof(int),
            typeof(AvatarImage),
            new PropertyMetadata(0, OnAvatarIndexChanged));

    public static readonly DependencyProperty FallbackInitialsProperty =
        DependencyProperty.Register(
            nameof(FallbackInitials),
            typeof(string),
            typeof(AvatarImage),
            new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(
            nameof(Size),
            typeof(double),
            typeof(AvatarImage),
            new PropertyMetadata(40.0, OnSizeChanged));

    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(AvatarImage),
            new PropertyMetadata(new CornerRadius(20)));

    public static readonly DependencyProperty InitialsFontSizeProperty =
        DependencyProperty.Register(
            nameof(InitialsFontSize),
            typeof(double),
            typeof(AvatarImage),
            new PropertyMetadata(14.0));

    private int _loadVersion;

    public AvatarImage()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        ApplySize();
    }

    public int AvatarIndex
    {
        get => (int)GetValue(AvatarIndexProperty);
        set => SetValue(AvatarIndexProperty, value);
    }

    public string FallbackInitials
    {
        get => (string)GetValue(FallbackInitialsProperty);
        set => SetValue(FallbackInitialsProperty, value);
    }

    public double Size
    {
        get => (double)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public double InitialsFontSize
    {
        get => (double)GetValue(InitialsFontSizeProperty);
        set => SetValue(InitialsFontSizeProperty, value);
    }

    private static void OnAvatarIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AvatarImage control && control.IsLoaded)
            _ = control.LoadAvatarAsync();
    }

    private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AvatarImage control)
            control.ApplySize();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ApplySize();
        _ = LoadAvatarAsync();
    }

    private void ApplySize()
    {
        var size = Size > 0 ? Size : 40;
        Width = size;
        Height = size;
        MinWidth = size;
        MaxWidth = size;
        MinHeight = size;
        MaxHeight = size;
        CornerRadius = new CornerRadius(size / 2);
        InitialsFontSize = Math.Max(8, size * 0.3);
    }

    private async Task LoadAvatarAsync()
    {
        var version = ++_loadVersion;
        AvatarImg.Source = null;
        Placeholder.Visibility = Visibility.Visible;

        var index = Math.Clamp(AvatarIndex, 0, AvatarCatalog.Count - 1);
        var image = await AvatarImageLoader.LoadAsync(AvatarCatalog.GetUrl(index));
        if (version != _loadVersion)
            return;

        if (image != null)
        {
            AvatarImg.Source = image;
            Placeholder.Visibility = Visibility.Collapsed;
        }
    }
}

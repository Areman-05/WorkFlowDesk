using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WorkFlowDesk.UI.Models;

namespace WorkFlowDesk.UI.Views;

public partial class SplashScreenWindow : Window
{
    public SplashScreenKind Kind { get; }

    public SplashScreenWindow(SplashScreenKind kind = SplashScreenKind.PostLogin)
    {
        Kind = kind;
        InitializeComponent();
        ApplyKindLayout();
    }

    private void ApplyKindLayout()
    {
        if (Kind == SplashScreenKind.Quick)
        {
            Width = 480;
            Height = 520;
            PostLoginPanel.Visibility = Visibility.Collapsed;
            QuickPanel.Visibility = Visibility.Visible;
            return;
        }

        Width = 520;
        Height = 640;
        PostLoginPanel.Visibility = Visibility.Visible;
        QuickPanel.Visibility = Visibility.Collapsed;
    }

    public Task PlayIntroAsync()
    {
        if (Kind == SplashScreenKind.Quick)
            return PlayQuickIntroAsync();

        return PlayPostLoginIntroAsync();
    }

    public async Task RunPostLoginBarSequenceAsync()
    {
        await PulseSegmentAsync(0.38, 920);
        await Task.Delay(140);
        ResetBar();
        await PulseSegmentAsync(0.74, 980);
        await Task.Delay(140);
        ResetBar();
        await PulseSegmentAsync(1.0, 1100);
    }

    public async Task RunQuickLoaderSequenceAsync()
    {
        QuickLoadingText.Text = "Restaurando tu sesión...";

        var tcs = new TaskCompletionSource();
        var animation = new DoubleAnimation(0, 220, TimeSpan.FromMilliseconds(1100))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        };
        animation.Completed += (_, _) => tcs.TrySetResult();
        QuickProgressFill.BeginAnimation(WidthProperty, animation);
        await tcs.Task;

        QuickLoadingText.Text = "Preparando tu espacio de trabajo...";
        await Task.Delay(400);
    }

    public Task FadeOutAsync()
    {
        var tcs = new TaskCompletionSource();

        var fade = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(650))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
        };
        fade.Completed += (_, _) => tcs.TrySetResult();
        RootGrid.BeginAnimation(OpacityProperty, fade);

        return tcs.Task;
    }

    private Task PlayPostLoginIntroAsync()
    {
        var tcs = new TaskCompletionSource();

        var rootFade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(700))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        var contentFade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(900))
        {
            BeginTime = TimeSpan.FromMilliseconds(180),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        var contentScale = new DoubleAnimation(0.94, 1, TimeSpan.FromMilliseconds(900))
        {
            BeginTime = TimeSpan.FromMilliseconds(180),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        contentFade.Completed += (_, _) => tcs.TrySetResult();

        RootGrid.BeginAnimation(OpacityProperty, rootFade);
        ContentCluster.BeginAnimation(OpacityProperty, contentFade);
        ContentCluster.RenderTransform = new ScaleTransform(1, 1);
        ContentCluster.RenderTransformOrigin = new Point(0.5, 0.5);
        ContentCluster.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, contentScale);
        ContentCluster.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, contentScale);

        return tcs.Task;
    }

    private Task PlayQuickIntroAsync()
    {
        var tcs = new TaskCompletionSource();

        var rootFade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(550))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        var contentFade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(750))
        {
            BeginTime = TimeSpan.FromMilliseconds(100),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        var footerFade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(650))
        {
            BeginTime = TimeSpan.FromMilliseconds(280),
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };

        footerFade.Completed += (_, _) => tcs.TrySetResult();

        RootGrid.BeginAnimation(OpacityProperty, rootFade);
        QuickContentCluster.BeginAnimation(OpacityProperty, contentFade);
        QuickFooterCluster.BeginAnimation(OpacityProperty, footerFade);

        return tcs.Task;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        MouseMove += OnMouseMove;

        if (Kind == SplashScreenKind.Quick)
        {
            if (Resources["QuickGradientShiftStoryboard"] is Storyboard quickGradient)
                quickGradient.Begin(this, true);
            if (Resources["QuickStatusPulseStoryboard"] is Storyboard quickPulse)
                quickPulse.Begin(this, true);
            return;
        }

        if (Resources["GradientShiftStoryboard"] is Storyboard gradient)
            gradient.Begin(this, true);
        if (Resources["OrbPulseStoryboard"] is Storyboard postOrbs)
            postOrbs.Begin(this, true);
        if (Resources["ShimmerStoryboard"] is Storyboard postShimmer)
            postShimmer.Begin(this, true);
        if (Resources["LoadingPulseStoryboard"] is Storyboard postPulse)
            postPulse.Begin(this, true);

        StartDotBounce(Dot1, 0);
        StartDotBounce(Dot2, 0.22);
        StartDotBounce(Dot3, 0.44);
    }

    private async Task PulseSegmentAsync(double target, int durationMs)
    {
        var trackWidth = ProgressTrack.ActualWidth > 0 ? ProgressTrack.ActualWidth : 280;
        var toWidth = trackWidth * target;

        ProgressFill.BeginAnimation(WidthProperty, null);
        ProgressFill.Width = 0;

        var tcs = new TaskCompletionSource();
        var animation = new DoubleAnimation(0, toWidth, TimeSpan.FromMilliseconds(durationMs))
        {
            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
        };
        animation.Completed += (_, _) => tcs.TrySetResult();
        ProgressFill.BeginAnimation(WidthProperty, animation);

        await tcs.Task;
    }

    private void ResetBar()
    {
        ProgressFill.BeginAnimation(WidthProperty, null);
        ProgressFill.Width = 0;
    }

    private static void StartDotBounce(FrameworkElement dot, double delaySeconds)
    {
        var transform = new TranslateTransform();
        dot.RenderTransform = transform;

        var animation = new DoubleAnimation(0, -5, TimeSpan.FromMilliseconds(520))
        {
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever,
            BeginTime = TimeSpan.FromSeconds(delaySeconds),
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
        };
        transform.BeginAnimation(TranslateTransform.YProperty, animation);
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (ActualWidth <= 0 || ActualHeight <= 0)
            return;

        var pos = e.GetPosition(this);
        var x = (pos.X / ActualWidth - 0.5) * 24;
        var y = (pos.Y / ActualHeight - 0.5) * 24;

        if (Kind == SplashScreenKind.Quick)
            return;

        OrbPrimaryTranslate.X = x * 0.35;
        OrbPrimaryTranslate.Y = y * 0.35;
        OrbSecondaryTranslate.X = -x * 0.25;
        OrbSecondaryTranslate.Y = -y * 0.25;
        OrbTertiaryTranslate.X = x * 0.15;
        OrbTertiaryTranslate.Y = y * 0.15;
    }
}

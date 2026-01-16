using System.Windows.Controls;

namespace WorkFlowDesk.UI.Services;

public class NavigationService
{
    private ContentControl? _contentArea;

    public void Initialize(ContentControl contentArea)
    {
        _contentArea = contentArea;
    }

    public void NavigateTo(UserControl view)
    {
        _contentArea?.SetCurrentValue(ContentControl.ContentProperty, view);
    }

    public void NavigateTo<T>() where T : UserControl, new()
    {
        NavigateTo(new T());
    }
}

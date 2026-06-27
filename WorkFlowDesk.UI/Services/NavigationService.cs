using System.Windows.Controls;

namespace WorkFlowDesk.UI.Services;

/// <summary>Gestiona la navegación entre vistas del área principal.</summary>
public class NavigationService
{
    private ContentControl? _contentArea;

    public void Initialize(ContentControl contentArea) => _contentArea = contentArea;

    public void NavigateTo(UserControl view) =>
        _contentArea?.SetCurrentValue(ContentControl.ContentProperty, view);
}

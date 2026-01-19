using System.Windows.Controls;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
        var empleadoService = ServiceLocator.GetService<Services.Interfaces.IEmpleadoService>();
        var proyectoService = ServiceLocator.GetService<Services.Interfaces.IProyectoService>();
        var tareaService = ServiceLocator.GetService<Services.Interfaces.ITareaService>();
        var clienteService = ServiceLocator.GetService<Services.Interfaces.IClienteService>();
        
        DataContext = new DashboardViewModel(empleadoService, proyectoService, tareaService, clienteService);
    }
}

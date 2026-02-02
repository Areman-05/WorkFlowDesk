using System.Windows.Controls;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.ViewModels;

namespace WorkFlowDesk.UI.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
        var empleadoService = ServiceLocator.GetService<IEmpleadoService>();
        var proyectoService = ServiceLocator.GetService<IProyectoService>();
        var tareaService = ServiceLocator.GetService<ITareaService>();
        var clienteService = ServiceLocator.GetService<IClienteService>();
        
        DataContext = new DashboardViewModel(empleadoService, proyectoService, tareaService, clienteService);
    }
}

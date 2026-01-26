using CommunityToolkit.Mvvm.Input;
using System.Windows;
using WorkFlowDesk.Domain.Entities;
using WorkFlowDesk.Services.Interfaces;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.ViewModel.ViewModels;

public class ProyectosViewModel : ViewModelBase
{
    private readonly IProyectoService _proyectoService;
    private IEnumerable<Proyecto> _proyectos = new List<Proyecto>();
    private Proyecto? _proyectoSeleccionado;

    public ProyectosViewModel(IProyectoService proyectoService)
    {
        _proyectoService = proyectoService;
        CargarProyectosCommand = new AsyncRelayCommand(CargarProyectosAsync);
        CrearProyectoCommand = new RelayCommand(CrearProyecto);
        EditarProyectoCommand = new RelayCommand<Proyecto>(EditarProyecto, CanEditarProyecto);
        EliminarProyectoCommand = new AsyncRelayCommand<Proyecto>(EliminarProyectoAsync, CanEliminarProyecto);
        
        CargarProyectosCommand.ExecuteAsync(null);
    }

    public IEnumerable<Proyecto> Proyectos
    {
        get => _proyectos;
        set => SetProperty(ref _proyectos, value);
    }

    public Proyecto? ProyectoSeleccionado
    {
        get => _proyectoSeleccionado;
        set
        {
            SetProperty(ref _proyectoSeleccionado, value);
            EditarProyectoCommand.NotifyCanExecuteChanged();
            EliminarProyectoCommand.NotifyCanExecuteChanged();
        }
    }

    public IAsyncRelayCommand CargarProyectosCommand { get; }
    public IRelayCommand CrearProyectoCommand { get; }
    public IRelayCommand<Proyecto> EditarProyectoCommand { get; }
    public IAsyncRelayCommand<Proyecto> EliminarProyectoCommand { get; }

    public event EventHandler<Proyecto>? ProyectoCreado;
    public event EventHandler<Proyecto>? ProyectoEditado;

    private async Task CargarProyectosAsync()
    {
        IsLoading = true;
        try
        {
            Proyectos = await _proyectoService.GetAllAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar proyectos: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void CrearProyecto()
    {
        var nuevoProyecto = new Proyecto
        {
            Estado = EstadoProyecto.Planificacion,
            FechaInicio = DateTime.Now
        };
        ProyectoCreado?.Invoke(this, nuevoProyecto);
    }

    private void EditarProyecto(Proyecto? proyecto)
    {
        if (proyecto != null)
        {
            ProyectoEditado?.Invoke(this, proyecto);
        }
    }

    private bool CanEditarProyecto(Proyecto? proyecto) => proyecto != null;

    private async Task EliminarProyectoAsync(Proyecto? proyecto)
    {
        if (proyecto == null) return;

        var resultado = MessageBox.Show(
            $"¿Está seguro de que desea eliminar el proyecto '{proyecto.Nombre}'?",
            "Confirmar eliminación",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (resultado != MessageBoxResult.Yes)
            return;

        IsLoading = true;
        try
        {
            await _proyectoService.DeleteAsync(proyecto.Id);
            await CargarProyectosAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al eliminar proyecto: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private bool CanEliminarProyecto(Proyecto? proyecto) => proyecto != null;
}

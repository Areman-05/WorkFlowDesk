using System.Windows;
using WorkFlowDesk.UI.Services;
using WorkFlowDesk.ViewModel.Base;

namespace WorkFlowDesk.UI.Helpers;

/// <summary>Enlaza las solicitudes de confirmación del ViewModel con NotificationService.</summary>
public static class ViewConfirmationHelper
{
    public static void BindConfirmaciones(ViewModelBase viewModel)
    {
        viewModel.ConfirmacionSolicitada += (_, args) =>
        {
            args.Confirmado = NotificationService.ShowConfirmation(args.Mensaje, args.Titulo)
                == MessageBoxResult.Yes;
        };
    }
}

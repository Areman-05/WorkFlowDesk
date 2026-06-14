using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using WorkFlowDesk.Common.Services;
using WorkFlowDesk.ViewModel.Base;
using WorkFlowDesk.ViewModel.Models;

namespace WorkFlowDesk.ViewModel.ViewModels;

/// <summary>ViewModel del perfil de usuario y selección de avatar.</summary>
public class ProfileViewModel : ViewModelBase
{
    private int _selectedAvatarIndex;
    private string? _successMessage;

    public ProfileViewModel()
    {
        var user = SessionService.CurrentUser;
        UserName = SessionService.GetUserName();
        UserRole = SessionService.GetUserRole();
        Email = user?.Email ?? string.Empty;
        NombreUsuario = user?.NombreUsuario ?? string.Empty;

        _selectedAvatarIndex = user != null
            ? UserPreferencesService.GetAvatarIndex(user.Id)
            : 0;

        Avatares = new ObservableCollection<AvatarOption>(
            Enumerable.Range(0, AvatarCatalog.Count)
                .Select(i => new AvatarOption { Index = i, Url = AvatarCatalog.GetUrl(i) }));

        GuardarAvatarCommand = new RelayCommand(GuardarAvatar);
        SelectAvatarCommand = new RelayCommand<int>(index => SelectedAvatarIndex = index);
    }

    public string UserName { get; }
    public string UserRole { get; }
    public string Email { get; }
    public string NombreUsuario { get; }

    public ObservableCollection<AvatarOption> Avatares { get; }

    public int SelectedAvatarIndex
    {
        get => _selectedAvatarIndex;
        set => SetProperty(ref _selectedAvatarIndex, value);
    }

    public string? SuccessMessage
    {
        get => _successMessage;
        set => SetProperty(ref _successMessage, value);
    }

    public IRelayCommand<int> SelectAvatarCommand { get; }
    public IRelayCommand GuardarAvatarCommand { get; }

    private void GuardarAvatar()
    {
        var user = SessionService.CurrentUser;
        if (user == null)
            return;

        UserPreferencesService.SetAvatarIndex(user.Id, SelectedAvatarIndex);
        SuccessMessage = "Avatar actualizado correctamente.";
        ErrorMessage = null;
    }
}

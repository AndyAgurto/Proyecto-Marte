using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Marte.Application.Interfaces;
using Marte.Application.ViewModels;
using Marte.Domain.Entities;
using Marte.Infrastructure.Interfaces;

namespace Marte.WPF.ViewModels
{
    public class UserManagementViewModel : ViewModelBase
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IRolRepository _rolRepository;
        private readonly string _currentUser;

        private ObservableCollection<Usuario> _usuarios;
        private ObservableCollection<Rol> _roles;
        private Usuario? _selectedUsuario;
        private bool _isEditMode;
        private bool _isNewMode;

        // Propiedades del formulario
        private string _nombreUsuario = string.Empty;
        private string _contraseña = string.Empty;
        private string _confirmarContraseña = string.Empty;
        private string _nombreCompleto = string.Empty;
        private Rol? _selectedRol;

        public UserManagementViewModel(
            IUserManagementService userManagementService,
            IRolRepository rolRepository,
            string currentUser)
        {
            _userManagementService = userManagementService;
            _rolRepository = rolRepository;
            _currentUser = currentUser;

            _usuarios = new ObservableCollection<Usuario>();
            _roles = new ObservableCollection<Rol>();

            // Comandos
            NewUserCommand = new RelayCommand(() => ExecuteNewUser(null));
            SaveUserCommand = new RelayCommand(() => ExecuteSaveUser(null), () => CanSaveUser(null));
            EditUserCommand = new RelayCommand(() => ExecuteEditUser(null), () => CanEditUser(null));
            DisableUserCommand = new RelayCommand(() => ExecuteDisableUser(null), () => CanDisableUser(null));
            EnableUserCommand = new RelayCommand(() => ExecuteEnableUser(null), () => CanEnableUser(null));
            CancelCommand = new RelayCommand(() => ExecuteCancel(null));
            ChangePasswordCommand = new RelayCommand(() => ExecuteChangePassword(null), () => CanChangePassword(null));

            _ = LoadData();
        }

        #region Properties

        public ObservableCollection<Usuario> Usuarios
        {
            get => _usuarios;
            set => SetProperty(ref _usuarios, value);
        }

        public ObservableCollection<Rol> Roles
        {
            get => _roles;
            set => SetProperty(ref _roles, value);
        }

        public Usuario? SelectedUsuario
        {
            get => _selectedUsuario;
            set
            {
                SetProperty(ref _selectedUsuario, value);
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(CanDisable));
                OnPropertyChanged(nameof(CanEnable));
                CommandManager.InvalidateRequerySuggested();
                
                // Cargar datos del usuario seleccionado en el formulario (solo si no estamos en modo edición/nuevo)
                if (value != null && !IsEditMode && !IsNewMode)
                {
                    LoadUserDataToForm(value);
                }
                else if (value == null && !IsEditMode && !IsNewMode)
                {
                    // Si se deselecciona, limpiar formulario
                    ClearForm();
                }
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                SetProperty(ref _isEditMode, value);
                OnPropertyChanged(nameof(IsFormReadOnly));
                OnPropertyChanged(nameof(IsUsernameReadOnly));
                OnPropertyChanged(nameof(IsRolEnabled));
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(CanDisable));
                OnPropertyChanged(nameof(CanEnable));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsNewMode
        {
            get => _isNewMode;
            set
            {
                SetProperty(ref _isNewMode, value);
                OnPropertyChanged(nameof(IsFormReadOnly));
                OnPropertyChanged(nameof(IsUsernameReadOnly));
                OnPropertyChanged(nameof(IsRolEnabled));
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(CanDisable));
                OnPropertyChanged(nameof(CanEnable));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string NombreUsuario
        {
            get => _nombreUsuario;
            set
            {
                SetProperty(ref _nombreUsuario, value);
                CommandManager.InvalidateRequerySuggested(); // Re-evaluar comandos
            }
        }

        public string Contraseña
        {
            get => _contraseña;
            set
            {
                SetProperty(ref _contraseña, value);
                CommandManager.InvalidateRequerySuggested(); // Re-evaluar comandos
            }
        }

        public string ConfirmarContraseña
        {
            get => _confirmarContraseña;
            set
            {
                SetProperty(ref _confirmarContraseña, value);
                CommandManager.InvalidateRequerySuggested(); // Re-evaluar comandos
            }
        }

        public string NombreCompleto
        {
            get => _nombreCompleto;
            set
            {
                SetProperty(ref _nombreCompleto, value);
                CommandManager.InvalidateRequerySuggested(); // Re-evaluar comandos
            }
        }

        public Rol? SelectedRol
        {
            get => _selectedRol;
            set
            {
                SetProperty(ref _selectedRol, value);
                CommandManager.InvalidateRequerySuggested(); // Re-evaluar comandos
            }
        }

        public bool CanEdit => SelectedUsuario != null && !IsEditMode && !IsNewMode;
        public bool CanDisable => SelectedUsuario != null && SelectedUsuario.Estado && !IsEditMode && !IsNewMode;
        public bool CanEnable => SelectedUsuario != null && !SelectedUsuario.Estado && !IsEditMode && !IsNewMode;
        public bool IsFormReadOnly => !IsEditMode && !IsNewMode;
        public bool IsUsernameReadOnly => !IsNewMode; // Solo editable al crear nuevo usuario
        public bool IsRolEnabled => IsEditMode || IsNewMode; // Habilitado en modo edición o nuevo

        #endregion

        #region Commands

        public ICommand NewUserCommand { get; }
        public ICommand SaveUserCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DisableUserCommand { get; }
        public ICommand EnableUserCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ChangePasswordCommand { get; }

        #endregion

        #region Command Handlers

        private void ExecuteNewUser(object? parameter)
        {
            IsNewMode = true;
            IsEditMode = false;
            SelectedUsuario = null; // Limpiar selección
            ClearForm();
        }

        private bool CanSaveUser(object? parameter)
        {
            if (IsNewMode)
            {
                return !string.IsNullOrWhiteSpace(NombreUsuario) &&
                       !string.IsNullOrWhiteSpace(Contraseña) &&
                       !string.IsNullOrWhiteSpace(ConfirmarContraseña) &&
                       Contraseña == ConfirmarContraseña &&
                       !string.IsNullOrWhiteSpace(NombreCompleto) &&
                       SelectedRol != null;
            }

            if (IsEditMode)
            {
                return !string.IsNullOrWhiteSpace(NombreCompleto) &&
                       SelectedRol != null;
            }

            return false;
        }

        private async void ExecuteSaveUser(object? parameter)
        {
            try
            {
                if (IsNewMode)
                {
                    // Verificar si el usuario ya existe
                    if (await _userManagementService.UserExistsAsync(NombreUsuario))
                    {
                        MessageBox.Show("El nombre de usuario ya existe.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    await _userManagementService.CreateUserAsync(
                        NombreUsuario,
                        Contraseña,
                        NombreCompleto,
                        SelectedRol!.Id,
                        _currentUser);

                    MessageBox.Show("Usuario creado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (IsEditMode && SelectedUsuario != null)
                {
                    await _userManagementService.UpdateUserAsync(
                        SelectedUsuario.Id,
                        NombreCompleto,
                        SelectedRol!.Id,
                        _currentUser);

                    MessageBox.Show("Usuario actualizado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                await LoadData();
                ExecuteCancel(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanEditUser(object? parameter)
        {
            return CanEdit;
        }

        private void ExecuteEditUser(object? parameter)
        {
            if (SelectedUsuario == null) return;

            IsEditMode = true;
            IsNewMode = false;

            NombreUsuario = SelectedUsuario.NombreUsuario;
            NombreCompleto = SelectedUsuario.NombreCompleto;
            SelectedRol = Roles.FirstOrDefault(r => r.Id == SelectedUsuario.RolId);
        }

        private bool CanDisableUser(object? parameter)
        {
            return CanDisable;
        }

        private async void ExecuteDisableUser(object? parameter)
        {
            if (SelectedUsuario == null) return;

            var result = MessageBox.Show(
                $"¿Está seguro que desea deshabilitar al usuario '{SelectedUsuario.NombreUsuario}'?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _userManagementService.DisableUserAsync(SelectedUsuario.Id, _currentUser);
                    MessageBox.Show("Usuario deshabilitado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al deshabilitar usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanEnableUser(object? parameter)
        {
            return CanEnable;
        }

        private async void ExecuteEnableUser(object? parameter)
        {
            if (SelectedUsuario == null) return;

            var result = MessageBox.Show(
                $"¿Está seguro que desea habilitar al usuario '{SelectedUsuario.NombreUsuario}'?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _userManagementService.EnableUserAsync(SelectedUsuario.Id, _currentUser);
                    MessageBox.Show("Usuario habilitado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al habilitar usuario: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteCancel(object? parameter)
        {
            IsEditMode = false;
            IsNewMode = false;
            SelectedUsuario = null; // Limpiar selección
            ClearForm();
        }

        private bool CanChangePassword(object? parameter)
        {
            return SelectedUsuario != null && !IsEditMode && !IsNewMode;
        }

        private async void ExecuteChangePassword(object? parameter)
        {
            if (SelectedUsuario == null) return;

            // Crear un diálogo para cambiar contraseña
            var dialog = new Views.ChangePasswordDialog(SelectedUsuario.NombreUsuario);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    await _userManagementService.ChangePasswordAsync(
                        SelectedUsuario.Id, 
                        dialog.NewPassword, 
                        _currentUser);
                    
                    MessageBox.Show("Contraseña actualizada exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cambiar contraseña: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion

        #region Private Methods

        private async Task LoadData()
        {
            try
            {
                var usuarios = await _userManagementService.GetAllUsersAsync();
                Usuarios.Clear();
                foreach (var usuario in usuarios)
                {
                    Usuarios.Add(usuario);
                }

                var roles = await _rolRepository.GetAllAsync();
                Roles.Clear();
                foreach (var rol in roles)
                {
                    Roles.Add(rol);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            NombreUsuario = string.Empty;
            Contraseña = string.Empty;
            ConfirmarContraseña = string.Empty;
            NombreCompleto = string.Empty;
            SelectedRol = null;
        }

        private void LoadUserDataToForm(Usuario usuario)
        {
            NombreUsuario = usuario.NombreUsuario;
            NombreCompleto = usuario.NombreCompleto;
            SelectedRol = Roles.FirstOrDefault(r => r.Id == usuario.RolId);
            // No cargamos contraseñas por seguridad
            Contraseña = string.Empty;
            ConfirmarContraseña = string.Empty;
        }

        #endregion
    }
}

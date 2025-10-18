using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Marte.Application.Interfaces;
using Marte.Application.ViewModels;
using Marte.Domain.Entities;

namespace Marte.WPF.ViewModels
{
    public class AsistenteManagementViewModel : ViewModelBase
    {
        private readonly IAsistenteService _asistenteService;
        private readonly ICategoriaService _categoriaService;
        private readonly string _currentUser;

        private ObservableCollection<Asistente> _asistentes;
        private ObservableCollection<Categoria> _categorias;
        private Asistente? _selectedAsistente;
        private bool _isEditMode;
        private bool _isNewMode;

        // Propiedades del formulario
        private string _nombres = string.Empty;
        private string _apellidos = string.Empty;
        private string _dni = string.Empty;
        private Categoria? _selectedCategoria;
        private string _numeroGrupo = string.Empty;
        private bool _mostrarSoloActivos = true;

        public AsistenteManagementViewModel(
            IAsistenteService asistenteService,
            ICategoriaService categoriaService,
            string currentUser)
        {
            _asistenteService = asistenteService;
            _categoriaService = categoriaService;
            _currentUser = currentUser;

            _asistentes = new ObservableCollection<Asistente>();
            _categorias = new ObservableCollection<Categoria>();

            // Comandos
            NewAsistenteCommand = new RelayCommand(() => ExecuteNewAsistente(null));
            SaveAsistenteCommand = new RelayCommand(() => ExecuteSaveAsistente(null), () => CanSaveAsistente(null));
            EditAsistenteCommand = new RelayCommand(() => ExecuteEditAsistente(null), () => CanEditAsistente(null));
            DeleteAsistenteCommand = new RelayCommand(() => ExecuteDeleteAsistente(null), () => CanDeleteAsistente(null));
            InactivarAsistenteCommand = new RelayCommand(() => ExecuteInactivarAsistente(null), () => CanInactivarAsistente(null));
            ActivarAsistenteCommand = new RelayCommand(() => ExecuteActivarAsistente(null), () => CanActivarAsistente(null));
            CancelCommand = new RelayCommand(() => ExecuteCancel(null));

            _ = LoadData();
        }

        #region Properties

        public ObservableCollection<Asistente> Asistentes
        {
            get => _asistentes;
            set => SetProperty(ref _asistentes, value);
        }

        public ObservableCollection<Categoria> Categorias
        {
            get => _categorias;
            set => SetProperty(ref _categorias, value);
        }

        public Asistente? SelectedAsistente
        {
            get => _selectedAsistente;
            set
            {
                SetProperty(ref _selectedAsistente, value);
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(CanDelete));
                OnPropertyChanged(nameof(CanInactivar));
                OnPropertyChanged(nameof(CanActivar));
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();

                if (value != null && !IsEditMode && !IsNewMode)
                {
                    LoadAsistenteDataToForm(value);
                }
                else if (value == null && !IsEditMode && !IsNewMode)
                {
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
                OnPropertyChanged(nameof(IsDNIReadOnly));
                OnPropertyChanged(nameof(IsCategoriaEnabled));
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(CanDelete));
                OnPropertyChanged(nameof(CanInactivar));
                OnPropertyChanged(nameof(CanActivar));
                OnPropertyChanged(nameof(IsNumeroGrupoEnabled));
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsNewMode
        {
            get => _isNewMode;
            set
            {
                SetProperty(ref _isNewMode, value);
                OnPropertyChanged(nameof(IsFormReadOnly));
                OnPropertyChanged(nameof(IsDNIReadOnly));
                OnPropertyChanged(nameof(IsCategoriaEnabled));
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(CanDelete));
                OnPropertyChanged(nameof(CanInactivar));
                OnPropertyChanged(nameof(CanActivar));
                OnPropertyChanged(nameof(IsNumeroGrupoEnabled));
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public string Nombres
        {
            get => _nombres;
            set
            {
                SetProperty(ref _nombres, value);
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public string Apellidos
        {
            get => _apellidos;
            set
            {
                SetProperty(ref _apellidos, value);
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public string DNI
        {
            get => _dni;
            set
            {
                SetProperty(ref _dni, value);
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public Categoria? SelectedCategoria
        {
            get => _selectedCategoria;
            set
            {
                SetProperty(ref _selectedCategoria, value);
                OnPropertyChanged(nameof(IsNumeroGrupoVisible));
                OnPropertyChanged(nameof(IsNumeroGrupoEnabled));
                
                // Si no es categoría "Miembros", limpiar el número de grupo
                if (value != null && !value.Nombre.Equals("Miembros", StringComparison.OrdinalIgnoreCase))
                {
                    NumeroGrupo = string.Empty;
                }
                
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public string NumeroGrupo
        {
            get => _numeroGrupo;
            set
            {
                SetProperty(ref _numeroGrupo, value);
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool MostrarSoloActivos
        {
            get => _mostrarSoloActivos;
            set
            {
                SetProperty(ref _mostrarSoloActivos, value);
                _ = LoadAsistentes();
            }
        }

        public bool IsFormReadOnly => !IsEditMode && !IsNewMode;
        public bool IsDNIReadOnly => !IsNewMode; // DNI solo editable al crear nuevo
        public bool IsCategoriaEnabled => IsEditMode || IsNewMode;
        public bool IsNumeroGrupoVisible => SelectedCategoria?.Nombre.Equals("Miembros", StringComparison.OrdinalIgnoreCase) ?? false;
        public bool IsNumeroGrupoEnabled => (IsEditMode || IsNewMode) && IsNumeroGrupoVisible;

        public bool CanEdit => SelectedAsistente != null && !IsEditMode && !IsNewMode;
        public bool CanDelete => SelectedAsistente != null && !IsEditMode && !IsNewMode;
        public bool CanInactivar => SelectedAsistente != null && SelectedAsistente.Estado && !IsEditMode && !IsNewMode;
        public bool CanActivar => SelectedAsistente != null && !SelectedAsistente.Estado && !IsEditMode && !IsNewMode;

        #endregion

        #region Commands

        public ICommand NewAsistenteCommand { get; }
        public ICommand SaveAsistenteCommand { get; }
        public ICommand EditAsistenteCommand { get; }
        public ICommand DeleteAsistenteCommand { get; }
        public ICommand InactivarAsistenteCommand { get; }
        public ICommand ActivarAsistenteCommand { get; }
        public ICommand CancelCommand { get; }

        #endregion

        #region Command Handlers

        private void ExecuteNewAsistente(object? parameter)
        {
            IsNewMode = true;
            IsEditMode = false;
            SelectedAsistente = null;
            ClearForm();
        }

        private bool CanSaveAsistente(object? parameter)
        {
            if (IsNewMode)
            {
                bool basicFieldsValid = !string.IsNullOrWhiteSpace(Nombres) &&
                                       !string.IsNullOrWhiteSpace(Apellidos) &&
                                       !string.IsNullOrWhiteSpace(DNI) &&
                                       SelectedCategoria != null;

                // Si es categoría "Miembros", también validar número de grupo
                if (basicFieldsValid && SelectedCategoria!.Nombre.Equals("Miembros", StringComparison.OrdinalIgnoreCase))
                {
                    return !string.IsNullOrWhiteSpace(NumeroGrupo);
                }

                return basicFieldsValid;
            }

            if (IsEditMode)
            {
                bool basicFieldsValid = !string.IsNullOrWhiteSpace(Nombres) &&
                                       !string.IsNullOrWhiteSpace(Apellidos) &&
                                       SelectedCategoria != null;

                if (basicFieldsValid && SelectedCategoria!.Nombre.Equals("Miembros", StringComparison.OrdinalIgnoreCase))
                {
                    return !string.IsNullOrWhiteSpace(NumeroGrupo);
                }

                return basicFieldsValid;
            }

            return false;
        }

        private async void ExecuteSaveAsistente(object? parameter)
        {
            try
            {
                if (SelectedCategoria == null) return;

                if (IsNewMode)
                {
                    var result = await _asistenteService.CreateAsistenteAsync(
                        Nombres,
                        Apellidos,
                        DNI,
                        SelectedCategoria.Id,
                        NumeroGrupo,
                        _currentUser);

                    if (result.Success)
                    {
                        MessageBox.Show(result.Message, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        IsNewMode = false;
                        await LoadAsistentes();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else if (IsEditMode && SelectedAsistente != null)
                {
                    var result = await _asistenteService.UpdateAsistenteAsync(
                        SelectedAsistente.Id,
                        Nombres,
                        Apellidos,
                        DNI,
                        SelectedCategoria.Id,
                        NumeroGrupo,
                        _currentUser);

                    if (result.Success)
                    {
                        MessageBox.Show(result.Message, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        IsEditMode = false;
                        await LoadAsistentes();
                        ClearForm();
                    }
                    else
                    {
                        MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar asistente: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanEditAsistente(object? parameter)
        {
            return CanEdit;
        }

        private void ExecuteEditAsistente(object? parameter)
        {
            if (SelectedAsistente == null) return;

            IsEditMode = true;
            IsNewMode = false;
        }

        private bool CanDeleteAsistente(object? parameter)
        {
            return CanDelete;
        }

        private async void ExecuteDeleteAsistente(object? parameter)
        {
            if (SelectedAsistente == null) return;

            var result = MessageBox.Show(
                $"¿Está seguro de eliminar permanentemente al asistente {SelectedAsistente.Nombres} {SelectedAsistente.Apellidos}?\n\nEsta acción NO se puede deshacer.",
                "Confirmar Eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var deleteResult = await _asistenteService.DeleteAsistenteAsync(SelectedAsistente.Id, _currentUser);

                if (deleteResult.Success)
                {
                    MessageBox.Show(deleteResult.Message, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadAsistentes();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(deleteResult.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanInactivarAsistente(object? parameter)
        {
            return CanInactivar;
        }

        private async void ExecuteInactivarAsistente(object? parameter)
        {
            if (SelectedAsistente == null) return;

            var result = MessageBox.Show(
                $"¿Está seguro de inactivar al asistente {SelectedAsistente.Nombres} {SelectedAsistente.Apellidos}?",
                "Confirmar Inactivación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var inactivarResult = await _asistenteService.InactivarAsistenteAsync(SelectedAsistente.Id, _currentUser);

                if (inactivarResult.Success)
                {
                    MessageBox.Show(inactivarResult.Message, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadAsistentes();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(inactivarResult.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanActivarAsistente(object? parameter)
        {
            return CanActivar;
        }

        private async void ExecuteActivarAsistente(object? parameter)
        {
            if (SelectedAsistente == null) return;

            var result = MessageBox.Show(
                $"¿Está seguro de activar al asistente {SelectedAsistente.Nombres} {SelectedAsistente.Apellidos}?",
                "Confirmar Activación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var activarResult = await _asistenteService.ActivarAsistenteAsync(SelectedAsistente.Id, _currentUser);

                if (activarResult.Success)
                {
                    MessageBox.Show(activarResult.Message, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadAsistentes();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show(activarResult.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteCancel(object? parameter)
        {
            IsEditMode = false;
            IsNewMode = false;
            SelectedAsistente = null;
            ClearForm();
        }

        #endregion

        #region Helper Methods

        private async Task LoadData()
        {
            await LoadCategorias();
            await LoadAsistentes();
        }

        private async Task LoadCategorias()
        {
            try
            {
                var categorias = await _categoriaService.GetAllCategoriasAsync();
                Categorias.Clear();
                foreach (var categoria in categorias.OrderBy(c => c.Nombre))
                {
                    Categorias.Add(categoria);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar categorías: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadAsistentes()
        {
            try
            {
                IEnumerable<Asistente> asistentes;
                
                if (MostrarSoloActivos)
                {
                    asistentes = await _asistenteService.GetAsistentesActivosAsync();
                }
                else
                {
                    asistentes = await _asistenteService.GetAllAsistentesAsync();
                }

                Asistentes.Clear();
                foreach (var asistente in asistentes)
                {
                    Asistentes.Add(asistente);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar asistentes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadAsistenteDataToForm(Asistente asistente)
        {
            Nombres = asistente.Nombres;
            Apellidos = asistente.Apellidos;
            DNI = asistente.DNI;
            SelectedCategoria = Categorias.FirstOrDefault(c => c.Id == asistente.CategoriaId);
            NumeroGrupo = asistente.NumeroGrupo ?? string.Empty;
        }

        private void ClearForm()
        {
            Nombres = string.Empty;
            Apellidos = string.Empty;
            DNI = string.Empty;
            SelectedCategoria = null;
            NumeroGrupo = string.Empty;
        }

        #endregion
    }
}

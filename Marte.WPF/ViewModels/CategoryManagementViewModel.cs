using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Marte.Application.Interfaces;
using Marte.Application.ViewModels;
using Marte.Domain.Entities;

namespace Marte.WPF.ViewModels
{
    public class CategoryManagementViewModel : INotifyPropertyChanged
    {
        private readonly ICategoriaService _categoriaService;
        private Categoria? _selectedCategoria;
        private string _nombre = string.Empty;
        private bool _isEditMode = false;
        private bool _isNewMode = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        public CategoryManagementViewModel(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
            Categorias = new ObservableCollection<Categoria>();

            NewCategoryCommand = new RelayCommand(StartNewCategory);
            SaveCategoryCommand = new RelayCommand(async () => await SaveCategoryAsync(), CanSave);
            EditCategoryCommand = new RelayCommand(StartEditCategory, () => SelectedCategoria != null && !IsEditMode && !IsNewMode);
            DeleteCategoryCommand = new RelayCommand(async () => await DeleteCategoryAsync(), () => SelectedCategoria != null && !IsEditMode && !IsNewMode);
            CancelCommand = new RelayCommand(CancelEdit);

            _ = LoadCategoriasAsync();
        }

        public ObservableCollection<Categoria> Categorias { get; }

        public Categoria? SelectedCategoria
        {
            get => _selectedCategoria;
            set
            {
                _selectedCategoria = value;
                OnPropertyChanged();
                
                if (value != null && !IsEditMode && !IsNewMode)
                {
                    Nombre = value.Nombre;
                }
            }
        }

        public string Nombre
        {
            get => _nombre;
            set
            {
                _nombre = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        public bool IsNewMode
        {
            get => _isNewMode;
            set
            {
                _isNewMode = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }

        public bool IsReadOnly => !IsEditMode && !IsNewMode;

        public ICommand NewCategoryCommand { get; }
        public ICommand SaveCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task LoadCategoriasAsync()
        {
            try
            {
                var categorias = await _categoriaService.GetAllCategoriasAsync();
                Categorias.Clear();
                foreach (var categoria in categorias)
                {
                    Categorias.Add(categoria);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar categorías: {ex.Message}", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
        }

        private void StartNewCategory()
        {
            IsNewMode = true;
            IsEditMode = false;
            SelectedCategoria = null;
            Nombre = string.Empty;
        }

        private void StartEditCategory()
        {
            if (SelectedCategoria != null)
            {
                IsEditMode = true;
                IsNewMode = false;
                Nombre = SelectedCategoria.Nombre;
            }
        }

        private async Task SaveCategoryAsync()
        {
            try
            {
                if (IsNewMode)
                {
                    await _categoriaService.CreateCategoriaAsync(Nombre);
                    MessageBox.Show("Categoría creada exitosamente.", 
                                  "Éxito", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Information);
                }
                else if (IsEditMode && SelectedCategoria != null)
                {
                    await _categoriaService.UpdateCategoriaAsync(SelectedCategoria.Id, Nombre);
                    MessageBox.Show("Categoría actualizada exitosamente.", 
                                  "Éxito", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Information);
                }

                await LoadCategoriasAsync();
                CancelEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar categoría: {ex.Message}", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
        }

        private async Task DeleteCategoryAsync()
        {
            if (SelectedCategoria == null) return;

            var resultado = MessageBox.Show(
                $"¿Está seguro que desea eliminar la categoría '{SelectedCategoria.Nombre}'?\n\nEsta acción no se puede deshacer.", 
                "Confirmar Eliminación", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Warning);

            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    await _categoriaService.DeleteCategoriaAsync(SelectedCategoria.Id);
                    MessageBox.Show("Categoría eliminada exitosamente.", 
                                  "Éxito", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Information);
                    await LoadCategoriasAsync();
                    CancelEdit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar categoría: {ex.Message}", 
                                  "Error", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Error);
                }
            }
        }

        private void CancelEdit()
        {
            IsEditMode = false;
            IsNewMode = false;
            Nombre = string.Empty;
            SelectedCategoria = null;
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Nombre) &&
                   (IsEditMode || IsNewMode);
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

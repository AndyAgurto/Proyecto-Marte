using System.Windows.Input;

namespace Marte.Application.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Func<Task>? _executeAsync;
        private readonly Func<bool>? _canExecute;
        private readonly Action? _execute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public RelayCommand(Func<Task> executeAsync, Func<bool>? canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { System.Windows.Input.CommandManager.RequerySuggested += value; }
            remove { System.Windows.Input.CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public async void Execute(object? parameter)
        {
            if (_executeAsync != null)
            {
                await _executeAsync();
            }
            else
            {
                _execute?.Invoke();
            }
        }
    }
}
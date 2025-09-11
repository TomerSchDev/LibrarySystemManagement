using System;
using System.Windows.Input;

namespace LibrarySystemModels.Helpers
{
    public class RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        : ICommand
    {
        private readonly Action<object?> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value!;
            remove => CommandManager.RequerySuggested -= value!;
        }
    }
}
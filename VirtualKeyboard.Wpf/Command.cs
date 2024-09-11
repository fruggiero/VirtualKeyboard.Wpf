using System;
using System.Windows.Input;

namespace VirtualKeyboard.Wpf
{
    internal class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private bool _canExecuteCommand = true;
        public bool CanExecuteCommand
        {
            get => _canExecuteCommand;
            set
            {
                _canExecuteCommand = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private readonly Action<object> _execute;

        public Command(Action<object> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteCommand;
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
}

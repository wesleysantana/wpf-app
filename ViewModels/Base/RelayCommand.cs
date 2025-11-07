using System;
using System.Windows.Input;

namespace WpfApp.ViewModels.Base
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _exec;
        private readonly Func<object, bool> _can;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }

            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> exec, Func<object, bool> can = null)
        { 
            _exec = exec; 
            _can = can; 
        }

        public bool CanExecute(object p) => _can?.Invoke(p) ?? true;

        public void Execute(object p) => _exec(p);       
    }
}
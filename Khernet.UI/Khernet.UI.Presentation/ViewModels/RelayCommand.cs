using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// Command to be executed by controls.
    /// </summary>
    [DebuggerStepThrough]
    public class RelayCommand : ICommand
    {

        public event EventHandler CanExecuteChanged
        {
            //Weird: In Visual Studio 2015 this event is not marked as "not used"

            //CommandManager.RequerySuggested is responsible for execute method CanExcecute when conditions could change its result
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        /// Method to be executed, does not any value.
        /// </summary>
        private Action<object> parameterizedExecute;


        /// <summary>
        /// Method to be executed, does not any value.
        /// </summary>
        private Action execute;

        /// <summary>
        /// Method to verify if this command can be executed, only return true or false.
        /// </summary>
        private Predicate<object> canExecute;

        /// <summary>
        /// Parameterless method to verify if this command can be executed, only return true or false.
        /// </summary>
        private Func<bool> parameterlessCanExecute;

        public RelayCommand()
        { }
        public RelayCommand(Action executeMethod)
        {
            execute = executeMethod;
        }

        public RelayCommand(Action<object> executeMethod)
        {
            parameterizedExecute = executeMethod;
        }

        public RelayCommand(Action<object> executeMethod, Func<bool> canExecuteMethod)
        {
            parameterizedExecute = executeMethod;
            parameterlessCanExecute = canExecuteMethod;
        }
        public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            execute = executeMethod;
            parameterlessCanExecute = canExecuteMethod;
        }

        public RelayCommand(Action<object> executeMethod, Predicate<object> canExecuteMethod)
        {
            parameterizedExecute = executeMethod;
            canExecute = canExecuteMethod;
        }

        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
                return canExecute(parameter);
            else if (parameterlessCanExecute != null)
                return parameterlessCanExecute();

            return true;
        }

        public void Execute(object parameter)
        {
            if (parameterizedExecute != null)
                parameterizedExecute(parameter);
            else
                execute();
        }
    }
}

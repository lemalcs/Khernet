﻿using System;
using System.Windows.Input;

namespace Khernet.UI
{
    /// <summary>
    /// Command to be executed by controls
    /// </summary>
    public class RelayCommand : ICommand
    {

        public event EventHandler CanExecuteChanged
        {
            //Weird: In Visual Studio this event is not marked as "not used"

            //CommandManager.RequerySuggested  is resposible for execute method CanExcecute when conditions could change its result
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
        /// Method to be executed, does not any value
        /// </summary>
        private Action<object> parameterizedExecute;


        /// <summary>
        /// Method to be executed, does not any value
        /// </summary>
        private Action execute;

        /// <summary>
        /// Method to verify if this command can be executed, only return true or false
        /// </summary>
        private Predicate<object> canExecute;

        public RelayCommand()
        { }

        public RelayCommand(Action<object> executeMethod, Predicate<object> canExecuteMethod)
        {
            parameterizedExecute = executeMethod;
            canExecute = canExecuteMethod;
        }

        public RelayCommand(Action<object> executeMethod)
        {
            parameterizedExecute = executeMethod;
        }

        public RelayCommand(Action executeMethod)
        {
            execute = executeMethod;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute != null ? canExecute(parameter) : true;
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
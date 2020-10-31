using System;

namespace Khernet.UI.IoC
{
    public interface IApplicationContext
    {
        /// <summary>
        /// Converts a <see cref="string"/> to XAML document byte array.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns></returns>
        byte[] ConvertStringToDocument(string value);

        /// <summary>
        /// Indicates if it is design time.
        /// </summary>
        /// <returns>True if it is design time otherwise false.</returns>
        bool IsInDesignTime();

        /// <summary>
        /// Execute a task on user interface thread.
        /// </summary>
        void Execute(Action action);
    }
}

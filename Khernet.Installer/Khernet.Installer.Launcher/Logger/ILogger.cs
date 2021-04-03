using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khernet.Installer.Launcher.Logger
{
    public interface ILogger
    {
        /// <summary>
        /// Writes an message to log.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        void Log(string message);

        /// <summary>
        /// Writes a message and exception details to log.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="exception">The exception details to write to.</param>
        void Log(string message, Exception exception);
    }
}

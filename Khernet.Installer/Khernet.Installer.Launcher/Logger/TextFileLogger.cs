using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khernet.Installer.Launcher.Logger
{
    /// <summary>
    /// Writes log to a text file.
    /// </summary>
    public class TextFileLogger : ILogger
    {
        private string logPath;
        public TextFileLogger(string logPath)
        {
            this.logPath = logPath;
        }

        /// <summary>
        /// Writes an entry log in text file.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        public void Log(string message)
        {
            using(StreamWriter streamWriter=new StreamWriter(logPath,true))
            {
                streamWriter.WriteLine($"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()} > {message}");
            }
        }

        /// <summary>
        /// Writes an entry log in text file including message and exception details.
        /// </summary>
        /// <param name="message">The message to write to.</param>
        /// <param name="exception">The exception details to write to</param>
        public void Log(string message, Exception exception)
        {
            using (StreamWriter streamWriter = new StreamWriter(logPath, true))
            {
                streamWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd")} {DateTime.Now.ToLongTimeString()} > {message}");

                if(exception!=null)
                {
                    WriteLogLines(streamWriter, exception);
                }

                if(exception!=null&&exception.InnerException!=null)
                {
                    WriteLogLines(streamWriter, exception.InnerException);

                }
            }
        }

        /// <summary>
        /// Writes en entry to log with date and time for every line
        /// </summary>
        /// <param name="stream">The <see cref="TextWriter"/> to write to.</param>
        /// <param name="exception">The exception that needs to be written to log.</param>
        private void WriteLogLines(TextWriter stream,Exception exception)
        {
            if (exception == null)
                return;

            string[] lines = exception.Message.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                stream.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd")} {DateTime.Now.ToLongTimeString()} > {lines[i]}");
            }

            lines = exception.StackTrace.Split(new char[] { '\r', '\n' },StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                stream.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd")} {DateTime.Now.ToLongTimeString()} > {lines[i]}");
            }
        }
    }
}

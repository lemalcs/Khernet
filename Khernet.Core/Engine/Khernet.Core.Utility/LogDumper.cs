using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.IO;

namespace Khernet.Core.Utility
{
    public static class LogDumper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static LogDumper()
        {
            LoggingConfiguration config = new LoggingConfiguration();
            FileTarget target = new FileTarget("KhernetLog");
            target.Layout = Layout.FromString("${longdate} > | ${level:uppercase=true} | ${message}");

            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            target.FileName = Path.Combine(Path.GetDirectoryName(assembly.Location), "Khernet_LOG.txt");

            config.AddRuleForAllLevels(target);

            LogManager.Configuration = config;
        }


        public static void WriteLog(Exception exception, string message)
        {
            string formatted_message = string.Concat(
                "---------------------------------------------------------------------------------------------",
                "\r\n",
                message,
                "\r\n",
                "---------------------------------------------------------------------------------------------",
                "\r\n");
            WriteInformationLines(formatted_message);

            if (exception == null)
                return;

            WriteExceptionLines(exception);

            if (exception.InnerException != null)
                WriteExceptionLines(exception.InnerException);
        }

        public static void WriteLog(Exception exception)
        {
            if (exception == null)
                return;

            WriteExceptionLines(exception);

            if (exception.InnerException != null)
                WriteExceptionLines(exception.InnerException);
        }
        public static void WriteInformation(string text)
        {
            WriteInformationLines(text);
        }

        /// <summary>
        /// Writes en entry to log with details of <see cref="Exception "/>.
        /// </summary>
        /// <param name="exception">The exception that needs to be written to log.</param>
        private static void WriteExceptionLines(Exception exception)
        {
            if (exception == null)
                return;

            string[] lines = exception.Message.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                logger.Log(LogLevel.Error, lines[i]);
            }

            if (exception.StackTrace == null)
                return;

            lines = exception.StackTrace.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                logger.Log(LogLevel.Error, lines[i]);
            }
        }

        /// <summary>
        /// Writes a message to log.
        /// </summary>
        /// <param name="text">The message to write to.</param>
        private static void WriteInformationLines(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            string[] lines = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lines.Length; i++)
            {
                logger.Log(LogLevel.Info, lines[i]);
            }
        }
    }
}

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
        #if DEBUG
        private static Logger logger = LogManager.GetCurrentClassLogger();
        #endif
        static LogDumper()
        {
            #if DEBUG
            LoggingConfiguration config = new LoggingConfiguration();
            FileTarget target = new FileTarget("KhernetLog");
            target.Layout = Layout.FromString(
                "${longdate}|${level:uppercase=true}|${logger}|${identity}|${processinfo}${newline}${message}${newline}${stacktrace}${newline}${exception}${newline}"
                );

            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            target.FileName = Path.Combine(Path.GetDirectoryName(assembly.Location), "Khernet_LOG.txt");

            config.AddRuleForAllLevels(target);

            LogManager.Configuration = config;

            #endif
        }


        public static void WriteLog(Exception exception, string sourceMethod)
        {
            #if DEBUG
            logger.Error(exception, sourceMethod);
            #endif
        }

        public static void WriteLog(Exception exception)
        {
            #if DEBUG
            logger.Error(exception); 
            #endif
        }
        public static void WriteInformation(string text)
        {
            #if DEBUG
            logger.Log(LogLevel.Info, text); 
            #endif

        }
    }
}

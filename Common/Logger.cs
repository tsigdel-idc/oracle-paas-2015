using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using NLog;

namespace IDC.Common
{
    //  Level	Example
    //
    //  Fatal	Highest level: important stuff down
    //  Error	Application crashes / exceptions
    //  Warn	Incorrect behavior but the application can continue
    //  Info	Normal behavior like mail sent, user updated profile etc.
    //  Debug	Executed queries, user authenticated, session expired
    //  Trace	Begin method X, end method X etc
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warn = 3,
        Error = 4,
        Fatal = 5
    }

    public class Logger
    {
        private static NLog.Logger logger =
    ConfigurationManager.AppSettings["ApplicationName"] != null ?
        LogManager.GetLogger(string.Format("{0}.{1}",
            Environment.MachineName,
            ConfigurationManager.AppSettings["ApplicationName"]))
    : LogManager.GetLogger(Environment.MachineName);

        /// <summary>
        /// Log the input message in a specified level
        /// </summary>
        /// <param name="level">The desired logging level of the input message.</param>
        /// <param name="msg">The message to log.</param>
        public static void Log(LogLevel level, string msg)
        {
            // check if the internal logger is initialized
            if (logger == null)
            {
                return;
            }

            switch (level)
            {
                case LogLevel.Trace:
                    logger.Trace(msg);
                    break;
                case LogLevel.Debug:
                    logger.Debug(msg);
                    break;
                case LogLevel.Info:
                    logger.Info(msg);
                    break;
                case LogLevel.Warn:
                    logger.Warn(msg);
                    break;
                case LogLevel.Error:
                    logger.Error(msg);
                    break;
                case LogLevel.Fatal:
                    logger.Fatal(msg);
                    break;
            }
        }

        /// <summary>
        /// Log the input message in a specified level
        /// </summary>
        /// <param name="level">The desired logging level of the input message.</param>
        /// <param name="msg">A string containing specified parameters</param>
        /// <param name="largs">Parameter values</param>
        public static void Log(LogLevel level, string msg, params object[] args)
        {
            //check if the internal logger is initialized
            if (logger == null)
            {
                return;
            }

            switch (level)
            {
                case LogLevel.Trace:
                    logger.Trace(msg, args);
                    break;
                case LogLevel.Debug:
                    logger.Debug(msg, args);
                    break;
                case LogLevel.Info:
                    logger.Info(msg, args);
                    break;
                case LogLevel.Warn:
                    logger.Warn(msg, args);
                    break;
                case LogLevel.Error:
                    logger.Error(msg, args);
                    break;
                case LogLevel.Fatal:
                    logger.Fatal(msg, args);
                    break;
            }
        }
    }
}

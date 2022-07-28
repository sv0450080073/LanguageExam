using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace MyLanguage.ManagerLog
{
    public interface ILoggerManager
    {
        void Error(Type callingType, string message, Exception exception);
        void ErrorMessage(string message);
        void Warn(Type callingType, string message, params Func<object>[] formatItems);
        void Warn(Type callingType, string message, bool showHttpTrace, params Func<object>[] formatItems);
        void WarnWithException(Type callingType, string message, Exception e, params Func<object>[] formatItems);
        void WarnWithException(Type callingType, string message, bool showHttpTrace, Exception e, params Func<object>[] formatItems);
        void Warn<T>(string message, params Func<object>[] formatItems);
        void Warn<T>(string message, bool showHttpTrace, params Func<object>[] formatItems);
        void WarnWithException<T>(string message, Exception e, params Func<object>[] formatItems);
        void WarnWithException<T>(string message, bool showHttpTrace, Exception e, params Func<object>[] formatItems);
        void Info<T>(Func<string> generateMessage);
        void Info(Type callingType, Func<string> generateMessage);
        void Info(Type type, string generateMessageFormat, params Func<object>[] formatItems);
        void Info<T>(string generateMessageFormat, params Func<object>[] formatItems);
        void Debug<T>(string generateMessageFormat, bool showHttpTrace, params Func<object>[] formatItems);
        void Debug<T>(string generateMessageFormat, params Func<object>[] formatItems);
        void Debug(Type type, string generateMessageFormat, params Func<object>[] formatItems);
        void Debug(Type callingType, Func<string> generateMessage);
        void Debug<T>(Func<string> generateMessage);
    }
    public class LoggerManager : ILoggerManager
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(LoggerManager));
        private static string PrefixThreadId(string generateMessageFormat)
        {
            return "[Thread " + Thread.CurrentThread.ManagedThreadId + "] " + generateMessageFormat;
        }
        public LoggerManager()
        {
            try
            {
                XmlDocument log4netConfig = new XmlDocument();

                using (var fs = File.OpenRead("log4net.config"))
                {
                    log4netConfig.Load(fs);
                    var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
                    XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
                    _logger.Info("Log System Initialized");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error", ex);
            }
        }

        #region Error
        public void Error(Type callingType, string message, Exception exception)
        {
            _logger.Error(PrefixThreadId(message), exception);

        }
        public  void ErrorMessage(string message)
        {
            _logger.Error(PrefixThreadId(message));
        }
        #endregion
        #region WARM
        public  void Warn(Type callingType, string message, params Func<object>[] formatItems)
        {
            _logger.WarnFormat(PrefixThreadId(message), formatItems.Select(x => x.Invoke()).ToArray());
        }

        public  void Warn(Type callingType, string message, bool showHttpTrace, params Func<object>[] formatItems)
        {
            _logger.WarnFormat(PrefixThreadId(message), formatItems.Select(x => x.Invoke()).ToArray());
        }

        public  void WarnWithException(Type callingType, string message, Exception e, params Func<object>[] formatItems)
        {
            WarnWithException(callingType, message, false, e, formatItems);
        }

        public  void WarnWithException(Type callingType, string message, bool showHttpTrace, Exception e, params Func<object>[] formatItems)
        {
            var executedParams = formatItems.Select(x => x.Invoke()).ToArray();
            _logger.WarnFormat(PrefixThreadId(message) + ". Exception: " + e, executedParams);
        }

        /// <summary>
        /// Adds a warn log
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="formatItems"></param>
        public  void Warn<T>(string message, params Func<object>[] formatItems)
        {
            Warn(typeof(T), message, formatItems);
        }

        public  void Warn<T>(string message, bool showHttpTrace, params Func<object>[] formatItems)
        {
            Warn(typeof(T), message, showHttpTrace, formatItems);
        }

        public  void WarnWithException<T>(string message, Exception e, params Func<object>[] formatItems)
        {
            WarnWithException(typeof(T), message, e, formatItems);
        }
        public  void WarnWithException<T>(string message, bool showHttpTrace, Exception e, params Func<object>[] formatItems)
        {
            WarnWithException(typeof(T), message, showHttpTrace, e, formatItems);
        }
        #endregion
        #region INFO
        /// <summary>
        /// Traces a message, only generating the message if tracing is actually enabled. Use this method to avoid calling any long-running methods such as "ToDebugString" if logging is disabled.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="generateMessage">The delegate to generate a message.</param>
        /// <remarks></remarks>
        public  void Info<T>(Func<string> generateMessage)
        {
            Info(typeof(T), generateMessage);
        }

        /// <summary>
        /// Traces if tracing is enabled.
        /// </summary>
        /// <param name="callingType"></param>
        /// <param name="generateMessage"></param>
        public  void Info(Type callingType, Func<string> generateMessage)
        {
            _logger.Info(PrefixThreadId(generateMessage.Invoke()));
        }

        /// <summary>
        /// Traces if tracing is enabled.
        /// </summary>
        /// <param name="type">The type for the logging namespace.</param>
        /// <param name="generateMessageFormat">The message format.</param>
        /// <param name="formatItems">The format items.</param>
        public  void Info(Type type, string generateMessageFormat, params Func<object>[] formatItems)
        {
            var executedParams = formatItems.Select(x => x.Invoke()).ToArray();
            _logger.InfoFormat(PrefixThreadId(generateMessageFormat), executedParams);
        }

        /// <summary>
        /// Traces a message, only generating the message if tracing is actually enabled. Use this method to avoid calling any long-running methods such as "ToDebugString" if logging is disabled.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="generateMessageFormat">The generate message format.</param>
        /// <param name="formatItems">The format items.</param>
        /// <remarks></remarks>
        public  void Info<T>(string generateMessageFormat, params Func<object>[] formatItems)
        {
            Info(typeof(T), generateMessageFormat, formatItems);
        }
        #endregion
        #region DEBUG
        /// <summary>
        /// Debugs a message, only generating the message if tracing is actually enabled. Use this method to avoid calling any long-running methods such as "ToDebugString" if logging is disabled.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="generateMessage">The delegate to generate a message.</param>
        /// <remarks></remarks>
        public  void Debug<T>(Func<string> generateMessage)
        {
            Debug(typeof(T), generateMessage);
        }

        /// <summary>
        /// Debugs if tracing is enabled.
        /// </summary>
        /// <param name="callingType"></param>
        /// <param name="generateMessage"></param>
        public  void Debug(Type callingType, Func<string> generateMessage)
        {
            _logger.Debug(PrefixThreadId(generateMessage.Invoke()));
        }

        /// <summary>
        /// Debugs if tracing is enabled.
        /// </summary>
        /// <param name="type">The type for the logging namespace.</param>
        /// <param name="generateMessageFormat">The message format.</param>
        /// <param name="formatItems">The format items.</param>
        public  void Debug(Type type, string generateMessageFormat, params Func<object>[] formatItems)
        {
            var executedParams = formatItems.Select(x => x.Invoke()).ToArray();
            _logger.DebugFormat(PrefixThreadId(generateMessageFormat), executedParams);
        }

        /// <summary>
        /// Debugs a message, only generating the message if debug is actually enabled. Use this method to avoid calling any long-running methods such as "ToDebugString" if logging is disabled.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="generateMessageFormat">The generate message format.</param>
        /// <param name="formatItems">The format items.</param>
        /// <remarks></remarks>
        public  void Debug<T>(string generateMessageFormat, params Func<object>[] formatItems)
        {
            Debug(typeof(T), generateMessageFormat, formatItems);
        }

        /// <summary>
        /// Debugs a message and also writes to the TraceContext specified, useful for when you would like the debug
        /// output also displayed in the Http trace output.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="generateMessageFormat"></param>
        /// <param name="showHttpTrace"></param>
        /// <param name="formatItems"></param>
        public  void Debug<T>(string generateMessageFormat, bool showHttpTrace, params Func<object>[] formatItems)
        {
            Debug(typeof(T), generateMessageFormat, formatItems);
        }
        #endregion
    }
}

using System;

namespace TerrariaModCore.API
{
    /// <summary>
    /// Log severity levels.
    /// </summary>
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    /// <summary>
    /// Centralized logging interface with mod-scoped prefixes.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs an informational message.
        /// </summary>
        void Info(string message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        void Warning(string message);

        /// <summary>
        /// Logs an error message, optionally with an exception and stack trace.
        /// </summary>
        void Error(string message, Exception exception = null);

        /// <summary>
        /// Logs a diagnostic debug message.
        /// </summary>
        void Debug(string message);

        /// <summary>
        /// Logs a formatted message at the specified log level.
        /// </summary>
        void Log(LogLevel level, string message, Exception exception = null);
    }
}

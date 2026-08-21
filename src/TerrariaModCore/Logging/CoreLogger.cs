using System;
using System.IO;
using System.Text;
using TerrariaModCore.API;

namespace TerrariaModCore.Logging
{
    /// <summary>
    /// Central thread-safe logger writing to Console and TMC log files.
    /// Formats all log entries with timestamp, level, and scope prefixes.
    /// </summary>
    public class CoreLogger : ILogger
    {
        private readonly object _lock = new object();
        private readonly string _logFilePath;
        private readonly string _prefix;
        public LogLevel MinimumLevel { get; set; } = LogLevel.Debug;

        public CoreLogger(string logFilePath, string prefix = "TMC")
        {
            _logFilePath = logFilePath;
            _prefix = prefix;

            try
            {
                if (!string.IsNullOrEmpty(_logFilePath))
                {
                    string dir = Path.GetDirectoryName(_logFilePath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }
            }
            catch { }
        }

        public void Info(string message) => Log(LogLevel.Info, message);
        public void Warning(string message) => Log(LogLevel.Warning, message);
        public void Error(string message, Exception exception = null) => Log(LogLevel.Error, message, exception);
        public void Debug(string message) => Log(LogLevel.Debug, message);

        public void Log(LogLevel level, string message, Exception exception = null)
        {
            if (level < MinimumLevel) return;

            string timeStr = DateTime.Now.ToString("HH:mm:ss");
            string levelStr = level.ToString().ToUpperInvariant();
            string tag = string.IsNullOrEmpty(_prefix) ? "[TMC]" : $"[{_prefix}]";
            string formatted = $"[{timeStr}] [{levelStr}] {tag} {message}";

            if (exception != null)
            {
                formatted += $"\nException: {exception.GetType().FullName}: {exception.Message}\nStack Trace:\n{exception.StackTrace}";
            }

            lock (_lock)
            {
                // Console Output with Colors
                ConsoleColor oldColor = Console.ForegroundColor;
                switch (level)
                {
                    case LogLevel.Debug: Console.ForegroundColor = ConsoleColor.DarkGray; break;
                    case LogLevel.Info: Console.ForegroundColor = ConsoleColor.Gray; break;
                    case LogLevel.Warning: Console.ForegroundColor = ConsoleColor.Yellow; break;
                    case LogLevel.Error: Console.ForegroundColor = ConsoleColor.Red; break;
                }

                Console.WriteLine(formatted);
                Console.ForegroundColor = oldColor;

                // File Output
                if (!string.IsNullOrEmpty(_logFilePath))
                {
                    try
                    {
                        File.AppendAllText(_logFilePath, formatted + Environment.NewLine, Encoding.UTF8);
                    }
                    catch { }
                }
            }
        }
    }
}

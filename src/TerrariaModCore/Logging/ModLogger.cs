using System;
using TerrariaModCore.API;

namespace TerrariaModCore.Logging
{
    /// <summary>
    /// Mod-scoped logger that forwards messages to the central logger formatted as [TMC:ModId].
    /// </summary>
    public class ModLogger : ILogger
    {
        private readonly ILogger _coreLogger;
        private readonly string _modId;

        public ModLogger(ILogger coreLogger, string modId)
        {
            _coreLogger = coreLogger;
            _modId = modId;
        }

        public void Info(string message) => Log(LogLevel.Info, message);
        public void Warning(string message) => Log(LogLevel.Warning, message);
        public void Error(string message, Exception exception = null) => Log(LogLevel.Error, message, exception);
        public void Debug(string message) => Log(LogLevel.Debug, message);

        public void Log(LogLevel level, string message, Exception exception = null)
        {
            string prefixed = $"[TMC:{_modId}] {message}";
            _coreLogger.Log(level, prefixed, exception);
        }
    }
}

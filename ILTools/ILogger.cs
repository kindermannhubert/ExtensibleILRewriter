using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ILTools
{
    public interface ILogger
    {
        void Message(LogLevel level, string message);
    }

    public static class ILoggerExtensions
    {
        public static void Notice(this ILogger logger, string message)
        {
            logger.Message(LogLevel.Notice, message);
        }

        public static void Progress(this ILogger logger, string message)
        {
            logger.Message(LogLevel.Progress, message);
        }

        public static void Warning(this ILogger logger, string message)
        {
            logger.Message(LogLevel.Warning, message);
        }

        public static void Error(this ILogger logger, string message)
        {
            logger.Message(LogLevel.Error, message);
        }
    }

    public enum LogLevel
    {
        Notice,
        Progress,
        Warning,
        Error
    }

    public class DummyLogger : ILogger
    {
        public void Message(LogLevel level, string message) { }
    }
}

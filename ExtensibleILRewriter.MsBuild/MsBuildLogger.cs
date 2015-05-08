using ExtensibleILRewriter.Logging;
using Microsoft.Build.Utilities;
using System;

namespace ExtensibleILRewriter.MsBuild
{
    internal class MsBuildLogger : ILogger
    {
        private readonly TaskLoggingHelper msBuildLogger;

        public MsBuildLogger(TaskLoggingHelper msBuildLogger)
        {
            this.msBuildLogger = msBuildLogger;
        }

        public void Message(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Notice:
                case LogLevel.Progress:
                    msBuildLogger.LogMessage(message);
                    break;
                case LogLevel.Warning:
                    msBuildLogger.LogWarning(message);
                    break;
                case LogLevel.Error:
                    msBuildLogger.LogError(message);
                    break;
                default:
                    throw new NotSupportedException($"Unknown {nameof(LogLevel)}: '{level}'");
            }
        }

        public void MessageDetailed(LogLevel level, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message)
        {
            switch (level)
            {
                case LogLevel.Notice:
                case LogLevel.Progress:
                    msBuildLogger.LogMessage(null, null, null, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message);
                    break;
                case LogLevel.Warning:
                    msBuildLogger.LogWarning(null, null, null, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message);
                    break;
                case LogLevel.Error:
                    msBuildLogger.LogError(null, null, null, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message);
                    break;
                default:
                    throw new NotSupportedException($"Unknown {nameof(LogLevel)}: '{level}'");
            }
        }
    }
}

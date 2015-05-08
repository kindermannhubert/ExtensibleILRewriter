using ExtensibleILRewriter.Logging;
using Mono.Cecil;
using System.Linq;

namespace ExtensibleILRewriter.Extensions
{
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

        public static void LogErrorWithSource(this ILogger logger, MethodDefinition method, string message)
        {
            var firstSequencePoint = method.HasBody ? method.Body.Instructions.FirstOrDefault(i => i.SequencePoint != null)?.SequencePoint : null;
            logger.MessageDetailed(
                LogLevel.Error,
                firstSequencePoint?.Document.Url ?? "Unknown",
                firstSequencePoint?.StartLine ?? 0, firstSequencePoint?.StartColumn ?? 0,
                firstSequencePoint?.EndLine ?? 0, firstSequencePoint?.EndColumn ?? 0,
                message);
        }
    }
}

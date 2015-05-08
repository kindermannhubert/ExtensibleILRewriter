namespace ExtensibleILRewriter.Logging
{
    public interface ILogger
    {
        void Message(LogLevel level, string message);

        void MessageDetailed(LogLevel level, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message);
    }
}

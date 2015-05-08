namespace ExtensibleILRewriter.Logging
{
    public class DummyLogger : ILogger
    {
        public void Message(LogLevel level, string message) { }

        public void MessageDetailed(LogLevel level, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message) { }
    }
}

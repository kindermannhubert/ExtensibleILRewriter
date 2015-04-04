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
        void MessageDetailed(LogLevel level, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message);
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
        public void MessageDetailed(LogLevel level, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message) { }
    }
}

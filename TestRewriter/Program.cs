using ILTools;
using ILTools.MsBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestRewriter
{
    class Program
    {
#if DEBUG
        private const string Configuration = "Debug";
#else
        private const string Configuration = "Release";
#endif

        static void Main(string[] args)
        {
            const string ProjectName = "TestApplication";
            var executingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
            var projectBinariesPath = Path.Combine(Path.GetDirectoryName(executingAssemblyLocation), @"..\..\..", ProjectName, "bin", Configuration);
            var assemblyToRewritePath = Path.Combine(projectBinariesPath, ProjectName + ".exe");
            var rewrittenAssemblyPath = Path.Combine(projectBinariesPath, Path.GetFileNameWithoutExtension(assemblyToRewritePath) + "_Rewritten.exe");

            Console.WriteLine("Rewriting:\t\{assemblyToRewritePath}");
            Console.WriteLine("Output:\t\t\{rewrittenAssemblyPath}");
            Console.WriteLine();

            //Environment.CurrentDirectory = @"D:\SourcesPrivate\ILTools\TestApplication";

            var rewriteTask = new AssemblyRewrite()
            {
                AssemblyPath = assemblyToRewritePath,
                ConfigurationPath = Path.Combine( projectBinariesPath , @"..\..\RewriteConfiguration.xml")
            };
            rewriteTask.Execute(rewrittenAssemblyPath, new ConsoleLogger());

            Console.WriteLine();
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }

    class ConsoleLogger : ILogger
    {
        public void Message(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Notice:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Progress:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    throw new NotSupportedException("Unknown \{nameof(LogLevel)}: '\{level}'");
            }
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void MessageDetailed(LogLevel level, string file, int lineNumber, int columnNumber, int endLineNumber, int endColumnNumber, string message)
        {
            switch (level)
            {
                case LogLevel.Notice:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Progress:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    throw new NotSupportedException("Unknown \{nameof(LogLevel)}: '\{level}'");
            }
            Console.WriteLine("File: \{file}; lineNumber: \{lineNumber}; columnNumber: \{columnNumber}; endLineNumber: \{endLineNumber}; endColumnNumber: \{endColumnNumber}; \{message}");
            Console.ResetColor();
        }
    }
}

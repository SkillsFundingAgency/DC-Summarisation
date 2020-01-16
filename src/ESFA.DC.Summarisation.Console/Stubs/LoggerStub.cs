using System;
using System.Runtime.CompilerServices;
using ESFA.DC.Logging.Interfaces;

namespace ESFA.DC.Summarisation.Console.Stubs
{
    public class LoggerStub : ILogger
    {
        public void Dispose()
        {
            // Do nothing
        }

        public void LogDebug(string message, object[] parameters = null, long jobIdOverride = -1, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            System.Console.WriteLine($"JobId: {jobIdOverride}, Message: {message}");
        }

        public void LogError(string message, Exception exception = null, object[] parameters = null, long jobIdOverride = -1, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            System.Console.WriteLine($"JobId: {jobIdOverride}, Message: {message}");
        }

        public void LogFatal(string message, Exception exception = null, object[] parameters = null, long jobIdOverride = -1, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            System.Console.WriteLine($"JobId: {jobIdOverride}, Message: {message}");
        }

        public void LogInfo(string message, object[] parameters = null, long jobIdOverride = -1, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            System.Console.WriteLine($"JobId: {jobIdOverride}, Message: {message}");
        }

        public void LogVerbose(string message, object[] parameters = null, long jobIdOverride = -1, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            System.Console.WriteLine($"JobId: {jobIdOverride}, Message: {message}");
        }

        public void LogWarning(string message, object[] parameters = null, long jobIdOverride = -1, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            System.Console.WriteLine($"JobId: {jobIdOverride}, Message: {message}");
        }
    }
}

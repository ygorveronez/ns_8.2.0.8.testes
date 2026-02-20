using System.Runtime.CompilerServices;
using Infrastructure.Interfaces.Logging;

namespace Infrastructure.Services.Logging
{
    /// <summary>
    /// App logger default para servir de fallback caso a aplicação não configure o logger corretamente. 
    /// Vai escrever no Console, temporariamente.
    /// </summary>
    public class DefaultAppLogger : IAppLogger
    {
        public static readonly IAppLogger Instance = new DefaultAppLogger();

        private DefaultAppLogger() { }

        private static void WriteLog(string level, string mensagem, string? prefixo, string caller, int line, string file, Exception? ex = null)
        {
            var prefix = string.IsNullOrWhiteSpace(prefixo) ? "" : $"[{prefixo}] ";
            var exceptionMsg = ex != null ? $" | Exception: {ex}" : "";

            Console.WriteLine(
                $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{level}] {prefix}{mensagem}{exceptionMsg} " +
                $"(Caller: {caller}, Line: {line}, File: {file})"
            );
        }

        public void Debug(string mensagem, string? prefixo = null,
            [CallerMemberName] string caller = "",
            [CallerLineNumber] int line = 0,
            [CallerFilePath] string file = "")
            => WriteLog("DEBUG", mensagem, prefixo, caller, line, file);

        public void Info(string mensagem, string? prefixo = null,
            [CallerMemberName] string caller = "",
            [CallerLineNumber] int line = 0,
            [CallerFilePath] string file = "")
            => WriteLog("INFO", mensagem, prefixo, caller, line, file);

        public void Warn(string mensagem, string? prefixo = null,
            [CallerMemberName] string caller = "",
            [CallerLineNumber] int line = 0,
            [CallerFilePath] string file = "")
            => WriteLog("WARN", mensagem, prefixo, caller, line, file);

        public void Error(string mensagem, string? prefixo = null,
            [CallerMemberName] string caller = "",
            [CallerLineNumber] int line = 0,
            [CallerFilePath] string file = "")
            => WriteLog("ERROR", mensagem, prefixo, caller, line, file);

        public void Error(Exception ex, string? prefixo = null,
            [CallerMemberName] string caller = "",
            [CallerLineNumber] int line = 0,
            [CallerFilePath] string file = "")
            => WriteLog("ERROR", ex.Message, prefixo, caller, line, file, ex);
    }
}

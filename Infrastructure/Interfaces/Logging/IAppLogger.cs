using System.Runtime.CompilerServices;

namespace Infrastructure.Interfaces.Logging
{
    public interface IAppLogger
    {
        void Error(Exception ex, string? prefixo = null,
            [CallerMemberName] string caller = null!,
            [CallerLineNumber] int line = 0,
            [CallerFilePath] string file = null!);

        void Error(string mensagem, string? prefixo = null,
            [CallerMemberName] string caller = null!,
            [CallerLineNumber] int line = 0,
            [CallerFilePath] string file = null!);

        void Info(string mensagem, string? prefixo = null,
            [CallerMemberName] string caller = null!,
            [CallerLineNumber] int line = 0,
            [CallerFilePath] string file = null!);

        void Warn(string mensagem, string? prefixo = null,
            [CallerMemberName] string caller = null!,
            [CallerLineNumber] int line = 0,
            [CallerFilePath] string file = null!);

        void Debug(string mensagem, string? prefixo = null,
            [CallerMemberName] string caller = null!,
            [CallerLineNumber] int line = 0,
            [CallerFilePath] string file = null!);
    }
}

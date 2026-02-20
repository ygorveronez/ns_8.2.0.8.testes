using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Interfaces.Logging;
using System;

namespace Servicos.Logging
{
    public class ServicosLogger : IAppLogger
    {
        public void Error(Exception ex, string? prefixo = null, string caller = null!, int line = 0, string file = null!) =>
            Log.TratarErro(ex, prefixo ?? "", caller, line, file);

        public void Error(string mensagem, string? prefixo = null, string caller = null!, int line = 0, string file = null!) =>
            Log.TratarErro(mensagem, prefixo ?? "", TipoLogSistema.Error, caller, line, file);

        public void Info(string mensagem, string? prefixo = null, string caller = null!, int line = 0, string file = null!) =>
            Log.GravarInfo(mensagem, prefixo ?? "", caller, line, file);

        public void Warn(string mensagem, string? prefixo = null, string caller = null!, int line = 0, string file = null!) =>
            Log.GravarAdvertencia(mensagem, prefixo ?? "", caller, line, file);

        public void Debug(string mensagem, string? prefixo = null, string caller = null!, int line = 0, string file = null!) =>
            Log.GravarDebug(mensagem, prefixo ?? "", caller, line, file);
    }
}

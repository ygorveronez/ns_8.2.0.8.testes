using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class ConfiguracaoWsHttpBinding
    {
        public long MaxReceivedMessageSize { get; set; }
        public TimeSpan ReceiveTimeout { get; set; }
        public TimeSpan SendTimeout { get; set; }
        public ConfiguracaoWsHttpBindingSecurity Security { get; set; }
        public TimeSpan CloseTimeout { get; set; }
        public TimeSpan OpenTimeout { get; set; }
    }
}

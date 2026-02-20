using System;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class ConfiguracaoBasicHttpBinding
    {
        public long MaxReceivedMessageSize { get; set; }
        public TimeSpan ReceiveTimeout { get; set; }
        public TimeSpan SendTimeout { get; set; }
        public ConfiguracaoBasicHttpBindingSecurity Security { get; set; }
        public TimeSpan CloseTimeout { get; set; }
        public TimeSpan OpenTimeout { get; set; }
    }
}

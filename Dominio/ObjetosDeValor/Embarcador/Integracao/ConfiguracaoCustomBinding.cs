using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class ConfiguracaoCustomBinding
    {
        public string MessageVersion { get; set; }
        public string Namespace { get; set; }
        public string Name { get; set; }
        public TimeSpan OpenTimeout { get; set; }
        public TimeSpan CloseTimeout { get; set; }
        public TimeSpan SendTimeout { get; set; }
        public TimeSpan ReceiveTimeout { get; set; }
        public List<ConfiguracaoCustomBindingElement> Elements { get; set; } = new List<ConfiguracaoCustomBindingElement>();
    }
}

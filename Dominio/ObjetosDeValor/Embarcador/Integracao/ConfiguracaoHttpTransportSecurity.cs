using System;
using System.Collections.Generic;
using System.Text;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class ConfiguracaoHttpTransportSecurity
    {
        public string ClientCredentialType { get; set; }
        public string ProxyCredentialType { get; set; }
        public string Realm { get; set; }
    }
}

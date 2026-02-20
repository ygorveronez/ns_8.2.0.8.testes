using System.Configuration;

namespace Dominio.ObjetosDeValor.Embarcador.Monitoramento.Configuracao.Integracao
{
    public class OpcaoElement : ConfigurationElement
    {

        private const string KeyKey = "Key";
        private const string ValueKey = "Value";

        [ConfigurationProperty(KeyKey, IsRequired = true)]
        public string Key => (string)this[KeyKey];

        [ConfigurationProperty(ValueKey, IsRequired = true)]
        public string Value => (string)this[ValueKey];

    }
}

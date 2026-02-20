using System.Configuration;

namespace Dominio.ObjetosDeValor.Embarcador.Monitoramento.Configuracao.Integracao
{
    [ConfigurationCollection(typeof(OpcaoElement), AddItemName = OpcaoKey)]
    public class OpcaoElementCollection : ConfigurationElementCollection
    {
        private const string OpcaoKey = "Opcao";

        protected override ConfigurationElement CreateNewElement()
        {
            return new OpcaoElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((OpcaoElement)element).Key;
        }

        public string GetValue(string key)
        {
            foreach (OpcaoElement item in this)
            {
                if (item.Key == key)
                {
                    return item.Value.Trim();
                }
            }
            return string.Empty;
        }
    }
}

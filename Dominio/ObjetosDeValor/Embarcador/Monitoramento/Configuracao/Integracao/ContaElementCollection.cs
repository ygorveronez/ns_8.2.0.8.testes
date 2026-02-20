using System.Configuration;

namespace Dominio.ObjetosDeValor.Embarcador.Monitoramento.Configuracao.Integracao
{
    [ConfigurationCollection(typeof(ContaElement), AddItemName = ContaKey)]
    public class ContaElementCollection : ConfigurationElementCollection
    {
        private const string ContaKey = "Conta";

        protected override ConfigurationElement CreateNewElement()
        {
            return new Integracao.ContaElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ContaElement)element).Nome;
        }
    }
}

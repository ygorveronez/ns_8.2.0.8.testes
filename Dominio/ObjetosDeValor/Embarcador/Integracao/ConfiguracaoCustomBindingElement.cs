using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao
{
    public class ConfiguracaoCustomBindingElement
    {
        public string ElementType { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }
}

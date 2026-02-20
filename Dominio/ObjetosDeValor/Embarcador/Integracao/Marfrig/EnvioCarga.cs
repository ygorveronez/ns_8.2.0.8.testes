using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig
{
    public class EnvioCarga
    {
        public string cnpjFilial { get; set; }
        public int protocoloCarga { get; set; }
        public List<int> protocolosPedido { get; set; }
        public int statusEmissao { get; set; }
        public string mensagem { get; set; }
    }
}

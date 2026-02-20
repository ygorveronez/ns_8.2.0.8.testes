using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Marfrig.PreCalculoFrete
{
    public class PreCalculoFrete
    {
        public int protocoloCarga { get; set; }
        public string retorno { get; set; }
        public List<PreCalculoFretePedido> pedidos { get; set; }
    }
}

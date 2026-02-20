using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Integracao.Digibee
{
    public class CancelamentoReservaConsinco
    {
        public int numeroPedido { get; set; }
        public int codigoOrigem { get; set; }
        public int codigoDestino { get; set; }
        public string dataCancelamento { get; set; }
        public string usuario { get; set; }
        public List<Digibee.ProdutoCancelamentoReservaConsinco> itens { get; set; }

    }
}

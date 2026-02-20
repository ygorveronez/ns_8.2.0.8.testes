using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class FiltroPesquisaAlteracaoPedidoAprovacaoTransportador
    {
        public int CodigoTransportador { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataLimite { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }

        public Enumeradores.SituacaoAlcadaRegra? SituacaoAprovacao { get; set; }
    }
}

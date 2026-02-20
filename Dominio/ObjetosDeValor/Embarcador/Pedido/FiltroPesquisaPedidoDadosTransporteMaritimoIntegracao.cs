using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class FiltroPesquisaPedidoDadosTransporteMaritimoIntegracao
    {
        public int CodigoFilial { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }

        public DateTime? DataInicial { get; set; }

        public DateTime? DataLimite { get; set; }

        public string NumeroEXP { get; set; }

        public Enumeradores.SituacaoIntegracao? SituacaoIntegracao { get; set; }
    }
}

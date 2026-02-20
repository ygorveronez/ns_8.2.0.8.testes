using System;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public sealed class FiltroPesquisaAlteracaoPedidoAprovacao
    {
        public int CodigoUsuario { get; set; }

        public DateTime? DataInicio { get; set; }

        public DateTime? DataLimite { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }

        public Enumeradores.SituacaoAlteracaoPedido? SituacaoAlteracaoPedido { get; set; }
    }
}

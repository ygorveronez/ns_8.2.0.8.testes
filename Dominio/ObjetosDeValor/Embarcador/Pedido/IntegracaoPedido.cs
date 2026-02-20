using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido
{
    public class IntegracaoPedido
    {

        public int ProtocoloPedido { get; set; }

        public int CodigoCargaMultiEmbarcador { get; set; }

        public string CodigoPedido { get; set; }

        public string CodigoCargaEmbarcador { get; set; }

        public string CodigoFilial { get; set; }

        public string CodigoFilialTomadora { get; set; }

        public bool UsarOutroEnderecoOrigem { get; set; }

        public bool UsarOutroEnderecoDestino { get; set; }

        public int CodigoIBGEOrigem { get; set; }

        public int CodigoIBGEDestino { get; set; }

        public Dominio.ObjetosDeValor.Endereco Origem { get; set; }

        public Dominio.ObjetosDeValor.Endereco Destino { get; set; }

        public string CodigoFronteiraEmbarcador { get; set; }

        public string CodigoPedidoCliente { get; set; }

        public string DataCarregamento { get; set; }

        public string DataInicialPrevisaoCarregamento { get; set; }

        public string DataFinalPrevisaoCarregamento { get; set; }

        public string DataPrevisaoEntrega { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoEmissao TipoOperacaoEmissao { get; set; }

        public string CodigoModeloVeicularEmbarcador { get; set; }

        public Dominio.ObjetosDeValor.CTe.Cliente Tomador { get; set; }

        public Dominio.ObjetosDeValor.CTe.Cliente Remetente { get; set; }

        public Dominio.ObjetosDeValor.CTe.Cliente Cliente { get; set; }

        public Dominio.ObjetosDeValor.CTe.Cliente Recebedor { get; set; }

        public Dominio.ObjetosDeValor.CTe.Cliente Expedidor { get; set; }

        public int NumeroPaletes { get; set; }

        public decimal PesoTotalPaletes { get; set; }

        public decimal ValorTotalPaletes { get; set; }

        public decimal ValorDescarga { get; set; }

        public List<IntegracaoProdutos> ProdutosDoPedido { get; set; }

        public int CodigoCargaMultiEmbarcadorOrigemCancelada { get; set; }

        public Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }

        public Dominio.Enumeradores.TipoPagamento TipoPagamento { get; set; }

    }
}

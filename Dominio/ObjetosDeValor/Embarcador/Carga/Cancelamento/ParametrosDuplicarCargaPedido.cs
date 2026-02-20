using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Carga.Cancelamento
{
    public sealed class ParametrosDuplicarCargaPedido
    {

        public ParametrosDuplicarCargaPedido()
        {
            PedidosXMLNotasFiscaisAntigas = new List<Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal>();
            CargaEntregasNotasFiscaisAntigas = new List<Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal>();
            PedidosXMLsComponenteAntigos = new List<Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete>();
            PedidoCTesParaSubcontratacaoAntigos = new List<Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao>();
            PedidoCTeParaSubContratacaoPedidoNotaFiscalAntigos = new List<Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal>();
            PedidoCteParaSubContratacaoComponenteFretesAntigos = new List<Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete>();
            CargaPedidoDocumentoMDFesAntigos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe>();
            CargaPedidoDocumentoCTesAntigos = new List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe>();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaPedido> CargaEntregasPedidosAntigas { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaIntegracaoNatura> CargaIntegracoesNaturaAntiga { get; set; }
        public List<Dominio.Entidades.Embarcador.Seguros.ApoliceSeguroAverbacao> ApolicesSegurosAntigas { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotaFrete> CargaPedidoRotasFreteAntigas { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoComponentesFrete> CargaPedidoComponentesFreteAntigos { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoProduto> CargaPedidoProdutosAntigos { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoQuantidades> CargaPedidoQuantidadesAntigos { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoRotas> CargaPedidoRotasAntigas { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoEspelhoIntercement> PedidosEspelhosAntigos { get; set; }
        public List<Dominio.Entidades.Usuario> Motoristas { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> PedidosXMLNotasFiscaisAntigas { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> CargaEntregasNotasFiscaisAntigas { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscalComponenteFrete> PedidosXMLsComponenteAntigos { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacao> PedidoCTesParaSubcontratacaoAntigos { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCTeParaSubContratacaoPedidoNotaFiscal> PedidoCTeParaSubContratacaoPedidoNotaFiscalAntigos { get; set; }
        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoCteParaSubContratacaoComponenteFrete> PedidoCteParaSubContratacaoComponenteFretesAntigos { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoMDFe> CargaPedidoDocumentoMDFesAntigos { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTe> CargaPedidoDocumentoCTesAntigos { get; set; }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDocumentoCTeComponenteFrete> CargaPedidoDocumentoCTeComponentesFreteAntigos { get; set; }

        public List<Dominio.Entidades.Embarcador.Pedidos.PedidoXMLNotaFiscal> PedidosXMLNotasFiscaisNovas { get; set; }
    }
}

using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Pedido.DadosPedidoXmlNotaFiscal
{
    public sealed class XmlNotaFiscal
    {
        public int Codigo { get; set; }

        public int Numero { get; set; }
        public string CFOP { get; set; }
        public string Chave { get; set; }

        public string NumeroOrdemPedido { get; set; }

        public decimal Peso { get; set; }

        public decimal PesoLiquido { get; set; }

        public Enumeradores.ModalidadePagamentoFrete ModalidadeFrete { get; set; }

        public Enumeradores.TipoNotaFiscalIntegrada? TipoNotaFiscalIntegrada { get; set; }
        public Enumeradores.TipoOperacaoNotaFiscal? TipoOperacaoNFe { get; set; }

        public Cliente Emitente { get; set; }

        public Cliente Destinatario { get; set; }

        public Cliente Expedidor { get; set; }

        public Cliente Recebedor { get; set; }

        public Empresa Empresa { get; set; }

        public List<XmlNotaFiscalProduto> Produtos { get; set; }

        public int CodigoCFOP
        {
            get
            {
                int.TryParse(this.CFOP, out int codigo);
                return codigo;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Dominio.ObjetosDeValor.Embarcador.Carga
{
    public class CargaPedido
    {
        #region Propriedades

        public int Codigo { get; set; }

        public int CodigoCarga { get; set; }

        public int CodigoPedido { get; set; }

        public string NumeroPedidoEmbarcador { get; set; }
        public string NumeroControlePedido { get; set; }

        public bool CienciaDoEnvioDaNotaInformado { get; set; }

        public bool ReentregaSolicitada { get; set; }

        public bool PedidoTransbordo { get; set; }

        public bool AgInformarRecebedor { get; set; }

        public bool CTeEmitidoNoEmbarcador { get; set; }

        public Enumeradores.TipoContratacaoCarga TipoContratacaoCarga { get; set; }

        public bool AdicionadaManualmente { get; set; }

        public Enumeradores.ModalidadePagamentoFrete? modalidadePagamentoFrete { get; set; }

        public Enumeradores.TipoNotaFiscal TipoNotaFiscal { get; set; }

        public decimal ValorTotalNotaFiscal { get; set; }

        public decimal QuantidadePallets { get; set; }

        public decimal Cubagem { get; set; }

        public decimal MetrosCubicos { get; set; }

        public decimal ValorCustoFrete { get; set; }

        public DateTime? DataPrevisaoEntrega { get; set; }

        public int TotalNotasFiscais { get; set; }

        public DateTime? PrevisaoEntregaTransportador { get; set; }

        public Cliente Remetente { get; set; }

        public Cliente Destinatario { get; set; }

        public ObjetosDeValor.Localidade Origem { get; set; }

        public ObjetosDeValor.Localidade Destino { get; set; }

        public string Ordem { get; set; }

        public string NumeroBooking { get; set; }

        public string NumeroContainer { get; set; }

        public string NumeroPedido { get; set; }

        public bool PossuiGenset { get; set; }

        public string CodigoPedidoCliente { get; set; }

        public Cliente Armador { get; set; }

        public bool CanceladoAposVinculoCarga { get; set; }

        public string IdMontagemContainer { get; set; }

        public string NumeroContainerCarga { get; set; }

        public DateTime? DataBaseCRT { get; set; }

        public List<CargaPedidoXMLNotaFiscal> NotasFiscais { get; set; }

        public ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal TipoPropostaMultimodal { get; set; }

        public ObjetosDeValor.Embarcador.Enumeradores.ModalPropostaMultimodal ModalPropostaMultimodal { get; set; }

        public ObjetosDeValor.Embarcador.Enumeradores.TipoServicoMultimodal TipoServicoMultimodal { get; set; }

        public ObjetosDeValor.Embarcador.Enumeradores.TipoCobrancaMultimodal TipoCobrancaMultimodal { get; set; }

        public bool IndicadorRemessaVenda { get; set; }

        public decimal PesoPallet { get; set; }     

        #endregion Propriedades

        #region Propriedades com Regras

        public bool PossuiNFeLancada
        {
            get
            {
                return NotasFiscais.Any(notaFiscal => notaFiscal.Ativa);
            }
        }

        public bool PossuiNFePesoZerado
        {
            get
            {
                return NotasFiscais.Any(notaFiscal => (notaFiscal.Peso == 0m) && notaFiscal.Ativa);
            }
        }

        #endregion Propriedades com Regras

        #region Contrutores

        public CargaPedido()
        {
            NotasFiscais = new List<CargaPedidoXMLNotaFiscal>();
        }

        #endregion Construtores
    }
}

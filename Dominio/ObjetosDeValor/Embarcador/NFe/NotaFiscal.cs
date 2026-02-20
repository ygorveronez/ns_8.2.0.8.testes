using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.NFe
{
    public class NotaFiscal
    {
        public int Protocolo { get; set; }
        public string Chave { get; set; }
        public string Rota { get; set; }
        public int Numero { get; set; }
        public string Serie { get; set; }
        public string Modelo { get; set; }
        public decimal Valor { get; set; }
        public decimal BaseCalculoICMS { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal BaseCalculoST { get; set; }
        public decimal ValorFreteLiquido { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorST { get; set; }
        public decimal ValorTotalProdutos { get; set; }
        public decimal ValorSeguro { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorImpostoImportacao { get; set; }
        public decimal ValorPIS { get; set; }
        public decimal ValorCOFINS { get; set; }
        public decimal ValorOutros { get; set; }
        public decimal ValorIPI { get; set; }
        public decimal PesoBruto { get; set; }
        public decimal PesoLiquido { get; set; }
        public decimal Cubagem { get; set; }
        public decimal MetroCubico { get; set; }
        public decimal VolumesTotal { get; set; }
        public string DataEmissao { get; set; }
        public string NaturezaOP { get; set; }
        public decimal QuantidadePallets { get; set; }
        public string InformacoesComplementares { get; set; }
        public string TipoDocumento { get; set; }
        public string TipoDeCarga { get; set; }
        public string NumeroCarregamento { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.ModalidadePagamentoFrete ModalidadeFrete { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Emitente { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Destinatario { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Recebedor { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Expedidor { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Pessoa Tomador { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Pessoas.Empresa Transportador { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frota.Veiculo Veiculo { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.NFe.Volume> Volumes { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Carga.TipoCargaEmbarcador TipoCarga { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoOperacaoNotaFiscal TipoOperacaoNotaFiscal { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.SituacaoNFeSefaz SituacaoNFeSefaz { get; set; }
        public List<Dominio.ObjetosDeValor.Embarcador.Pedido.Produto> Produtos { get; set; }
        public NotaFiscalContabilizacao Contabilizacao { get; set; }
        public Canhoto Canhoto { get; set; }
        public string NumeroDT { get; set; }
        public string DataEmissaoDT { get; set; }
        public string CodigoIntegracaoCliente { get; set; }
        public string NumeroSolicitacao { get; set; }
        public string NumeroPedido { get; set; }
        public int ProtocoloPedido { get; set; }
        public string Ultimos7DigitosNumeroPedido { get; set; }
        public string NumeroRomaneio { get; set; }
        public string SubRota { get; set; }
        public string GrauRisco { get; set; }
        public decimal AliquotaICMS { get; set; }
        public string Observacao { get; set; }
        public string DataPrevisao { get; set; }

        public decimal KMRota { get; set; }
        public decimal ValorComponenteFreteCrossDocking { get; set; }
        public decimal ValorComponenteAdValorem { get; set; }
        public decimal ValorComponenteDescarga { get; set; }
        public decimal ValorComponentePedagio { get; set; }
        public decimal ValorComponenteAdicionalEntrega { get; set; }

        public string NCMPredominante { get; set; }
        public string CodigoProduto { get; set; }
        public string CFOPPredominante { get; set; }
        public string NumeroControleCliente { get; set; }
        public string NumeroCanhoto { get; set; }
        public string NumeroReferenciaEDI { get; set; }
        public string PINSuframa { get; set; }
        public string DescricaoMercadoria { get; set; }
        public decimal PesoAferido { get; set; }
        public string TipoOperacao { get; set; }
        public string ModeloVeicular { get; set; }
        public string ObsPlaca { get; set; }
        public string ObsTransporte { get; set; }
        public string ChaveCTe { get; set; }
        public string NumeroDocumentoEmbarcador { get; set; }
        public string NumeroTransporte { get; set; }
        public string DataHoraCriacaoEmbrcador { get; set; }
        public string IBGEInicioPrestacao { get; set; }
        public string MasterBL { get; set; }
        public string NumeroOutroDocumento { get; set; }

        [System.ComponentModel.DefaultValue("0")]
        public virtual string PesoMiligrama
        {
            get
            {
                return PesoBruto > 0 ? (PesoBruto * 1000000).ToString() : "0";
            }
            set
            {
                //PesoBruto = !String.IsNullOrWhiteSpace(Utilidades.String.OnlyNumbers(value)) && decimal.Parse(Utilidades.String.OnlyNumbers(value)) > 0 ? decimal.Parse(Utilidades.String.OnlyNumbers(value)) / 1000000 : 0;
            }
        }

        public bool DocumentoRecebidoViaNOTFIS { get; set; }
        public List<Embarcador.Carga.Container> Containeres { get; set; }
        public ClassificacaoNFe? ClassificacaoNFe { get; set; }
        public List<Embarcador.NFe.Boleto> Boletos { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormaIntegracao? FormaIntegracao { get; set; }
    }
}

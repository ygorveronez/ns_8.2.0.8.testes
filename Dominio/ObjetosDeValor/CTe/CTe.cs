using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.CTe
{
    public class CTe
    {
        public Empresa Emitente { get; set; }

        public int Numero { get; set; }
        public int Serie { get; set; }
        public string ChaveCTe { get; set; }

        public int NumeroCarga { get; set; }

        public int NumeroUnidade { get; set; }

        public string FinalizarCarga { get; set; } //Parametro criado para a Marfrig (Para identificar quando a carga vai ser finalizada, para emissão dos CTes com rateio de frete, e emissão de MDFe)

        public string CodigoControleInternoCliente { get; set; }

        public int CFOP { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Dominio.Enumeradores.TipoPagamento TipoPagamento { get; set; }

        public string DataEmissao { get; set; }

        public Dominio.Enumeradores.TipoImpressao TipoImpressao { get; set; }

        public string CaracteristicaTransporte { get; set; }

        public string CaracteristicaServico { get; set; }

        public Dominio.Enumeradores.TipoCTE TipoCTe { get; set; }

        public Dominio.Enumeradores.TipoServico TipoServico { get; set; }

        public Dominio.Enumeradores.OpcaoSimNao Retira { get; set; }

        public string DetalhesRetira { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Dominio.Enumeradores.TipoTomador TipoTomador { get; set; }

        public int CodigoIBGECidadeInicioPrestacao { get; set; }

        public int CodigoIBGECidadeTerminoPrestacao { get; set; }

        public Cliente Remetente { get; set; }

        public Cliente Expedidor { get; set; }

        public Cliente Destinatario { get; set; }

        public Cliente Recebedor { get; set; }

        public Cliente Tomador { get; set; }

        public Cliente ClienteRetira { get; set; }

        public Cliente ClienteEntrega { get; set; }

        public decimal ValorTotalPrestacaoServico { get; set; }

        public decimal ValorTotalDocumentoFiscal { get; set; }

        public decimal ValorAReceber { get; set; }

        public decimal ValorFreteComICMSIncluso { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorTotalMercadoria { get; set; }

        public decimal ValorImpostoSuspenso { get; set; }

        public string ProdutoPredominante { get; set; }
        public string ProdutoPredominanteCEAN { get; set; }

        public string ProdutoPredominanteNCM { get; set; }

        public string ObservacaoDaCarga { get; set; }

        public string Container { get; set; }

        public string DataPrevistaContainer { get; set; }

        public string LacreContainer { get; set; }

        public string ChaveCTESubstituicaoComplementar { get; set; }

        public bool SubstituicaoTomador { get; set; }

        public string DataAnulacao { get; set; }

        public string DataPrevistaEntrega { get; set; }

        public Dominio.Enumeradores.OpcaoSimNao Lotacao { get; set; }

        public string CIOT { get; set; }

        public string ObservacoesGerais { get; set; }

        public Dominio.Enumeradores.OpcaoSimNao IncluirICMSNoFrete { get; set; }

        public decimal PercentualICMSIncluirNoFrete { get; set; }

        public string OutrasCaracteristicasDaCarga { get; set; }

        public ImpostoCOFINS COFINS { get; set; }

        public ImpostoPIS PIS { get; set; }

        public ImpostoICMS ICMS { get; set; }

        public ImpostoISS ISS { get; set; }

        public ImpostoIR IR { get; set; }

        public ImpostoCSLL CSLL { get; set; }

        public ImpostoIBSCBS IBSCBS { get; set; }

        public List<Documento> Documentos { get; set; }

        public List<Entrega> Entregas { get; set; }

        public List<QuantidadeCarga> QuantidadesCarga { get; set; }

        public List<DocumentoTransporteAnterior> DocumentosTransporteAnteriores { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.CTe.DocumentoAnteriorPapel> DocumentosAnterioresDePapel { get; set; }

        public List<Veiculo> Veiculos { get; set; }

        public List<Motorista> Motoristas { get; set; }

        public List<Seguro> Seguros { get; set; }

        public List<ProdutoPerigoso> ProdutosPerigosos { get; set; }

        public List<ComponentePrestacao> ComponentesDaPrestacao { get; set; }

        public List<Observacao> ObservacoesContribuinte { get; set; }

        public List<Observacao> ObservacoesFisco { get; set; }

        public List<ValePedagio> ValesPedagio { get; set; }

        public bool ExibeICMSNaDACTE { get; set; }

        public Dominio.Enumeradores.OpcaoSimNao indicadorGlobalizado { get; set; }

        public decimal ValorCargaAverbacao { get; set; }

        public ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE? indicadorIETomador { get; set; }

        public string Versao { get; set; }

        public string CodigoTabelaFreteIntegracao { get; set; }

        public string CodigoTipoOperacao { get; set; }

        public string Romaneio { get; set; }

        public string TipoVeiculo { get; set; }

        public string TipoCalculo { get; set; }

        public decimal ValorDespesa { get; set; }

        public string TipoCTeIntegracao { get; set; }

        public string CodigoProdutoEmbarcador { get; set; }

        public decimal PesoCubado { get; set; }
        public decimal Peso { get; set; }
        public decimal PesoLiquido { get; set; }
        public decimal PesoFaturado { get; set; }
        public decimal Volumes { get; set; }
        public decimal MetrosCubicos { get; set; }
        public decimal Pallets { get; set; }
        public decimal FatorCubagem { get; set; }
        public decimal PercentualPagamentoAgregado { get; set; }

        public decimal? ValorTotalMoeda { get; set; }
        public decimal? ValorCotacaoMoeda { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? Moeda { get; set; }

        public virtual Cliente TomadorDoCTe
        {
            get
            {
                switch (this.TipoTomador)
                {
                    case Enumeradores.TipoTomador.Destinatario:
                        return this.Destinatario;
                    case Enumeradores.TipoTomador.Expedidor:
                        return this.Expedidor;
                    case Enumeradores.TipoTomador.Outros:
                        return this.Tomador;
                    case Enumeradores.TipoTomador.Recebedor:
                        return this.Recebedor;
                    case Enumeradores.TipoTomador.Remetente:
                        return this.Remetente;
                    default:
                        return null;
                }
            }
        }

        //OUTROS MODAIS
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoModal TipoModal { get; set; }

        #region Aquaviario
        public decimal ValorPrestacaoAFRMM { get; set; }
        public decimal ValorAdicionalAFRMM { get; set; }
        public Navio Navio { get; set; }
        public string NumeroViagem { get; set; }
        public string Direcao { get; set; }
        public List<Balsa> Balsas { get; set; }
        public List<Container> Containeres { get; set; }

        #endregion

        #region Multimodal
        public string NumeroCOTM { get; set; }

        #endregion

        public Porto PortoOrigem { get; set; }

        public Porto PortoPassagemUm { get; set; }

        public Porto PortoPassagemDois { get; set; }

        public Porto PortoPassagemTres { get; set; }

        public Porto PortoPassagemQuatro { get; set; }

        public Porto PortoPassagemCinco { get; set; }


        public Porto PortoDestino { get; set; }
        public Terminal TerminalOrigem { get; set; }
        public Terminal TerminalDestino { get; set; }
        public Viagem Viagem { get; set; }

        public Viagem ViagemPassagemUm { get; set; }
        public Viagem ViagemPassagemDois { get; set; }
        public Viagem ViagemPassagemTres { get; set; }
        public Viagem ViagemPassagemQuatro { get; set; }
        public Viagem ViagemPassagemCinco { get; set; }
        public string NumeroControle { get; set; }
        public string NumeroBooking { get; set; }
        public string NumeroOS { get; set; }
        public string Embarque { get; set; }
        public string MasterBL { get; set; }
        public string NumeroDI { get; set; }
        public string BookingReference { get; set; }
        public Cliente ClienteProvedorOS { get; set; }
        public string DescricaoCarrier { get; set; }
        public Enumeradores.TipoPropostaFeeder TipoPropostaFeeder { get; set; }
        public bool SVMTerceiro { get; set; }
        public bool SVMProprio { get; set; }
        public string TipoConhecimentoProceda { get; set; }
        public CentroCustoViagem CentroDeCustoViagem { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoOSConvertido? TipoOSConvertido { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoOS? TipoOS { get; set; }

        #region NFSe

        public string SerieRPS { get; set; }
        public Dominio.Enumeradores.OpcaoSimNao IncluirISSNoFrete { get; set; }

        public NaturezaNFSe NaturezaNFSe { get; set; }

        public decimal ValorOutrasRetencoes { get; set; }

        public decimal ValorDescontoIncondicionado { get; set; }

        public decimal ValorDeducoes { get; set; }

        public decimal ValorDescontoCondicionado { get; set; }

        public List<ItemNFSe> ItensNFSe { get; set; }

        #endregion
        public ObjetosDeValor.Embarcador.Enumeradores.TipoEmissao? TipoEmissao { get; set; }

        public bool NaoEnviarAliquotaEValorISS { get; set; }
        public bool DocumentoOperacaoContainer { get; set; }
    }

}
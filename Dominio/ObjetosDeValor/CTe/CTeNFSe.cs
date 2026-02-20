using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.CTe
{
    public class CTeNFSe
    {
        public Empresa Emitente { get; set; }

        public int Numero { get; set; }

        public int? NumeroRPS { get; set; }

        public int Serie { get; set; }

        public string ChaveCTe { get; set; }

        public int NumeroCarga { get; set; }

        public int NumeroUnidade { get; set; }


        public string CodigoControleInternoCliente { get; set; }

        public int CFOP { get; set; }

        public Dominio.Enumeradores.TipoPagamento TipoPagamento { get; set; }

        public string DataEmissao { get; set; }

        public Dominio.Enumeradores.TipoImpressao TipoImpressao { get; set; }

        public string CaracteristicaTransporte { get; set; }

        public string CaracteristicaServico { get; set; }

        public Dominio.Enumeradores.TipoCTE TipoCTe { get; set; }

        public Dominio.Enumeradores.TipoServico TipoServico { get; set; }

        public Dominio.Enumeradores.OpcaoSimNao Retira { get; set; }

        public string DetalhesRetira { get; set; }

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

        public decimal ValorAReceber { get; set; }

        public decimal ValorFrete { get; set; }

        public decimal ValorTotalMercadoria { get; set; }

        public string ProdutoPredominante { get; set; }

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

        public List<Documento> Documentos { get; set; }

        public List<QuantidadeCarga> QuantidadesCarga { get; set; }

        public List<DocumentoTransporteAnterior> DocumentosTransporteAnteriores { get; set; }

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

        public ObjetosDeValor.Embarcador.Enumeradores.IndicadorIE indicadorIETomador { get; set; }

        public string Versao { get; set; }

        public string CodigoTabelaFreteIntegracao { get; set; }

        public string CodigoTipoOperacao { get; set; }

        public string Romaneio { get; set; }

        public string TipoVeiculo { get; set; }

        public string TipoCalculo { get; set; }

        public decimal ValorDespesa { get; set; }

        public string ISSNFSeRetido { get; set; }

        public string NumeroContainer { get; set; }
        public string NumeroLacre1 { get; set; }
        public string NumeroLacre2 { get; set; }
        public string NumeroLacre3 { get; set; }
        public string NomeNavio { get; set; }
        public string PortoOrigem { get; set; }
        public string PortoDestino { get; set; }

        public ServicoNFSe ServicoNFSe { get; set; }

        public TributacaoNFSe TributacaoNFSe { get; set; }

        public IBSCBS IBSCBS { get; set; }

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
    }

}
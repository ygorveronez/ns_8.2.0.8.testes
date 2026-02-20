using System;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Fatura
{
    public class FaturaIntegracao
    {
        public int Codigo { get; set; }
        public Pessoas.Pessoa Cliente { get; set; }
        public Pessoas.GrupoPessoa GrupoPessoas { get; set; }
        public Embarcador.Carga.TipoOperacao TipoOperacao { get; set; }
        public Embarcador.Carga.TipoCargaEmbarcador TipoCarga { get; set; }
        public Embarcador.Frota.Veiculo Veiculo { get; set; }
        public decimal? AliquotaICMS { get; set; }
        public int Numero { get; set; }
        public long NumeroPreFatura { get; set; }
        public long NumeroFaturaOriginal { get; set; }
        public bool GerarDocumentosAutomaticamente { get; set; }
        public DateTime DataFatura { get; set; }
        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public string Observacao { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoPessoa TipoPessoa { get; set; }
        public decimal TotalLiquido { get; set; }
        public decimal Total { get; set; }
        public decimal Desconto { get; set; }
        public decimal Acrescimo { get; set; }
        public string ObservacaoFatura { get; set; }
        public bool ImprimeObservacaoFatura { get; set; }
        public Pessoas.Pessoa ClienteTomadorFatura { get; set; }
        public Embarcador.Pessoas.Empresa Empresa { get; set; }
        public string Banco { get; set; }
        public Financeiro.TipoMovimento TipoMovimentoUso { get; set; }
        public Financeiro.TipoMovimento TipoMovimentoReversao { get; set; }
        public Financeiro.TipoMovimento TipoMovimentoUsoDesconto { get; set; }
        public Financeiro.TipoMovimento TipoMovimentoReversaoDesconto { get; set; }
        public Financeiro.TipoMovimento TipoMovimentoUsoAcrescimo { get; set; }
        public Financeiro.TipoMovimento TipoMovimentoReversaoAcrescimo { get; set; }
        public string Agencia { get; set; }
        public string DigitoAgencia { get; set; }
        public string NumeroConta { get; set; }
        public ObjetosDeValor.Embarcador.Enumeradores.TipoContaBanco? TipoContaBanco { get; set; }
        //public ObjetosDeValor.Embarcador.Enumeradores.TipoArredondamento TipoArredondamentoParcelas { get; set; }
        public DateTime? DataCancelamentoFatura { get; set; }
        public bool NovoModelo { get; set; }
        public string NomeCliente { get; set; }
        public string CodigoDeposito { get; set; }
        public string TipoFrete { get; set; }
        public string CodigoTransportadora { get; set; }
        public string ModalidadeFrete { get; set; }
        public DateTime? DataFechamento { get; set; }
        public bool ReverteuAcrescimoDesconto { get; set; }
        public string MotivoCancelamento { get; set; }
        public Embarcador.Carga.Viagem PedidoViagemNavio { get; set; }
        public Embarcador.Carga.TerminalPorto TerminalOrigem { get; set; }
        public Embarcador.Carga.TerminalPorto TerminalDestino { get; set; }
        public Dominio.ObjetosDeValor.Localidade Origem { get; set; }
        public Dominio.ObjetosDeValor.Localidade Destino { get; set; }
        public string NumeroBooking { get; set; }        
        public int Carga { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Localidade.Pais PaisOrigem { get; set; }
        public ICollection<ObjetosDeValor.Embarcador.Enumeradores.TipoPropostaMultimodal> TipoPropostaMultimodal { get; set; }
        public string MDFe { get; set; }
        public Embarcador.Carga.Container Container { get; set; }
        public string NumeroControleCliente { get; set; }
        public string IETomador { get; set; }
        public string NumeroReferenciaEDI { get; set; }
        public string CTe { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }
        public DateTime? DataBaseCRT { get; set; }
        public decimal ValorMoedaCotacao { get; set; }
        public decimal TotalMoedaEstrangeira { get; set; }
        public bool NaoUtilizarMoedaEstrangeira { get; set; }        
        public bool FaturamentoExclusivo { get; set; }
        public List<FaturaIntegracaoCTe> CTes { get; set; }
        public List<FaturaIntegracaoParcela> Parcelas { get; set; }
        public List<FaturaIntegracaoIntegracao> Integracoes { get; set; }
        public virtual ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura Situacao { get; set; }        
        /// <summary>
        /// Base64 format
        /// </summary>
        public string PDF { get; set; }
    }
}

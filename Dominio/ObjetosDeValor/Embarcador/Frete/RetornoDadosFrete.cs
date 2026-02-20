using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class RetornoDadosFrete
    {
        public bool CargaAgrupada { get; set; }
        public bool ExibirCalculoCargaAgrupada { get; set; }
        public bool AgrupadaPosEmissaoDocumento { get; set; }
        public Enumeradores.SituacaoRetornoDadosFrete situacao { get; set; }
        public Enumeradores.SituacaoCarga situacaoCarga { get; set; }
        public dynamic rotasNaoEncontradas { get; set; }
        public dynamic dadosRetornoTipoFrete { get; set; }
        public decimal valorFrete { get; set; }
        public decimal ValorFreteLiquido { get; set; }
        public decimal valorFreteContratoFrete { get; set; }
        public decimal valorFreteAPagar { get; set; }
        public decimal valorFreteAPagarComICMSeISS { get; set; }
        public decimal valorFreteTabelaFrete { get; set; }
        public decimal valorFreteOperador { get; set; }
        public decimal valorFreteLeilao { get; set; }
        public decimal valorFreteEmbarcador { get; set; }
        public decimal valorICMS { get; set; }
        public decimal valorICMSIncluso { get; set; }
        public decimal valorISS { get; set; }
        public decimal ValorRetencaoISS { get; set; }
        public decimal valorISSIncluso { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Frete.FreteSubContratacao freteSubContratacao { get; set; }
        public dynamic componentesFrete { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTabelaFrete tipoTabelaFrete { get; set; }
        public dynamic complementosDoFrete { get; set; }
        public string mensagem { get; set; }
        public RetornoDadosFrete DadosFreteFilialEmissora { get; set; }
        public dynamic ComposicaoFrete { get; set; }
        public dynamic ComposicaoFreteCarga { get; set; }
        public dynamic ComposicaoFretePedido { get; set; }
        public dynamic ComposicaoFreteStage { get; set; }
        public dynamic ComposicaoFreteDocumento { get; set; }
        public dynamic ComposicaoFreteCargaSubTrecho { get; set; }

        public bool BloqueadaDiferencaValorFrete { get; set; }
        public bool PermiteRoterizarNovamente { get; set; }
        public bool GerarOcorrenciaDiferencaValorFrete { get; set; }
        public bool VeiculoPossuiContratoFrete { get; set; }
        public decimal ValorDiferencaValorFrete { get; set; }
        public dynamic TipoOcorrenciaDiferencaValorFrete { get; set; }

        public List<Dominio.ObjetosDeValor.Embarcador.Frete.ListaCargasRetornoFrete> cargas { get; set; }

        public decimal aliquotaICMS { get; set; }
        public string CodigoClassificacaoTributaria { get; set; }
        public decimal aliquotaISS { get; set; }
        public decimal valorMercadoria { get; set; }
        public decimal AliquotaIBSUF { get; set; }
        public decimal ValorIBSUF { get; set; }
        public decimal ReducaoIBSUF { get; set; }
        public decimal AliquotaIBSMunicipio { get; set; }
        public decimal ValorIBSMunicipio { get; set; }
        public decimal ReducaoIBSMunicipio { get; set; }
        public decimal AliquotaCBS { get; set; }
        public decimal ValorCBS { get; set; }
        public decimal ReducaoCBS { get; set; }
        public decimal peso { get; set; }
        public string csts { get; set; }
        public string taxaDocumentacao { get; set; }
        public decimal ValorFreteNegociado { get; set; }
        public decimal ValorTotalMoeda { get; set; }
        public decimal ValorCotacaoMoeda { get; set; }
        public decimal ValorTotalMoedaPagar { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral Moeda { get; set; }
        public bool PossuiIntegracao { get; set; }
        public string MensagemProblemaIntegracaoGrMotoristaVeiculo { get; set; }
        public bool PermitirLiberarComProblemaIntegracaoGrMotoristaVeiculo { get; set; }
        public bool ProblemaIntegracaoGrMotoristaVeiculo { get; set; }
        public bool LicencaInvalida { get; set; }
        public bool PermiteLiberarComLicencaInvalida { get; set; }
        public bool LiberarComLicencaInvalida { get; set; }
        public string MensagemLicencaInvalida { get; set; }
        public string Placas { get; set; }
        public int CEPDestinoDiasUteis { get; set; }
        public string DescricaoBonificacaoTransportador { get; set; }
        public decimal PercentualBonificacaoTransportador { get; set; }
        public string CustoFrete { get; set; }
        public string DataVigenciaTabelaFrete { get; set; }
        public bool PermiteAlterarValorFretePedidoPosCalculoFrete { get; set; }
        public bool NaoIncluirValorICMSBasecalculoQuandoValorFreteInformadoOperador { get; set; }
        public bool ExigirConferenciaManual { get; set; }
        public decimal DiferencaRelacaoValorFrete
        {
            get
            {
                return valorFreteTabelaFrete - valorFreteOperador;
            }
        }
    }
}
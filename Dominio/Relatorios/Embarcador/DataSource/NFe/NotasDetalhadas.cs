using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class NotasDetalhadas
    {
        #region Propriedades

        public Int64 Numero { get; set; }
        public string Serie { get; set; }
        public string DescricaoTipo { get; set; }
        private DateTime DataEmissao { get; set; }
        private DateTime DataEntrada { get; set; }
        public string Pessoa { get; set; }
        public string Cidade { get; set; }
        public string Modelo { get; set; }
        public string NaturezaOperacao { get; set; }
        public string Chave { get; set; }
        public string DescricaoStatus { get; set; }
        public decimal ValorTotal { get; set; }

        public string OperadorLancamentoDocumento { get; set; }
        public string OperadorFinalizaDocumento { get; set; }

        public int CodigoProduto { get; set; }
        public string Produto { get; set; }
        public UnidadeDeMedida UnidadeMedida { get; set; }
        public string CFOP { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorUnitarioLiquido { get; set; }
        public decimal ValorLiquido { get; set; }

        public string CstICMS { get; set; }
        public decimal BaseICMS { get; set; }
        public decimal AliquotaICMS { get; set; }
        public decimal ValorICMS { get; set; }

        public decimal BaseICMSST { get; set; }
        public decimal MVA { get; set; }
        public decimal AliquotaICMSST { get; set; }
        public decimal ValorICMSST { get; set; }

        public string CstPIS { get; set; }
        public decimal BasePIS { get; set; }
        public decimal AliquotaPIS { get; set; }
        public decimal ValorPIS { get; set; }

        public string CstCOFINS { get; set; }
        public decimal BaseCOFINS { get; set; }
        public decimal AliquotaCOFINS { get; set; }
        public decimal ValorCOFINS { get; set; }

        public string CstIPI { get; set; }
        public decimal BaseIPI { get; set; }
        public decimal AliquotaIPI { get; set; }
        public decimal ValorIPI { get; set; }

        public decimal RetencaoPIS { get; set; }
        public decimal RetencaoCOFNIS { get; set; }
        public decimal RetencaoINSS { get; set; }
        public decimal RetencaoIPI { get; set; }
        public decimal RetencaoCSLL { get; set; }
        public decimal RetencaoOUTRAS { get; set; }
        public decimal RetencaoIR { get; set; }
        public decimal RetencaoISS { get; set; }

        public string Veiculo { get; set; }
        public int CodigoVeiculo { get; set; }
        public string Empresa { get; set; }
        public int CodigoEmpresa { get; set; }
        public string SituacaoFinanceiraNota { get; set; }
        public string DataVencimento { get; set; }
        public string DataPagamento { get; set; }
        public string EstadoPessoa { get; set; }
        private double CPFCNPJPessoa { get; set; }
        public string TipoVeiculo { get; set; }
        public string TipoCliente { get; set; }

        public decimal BaseSTRetido { get; set; }
        public decimal ValorSTRetido { get; set; }
        public int CodigoNota { get; set; }
        public string GrupoProduto { get; set; }
        public string Segmento { get; set; }
        public decimal ValorImpostosFora { get; set; }
        public string TipoMovimento { get; set; }
        public string Equipamento { get; set; }
        public int KmAbastecimento { get; set; }
        public int Horimetro { get; set; }
        public decimal Desconto { get; set; }
        public RegimeTributario RegimeTributario { get; set; }
        public string UnidadeMedidaFornecedor { get; set; }
        public decimal QuantidadeFornecedor { get; set; }
        public decimal ValorUnitarioFornecedor { get; set; }
        public string ProdutoCodigoProduto { get; set; }
        public string Servico { get; set; }
        public string LocalidadePrestacaoServico { get; set; }
        private TipoDocumentoServico TipoDocumento { get; set; }
        private CSTServico CSTServico { get; set; }
        public decimal AliquotaSimplesNacional { get; set; }
        private bool DocumentoFiscalProvenienteSimplesNacional { get; set; }
        private bool TributaISSNoMunicipio { get; set; }
        public decimal ValorAbastecimentoTabelaFornecedor { get; set; }
        private bool ValorAbastecimentoComDivergencia { get; set; }
        public string LocalArmazenamento { get; set; }
        public string DescricaoCFOP { get; set; }
        public string DataFinalizacao { get; set; }
        public decimal OutrasDespesas { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorSeguro { get; set; }
        public decimal ValorDiferencial { get; set; }
        private DateTime DataAbastecimento { get; set; }
        private string CNPJFilial { get; set; }
        public int OrdemCompra { get; set; }
        public int OrdemServico { get; set; }
        public decimal CustoUnitario { get; set; }
        public decimal CustoTotal { get; set; }
        public string CstIcmsFornecedor { get; set; }
        public string CfopFornecedor { get; set; }
        public decimal BaseCalculoICMSFornecedor { get; set; }
        public decimal ValorICMSFornecedor { get; set; }
        public decimal AliquotaICMSFornecedor { get; set; }
        #endregion

        #region Propriedades com Regras

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataEntradaFormatada
        {
            get { return DataEntrada != DateTime.MinValue ? DataEntrada.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string CPFCNPJFormatado
        {
            get
            {
                if (TipoCliente == "J")
                    return string.Format(@"{0:00\.000\.000\/0000\-00}", CPFCNPJPessoa);
                else if (TipoCliente == "F")
                    return string.Format(@"{0:000\.000\.000\-00}", CPFCNPJPessoa);
                else
                    return string.Empty;
            }
        }
        public string CNPJFilialFormatado
        {
            get { return !string.IsNullOrEmpty(CNPJFilial) ? string.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(CNPJFilial)) : string.Empty; }
        }

        public string UnidadeMedidaFormatada
        {
            get { return UnidadeMedida.ObterSigla(); }
        }

        public string RegimeTributarioFormatada
        {
            get { return RegimeTributario.ObterDescricao(); }
        }

        public string TipoDocumentoFormatada
        {
            get { return TipoDocumento.ObterDescricao(); }
        }

        public string CSTServicoFormatada
        {
            get { return CSTServico.ObterDescricao(); }
        }

        public string DocumentoFiscalProvenienteSimplesNacionalFormatado
        {
            get { return DocumentoFiscalProvenienteSimplesNacional ? "Sim" : "Não"; }
        }

        public string TributaISSNoMunicipioFormatado
        {
            get { return TributaISSNoMunicipio ? "Sim" : "Não"; }
        }

        public string ValorAbastecimentoComDivergenciaFormatdo
        {
            get { return ValorAbastecimentoComDivergencia ? "Sim" : "Não"; }
        }

        public string DataAbastecimentoFormatada
        {
            get { return DataAbastecimento != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }
        #endregion
    }
}

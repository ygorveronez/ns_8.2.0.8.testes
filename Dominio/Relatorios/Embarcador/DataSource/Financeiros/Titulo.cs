using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class Titulo
    {
        public int Codigo { get; set; }
        public int Fatura { get; set; }
        public int Bordero { get; set; }
        public string Filial { get; set; }
        public string PortadorConta { get; set; }
        public double CNPJCPF { get; set; }
        public double CPFCNPJPessoa { get; set; }
        public string TipoPessoa { get; set; }
        public string NomePessoa { get; set; }
        public string TipoTitulo { get; set; }
        public string StatusTitulo { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public string DataPagamento { get; set; }
        public string DataAutorizacao { get; set; }
        public string DataCancelamento { get; set; }
        public string DataBaseLiquidacao { get; set; }
        public string DataLancamentoNota { get; set; }
        public string DataEmissaoDocumentos { get; set; }
        public decimal ValorDesonto { get; set; }
        public decimal ValorAcrescimo { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Parcela { get; set; }
        public string ObservacaoInterna { get; set; }
        public string Observacao { get; set; }
        public string ObservacaoFatura { get; set; }
        public decimal ValorSaldo { get; set; }
        public decimal ValorPago { get; set; }
        public decimal ValorTitulo { get; set; }
        public decimal ValorPendente { get; set; }
        public string ModeloDocumento { get; set; }
        public string TipoDocumentoTituloOriginal { get; set; }
        public string NumeroDocumentoTituloOriginal { get; set; }
        public string NumeroBoleto { get; set; }
        public string GrupoPessoa { get; set; }
        public string Conhecimentos { get; set; }
        public decimal AcrescimoGeracao { get; set; }
        public decimal AcrescimoBaixa { get; set; }
        public decimal DescontoGeracao { get; set; }
        public int OrdemCompra { get; set; }
        public int OrdemServico { get; set; }
        public decimal DescontoBaixa { get; set; }
        public string BancoCliente { get; set; }
        public string TipoContaCliente { get; set; }
        public string AgenciaCliente { get; set; }
        public string DigitoAgenciaCliente { get; set; }
        public string NumeroContaCliente { get; set; }
        public string Adiantado { get; set; }
        public string Portador { get; set; }
        public string NaturezaOperacao { get; set; }
        public string CNPJEmpresa { get; set; }
        public string Empresa { get; set; }
        public string TipoOperacao { get; set; }
        public string TipoPagamentoRecebimento { get; set; }
        public string EmpresaConhecimentos { get; set; }
        public string SeriesConhecimentos { get; set; }
        public string Cargas { get; set; }
        public string TipoMovimento { get; set; }
        public FormaTitulo FormaTitulo { get; set; }
        public string Provisao { get; set; }
        public string Remessa { get; set; }
        public string NumerosCheques { get; set; }

        public string DataProgramacaoPagamento { get; set; }

        public decimal ValorMoedaCotacao { get; set; }
        public decimal ValorOriginalMoedaEstrangeira { get; set; }
        public string DataBaseCRT { get; set; }
        public string MoedaCotacaoBancoCentral { get; set; }

        public string CodigoRemessa { get; set; }
        public string ArquivoRemessa { get; set; }
        public string Categoria { get; set; }
        public decimal VariacaoCambial { get; set; }

        public string ChavePix { get; set; }
        public string ContaFornecedorEBS { get; set; }
        public string Usuario { get; set; }
        public DateTime DataLancamento { get; set; }
        private TipoChavePix TipoChavePix { get; set; }
        public decimal ValorCapitalContratoFinanciamento { get; set; }
        public decimal ValorAcrescimoContratoFinanciamento { get; set; }
        public decimal AcrescimoCalculado { get; set; }
        public decimal AcrescimoNaBaixa { get; set; }
        public decimal AcrescimoLancamentoTitulo { get; set; }
        public decimal DescontoLancamentoTitulo { get; set; }
        public string ComandoBanco { get; set; }
        public string Veiculo { get; set; }
        public string TipoProposta { get; set; }
        public string NumeroProposta { get; set; }
        public string NumeroControleCTe { get; set; }
        public string NumeroBookingCTe { get; set; }
        public string NavioViagemDirecao { get; set; }
        public string Renegociado { get; set; }
        public string AprovadorOrdemCompra { get; set; }
        public string TipoContato { get; set; }
        public string SituacaoContato { get; set; }

        #region Propriedades com Regras

        public string CNPJEmpresaFormatado
        {
            get
            {
                if (CNPJEmpresa != null && CNPJEmpresa != "")
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", long.Parse(this.CNPJEmpresa));
                else
                    return "";
            }
        }

        public string CPFCNPJPessoaFormatado
        {
            get
            {
                if (TipoPessoa == "E")
                    return "00.000.000/0000-00";
                else
                    return TipoPessoa == "J" ? string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJCPF) : string.Format(@"{0:000\.000\.000\-00}", CNPJCPF);
            }
        }

        public string DescricaoFormaTitulo
        {
            get { return FormaTitulo.ObterDescricao(); }
        }

        public string TipoChavePixFormatada
        {
            get { return TipoChavePix.ObterDescricao(); }
        }

        public string DataVencimentoFormatada
        {
            get { return DataVencimento != DateTime.MinValue ? DataVencimento.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataEmissaoFormatada
        {
            get { return this.DataEmissao != DateTime.MinValue ? this.DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        #endregion
    }
}

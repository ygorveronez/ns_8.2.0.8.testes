using System;

namespace Dominio.Relatorios.Embarcador.DataSource.NFe
{
    public class Notas
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
        public string CFOP { get; set; }
        public decimal ValorICMS { get; set; }
        public decimal ValorICMSST { get; set; }
        public decimal ValorPIS { get; set; }
        public decimal ValorCOFINS { get; set; }
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
        public double CPFCNPJPessoa { get; set; }
        public string TipoVeiculo { get; set; }
        public string TipoCliente { get; set; }
        public decimal BaseSTRetido { get; set; }
        public decimal ValorSTRetido { get; set; }
        public int CodigoNota { get; set; }
        public string Segmento { get; set; }
        public decimal ValorImpostosFora { get; set; }
        public string Equipamento { get; set; }
        public string CentroResultado { get; set; }
        public string OperadorLancamentoDocumento { get; set; }
        public string OperadorFinalizouDocumento { get; set; }
        private string CNPJFilial { get; set; }
        public string Observacao { get; set; }
        public int OrdemCompra { get; set; }
        public int OrdemServico { get; set; }
        public string DataFinalizacao { get; set; }
        public string DocFinalizadoAutomaticamente { get; set; }
        public string MotivoCancelamento { get; set; }
        public decimal TotalLitrosAbastecimento { get; set; }
        public decimal ValorTotalAbastecimento { get; set; }
        public string CategoriaPessoa { get; set; }
        public string StatusLancamento { get; set; }


        #endregion

        #region Propriedades com Regras

        public string CPFCNPJFormatado
        {
            get
            {
                if (TipoCliente == "J")
                    return String.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJPessoa);
                else if (TipoCliente == "F")
                    return String.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJPessoa);
                else
                    return "";
            }
        }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }

        public string DataEntradaFormatada
        {
            get { return DataEntrada != DateTime.MinValue ? DataEntrada.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string CNPJFilialFormatado
        {
            get { return this.CNPJFilial.Length == 14 ? this.CNPJFilial.ToString().ObterCpfOuCnpjFormatado(this.TipoCliente) : ""; }
        }

        #endregion
    }
}

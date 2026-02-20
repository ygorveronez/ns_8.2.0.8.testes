using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.Relatorios.Embarcador.DataSource.Financeiros
{
    public class PosicaoContasPagar
    {
        public int Codigo { get; set; }
        public string TipoFornecedor { get; set; }
        public string Filial { get; set; }        
        public double CPFCNPJFornecedor { get; set; }
        public string Fornecedor { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoTitulo Tipo { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime DataVencimento { get; set; }
        public DateTime DataBaseBaixa { get; set; }
        public DateTime DataPagamento { get; set; }
        public string TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Parcela { get; set; }
        public decimal ValorPendente { get; set; }
        public decimal ValorTitulo { get; set; }
        public decimal ValorAcrescimo { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorSaldo { get; set; }
        public decimal ValorPago { get; set; }
        public string CategoriaFornecedor { get; set; }
        public string ContaContabilFornecedor { get; set; }
        public string TipoFormatada
        {
            get { return this.Tipo.ObterDescricao(); }
        }

        public string CPFCNPJFornecedorFormatada
        {
            get
            {
                if (this.Tipo.Equals("E"))
                {
                    return "00.000.000/0000-00";
                }
                else
                {
                    return this.Tipo.Equals("J") ? String.Format(@"{0:00\.000\.000\/0000\-00}", this.CPFCNPJFornecedor) : String.Format(@"{0:000\.000\.000\-00}", this.CPFCNPJFornecedor);
                }
            }
        }

        public string DataEmissaoFormatada
        {
            get { return DataEmissao != DateTime.MinValue ? DataEmissao.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string DataVencimentoFormatada
        {
            get { return DataVencimento != DateTime.MinValue ? DataVencimento.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string DataBaseFormatada
        {
            get { return DataBaseBaixa != DateTime.MinValue ? DataBaseBaixa.ToString("dd/MM/yyyy") : string.Empty; }
        }
        public string DataPagamentoFormatada
        {
            get { return DataPagamento != DateTime.MinValue ? DataPagamento.ToString("dd/MM/yyyy") : string.Empty; }
        }
    }
}

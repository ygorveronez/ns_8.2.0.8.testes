using System;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaConciliacaoBancaria
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public SituacaoConciliacaoBancaria SituacaoConciliacaoBancaria { get; set; }
        public int CodigoPlanoConta { get; set; }
        public int CodigoEmpresa { get; set; }
        public int CodigoOperador { get; set; }
        public decimal ValorExtratoInicial { get; set; }
        public decimal ValorExtratoFinal { get; set; }
        public decimal ValorMovimentoInicial { get; set; }
        public decimal ValorMovimentoFinal { get; set; }
        public DateTime DataGeracaoMovimentoFinanceiro { get; set; }
        public string NumeroDocumentoMovimentoFinanceiro { get; set; }
        public decimal ValorMovimentoFinanceiroInicial { get; set; }
        public decimal ValorMovimentoFinanceiroFinal { get; set; }
        public int CodigoTitulo { get; set; }
    }
}

using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaMovimentoFinanceiro
    {
        public int CodigoMovimento { get; set; }
        public DateTime DataBase { get; set; }
        public int CodigoGrupoPessoa { get; set; }
        public double CnpjPessoa { get; set; }
        public int CodigoEmpresa { get; set; }
        public DateTime DataMovimentoInicial { get; set; }
        public DateTime DataMovimentoFinal { get; set; }
        public decimal ValorMovimento { get; set; }
        public string NumeroDocumento { get; set; }
        public string Observacao { get; set; }
        public int CodigoTipoMovimento { get; set; }
        public int CodigoCentroResultado { get; set; }
        public int CodigoPlanoDebito { get; set; }
        public int CodigoPlanoCredito { get; set; }
        public Dominio.Enumeradores.TipoAmbiente TipoAmbiente { get; set; }
        public TipoConsolidacaoMovimentoFinanceiro? SituacaoMovimento { get; set; }
        public TipoDocumentoMovimento TipoDocumento { get; set; }
        public MoedaCotacaoBancoCentral MoedaCotacaoBancoCentral { get; set; }
        public bool VisualizarTitulosPagamentoSalario { get; set; }
    }
}

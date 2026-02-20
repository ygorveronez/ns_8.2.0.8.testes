using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class FiltroPesquisaConciliacaoBancariaMovimento
    {
        public int CodigoConciliacaoBancaria { get; set; }
        public DateTime DataPesquisaMovimento { get; set; }
        public DateTime DataAtePesquisaMovimento { get; set; }
        public decimal ValorPesquisaMovimento { get; set; }
        public decimal ValorAtePesquisaMovimento { get; set; }
        public string NumeroDocumentoPesquisaMovimento { get; set; }
        public string NumeroChequePesquisaMovimento { get; set; }
        public double CnpjPessoa { get; set; }
        public int CodigoGrupoPessoa { get; set; }
        public Enumeradores.DebitoCredito DebitoCreditoMovimento { get; set; }
        public string ObservacaoMovimento { get; set; }
        public int CodigoTitulo { get; set; }
    }
}
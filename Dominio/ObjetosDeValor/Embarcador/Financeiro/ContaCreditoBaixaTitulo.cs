using System;

namespace Dominio.ObjetosDeValor.Embarcador.Financeiro
{
    public class ContaCreditoBaixaTitulo
    {
        public int CodigoConta { get; set; }
        public decimal Valor { get; set; }
        public decimal ValorOriginal { get; set; }
        public Dominio.Entidades.Embarcador.Financeiro.TipoPagamentoRecebimento TipoPagamentoRecebimento { get; set; }

        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.MoedaCotacaoBancoCentral? MoedaCotacaoBancoCentral { get; set; }
        public DateTime? DataBaseCRT { get; set; }
        public decimal ValorMoedaCotacao { get; set; }
        public decimal ValorOriginalMoedaEstrangeira { get; set; }
    }
}

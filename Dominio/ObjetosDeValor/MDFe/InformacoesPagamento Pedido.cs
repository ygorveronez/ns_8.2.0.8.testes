using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;

namespace Dominio.ObjetosDeValor.MDFe
{
    public class InformacoesPagamentoPedido
    {
        public FormasPagamento? IndicadorPagamento { get; set; }
        public bool IndicadorAltoDesempenho { get; set; }
        public decimal ValorAdiantamento { get; set; }
        public decimal ValorFrete { get; set; }
        public TipoPagamentoMDFe TipoInformacaoBancaria { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string ChavePix { get; set; }
        public string Ipef { get; set; }
        public DateTime? DataVencimentoCIOT { get; set; }
    }
}

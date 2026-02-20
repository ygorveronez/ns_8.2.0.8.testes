using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Enumerador;
using System.Collections.Generic;

namespace Dominio.ObjetosDeValor.MDFe
{
    public class InformacoesPagamento
    {
        public List<ComponentePagamento> ComponentesPagamento { get; set; }
        public FormasPagamento? IndicadorPagamento { get; set; }
        public bool IndicadorAltoDesempenho { get; set; }
        public decimal ValorAdiantamento { get; set; }
        public List<ParcelaPagamento> ParcelasPagamento { get; set; }
        public TipoPagamentoMDFe TipoInformacaoBancaria { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string ChavePix { get; set; }
        public string Ipef { get; set; }
    }
}

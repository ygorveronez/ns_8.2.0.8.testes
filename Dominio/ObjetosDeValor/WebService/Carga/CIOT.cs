using System;

namespace Dominio.ObjetosDeValor.WebService.Carga
{
    public class CIOT
    {
        public int ProtocoloCarga { get; set; }
        public string ChaveCTE { get; set; }
        public string NumeroCIOT { get; set; }
        public DateTime? DataFinalizacao { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.FormasPagamento FormaPagamento { get; set; }
        public DateTime DataAbertura { get; set; }
        public string CNPJTerceiro { get; set; }
        public string ChavePIX { get; set; }
        public string Agencia { get; set; }
        public string Banco { get; set; }
        public string CNPJInstituicaoPagamentoCIOT { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoPagamentoMDFe TipoPagamentoCIOT { get; set; }
        public decimal ValorAdiantamento { get; set; }
        public decimal ValorFrete { get; set; }
        public DateTime DataPagamento { get; set; }
    }
}

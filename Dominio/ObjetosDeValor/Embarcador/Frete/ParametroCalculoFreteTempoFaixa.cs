using System;

namespace Dominio.ObjetosDeValor.Embarcador.Frete
{
    public class ParametroCalculoFreteTempoFaixa
    {
        public TimeSpan HoraInicial { get; set; }
        public TimeSpan HoraFinal { get; set; }
        public bool PeriodoInicial { get; set; }
        public TimeSpan? HoraInicialCobrancaMinima { get; set; }
        public TimeSpan? HoraFinalCobrancaMinima { get; set; }
        public Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoCampoValorTabelaFrete TipoCampoValorTabelaFrete { get; set; }
        public decimal Valor { get; set; }
    }
}

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PontoPlanejamentoTransporte
    {
        BR01 = 1,
        BR02 = 2,
        BR04 = 3,
    }

    public static class PontoPlanejamentoTransporterHelper
    {
        public static string ObterDescricao(this PontoPlanejamentoTransporte pontoPlanejamentoTransporte)
        {
            switch (pontoPlanejamentoTransporte)
            {
                case PontoPlanejamentoTransporte.BR01: return "BR01 - Unilever Brasil";
                case PontoPlanejamentoTransporte.BR02: return "BR02 - Unilever Brasil Industrial Ltda";
                case PontoPlanejamentoTransporte.BR04: return "BR04 – Com.Alim.ICE VA";
                default: return string.Empty;
            }
        }
    }
}

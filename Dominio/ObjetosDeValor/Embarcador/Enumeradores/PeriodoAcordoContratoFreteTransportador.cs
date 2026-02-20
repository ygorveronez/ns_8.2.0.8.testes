namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PeriodoAcordoContratoFreteTransportador
    {
        Semanal = 7,
        Decendial = 10,
        Quinzenal = 15,
        Mensal = 30,
        NaoPossui = 100
    }

    public static class PeriodoAcordoContratoFreteTransportadorHelper
    {
        public static string ObterDescricao(this PeriodoAcordoContratoFreteTransportador periodoAcordo)
        {
            switch (periodoAcordo)
            {
                case PeriodoAcordoContratoFreteTransportador.Semanal: return "Semanal";
                case PeriodoAcordoContratoFreteTransportador.Mensal: return "Mensal";
                case PeriodoAcordoContratoFreteTransportador.Quinzenal: return "Quinzenal";
                case PeriodoAcordoContratoFreteTransportador.Decendial: return "Decendial";
                default: return string.Empty;
            }
        }

        public static string ObterDescricaoLBC(this PeriodoAcordoContratoFreteTransportador periodoAcordo)
        {
            switch (periodoAcordo)
            {
                case PeriodoAcordoContratoFreteTransportador.Semanal: return "Semanalmente";
                case PeriodoAcordoContratoFreteTransportador.Mensal: return "Mensalmente";
                case PeriodoAcordoContratoFreteTransportador.Quinzenal: return "Quinzenal";
                default: return string.Empty;
            }
        }

        public static int ObterQuantidade(this PeriodoAcordoContratoFreteTransportador periodoAcordo)
        {
            switch (periodoAcordo)
            {
                case PeriodoAcordoContratoFreteTransportador.Mensal: return 1;
                case PeriodoAcordoContratoFreteTransportador.Quinzenal: return 2;
                case PeriodoAcordoContratoFreteTransportador.Decendial: return 3;
                case PeriodoAcordoContratoFreteTransportador.Semanal: return 4;
                default: return 0;
            }
        }
    }
}

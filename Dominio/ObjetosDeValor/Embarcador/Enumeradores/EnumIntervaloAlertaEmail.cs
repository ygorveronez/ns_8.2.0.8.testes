namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EnumIntervaloAlertaEmail
    {
        Dia = 1,
        Semana = 2,
        Mes = 3,
        Ano = 4,
    }

    public static class EnumIntervaloAlertaEmailHelper
    {
        public static string ObterDescricao(this EnumIntervaloAlertaEmail periodicidade)
        {
            switch (periodicidade)
            {
                case EnumIntervaloAlertaEmail.Dia: return "Dia";
                case EnumIntervaloAlertaEmail.Semana: return "Semana";
                case EnumIntervaloAlertaEmail.Mes: return "Mes";
                case EnumIntervaloAlertaEmail.Ano: return "Ano";
                default: return string.Empty;
            }
        }
    }
}

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum DiaSemanaMesAno
    {
        Dia = 1,
        Semana = 2,
        Mes = 3,
        Ano = 4
    }

    public static class DiaSemanaMesAnoHelper
    {
        public static string ObterDescricao(this DiaSemanaMesAno diaSemanaMesAno)
        {
            switch (diaSemanaMesAno)
            {
                case DiaSemanaMesAno.Dia: return Localization.Resources.Gerais.Geral.Dia;
                case DiaSemanaMesAno.Semana: return Localization.Resources.Gerais.Geral.Semana;
                case DiaSemanaMesAno.Mes: return Localization.Resources.Gerais.Geral.Mes;
                case DiaSemanaMesAno.Ano: return Localization.Resources.Gerais.Geral.Ano;
                default: return string.Empty;
            }
        }

        public static int RetornarQuantidadeDias(this DiaSemanaMesAno diaSemanaMesAno, int tempo)
        {
            switch (diaSemanaMesAno)
            {
                case DiaSemanaMesAno.Dia: return tempo;
                case DiaSemanaMesAno.Semana: return tempo * 7;
                case DiaSemanaMesAno.Mes: return tempo * 30;
                case DiaSemanaMesAno.Ano: return tempo * 365;
                default: return 0;
            }
        }
    }
}

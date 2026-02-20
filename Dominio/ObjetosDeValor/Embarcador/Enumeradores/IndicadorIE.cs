namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum IndicadorIE
    {
        ContribuinteICMS = 1,
        ContribuinteIsento = 2,
        NaoContribuinte = 9
    }

    public static class IndicadorIEHelper
    {
        public static string ObterDescricao(this IndicadorIE indicadorIE)
        {
            switch (indicadorIE)
            {
                case IndicadorIE.ContribuinteICMS: return "1 - Contribuinte ICMS (informar a IE do destinatário)";
                case IndicadorIE.ContribuinteIsento: return "2 - Contribuinte isento de Inscrição no cadastro de Contribuintes do ICMS";
                case IndicadorIE.NaoContribuinte: return "9 - Não Contribuinte, que pode ou não possuir Inscrição Estadual no Cadastro de Contribuintes do ICMS";
                default: return string.Empty;
            }
        }

        public static string ObterDescricaoAbreviada(this IndicadorIE? indicadorIE)
        {
            switch (indicadorIE)
            {
                case IndicadorIE.ContribuinteICMS: return "1 - Contribuinte ICMS";
                case IndicadorIE.ContribuinteIsento: return "2 - Contribuinte Isento";
                case IndicadorIE.NaoContribuinte: return "9 - Não Contribuinte";
                default: return string.Empty;
            }
        }
    }
}

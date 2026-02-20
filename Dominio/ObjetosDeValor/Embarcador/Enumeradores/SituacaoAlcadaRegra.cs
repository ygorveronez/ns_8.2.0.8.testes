namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAlcadaRegra
    {
        Pendente = 0,
        Aprovada = 1,   
        Rejeitada = 9
    }

    public static class SituacaoAlcadaRegraHelper
    {
        public static string ObterDescricao(this SituacaoAlcadaRegra situacaoAlcadaRegra)
        {
            switch (situacaoAlcadaRegra)
            {
                case SituacaoAlcadaRegra.Aprovada: return Localization.Resources.Gerais.Geral.Aprovada;
                case SituacaoAlcadaRegra.Rejeitada: return Localization.Resources.Gerais.Geral.Rejeitada;
                default: return Localization.Resources.Gerais.Geral.Pendente;
            }
        }

        public static string ObterCorGrid(this SituacaoAlcadaRegra situacaoAlcadaRegra)
        {
            switch (situacaoAlcadaRegra)
            {
                case SituacaoAlcadaRegra.Aprovada: return CorGrid.Success;
                case SituacaoAlcadaRegra.Rejeitada: return CorGrid.Danger;
                default: return CorGrid.Warning;
            }
        }
    }
}

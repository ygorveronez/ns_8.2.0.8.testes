namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoControleCarregamento
    {
        Aguardando = 1,
        EmCarregamento = 2,
        Finalizado = 3,
        EmDoca = 4
    }

    public static class SituacaoControleCarregamentoHelper
    {
        public static string ObterCorFonte(this SituacaoControleCarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoControleCarregamento.Finalizado: return "#ffffff";
                default: return string.Empty;
            }
        }

        public static string ObterCorLinha(this SituacaoControleCarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoControleCarregamento.Aguardando: return "#85de7b";
                case SituacaoControleCarregamento.EmCarregamento: return "#c8e8ff";
                case SituacaoControleCarregamento.EmDoca: return "#ffe699";
                case SituacaoControleCarregamento.Finalizado: return "#008e83";
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this SituacaoControleCarregamento situacao)
        {
            switch (situacao)
            {
                case SituacaoControleCarregamento.Aguardando: return "Aguardando";
                case SituacaoControleCarregamento.EmCarregamento: return "Em Carregamento";
                case SituacaoControleCarregamento.EmDoca: return "Em Doca";
                case SituacaoControleCarregamento.Finalizado: return "Finalizado";
                default: return string.Empty;
            }
        }
    }
}

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAtivaPesquisa
    {
        Todos = 0,
        Ativa = 1,
        Inativa = 2
    }

    public static class SituacaoAtivaPesquisaHelper
    {
        public static string ObterDescricao(this SituacaoAtivaPesquisa situacao)
        {
            switch (situacao)
            {
                case SituacaoAtivaPesquisa.Ativa: return "Ativa";
                case SituacaoAtivaPesquisa.Inativa: return "Inativa";
                default: return string.Empty;
            }
        }
        public static string ObterDescricao(bool ativa)
        {
            return (ativa) ? SituacaoAtivaPesquisa.Ativa.ObterDescricao() : SituacaoAtivaPesquisa.Inativa.ObterDescricao();
        }
    }
}

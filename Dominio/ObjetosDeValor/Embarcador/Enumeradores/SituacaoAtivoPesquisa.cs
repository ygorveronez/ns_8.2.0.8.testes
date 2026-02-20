namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoAtivoPesquisa
    {
        Todos = 0,
        Ativo = 1,
        Inativo = 2
    }

    public static class SituacaoAtivoPesquisaHelper
    {
        public static string ObterDescricao(this SituacaoAtivoPesquisa situacao)
        {
            switch (situacao)
            {
                case SituacaoAtivoPesquisa.Ativo: return Localization.Resources.Gerais.Geral.Ativo;
                case SituacaoAtivoPesquisa.Inativo: return Localization.Resources.Gerais.Geral.Inativo;
                default: return string.Empty;
            }
        }
    }
}

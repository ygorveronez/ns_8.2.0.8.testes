namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoRoteirizacao
    {
        Todas = -1,
        SemDefinicao = 0,
        Aguardando = 1,
        Concluido = 2,
        Erro = 3,
        EmZonaExclusao = 4
    }

    public static class SituacaoRoteirizacaoHelper
    {
        public static string ObterDescricao(this SituacaoRoteirizacao situacaoRoteirizacao)
        {
            switch (situacaoRoteirizacao)
            {
                case SituacaoRoteirizacao.SemDefinicao: return Localization.Resources.Enumeradores.SituacaoRoteirizacao.SemDefinicao;
                case SituacaoRoteirizacao.Aguardando: return Localization.Resources.Enumeradores.SituacaoRoteirizacao.Aguardando;
                case SituacaoRoteirizacao.Concluido: return Localization.Resources.Enumeradores.SituacaoRoteirizacao.Concluido;
                case SituacaoRoteirizacao.Erro: return Localization.Resources.Enumeradores.SituacaoRoteirizacao.Erro;
                case SituacaoRoteirizacao.EmZonaExclusao: return Localization.Resources.Enumeradores.SituacaoRoteirizacao.EmZonaExclusao;
                default: return string.Empty;
            }
        }
    }
}

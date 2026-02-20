namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoIntegracao
    {
        AgIntegracao = 0,
        Integrado = 1,
        ProblemaIntegracao = 2,
        AgRetorno = 3
    }

    public static class SituacaoIntegracaoHelper
    {
        public static bool IntegracaoPendente(this SituacaoIntegracao situacao)
        {
            return (situacao == SituacaoIntegracao.AgIntegracao) || (situacao == SituacaoIntegracao.AgRetorno);
        }

        public static string ObterCorFonte(this SituacaoIntegracao situacao)
        {
            switch (situacao)
            {
                case SituacaoIntegracao.ProblemaIntegracao: return CorGrid.Branco;
                default: return string.Empty;
            }
        }

        public static string ObterCorLinha(this SituacaoIntegracao situacao)
        {
            switch (situacao)
            {
                case SituacaoIntegracao.AgRetorno: return CorGrid.Amarelo;
                case SituacaoIntegracao.AgIntegracao: return CorGrid.Azul;
                case SituacaoIntegracao.Integrado: return CorGrid.Verde;
                case SituacaoIntegracao.ProblemaIntegracao: return CorGrid.Vermelho;
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this SituacaoIntegracao situacao)
        {
            switch (situacao)
            {
                case SituacaoIntegracao.AgIntegracao: return Localization.Resources.Enumeradores.SituacaoIntegracao.AguardandoIntegracao;
                case SituacaoIntegracao.AgRetorno: return Localization.Resources.Enumeradores.SituacaoIntegracao.AguardandoRetorno;
                case SituacaoIntegracao.Integrado: return Localization.Resources.Enumeradores.SituacaoIntegracao.Integrado;
                case SituacaoIntegracao.ProblemaIntegracao: return Localization.Resources.Enumeradores.SituacaoIntegracao.FalhaAoIntegrar;
                default: return string.Empty;
            }
        }
    }
}

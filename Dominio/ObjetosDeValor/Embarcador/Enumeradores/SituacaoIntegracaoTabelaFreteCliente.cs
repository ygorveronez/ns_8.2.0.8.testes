namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoIntegracaoTabelaFreteCliente
    {
        SemIntegracao = 0,
        AguardandoIntegracao = 1,
        FalhaIntegracao = 2,
        Integrado = 3,
        AguardandoRetorno = 4
    }

    public static class SituacaoIntegracaoTabelaFreteClienteHelper
    {
        public static string ObterDescricao(this SituacaoIntegracaoTabelaFreteCliente situacao)
        {
            switch (situacao)
            {
                case SituacaoIntegracaoTabelaFreteCliente.AguardandoIntegracao: return Localization.Resources.Enumeradores.SituacaoIntegracaoTabelaFreteCliente.AguardandoIntegracao;
                case SituacaoIntegracaoTabelaFreteCliente.FalhaIntegracao: return Localization.Resources.Enumeradores.SituacaoIntegracaoTabelaFreteCliente.FalhaIntegracao;
                case SituacaoIntegracaoTabelaFreteCliente.Integrado: return Localization.Resources.Enumeradores.SituacaoIntegracaoTabelaFreteCliente.Integrado;
                case SituacaoIntegracaoTabelaFreteCliente.AguardandoRetorno: return Localization.Resources.Enumeradores.SituacaoIntegracaoTabelaFreteCliente.AguardandoRetorno;
                default: return string.Empty;
            }
        }
    }
}

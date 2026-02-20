namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoTabelaFreteIntegrarAlteracao
    {
        PendenteIntegracao = 0,
        IntegracaoIniciada = 1,
        FalhaIntegracao = 2,
        IntegracaoFinalizada = 3
    }

    public static class SituacaoTabelaFreteIntegrarAlteracaoHelper
    {
        public static string ObterCorFonte(this SituacaoTabelaFreteIntegrarAlteracao situacao)
        {
            switch (situacao)
            {
                case SituacaoTabelaFreteIntegrarAlteracao.FalhaIntegracao: return CorGrid.Branco;
                default: return string.Empty;
            }
        }

        public static string ObterCorLinha(this SituacaoTabelaFreteIntegrarAlteracao situacao)
        {
            switch (situacao)
            {
                case SituacaoTabelaFreteIntegrarAlteracao.PendenteIntegracao: return CorGrid.Azul;
                case SituacaoTabelaFreteIntegrarAlteracao.IntegracaoIniciada: return CorGrid.Amarelo;
                case SituacaoTabelaFreteIntegrarAlteracao.FalhaIntegracao: return CorGrid.Vermelho;
                case SituacaoTabelaFreteIntegrarAlteracao.IntegracaoFinalizada: return CorGrid.Verde;
                default: return string.Empty;
            }
        }

        public static string ObterDescricao(this SituacaoTabelaFreteIntegrarAlteracao situacao)
        {
            switch (situacao)
            {
                case SituacaoTabelaFreteIntegrarAlteracao.PendenteIntegracao: return "Pendente de Integração";
                case SituacaoTabelaFreteIntegrarAlteracao.IntegracaoIniciada: return "Integração Iniciada";
                case SituacaoTabelaFreteIntegrarAlteracao.FalhaIntegracao: return "Falha na Integração";
                case SituacaoTabelaFreteIntegrarAlteracao.IntegracaoFinalizada: return "Integracao Finalizada";
                default: return string.Empty;
            }
        }
    }
}

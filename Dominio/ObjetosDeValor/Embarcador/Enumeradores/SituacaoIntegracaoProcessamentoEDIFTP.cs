namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoIntegracaoProcessamentoEDIFTP
    {
        Todas = 0,
        AgIntegracao = 1,
        Integrado = 2,
        Falha = 3,
        EmAndamento = 4
    }

    public static class SituacaoIntegracaoProcessamentoEDIFTPHelper
    {
        public static string ObterDescricao(this SituacaoIntegracaoProcessamentoEDIFTP situacao)
        {
            switch (situacao)
            {
                case SituacaoIntegracaoProcessamentoEDIFTP.AgIntegracao: return "Ag. Integração";
                case SituacaoIntegracaoProcessamentoEDIFTP.Integrado: return "Integrado";
                case SituacaoIntegracaoProcessamentoEDIFTP.Falha: return "Falha";
                case SituacaoIntegracaoProcessamentoEDIFTP.EmAndamento: return "Em Andamento";
                default: return "Todas";
            }
        }
    }
}
namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EventoIntegracaoOcorrenciaTrizy
    {
        AssumirAtendimento = 1,
        SalvarTratativa = 2,
        LiberarOcorrencia = 3,
        Fechar = 4,
    }

    public static class EventoIntegracaoOcorrenciaTrizyHelper
    {
        public static string ObterDescricao(this EventoIntegracaoOcorrenciaTrizy situacao)
        {
            switch (situacao)
            {
                case EventoIntegracaoOcorrenciaTrizy.AssumirAtendimento: return "Assumir Atendimento";
                case EventoIntegracaoOcorrenciaTrizy.SalvarTratativa: return "Salvar Tratativa";
                case EventoIntegracaoOcorrenciaTrizy.LiberarOcorrencia: return "Liberar Ocorrência";
                case EventoIntegracaoOcorrenciaTrizy.Fechar: return "Fechar";
                default: return string.Empty;
            }
        }

        public static string ObterDescricaoEnvioIntegracao(this EventoIntegracaoOcorrenciaTrizy situacao)
        {
            switch (situacao)
            {
                case EventoIntegracaoOcorrenciaTrizy.AssumirAtendimento: return "Ocorrência em análise";
                case EventoIntegracaoOcorrenciaTrizy.SalvarTratativa: return "Tratativa salva";
                case EventoIntegracaoOcorrenciaTrizy.LiberarOcorrencia: return "Ocorrência liberada";
                case EventoIntegracaoOcorrenciaTrizy.Fechar: return "Ocorrência fechada";
                default: return string.Empty;
            }
        }
    }
}

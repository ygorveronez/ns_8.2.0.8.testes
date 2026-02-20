namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoOcorrenciaIntegracaoEmbarcador
    {
        AgConsultaCTes = 1,
        Pendente = 2,
        Problemas = 3,
        AgGeracaoOcorrencia = 4,
        Finalizado = 5,
        AgConsultaCTesCancelados = 6,
        Cancelado = 7,
        AgGeracaoCancelamento = 8,
    }

    public static class SituacaoOcorrenciaIntegracaoEmbarcadorHandle
    {
        public static string ObterDescricao(this SituacaoOcorrenciaIntegracaoEmbarcador situacao)
        {
            switch (situacao)
            {
                case SituacaoOcorrenciaIntegracaoEmbarcador.AgConsultaCTes: return "Ag. Consulta dos CT-es";
                case SituacaoOcorrenciaIntegracaoEmbarcador.Pendente: return "Pendente";
                case SituacaoOcorrenciaIntegracaoEmbarcador.Problemas: return "Falha";
                case SituacaoOcorrenciaIntegracaoEmbarcador.AgGeracaoOcorrencia : return "Ag. Geração Ocorrência";
                case SituacaoOcorrenciaIntegracaoEmbarcador.Finalizado: return "Finalizado";
                case SituacaoOcorrenciaIntegracaoEmbarcador.AgConsultaCTesCancelados: return "Ag. Consulta CT-es Cancelados";
                case SituacaoOcorrenciaIntegracaoEmbarcador.Cancelado: return "Cancelado";  
                case SituacaoOcorrenciaIntegracaoEmbarcador.AgGeracaoCancelamento: return "Ag. Geração do Cancelamento";
                default: return "";
            }
        }
    }
}

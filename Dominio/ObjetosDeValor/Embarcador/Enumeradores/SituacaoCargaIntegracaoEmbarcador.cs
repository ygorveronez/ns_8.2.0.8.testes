namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCargaIntegracaoEmbarcador
    {
        AgConsultaCTes = 1,
        AgConsultaMDFes = 2,
        AgGeracaoCarga = 3,
        Pendente = 4,
        Problemas = 5,
        Finalizado = 6,
        AgConsultaCTesCancelados = 7,
        AgConsultaMDFesCancelados = 8,
        AgGeracaoCancelamento = 9,
        Cancelado = 10,
        AjustadoManualmente = 11,
    }

    public static class SituacaoCargaIntegracaoEmbarcadorHandle
    {
        public static string ObterDescricao(this SituacaoCargaIntegracaoEmbarcador situacao)
        {
            switch (situacao)
            {
                case SituacaoCargaIntegracaoEmbarcador.AgConsultaCTes: return "Ag. Consulta dos CT-es";
                case SituacaoCargaIntegracaoEmbarcador.AgConsultaMDFes: return "Ag. Consulta dos MDF-es";
                case SituacaoCargaIntegracaoEmbarcador.AgGeracaoCarga: return "Ag. Geração da Carga";
                case SituacaoCargaIntegracaoEmbarcador.Pendente: return "Pendente";
                case SituacaoCargaIntegracaoEmbarcador.Problemas: return "Falha";
                case SituacaoCargaIntegracaoEmbarcador.Finalizado: return "Finalizado";
                case SituacaoCargaIntegracaoEmbarcador.AgConsultaCTesCancelados: return "Ag. Consulta CT-es Cancelados";
                case SituacaoCargaIntegracaoEmbarcador.AgConsultaMDFesCancelados: return "Ag. Consulta MDF-es Cancelados";
                case SituacaoCargaIntegracaoEmbarcador.AgGeracaoCancelamento: return "Ag. Geração do Cancelamento";
                case SituacaoCargaIntegracaoEmbarcador.Cancelado: return "Cancelado";
                case SituacaoCargaIntegracaoEmbarcador.AjustadoManualmente: return "Ajustado manualmente";
                default: return "";
            }
        }
    }
}

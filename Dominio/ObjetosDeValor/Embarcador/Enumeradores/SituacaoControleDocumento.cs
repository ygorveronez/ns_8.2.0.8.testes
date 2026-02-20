namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoControleDocumento
    {
        AguardandoAprovacao = 1,
        RejeitadoPeloTransportador = 2,
        ParqueadoManualmente = 3,
        Inconsistente = 4,
        Desparqueado = 5,
        ParqueadoAutomaticamente = 6,
        Liberado = 7,
        AguardandoValidacao = 8,
        InconsistenteSemTratativa = 9,
        InconsistenteFalhaAoProcessar = 10
    }

    public static class SituacaoControleDocumentoHelper
    {
        public static bool IsAprovado(this SituacaoControleDocumento situacao)
        {
            return (situacao == SituacaoControleDocumento.ParqueadoManualmente);
        }

        public static string ObterDescricao(this SituacaoControleDocumento situacao)
        {
            switch (situacao)
            {
                case SituacaoControleDocumento.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoControleDocumento.RejeitadoPeloTransportador: return "Rejeitado Pelo Transportador";
                case SituacaoControleDocumento.ParqueadoManualmente: return "Parqueado Manualmente";
                case SituacaoControleDocumento.Inconsistente: return "Inconsistente";
                case SituacaoControleDocumento.Desparqueado: return "Desparqueado";
                case SituacaoControleDocumento.ParqueadoAutomaticamente: return "Parqueado Automaticamente";
                case SituacaoControleDocumento.Liberado: return "Liberado";
                case SituacaoControleDocumento.AguardandoValidacao: return "Aguardando Validação";
                case SituacaoControleDocumento.InconsistenteSemTratativa: return "Inconsistente Sem Tratativa";
                case SituacaoControleDocumento.InconsistenteFalhaAoProcessar: return "Inconsistente Falha ao Processar";
                default: return string.Empty;
            }
        }
    }
}

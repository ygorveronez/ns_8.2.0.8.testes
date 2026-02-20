namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoChamadoTMS
    {
        Todos = 0,
        Aberto = 1,
        EmAnalise = 2,
        AguardandoAutorizacao = 3,
        LiberadaOcorrencia = 4,
        PagamentoNaoAutorizado = 5,
        Finalizado = 6,
        Cancelado = 7
    }

    public static class SituacaoChamadoTMSHelper
    {
        public static string ObterDescricao(this SituacaoChamadoTMS situacao)
        {
            switch (situacao)
            {
                case SituacaoChamadoTMS.Aberto: return "Aberto";
                case SituacaoChamadoTMS.EmAnalise: return "Em Análise";
                case SituacaoChamadoTMS.AguardandoAutorizacao: return "Aguardando Autorização";
                case SituacaoChamadoTMS.LiberadaOcorrencia: return "Liberado para Ocorrência";
                case SituacaoChamadoTMS.PagamentoNaoAutorizado: return "Pagamento não Autorizado";
                case SituacaoChamadoTMS.Finalizado: return "Finalizado";
                case SituacaoChamadoTMS.Cancelado: return "Cancelado";
                default: return string.Empty;
            }
        }
    }
}

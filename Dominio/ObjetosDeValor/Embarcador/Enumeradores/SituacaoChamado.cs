namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoChamado
    {
        Todas = 0,
        Aberto = 1,
        Finalizado = 2,
        SemRegra = 3,
        LiberadaOcorrencia = 4,
        Cancelada = 5,
        LiberadaValePallet = 6,
        EmTratativa = 7,
        RecusadoPeloCliente = 8,
        AgIntegracao = 9,
        FalhaIntegracao = 10,
        AgGeracaoLote = 11,
        AgAprovacaoLote = 12
    }

    public static class SituacaoChamadoHelper
    {
        public static string ObterDescricao(this SituacaoChamado situacao)
        {
            switch (situacao)
            {
                case SituacaoChamado.Aberto: return "Aberto";
                case SituacaoChamado.Finalizado: return "Finalizado";
                case SituacaoChamado.LiberadaOcorrencia: return "Liberado para Ocorrência";
                case SituacaoChamado.Cancelada: return "Cancelado";
                case SituacaoChamado.SemRegra: return "Sem Regra";
                case SituacaoChamado.LiberadaValePallet: return "Liberado para Vale Pallet";
                case SituacaoChamado.EmTratativa: return "Em Tratativa";
                case SituacaoChamado.RecusadoPeloCliente: return "Recusado";
                case SituacaoChamado.AgIntegracao: return "Ag. Integração";
                case SituacaoChamado.FalhaIntegracao: return "Falha na Integração";
                case SituacaoChamado.AgGeracaoLote: return "Ag. Geração Lote";
                case SituacaoChamado.AgAprovacaoLote: return "Ag. Aprovação Lote";

                default: return string.Empty;
            }
        }
    }
}

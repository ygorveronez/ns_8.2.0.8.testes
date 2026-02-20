namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoLancamentoNFSManual
    {
        Todas = 0,
        DadosNota = 1,
        AgAprovacao = 2,
        Reprovada = 3,
        AgIntegracao = 4,
        SemRegra = 5,
        AgEmissao = 6,
        EmEmissao = 7,
        FalhaEmissao = 8,
        FalhaIntegracao = 9,
        Finalizada = 10,
        Cancelada = 11,
        Anulada = 12
    }

    public static class SituacaoLancamentoNFSManualHelper
    {
        public static string ObterDescricao(this SituacaoLancamentoNFSManual situacao)
        {
            switch (situacao)
            {
                case SituacaoLancamentoNFSManual.AgAprovacao: return "Ag. Aprovação";
                case SituacaoLancamentoNFSManual.AgEmissao: return "Ag Emissão";
                case SituacaoLancamentoNFSManual.AgIntegracao: return "Ag. Integração";
                case SituacaoLancamentoNFSManual.Anulada: return "Anulada";
                case SituacaoLancamentoNFSManual.Cancelada: return "Cancelada";
                case SituacaoLancamentoNFSManual.DadosNota: return "Dados da Nota";
                case SituacaoLancamentoNFSManual.EmEmissao: return "Em Emissão";
                case SituacaoLancamentoNFSManual.FalhaEmissao: return "Falha na Emissão";
                case SituacaoLancamentoNFSManual.FalhaIntegracao: return "Falha na Integração";
                case SituacaoLancamentoNFSManual.Finalizada: return "Finalizada";
                case SituacaoLancamentoNFSManual.Reprovada: return "Reprovada";
                case SituacaoLancamentoNFSManual.SemRegra: return "Sem Regra de Aprovação";
                default: return string.Empty;
            }
        }
    }
}

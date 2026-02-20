namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoCancelamentoProvisao
    {
        Todos = 0,
        EmCancelamento = 1,
        PendenciaCancelamento = 2,
        AgIntegracao = 3,
        EmIntegracao = 4,
        FalhaIntegracao = 5,
        Cancelado = 6,
        SemRegraAprovacao = 7,
        SolicitacaoReprovada = 8,
        AgAprovacaoSolicitacao = 9,
        SolicitacaoAprovada = 10,
        NaoProcessado = 11,
        Estornado = 12
    }

    public static class SituacaoCancelamentoProvisaoHelper
    {
        public static string ObterDescricao(this SituacaoCancelamentoProvisao situacaoCanhoto)
        {
            switch (situacaoCanhoto)
            {
                case SituacaoCancelamentoProvisao.Todos: return "";
                case SituacaoCancelamentoProvisao.EmCancelamento: return "Em Cancelamento";
                case SituacaoCancelamentoProvisao.PendenciaCancelamento: return "Pendência no Cancelamento";
                case SituacaoCancelamentoProvisao.AgIntegracao: return "Ag. Integração";
                case SituacaoCancelamentoProvisao.EmIntegracao: return "Em Integração";
                case SituacaoCancelamentoProvisao.FalhaIntegracao: return "Falha na Integração";
                case SituacaoCancelamentoProvisao.Cancelado: return "Cancelado";
                case SituacaoCancelamentoProvisao.SemRegraAprovacao: return "Sem Regra Aprovação";
                case SituacaoCancelamentoProvisao.SolicitacaoReprovada: return "Solicitação Reprovada";
                case SituacaoCancelamentoProvisao.AgAprovacaoSolicitacao: return "Ag. Solicitação";
                case SituacaoCancelamentoProvisao.SolicitacaoAprovada: return "Solicitação Aprovada";
                case SituacaoCancelamentoProvisao.NaoProcessado: return "Não Processado";
                case SituacaoCancelamentoProvisao.Estornado: return "Estornado";
                default: return "Nenhuma";
            }
        }
    }
}

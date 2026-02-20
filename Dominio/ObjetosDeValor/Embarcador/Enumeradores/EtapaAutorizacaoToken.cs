namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum EtapaAutorizacaoToken
    {
       Todos = 0,
       SemRegraAprovacao = 1,
       Cancelado = 2,
       AgAprovacao = 3,
       SolicitacaoAprovada = 4,
       SolicitacaoReprovada = 5,
       Finalizada = 6,
       EmLiberacaoSistematica = 7,
       LiberacaoSistematicaProblema = 8,
    }

    public static class EtapaAutorizacaoTokenHelper
    {
        public static string ObterDescricao(this EtapaAutorizacaoToken etapaAprovacao)
        {
            switch (etapaAprovacao)
            {
                
                case EtapaAutorizacaoToken.SemRegraAprovacao: return "Solicitação sem regra de aprovação";
                case EtapaAutorizacaoToken.Cancelado: return "Solicitação cancelada";
                case EtapaAutorizacaoToken.AgAprovacao: return "Solicitação Aguardando aprovação";
                case EtapaAutorizacaoToken.SolicitacaoAprovada: return "Solicitação aprovada";
                case EtapaAutorizacaoToken.SolicitacaoReprovada: return "Solicitação reprovada";
                case EtapaAutorizacaoToken.Finalizada: return "Solicitação finalizada";
                case EtapaAutorizacaoToken.EmLiberacaoSistematica: return "Em Liberação Sistematica";
                case EtapaAutorizacaoToken.LiberacaoSistematicaProblema: return "Liberação Sistematica Problema";
                
                default: return string.Empty;
            }
        }
    }
}

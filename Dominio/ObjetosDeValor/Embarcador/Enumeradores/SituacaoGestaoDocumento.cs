namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoGestaoDocumento
    {
        Inconsistente = 1,
        Rejeitado = 2,
        Aprovado = 3,
        AprovadoComDesconto = 4,
        AguardandoAprovacao = 5,
        SemRegraAprovacao = 6,
        EmTratativa = 7
    }

    public static class SituacaoGestaoDocumentoHelper
    {
        public static bool IsAprovado(this SituacaoGestaoDocumento situacao)
        {
            return (situacao == SituacaoGestaoDocumento.Aprovado) || (situacao == SituacaoGestaoDocumento.AprovadoComDesconto);
        }

        public static string ObterDescricao(this SituacaoGestaoDocumento situacao)
        {
            switch (situacao)
            {
                case SituacaoGestaoDocumento.Inconsistente: return "Inconsistente";
                case SituacaoGestaoDocumento.Rejeitado: return "Rejeitado";
                case SituacaoGestaoDocumento.Aprovado: return "Aprovado";
                case SituacaoGestaoDocumento.AprovadoComDesconto: return "Aprovado com Desconto";
                case SituacaoGestaoDocumento.AguardandoAprovacao: return "Aguardando Aprovação";
                case SituacaoGestaoDocumento.SemRegraAprovacao: return "Sem Regra de Aprovação";
                case SituacaoGestaoDocumento.EmTratativa: return "Em Tratativa";
                default: return string.Empty;
            }
        }
    }
}

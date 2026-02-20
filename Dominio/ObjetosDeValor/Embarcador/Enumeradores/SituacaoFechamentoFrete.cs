namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoFechamentoFrete
    {
        Aberto = 1,
        Fechado = 2,
        Cancelado = 3,
        EmEmissaoComplemento = 4,
        PendenciaEmissao = 5,
        AgIntegracao = 6,
        ProblemaIntegracao = 7
    }

    public static class SituacaoFechamentoFreteHelper
    {
        public static bool IsFechamentoIniciado(this SituacaoFechamentoFrete situacao)
        {
            return (situacao != SituacaoFechamentoFrete.Aberto) && (situacao != SituacaoFechamentoFrete.Cancelado);
        }

        public static string ObterDescricao(this SituacaoFechamentoFrete situacao)
        {
            switch (situacao)
            {
                case SituacaoFechamentoFrete.Aberto: return "Pendente";
                case SituacaoFechamentoFrete.AgIntegracao: return "Ag. Integração";
                case SituacaoFechamentoFrete.Cancelado: return "Cancelado";
                case SituacaoFechamentoFrete.EmEmissaoComplemento: return "Em Emissão Complemento";
                case SituacaoFechamentoFrete.Fechado: return "Finalizado";
                case SituacaoFechamentoFrete.PendenciaEmissao: return "Pendência na Emissão";
                case SituacaoFechamentoFrete.ProblemaIntegracao: return "Problema Integração";
                default: return string.Empty;
            }
        }
    }
}

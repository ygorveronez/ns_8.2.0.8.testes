namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoContratoFreteAcrescimoDesconto
    {
        Todos = 0,
        Aprovado = 1,
        AgAprovacao = 2,
        Rejeitado = 3,
        Finalizado = 4,
        Cancelado = 5,
        SemRegra = 6,
        AgIntegracao = 7,
        FalhaIntegracao = 8,
        AplicacaoValorRejeitado = 9
    }

    public static class SituacaoContratoFreteAcrescimoDescontoHelper
    {
        public static string ObterDescricao(this SituacaoContratoFreteAcrescimoDesconto situacao)
        {
            switch (situacao)
            {
                case SituacaoContratoFreteAcrescimoDesconto.AgAprovacao: return "Ag. Aprovação";
                case SituacaoContratoFreteAcrescimoDesconto.Aprovado: return "Aprovado";
                case SituacaoContratoFreteAcrescimoDesconto.Rejeitado: return "Rejeitado";
                case SituacaoContratoFreteAcrescimoDesconto.Finalizado: return "Finalizado";
                case SituacaoContratoFreteAcrescimoDesconto.Cancelado: return "Cancelado";
                case SituacaoContratoFreteAcrescimoDesconto.SemRegra: return "Sem Regra";
                case SituacaoContratoFreteAcrescimoDesconto.AgIntegracao: return "Ag. Integração";
                case SituacaoContratoFreteAcrescimoDesconto.FalhaIntegracao: return "Falha na Integração";
                case SituacaoContratoFreteAcrescimoDesconto.AplicacaoValorRejeitado: return "Aplicação Valor Rejeitado";
                default: return string.Empty;
            }
        }
        public static bool IsPermiteCancelarContratoFreteAcrescimoDesconto(this SituacaoContratoFreteAcrescimoDesconto situacao)
        {
            switch (situacao)
            {
                case SituacaoContratoFreteAcrescimoDesconto.AplicacaoValorRejeitado:
                    return false;

                default:
                    return true;
            }
        }
    }
}

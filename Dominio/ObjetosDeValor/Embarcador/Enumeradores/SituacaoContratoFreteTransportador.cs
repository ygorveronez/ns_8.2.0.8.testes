namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoContratoFreteTransportador
    {
        Aprovado = 1,
        AgAprovacao = 2,
        Rejeitado = 3,
        SemRegra = 4,
        Novo = 5,
        Vencido = 6,
        AgIntegracao = 7,
        ProblemaIntegracao = 8,
    }

    public static class SituacaoContratoFreteTransportadorHelper
    {
        public static string ObterDescricao(this SituacaoContratoFreteTransportador situacao)
        {
            switch (situacao)
            {
                case SituacaoContratoFreteTransportador.AgAprovacao: return "Ag. Aprovação";
                case SituacaoContratoFreteTransportador.Aprovado: return "Aprovado";
                case SituacaoContratoFreteTransportador.Rejeitado: return "Rejeitado";
                case SituacaoContratoFreteTransportador.Novo: return "Novo";
                case SituacaoContratoFreteTransportador.SemRegra: return "Sem Regra";
                case SituacaoContratoFreteTransportador.Vencido: return "Vencido";
                case SituacaoContratoFreteTransportador.AgIntegracao: return "Ag. Integração";
                case SituacaoContratoFreteTransportador.ProblemaIntegracao: return "Problema Integração";
                default: return string.Empty;
            }
        }
    }
}

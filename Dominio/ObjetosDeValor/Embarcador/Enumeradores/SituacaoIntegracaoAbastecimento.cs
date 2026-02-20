namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum SituacaoIntegracaoAbastecimento
    {
        Pendente = 0,
        PendenteReserva = 1,
        PendenteAutorizacao = 2,
        AgRetornoAbastecimento = 3,
        Finalizado = 4,
        ProblemaAbastecimento = 5,
        AgRetornoReserva = 6,
        AgRetornoAutorizacao = 7,
        Autorizado = 8
    }

    public static class SituacaoIntegracaoAbastecimentoHelper
    {
        public static string ObterDescricao(this SituacaoIntegracaoAbastecimento situacao)
        {
            switch (situacao)
            {
                case SituacaoIntegracaoAbastecimento.Pendente: return "Pendente";
                case SituacaoIntegracaoAbastecimento.PendenteReserva: return "Pendente Reserva";
                case SituacaoIntegracaoAbastecimento.PendenteAutorizacao: return "Pendente Autorização";
                case SituacaoIntegracaoAbastecimento.AgRetornoAbastecimento: return "Em abastecimento";
                case SituacaoIntegracaoAbastecimento.Finalizado: return "Finalizado";
                case SituacaoIntegracaoAbastecimento.ProblemaAbastecimento: return "Problema no abastecimento";
                case SituacaoIntegracaoAbastecimento.AgRetornoReserva: return "Aguardando Retorno Reserva";
                case SituacaoIntegracaoAbastecimento.AgRetornoAutorizacao: return "Aguardando Retorno Autorização";
                case SituacaoIntegracaoAbastecimento.Autorizado: return "Autorizado";
                default: return string.Empty;
            }
        }
    }
}

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum GatilhoIntegracaoMondelezDrivin
    {
        ContatoCliente = 1,
        AgendamentoEntrega = 2,
        ReagendamentoEntrega = 3,
        ConfirmacaoEntrega = 4,
        RejeicaoEntrega = 5,
    }

    public static class TipoIntegracaoMondelezDrivinHelper
    {
        public static string ObterDescricao(this GatilhoIntegracaoMondelezDrivin tipoIntegracao)
        {
            switch (tipoIntegracao)
            {
                case GatilhoIntegracaoMondelezDrivin.ContatoCliente: return "Primeiro contato com o cliente";
                case GatilhoIntegracaoMondelezDrivin.AgendamentoEntrega: return "Agendamento de entrega";
                case GatilhoIntegracaoMondelezDrivin.ReagendamentoEntrega: return "Reagendamento de entrega";
                case GatilhoIntegracaoMondelezDrivin.ConfirmacaoEntrega: return "Confirmação de Entrega";
                case GatilhoIntegracaoMondelezDrivin.RejeicaoEntrega: return "Rejeitar Entrega";
                default: return string.Empty;
            }
        }
    }
}
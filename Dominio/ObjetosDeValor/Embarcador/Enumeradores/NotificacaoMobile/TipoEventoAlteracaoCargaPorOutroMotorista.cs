namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores.NotificacaoMobile
{
    public enum TipoEventoAlteracaoCargaPorOutroMotorista
    {
        InicioViagem = 1,
        FinalizacaoEntregaColeta = 2,
        RejeicaoEntregaColeta = 3,
        FimViagem = 4,
    }

    public static class TipoEventoAlteracaoCargaPorOutroMotoristaHelper
    {
        public static string ObterDescricao(this TipoEventoAlteracaoCargaPorOutroMotorista situacaoPedido)
        {
            switch (situacaoPedido)
            {
                case TipoEventoAlteracaoCargaPorOutroMotorista.InicioViagem:
                    return "Início de viagem";
                case TipoEventoAlteracaoCargaPorOutroMotorista.FinalizacaoEntregaColeta:
                    return "Finalização de entrega/coleta";
                case TipoEventoAlteracaoCargaPorOutroMotorista.RejeicaoEntregaColeta:
                    return "Rejeição de entrega/coleta";
                case TipoEventoAlteracaoCargaPorOutroMotorista.FimViagem:
                    return "Finalização da viagem";
                default:
                    return string.Empty;
            }
        }
    }

}

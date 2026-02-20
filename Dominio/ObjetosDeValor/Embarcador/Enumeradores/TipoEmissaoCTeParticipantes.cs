namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEmissaoCTeParticipantes
    {
        Normal = 0,
        ComRecebedor = 1,
        ComExpedidor = 2,
        ComTransbordo = 3,
        ComExpedidorERecebedor = 4
    }

    public static class TipoEmissaoCTeParticipantesHelper
    {
        public static string ObterDescricao(this TipoEmissaoCTeParticipantes tipoEmissaoCTeParticipantes)
        {
            switch (tipoEmissaoCTeParticipantes)
            {
                case TipoEmissaoCTeParticipantes.Normal:
                    return "Normal";
                case TipoEmissaoCTeParticipantes.ComRecebedor:
                    return "Com Recebedor";
                case TipoEmissaoCTeParticipantes.ComExpedidor:
                    return "Com Expedidor";
                case TipoEmissaoCTeParticipantes.ComTransbordo:
                    return "Com local de Transbordo";
                case TipoEmissaoCTeParticipantes.ComExpedidorERecebedor:
                    return "Com Expedidor e Recebedor";
                default:
                    return "";
            }
        }
    }
}

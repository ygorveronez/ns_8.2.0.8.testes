namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum AngelLiraRegraCodigoIdentificacaoViagem
    {
        NumeroPedidoEmbarcadorMaisPlaca = 1,
        IDCargaMaisNumeroPedidoEmbarcador = 2
    }

    public static class AngelLiraRegraCodigoIdentificacaoViagemHelper
    {
        public static string ObterDescricao(this AngelLiraRegraCodigoIdentificacaoViagem aceite)
        {
            switch (aceite)
            {
                case AngelLiraRegraCodigoIdentificacaoViagem.NumeroPedidoEmbarcadorMaisPlaca: return "Número pedido do embarcador + placa";
                case AngelLiraRegraCodigoIdentificacaoViagem.IDCargaMaisNumeroPedidoEmbarcador: return "ID da carga + número pedido embarcador";
                default: return string.Empty;
            }
        }
    }
}

namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum TipoEnvioHUBOfertas
    {
        EnvioTransportador = 0,
        EnvioDemandaOferta = 1,
        CancelamentoDemandaOferta = 2,
        FinalizacaoDemandaOferta = 3
    }

    public static class TipoEnvioHUBHelper
    {
        public static string ObterDescricao(this TipoEnvioHUBOfertas situacao)
        {
            switch (situacao)
            {
                case TipoEnvioHUBOfertas.EnvioTransportador: return "Envio Transportador";
                case TipoEnvioHUBOfertas.EnvioDemandaOferta: return "Envio Demanda Oferta";
                case TipoEnvioHUBOfertas.CancelamentoDemandaOferta: return "Cancelamento Demanda Oferta";
                case TipoEnvioHUBOfertas.FinalizacaoDemandaOferta: return "Finalização Demanda Oferta";
                default: return string.Empty;
            }
        }
    }
}

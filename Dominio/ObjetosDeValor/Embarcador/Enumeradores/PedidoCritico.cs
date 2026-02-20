namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PedidoCritico
    {
        Nao = 0,
        Sim = 1
    }

    public static class PedidoCriticoHelper
    {
        public static string ObterPedidoCriticoFormatado(this PedidoCritico status)
        {
            switch (status)
            {
                case PedidoCritico.Nao: return "Não";
                case PedidoCritico.Sim: return "Sim";

                default: return string.Empty;
            }
        }
    }
}

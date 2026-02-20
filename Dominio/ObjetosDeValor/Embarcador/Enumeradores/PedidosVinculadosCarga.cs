namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum PedidosVinculadosCarga
    {
        PedidoComCarga = 0,
        PedidoSemCarga = 1,
        PedidoCargaComMotoristaVinculado = 2,
        PedidoCargaSemMotoristaVinculado = 3,
        PedidoCargaComVeiculoVinculado = 4,
        PedidoCargaSemVeiculoVinculado = 5
    }

    public static class EnumPedidosVinculadosCargaHelper
    {
        public static string ObterDescricao(this PedidosVinculadosCarga situacao)
        {
            switch (situacao)
            {
                case PedidosVinculadosCarga.PedidoComCarga: return "Pedido com carga";
                case PedidosVinculadosCarga.PedidoSemCarga: return "Pedido sem carga";
                case PedidosVinculadosCarga.PedidoCargaComMotoristaVinculado: return "Pedido/Carga com motorista vinculado";
                case PedidosVinculadosCarga.PedidoCargaSemMotoristaVinculado: return "Pedido/Carga sem motorista vinculado";
                case PedidosVinculadosCarga.PedidoCargaComVeiculoVinculado: return "Pedido/Carga com veículo vinculado";
                case PedidosVinculadosCarga.PedidoCargaSemVeiculoVinculado: return "Pedido/Carga sem veículo vinculado";
                default: return string.Empty;
            }
        }
    }
}

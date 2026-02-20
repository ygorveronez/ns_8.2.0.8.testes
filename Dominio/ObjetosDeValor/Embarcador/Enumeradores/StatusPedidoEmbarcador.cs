namespace Dominio.ObjetosDeValor.Embarcador.Enumeradores
{
    public enum StatusPedidoEmbarcador
    {
        Todos = -1,
        NaoDefinido = 0,
        Aberto = 1,
        BloqueadoPorRegra = 2,
        BloqueadoPorVerba = 3,
        AguardandoSalesForce = 4,
        Liberado = 5,
        Encerrado = 6
    }

    public static class StatusPedidoEmbarcadorHelper
    {
        public static string ObterDescricao(this StatusPedidoEmbarcador status)
        {
            switch (status)
            {
                case StatusPedidoEmbarcador.NaoDefinido: return "Não Definido";
                case StatusPedidoEmbarcador.Aberto: return "Aberto";
                case StatusPedidoEmbarcador.BloqueadoPorRegra: return "Bloqueado Por Regra";
                case StatusPedidoEmbarcador.BloqueadoPorVerba: return "Bloqueado Por Verba";
                case StatusPedidoEmbarcador.AguardandoSalesForce: return "Aguardando Sales Force";
                case StatusPedidoEmbarcador.Liberado: return "Liberado";
                case StatusPedidoEmbarcador.Encerrado: return "Encerrado";
                default: return string.Empty;
            }
        }
    }
}

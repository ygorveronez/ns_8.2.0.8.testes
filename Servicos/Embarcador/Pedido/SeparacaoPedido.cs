namespace Servicos.Embarcador.Pedido
{
    public class SeparacaoPedido
    {
        public static void EncaminharPedidosParaReentrega(Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega cargaEntrega, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            repositorioCargaPedido.DisponibilizarPedidosParaReentregaComSeparacao(cargaEntrega.Codigo);
        }

        public static void EncaminharPedidosParaSeparacao(Dominio.Entidades.Embarcador.Cargas.Carga carga, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            if (carga.TipoOperacao?.DisponibilizarPedidosParaSeparacaoAposEmissaoDocumentos ?? false)
                repositorioCargaPedido.DisponibilizarPedidosParaSeparacao(carga.Codigo);
            else if (carga.TipoOperacao?.DisponibilizarPedidosComRecebedorParaSeparacaoAposEmissaoDocumentos ?? false)
                repositorioCargaPedido.DisponibilizarPedidosComRecebedorParaSeparacao(carga.Codigo);
            else
                repositorioCargaPedido.DisponibilizarPedidosEncaixadosComRecebedorParaSeparacao(carga.Codigo);
        }
    }
}

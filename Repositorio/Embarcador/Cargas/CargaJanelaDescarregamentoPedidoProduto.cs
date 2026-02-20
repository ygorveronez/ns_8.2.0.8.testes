using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaJanelaDescarregamentoPedidoProduto : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto>
    {
        #region Construtores

        public CargaJanelaDescarregamentoPedidoProduto(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Públicos
        public Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto> BuscarPorPedido(int codigoPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto>();
            var result = from obj in query where obj.CargaJanelaDescarregamentoPedido.Pedido.Codigo == codigoPedido select obj;
            return result.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto> BuscarPorCodigosPedidos(List<int> codigosPedidos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto>()
                .Where(x => codigosPedidos.Contains(x.CargaJanelaDescarregamentoPedido.Pedido.Codigo));

            return query.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto> BuscarPorCarga(int codigoCarga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaJanelaDescarregamentoPedidoProduto>();
            var result = from obj in query where obj.CargaJanelaDescarregamentoPedido.CargaJanelaDescarregamento.Carga.Codigo == codigoCarga select obj;
            return result.ToList();
        }

        #endregion
    }
}

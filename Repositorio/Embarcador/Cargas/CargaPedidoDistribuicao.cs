using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;


namespace Repositorio.Embarcador.Cargas
{
    public class CargaPedidoDistribuicao : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDistribuicao>
    {
        public CargaPedidoDistribuicao(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public CargaPedidoDistribuicao(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoDistribuicao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDistribuicao>();
            var result = from obj in query select obj;
            result = result.Where(p => p.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaPedidoDistribuicao BuscarPorCargaPedido(int cargaPedido)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaPedidoDistribuicao>();
            var result = from obj in query select obj;
            result = result.Where(p => p.CargaPedido.Codigo == cargaPedido);
            return result.FirstOrDefault();
        }
    }
}

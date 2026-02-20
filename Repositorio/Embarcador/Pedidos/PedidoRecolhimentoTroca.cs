using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoRecolhimentoTroca : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoRecolhimentoTroca>
    {
        public PedidoRecolhimentoTroca(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoRecolhimentoTroca BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoRecolhimentoTroca>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }
    }
}

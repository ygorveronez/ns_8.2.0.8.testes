using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PlanejamentoPedidoConfiguracao : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoConfiguracao>
    {
        public PlanejamentoPedidoConfiguracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoConfiguracao BuscarConfiguracao()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoConfiguracao>();

            var result = from obj in query select obj;

            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoConfiguracao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PlanejamentoPedidoConfiguracao>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }

    }
}

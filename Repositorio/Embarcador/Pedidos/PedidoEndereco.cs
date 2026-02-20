using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class PedidoEndereco: RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco>
    {
        public PedidoEndereco(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result
                .Fetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .ThenFetch(obj => obj.Pais)
                .Fetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Regiao)
                .Fetch(obj => obj.ClienteOutroEndereco)
                .FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco BuscarPorCodigoDocumento(string codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.PedidoEndereco>();
            var result = from obj in query where obj.Localidade.CodigoDocumento == codigo select obj;
            return result
                .Fetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Estado)
                .ThenFetch(obj => obj.Pais)
                .Fetch(obj => obj.Localidade)
                .ThenFetch(obj => obj.Regiao)
                .Fetch(obj => obj.ClienteOutroEndereco)
                .FirstOrDefault();
        }

    }
}

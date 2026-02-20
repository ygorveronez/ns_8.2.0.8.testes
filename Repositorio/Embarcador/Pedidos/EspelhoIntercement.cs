using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class EspelhoIntercement : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.EspelhoIntercement>
    {
        public EspelhoIntercement(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.EspelhoIntercement BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.EspelhoIntercement>();

            var result = from obj in query where obj.Codigo == codigo select obj;

            return result.FirstOrDefault();
        }
        public Dominio.Entidades.Embarcador.Pedidos.EspelhoIntercement BuscarPorVBELN(string VBELN)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.EspelhoIntercement>();

            var result = from obj in query where obj.VBELN == VBELN select obj;

            return result.FirstOrDefault();
        }

    }
}

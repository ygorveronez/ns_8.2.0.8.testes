using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Pedidos
{
    public class ContainerTipoAssociado : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ContainerTipoAssociado>
    {
        public ContainerTipoAssociado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.ContainerTipoAssociado BuscarPorTipoContainerEAssociado(int codigoTipoContainerAssociado, int codigoTipoContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ContainerTipoAssociado>();
            var result = from obj in query where obj.ContainerTipoVinculado.Codigo == codigoTipoContainerAssociado && obj.ContainerTipo.Codigo == codigoTipoContainer select obj;
            return result.FirstOrDefault();
        }
    }
}

using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class ColetaContainerCargaEntrega : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerCargaEntrega>
    {
        public ColetaContainerCargaEntrega(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public virtual Dominio.Entidades.Embarcador.Pedidos.ColetaContainer BuscarColetaContainerPorCargaEntrega(int codigoCargaEntrega)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerCargaEntrega>();

            var result = from obj in query where obj.CargaEntrega.Codigo == codigoCargaEntrega select obj;

            return result.FirstOrDefault()?.ColetaContainer ?? null;
        }


        public virtual Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntrega BuscarCargaEntregaPorColetaContainer(int codigoColetaContainer)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerCargaEntrega>();

            var result = from obj in query where obj.ColetaContainer.Codigo == codigoColetaContainer select obj;

            return result.FirstOrDefault()?.CargaEntrega ?? null;
        }
    }
}

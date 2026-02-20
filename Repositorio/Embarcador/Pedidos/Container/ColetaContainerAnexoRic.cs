using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class ColetaContainerAnexoRic : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexoRic>
    {
        public ColetaContainerAnexoRic(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexoRic BuscarPorNumeroContainer(string container)
        {
            var anexos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexoRic>();
            var ric = anexos.Where(a => a.ContainerDescricao == container)
                .OrderByDescending(x => x.Codigo)
                .FirstOrDefault();

            return ric;
        }

        public Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexoRic BuscarPorCodigoContainer(int codigoContainer)
        {
            var anexos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexoRic>();
            var ric = anexos.Where(a => a.Container.Codigo == codigoContainer)
                .OrderByDescending(x => x.Codigo)
                .FirstOrDefault();

            return ric;
        }
    }
}

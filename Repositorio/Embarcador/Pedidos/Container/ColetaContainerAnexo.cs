using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Pedidos
{
    public class ColetaContainerAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo>
    {
        public ColetaContainerAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo> BuscarPorColetaContainerECarga(int codigoColetaContainer, int codigoCarga = 0)
        {
            var anexos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo>();

            if (codigoColetaContainer > 0)
                anexos = anexos.Where(a => a.ColetaContainer.Codigo == codigoColetaContainer);

            if (codigoCarga > 0)
                anexos = anexos.Where(a => a.Carga.Codigo == codigoCarga);

            return anexos.OrderBy(x => x.Codigo).ToList();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo> BuscarPorContainerECarga(int codigoContainer, int codigoCarga = 0)
        {
            var anexos = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.ColetaContainerAnexo>();

            if (codigoContainer > 0)
                anexos = anexos.Where(a => a.ColetaContainer.Container.Codigo == codigoContainer);

            if (codigoCarga > 0)
                anexos = anexos.Where(a => a.Carga.Codigo == codigoCarga);

            return anexos.OrderBy(x => x.Codigo).ToList();
        }
    }
}

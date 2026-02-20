using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Pedidos
{
    public class RetiradaContainer : RepositorioBase<Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer>
    {
        #region Contrutores

        public RetiradaContainer(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer BuscarPorCarga(int codigoCarga)
        {
            var consultaRetiradaContainer = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer>()
                .Where(o => o.Carga.Codigo == codigoCarga);

            return consultaRetiradaContainer.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer> BuscarPorCargas(List<int> codigosCarga)
        {
            var consultaRetiradaContainer = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer>()
                .Where(o => codigosCarga.Contains(o.Carga.Codigo));

            return consultaRetiradaContainer.ToList();
        }

        public int BuscarCodigoContainerPorCarga(int codigoCarga)
        {
            var consultaRetiradaContainer = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Container != null);

            return consultaRetiradaContainer.Select(o => o.Container.Codigo).FirstOrDefault();
        }

        public Task<int> BuscarCodigoContainerPorCargaAsync(int codigoCarga)
        {
            var consultaRetiradaContainer = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Pedidos.RetiradaContainer>()
                .Where(o => o.Carga.Codigo == codigoCarga && o.Container != null);

            return consultaRetiradaContainer.Select(o => o.Container.Codigo).FirstOrDefaultAsync();
        }

        #endregion Métodos Públicos
    }
}

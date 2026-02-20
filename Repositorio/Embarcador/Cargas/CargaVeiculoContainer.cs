using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class CargaVeiculoContainer : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer>
    {
        #region Construtores

        public CargaVeiculoContainer(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public CargaVeiculoContainer(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }
        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer> BuscarPorCarga(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer>()
                .Where(obj => obj.Carga.Codigo == carga);

            return query.ToList();
        }
        public async Task<List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer>> BuscarPorCargaAsync(int carga)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer>()
                .Where(obj => obj.Carga.Codigo == carga);

            return await query.ToListAsync();
        }

        public Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer BuscarPorCargaEVeiculo(int carga, int veiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer>()
                .Where(obj => obj.Carga.Codigo == carga && obj.Veiculo.Codigo == veiculo);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer> BuscarPorCargas(List<int> cargas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.CargaVeiculoContainer>()
                .Where(obj => cargas.Contains(obj.Carga.Codigo));

            return query.ToList();
        }

        #endregion
    }
}

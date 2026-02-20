using NHibernate.Linq;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;


namespace Repositorio.Embarcador.Cargas
{
    public class TipoCargaModeloVeicularAutoConfig : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig>
    {
        public TipoCargaModeloVeicularAutoConfig(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public TipoCargaModeloVeicularAutoConfig(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig Buscar()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig>();
            return query.FirstOrDefault();
        }
        public async Task<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig> BuscarAsync()
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaModeloVeicularAutoConfig>();
            return await query.FirstOrDefaultAsync();
        }
    }
}

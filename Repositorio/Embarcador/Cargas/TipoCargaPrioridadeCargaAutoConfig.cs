using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;


namespace Repositorio.Embarcador.Cargas
{
    public class TipoCargaPrioridadeCargaAutoConfig : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig>
    {
        public TipoCargaPrioridadeCargaAutoConfig(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig> BuscarPorTipoCargaModeloAutoConfig(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig>();
            var result = query.Where(obj => obj.TipoCargaModeloVeicularAutoConfig.Codigo == codigo);
            return result.OrderBy(obj => obj.Posicao).ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig>> BuscarPorTipoCargaModeloAutoConfigAsync(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaPrioridadeCargaAutoConfig>();
            var result = query.Where(obj => obj.TipoCargaModeloVeicularAutoConfig.Codigo == codigo);
            return result.OrderBy(obj => obj.Posicao).ToListAsync();
        }

    }
}

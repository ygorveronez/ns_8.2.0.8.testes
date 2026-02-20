using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Cargas
{
    public class TipoCargaValorCargaAutoConfig : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig>
    {
        public TipoCargaValorCargaAutoConfig(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig BuscarPorCodigo(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig>();
            var result = query.Where(obj => obj.Codigo == codigo);
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig> BuscarPorTipoCargaModeloAutoConfig(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig>();
            var result = query.Where(obj => obj.TipoCargaModeloVeicularAutoConfig.Codigo == codigo);
            return result.OrderBy(obj => obj.Valor).ToList();
        }

        public Task<List<Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig>> BuscarPorTipoCargaModeloAutoConfigAsync(int codigo)
        {
            var query = SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.TipoCargaValorCargaAutoConfig>();
            var result = query.Where(obj => obj.TipoCargaModeloVeicularAutoConfig.Codigo == codigo);
            return result.OrderBy(obj => obj.Valor).ToListAsync();
        }
    }
}

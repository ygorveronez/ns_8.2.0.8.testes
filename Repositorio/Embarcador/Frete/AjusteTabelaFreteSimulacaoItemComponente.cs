using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class AjusteTabelaFreteSimulacaoItemComponente : RepositorioBase<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItemComponente>
    {
        public AjusteTabelaFreteSimulacaoItemComponente(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItemComponente> BuscarPorItem(int codigoItem)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AjusteTabelaFreteSimulacaoItemComponente>();
            query = query.Where(o => o.ItemSimulacao.Codigo == codigoItem);
            return query.ToList();
        }
    }
}

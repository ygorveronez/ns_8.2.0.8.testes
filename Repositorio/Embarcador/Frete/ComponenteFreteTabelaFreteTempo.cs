using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace Repositorio.Embarcador.Frete
{
    public class ComponenteFreteTabelaFreteTempo : RepositorioBase<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFreteTempo>
    {
        public ComponenteFreteTabelaFreteTempo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFreteTempo> BuscarPorTabelaFrete(int tabelaFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ComponenteFreteTabelaFreteTempo>();

            var result = from obj in query where obj.ComponenteFreteTabelaFrete.TabelaFrete.Codigo == tabelaFrete select obj;

            return result
                .Fetch(obj => obj.ComponenteFreteTabelaFrete)
                .ToList();
        }

    }
}

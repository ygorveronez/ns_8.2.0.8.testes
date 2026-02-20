using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteHora : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteHora>
    {
        public TabelaFreteHora(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteHora> BuscarPorCodigos(int[] codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteHora>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.ToList();
        }
    }
}

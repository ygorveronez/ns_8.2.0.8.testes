using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public class TabelaFreteTempo : RepositorioBase<Dominio.Entidades.Embarcador.Frete.TabelaFreteTempo>
    {
        public TabelaFreteTempo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Frete.TabelaFreteTempo> BuscarPorCodigos(int[] codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.TabelaFreteTempo>();

            query = query.Where(o => codigos.Contains(o.Codigo));

            return query.ToList();
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Cargas
{
    public class ContribuinteCargaCTeAnexo : RepositorioBase<Dominio.Entidades.Embarcador.Cargas.ContribuinteCargaCTeAnexo>
    {
        public ContribuinteCargaCTeAnexo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Cargas.ContribuinteCargaCTeAnexo> BuscarPorCargaCTe(int codigoCargaCTe)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Cargas.ContribuinteCargaCTeAnexo>()
                .Where(o => o.CargaCTe.Codigo == codigoCargaCTe);

            return query.ToList();
        }
    }
}

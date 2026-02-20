using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Checklist
{
    public class ChecklistPergunta : RepositorioBase<Dominio.Entidades.Embarcador.Checklist.ChecklistPergunta>
    {

        public ChecklistPergunta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Checklist.ChecklistPergunta> BuscarPorChecklist(int codigoChecklist)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Checklist.ChecklistPergunta>()
                .Where(o => o.Checklist.Codigo == codigoChecklist);

            return query.OrderBy(x => x.Ordem).ToList();
        }

    }
}

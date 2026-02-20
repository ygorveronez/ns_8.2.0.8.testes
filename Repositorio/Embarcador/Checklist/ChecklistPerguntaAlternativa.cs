using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Checklist
{
    public class ChecklistPerguntaAlternativa : RepositorioBase<Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa>
    {

        public ChecklistPerguntaAlternativa(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa> BuscarPorChecklistPergunta(int codigoChecklistPergunta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa>()
                .Where(o => o.ChecklistPergunta.Codigo == codigoChecklistPergunta);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa> BuscarPorChecklistPorPerguntas(List<int> codigoCheckPerguntas)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Checklist.ChecklistPerguntaAlternativa>()
                .Where(o => codigoCheckPerguntas.Contains(o.ChecklistPergunta.Codigo));

            return query.ToList();
        }

    }
}

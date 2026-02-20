using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Bidding.RFI
{
    public class RFIChecklistQuestionario : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionario>
    {
        public RFIChecklistQuestionario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionario> BuscarQuestionarios(Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklist RFIChecklist)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.RFI.RFIChecklistQuestionario>()
                .Where(o => o.Checklist.Codigo == RFIChecklist.Codigo);

            return query.ToList();
        }
    }
}

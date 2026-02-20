using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingChecklistQuestionario : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionario>
    {
        public BiddingChecklistQuestionario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public BiddingChecklistQuestionario(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionario> BuscarQuestionarios(Dominio.Entidades.Embarcador.Bidding.BiddingChecklist entidadeBiddingChecklist)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistQuestionario>()
                .Where(o => o.Checklist == entidadeBiddingChecklist);

            return query
                .Fetch(o => o.Anexos)
                .ToList();
        }
    }
}

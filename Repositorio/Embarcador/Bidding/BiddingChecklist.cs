using NHibernate.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingChecklist : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingChecklist>
    {
        public BiddingChecklist(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public BiddingChecklist(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Publicos

        public Dominio.Entidades.Embarcador.Bidding.BiddingChecklist BuscarChecklist(Dominio.Entidades.Embarcador.Bidding.BiddingConvite entidadeBiddingConvite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingChecklist>()
                .Where(o => o.BiddingConvite.Codigo == entidadeBiddingConvite.Codigo);

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Bidding.BiddingChecklist> BuscarChecklistAsync(Dominio.Entidades.Embarcador.Bidding.BiddingConvite entidadeBiddingConvite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingChecklist>()
                .Where(o => o.BiddingConvite.Codigo == entidadeBiddingConvite.Codigo);

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public Task<DateTime?> BuscarDataPrazoChecklistAsync(Dominio.Entidades.Embarcador.Bidding.BiddingConvite entidadeBiddingConvite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingChecklist>()
                .Where(o => o.BiddingConvite.Codigo == entidadeBiddingConvite.Codigo).Select(x => x.DataPrazo);

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        #endregion
    }
}

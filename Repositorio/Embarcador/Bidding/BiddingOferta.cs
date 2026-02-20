using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingOferta : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingOferta>
    {
        public BiddingOferta(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public BiddingOferta(UnitOfWork unitOfWork, CancellationToken cancelationToken) : base(unitOfWork, cancelationToken) { }

        public Dominio.Entidades.Embarcador.Bidding.BiddingOferta BuscarOferta(Dominio.Entidades.Embarcador.Bidding.BiddingConvite entidadeBiddingConvite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingOferta>()
                .Where(o => o.BiddingConvite.Codigo == entidadeBiddingConvite.Codigo);

            return query.FirstOrDefault();
        }
        
        public Task<Dominio.Entidades.Embarcador.Bidding.BiddingOferta> BuscarOfertaAsync(Dominio.Entidades.Embarcador.Bidding.BiddingConvite entidadeBiddingConvite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingOferta>()
                .Where(o => o.BiddingConvite.Codigo == entidadeBiddingConvite.Codigo);

            return query.FirstOrDefaultAsync(CancellationToken);
        }
    }
}

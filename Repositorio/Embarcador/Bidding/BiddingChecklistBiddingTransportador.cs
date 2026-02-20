using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingChecklistBiddingTransportador : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistBiddingTransportador>
    {
        public BiddingChecklistBiddingTransportador(UnitOfWork unitOfWork) : base(unitOfWork) { }
        
        public Dominio.Entidades.Embarcador.Bidding.BiddingChecklistBiddingTransportador BuscarPorBiddingTransportador(Dominio.Entidades.Empresa transportador, Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistBiddingTransportador>();

            return query.Where(o => o.Transportador == transportador && o.BiddingConvite == biddingConvite).FirstOrDefault();
        }
        
        public List<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistBiddingTransportador> BuscarPorBidding(Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingChecklistBiddingTransportador>();

            return query.Where(o => o.BiddingConvite == biddingConvite)
                        .Fetch(o => o.Transportador)
                        .ToList();
        }
    }
}

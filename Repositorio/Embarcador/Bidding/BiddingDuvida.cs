using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingDuvida : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingDuvida>
    {
        public BiddingDuvida(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingDuvida> BuscarPorConvidadoConvite(Dominio.Entidades.Empresa empresa, Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingDuvida>()
                .Where(o => o.BiddingConvite == biddingConvite && o.Empresa == empresa);

            return query.ToList();
        }
        public List<Dominio.Entidades.Embarcador.Bidding.BiddingDuvida> BuscarPorConvite(int codigo)
        { 
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingDuvida>()
                .Where(o => o.BiddingConvite.Codigo == codigo);

            return query.Fetch(obj => obj.Empresa).ToList();
        }
    }
}

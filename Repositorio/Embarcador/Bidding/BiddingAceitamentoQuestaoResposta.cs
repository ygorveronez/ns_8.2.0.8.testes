using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingAceitamentoQuestaoResposta : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingAceitamentoQuestaoResposta>
    {
        public BiddingAceitamentoQuestaoResposta(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingAceitamentoQuestaoResposta> BuscarPorTransportadorBidding(Dominio.Entidades.Empresa transportador, Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingAceitamentoQuestaoResposta>()
                .Where(o => o.ChecklistBiddingTransportador.Transportador == transportador && o.ChecklistBiddingTransportador.BiddingConvite == biddingConvite);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingAceitamentoQuestaoResposta> BuscarPorBidding(Dominio.Entidades.Embarcador.Bidding.BiddingConvite biddingConvite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingAceitamentoQuestaoResposta>()
                .Where(o => o.Pergunta.Checklist.BiddingConvite == biddingConvite);

            return query.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingAceitamentoQuestaoResposta> BuscarPorBiddingChecklistTransportador(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingAceitamentoQuestaoResposta>()
                .Where(o => o.ChecklistBiddingTransportador.Codigo == codigo);

            return query
                .Fetch(o => o.Pergunta)
                .ToList();
        }
    }
}

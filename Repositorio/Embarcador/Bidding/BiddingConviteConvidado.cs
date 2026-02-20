using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingConviteConvidado : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado>
    {
        public BiddingConviteConvidado(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public BiddingConviteConvidado(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Publicos

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado> BuscarConvidados(Dominio.Entidades.Embarcador.Bidding.BiddingConvite entidadeBiddingConvite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado>()
                .Where(o => o.BiddingConvite == entidadeBiddingConvite);

            return query
                .Fetch(o => o.Convidado)
                .ToList();
        }

        public async Task<StatusBiddingConvite> BuscarConvidadoStatusAsync(Dominio.Entidades.Embarcador.Bidding.BiddingConvite entidadeBiddingConvite, int codigoTransportador)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado>()
                .Where(o => o.BiddingConvite == entidadeBiddingConvite && o.Convidado.Codigo == codigoTransportador);

            var result = await query
                .Select(x => x.StatusBidding)
                .FirstOrDefaultAsync(CancellationToken);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado> BuscarConvidadosConfirmados(Dominio.Entidades.Embarcador.Bidding.BiddingConvite entidadeBiddingConvite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado>()
                .Where(o => o.BiddingConvite == entidadeBiddingConvite && o.Status == StatusBiddingConviteConvidado.Aceito);

            return query
                .Fetch(o => o.Convidado)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado BuscarPorConvite(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado>()
                .Where(o => o.BiddingConvite.Codigo == codigo);

            return query.FirstOrDefault();
        }

        public Task<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado> BuscarPorConviteAsync(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado>()
                .Where(o => o.BiddingConvite.Codigo == codigo);

            return query.FirstOrDefaultAsync(CancellationToken);
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado> BuscarConvidadosNaoAvisados()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado>()
                .Where(o => o.EmailAvisoConvidadoEnviado == false && o.Status != StatusBiddingConviteConvidado.Rejeitado);

            return query
                .Fetch(o => o.BiddingConvite)
                .ToList();
        }

        public Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado BuscarConvidado(Dominio.Entidades.Embarcador.Bidding.BiddingConvite entidadeBiddingConvite, Dominio.Entidades.Empresa entidadeConvidado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado>()
                .Where(o => o.BiddingConvite == entidadeBiddingConvite && o.Convidado == entidadeConvidado);

            return query
                .Fetch(o => o.BiddingConvite)
                .FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado> BuscarConvidadosPorBiddingEtapa(Dominio.Entidades.Embarcador.Bidding.BiddingConvite entidadeBiddingConvite, List<StatusBiddingConvite> status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingConviteConvidado>()
                .Where(o => o.BiddingConvite == entidadeBiddingConvite && status.Contains(o.StatusBidding));

            return query
                .ToList();
        }

        #endregion Métodos Publicos
    }
}

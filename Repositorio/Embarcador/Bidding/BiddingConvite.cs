using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace Repositorio.Embarcador.Bidding
{
    public class BiddingConvite : RepositorioBase<Dominio.Entidades.Embarcador.Bidding.BiddingConvite>
    {
        public BiddingConvite(UnitOfWork unitOfWork) : base(unitOfWork) { }
        public BiddingConvite(UnitOfWork unitOfWork, CancellationToken cancellationToken) : base(unitOfWork, cancellationToken) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Bidding.BiddingConvite BuscarPorCodigoAprovada(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingConvite>()
                .Where(cvt => cvt.Status == StatusBiddingConvite.Aguardando
                        || cvt.Status == StatusBiddingConvite.Checklist
                        || cvt.Status == StatusBiddingConvite.Fechamento
                        || cvt.Status == StatusBiddingConvite.Ofertas
                        );

            return query.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Bidding.BiddingConvite BuscarPorTipoBidding(int codigoTipoBidding)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingConvite>()
                .Where(o => o.TipoBidding.Codigo == codigoTipoBidding);

            return query.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingConvite> Consultar(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBidding filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(filtrosPesquisa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBidding filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingConvite> BuscarPorEtapa(StatusBiddingConvite etapa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingConvite>()
                        .Where(o => o.Status == etapa);

            return query.ToList();
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingConvite> Consultar(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBidding filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingConvite>();
            var subQueryAprovacaoAlcada = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite>();
            var subQueryBiddingOfertaRota = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingOfertaRota>();

            var result = from obj in query select obj;
            var subResult = from obj in subQueryAprovacaoAlcada select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                result = result.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                result = result.Where(o => o.DataInicio.Date >= filtrosPesquisa.DataInicio);

            if (filtrosPesquisa.DataLimite != DateTime.MinValue)
                result = result.Where(o => o.DataLimite.Date <= filtrosPesquisa.DataLimite);

            if (filtrosPesquisa.Empresa != null)
                result = result.Where(o => o.Convidados.Any(obj => obj.Convidado == filtrosPesquisa.Empresa && obj.Status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusBiddingConviteConvidado.Rejeitado && o.DataInicio <= DateTime.Now));

            if (filtrosPesquisa.CodigoTipoBidding > 0)
                result = result.Where(o => o.TipoBidding.Codigo == filtrosPesquisa.CodigoTipoBidding);

            if (filtrosPesquisa.CodigoSolicitante > 0)
                result = result.Where(o => o.Solicitante.Codigo == filtrosPesquisa.CodigoSolicitante);

            if (filtrosPesquisa.CodigosComprador != null && filtrosPesquisa.CodigosComprador.Count > 0)
                result = result.Where(o => (from obj in subQueryAprovacaoAlcada where obj.OrigemAprovacao.Codigo == o.Codigo && filtrosPesquisa.CodigosComprador.Contains(obj.Usuario.Codigo) && obj.Situacao == SituacaoAlcadaRegra.Aprovada select obj.OrigemAprovacao.Codigo).Contains(o.Codigo));

            if (filtrosPesquisa.Situacao != null && filtrosPesquisa.Situacao.Count > 0)
                result = result.Where(o => filtrosPesquisa.Situacao.Contains(o.Status));

            if (filtrosPesquisa.NumeroBidding > 0)
                result = result.Where(o => o.Codigo == filtrosPesquisa.NumeroBidding);

            if (filtrosPesquisa.CodigosTransportador != null && filtrosPesquisa.CodigosTransportador.Count > 0)
                result = result.Where(o => o.Convidados.Any(c => filtrosPesquisa.CodigosTransportador.Contains(c.Convidado.Codigo)));

            if (filtrosPesquisa.FiliaisParticipantes != null && filtrosPesquisa.FiliaisParticipantes.Count > 0)
            {
                List<int> codigosConvites = subQueryBiddingOfertaRota.Where(o => o.FiliaisParticipante.Any(x => filtrosPesquisa.FiliaisParticipantes.Contains(x.Codigo))).Select(o => o.BiddingOferta.BiddingConvite.Codigo).Distinct().ToList();

                result = result.Where(o => codigosConvites.Contains(o.Codigo));
            }

            return result;
        }

        #endregion Métodos Privados
    }
}

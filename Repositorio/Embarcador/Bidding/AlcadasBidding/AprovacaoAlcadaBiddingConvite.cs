using System;
using System.Linq;
using System.Collections.Generic;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace Repositorio.Embarcador.Bidding.AlcadasBidding
{
    public sealed class AprovacaoAlcadaBiddingConvite : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite,
        Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding,
        Dominio.Entidades.Embarcador.Bidding.BiddingConvite
    >
    {
        #region Construtores

        public AprovacaoAlcadaBiddingConvite(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion Construtores

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Bidding.BiddingConvite> Consultar(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaBiddingConvite = Consultar(filtrosPesquisa);

            return ObterLista(consultaBiddingConvite, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingAprovacao filtrosPesquisa)
        {
            var consultaBiddingConvite = Consultar(filtrosPesquisa);

            return consultaBiddingConvite.Count();
        }

        public bool PossuiRegrasCadastradas()
        {
            var consultaAlcadaBiddingConvite = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.RegraAutorizacaoBidding>()
                .Where(o => o.Ativo);

            return consultaAlcadaBiddingConvite.Any();
        }

        public List<string> BuscarEmailsAprovadoresPorBiddingConvite(int codigoConvite)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite>();

            var result = query.Where(o => o.OrigemAprovacao.Codigo == codigoConvite && o.Usuario.NotificarEtapasBidding);
            
            return result.Select(o => o.Usuario.Email).Distinct().ToList();
        }

        #endregion Métodos Públicos

        #region Métodos Privados


        private IQueryable<Dominio.Entidades.Embarcador.Bidding.BiddingConvite> Consultar(Dominio.ObjetosDeValor.Embarcador.Bidding.FiltroPesquisaBiddingAprovacao filtrosPesquisa)
        {
            var consultaBiddingConvite = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.BiddingConvite>()
                .Where(x => x.Status == StatusBiddingConvite.AguardandoAprovacao || x.Status == StatusBiddingConvite.AprovacaoRejeitada);
            var consultaAlcadaBiddingConvite = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Bidding.AlcadasBidding.AprovacaoAlcadaBiddingConvite>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.Numero > 0)
                consultaBiddingConvite = consultaBiddingConvite.Where(o => o.Codigo == filtrosPesquisa.Numero);

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                consultaBiddingConvite = consultaBiddingConvite.Where(o => o.DataInicio.Date >= filtrosPesquisa.DataInicio.Date);

            if (filtrosPesquisa.DataLimite != DateTime.MinValue)
                consultaBiddingConvite = consultaBiddingConvite.Where(o => o.DataLimite.Date <= filtrosPesquisa.DataLimite.Date);

            if (filtrosPesquisa.CodigoTipoBidding > 0)
                consultaBiddingConvite = consultaBiddingConvite.Where(o => o.TipoBidding.Codigo == filtrosPesquisa.CodigoTipoBidding);

            if (filtrosPesquisa.CodigoSolicitante > 0)
                consultaBiddingConvite = consultaBiddingConvite.Where(o => o.Solicitante.Codigo == filtrosPesquisa.CodigoSolicitante);

            return consultaBiddingConvite.Where(o => consultaAlcadaBiddingConvite.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());
        }

        #endregion Métodos Privados
    }
}

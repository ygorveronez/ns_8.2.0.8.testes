using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Logistica.AlcadasTermoQuitacao
{
    public class AprovacaoAlcadaTermoQuitacao : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao,
        Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.RegraAutorizacaoTermoQuitacao,
        Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao
    >
    {
        #region Construtores

        public AprovacaoAlcadaTermoQuitacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTermoQuitacaoAprovacao filtrosPesquisa)
        {
            var consultaTermoQuitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao>();
            var consultaAlcadaTermoQuitacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.AlcadasTermoQuitacao.AprovacaoAlcadaTermoQuitacao>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.Numero > 0)
                consultaTermoQuitacao = consultaTermoQuitacao.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.CodigoTransportador > 0)
                consultaTermoQuitacao = consultaTermoQuitacao.Where(o => o.Transportador.Codigo == filtrosPesquisa.CodigoTransportador);

            if (filtrosPesquisa.DataBaseInicial.HasValue)
                consultaTermoQuitacao = consultaTermoQuitacao.Where(o => o.DataBase >= filtrosPesquisa.DataBaseInicial.Value);

            if (filtrosPesquisa.DataBaseLimite.HasValue)
                consultaTermoQuitacao = consultaTermoQuitacao.Where(o => o.DataBase <= filtrosPesquisa.DataBaseLimite.Value.Add(System.DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao.HasValue)
                consultaTermoQuitacao = consultaTermoQuitacao.Where(o => o.Situacao == filtrosPesquisa.Situacao);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaTermoQuitacao = consultaAlcadaTermoQuitacao.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoTermoQuitacao.AguardandoAprovacao)
                consultaAlcadaTermoQuitacao = consultaAlcadaTermoQuitacao.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

            return consultaTermoQuitacao.Where(o => consultaAlcadaTermoQuitacao.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.TermoQuitacao.TermoQuitacao> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTermoQuitacaoAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaTermoQuitacao = Consultar(filtrosPesquisa);

            consultaTermoQuitacao = consultaTermoQuitacao
                .Fetch(o => o.Transportador);

            return ObterLista(consultaTermoQuitacao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaTermoQuitacaoAprovacao filtrosPesquisa)
        {
            var consultaTermoQuitacao = Consultar(filtrosPesquisa);

            return consultaTermoQuitacao.Count();
        }

        #endregion
    }
}

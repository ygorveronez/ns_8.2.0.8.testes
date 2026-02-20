using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota
{
    public class AprovacaoAlcadaInfracao : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AprovacaoAlcadaInfracao,
        Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.RegraAutorizacaoInfracao,
        Dominio.Entidades.Embarcador.Frota.Infracao
    >
    {
        #region Construtores

        public AprovacaoAlcadaInfracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.Infracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaInfracaoAprovacao filtrosPesquisa)
        {
            var consultaInfracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.Infracao>();
            var consultaAlcadaInfracao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AprovacaoAlcadaInfracao>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.CodigoTipoInfracao > 0)
                consultaInfracao = consultaInfracao.Where(o => o.TipoInfracao.Codigo == filtrosPesquisa.CodigoTipoInfracao);

            if (filtrosPesquisa.Numero > 0)
                consultaInfracao = consultaInfracao.Where(o => o.Numero == filtrosPesquisa.Numero);

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaInfracao = consultaInfracao.Where(o => o.Data >= filtrosPesquisa.DataInicio.Value);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaInfracao = consultaInfracao.Where(o => o.Data <= filtrosPesquisa.DataLimite.Value.Add(System.DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacao.HasValue)
                consultaInfracao = consultaInfracao.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaInfracao = consultaAlcadaInfracao.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoInfracao.AguardandoAprovacao)
                consultaAlcadaInfracao = consultaAlcadaInfracao.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

            return consultaInfracao.Where(o => consultaAlcadaInfracao.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frota.Infracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaInfracaoAprovacao filtrosPesquisa, string propriedadeOrdenacao, string direcaoOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var consultaInfracao = Consultar(filtrosPesquisa);

            return ObterLista(consultaInfracao, propriedadeOrdenacao, direcaoOrdenacao, inicioRegistros, maximoRegistros);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaInfracaoAprovacao filtrosPesquisa)
        {
            var consultaInfracao = Consultar(filtrosPesquisa);

            return consultaInfracao.Count();
        }
        public List<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AprovacaoAlcadaInfracao> BuscarDesbloqueadaPorInfracao(int codigoOrigem)
        {
            var aprovacoes = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.AlcadasInfracao.AprovacaoAlcadaInfracao>()
                .Where(o => !o.Bloqueada && o.OrigemAprovacao.Codigo == codigoOrigem);

            return aprovacoes.ToList();
        }
            
        #endregion
    }
}

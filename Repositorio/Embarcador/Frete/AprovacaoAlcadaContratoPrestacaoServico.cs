using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frete
{
    public sealed class AprovacaoAlcadaContratoPrestacaoServico : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.AprovacaoAlcadaContratoPrestacaoServico,
        Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.RegraAutorizacaoContratoPrestacaoServico,
        Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico
    >
    {
        #region Construtores

        public AprovacaoAlcadaContratoPrestacaoServico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServicoAprovacao filtrosPesquisa)
        {
            var consultaContratoPrestacaoServico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico>();
            var consultaAlcadaContratoPrestacaoServico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frete.AlcadasContratoPrestacaoServico.AprovacaoAlcadaContratoPrestacaoServico>()
                .Where(o => !o.Bloqueada);

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaContratoPrestacaoServico = consultaContratoPrestacaoServico.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.Situacao.HasValue)
                consultaContratoPrestacaoServico = consultaContratoPrestacaoServico.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaContratoPrestacaoServico = consultaAlcadaContratoPrestacaoServico.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoPrestacaoServico.AguardandoAprovacao)
                consultaAlcadaContratoPrestacaoServico = consultaAlcadaContratoPrestacaoServico.Where(o => o.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAlcadaRegra.Pendente);

            return consultaContratoPrestacaoServico.Where(o => consultaAlcadaContratoPrestacaoServico.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Frete.ContratoPrestacaoServico> Consultar(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServicoAprovacao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaCarga = Consultar(filtrosPesquisa);

            return ObterLista(consultaCarga, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaContratoPrestacaoServicoAprovacao filtrosPesquisa)
        {
            var consultaCarga = Consultar(filtrosPesquisa);

            return consultaCarga.Count();
        }

        #endregion
    }
}

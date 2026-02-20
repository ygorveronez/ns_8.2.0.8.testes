using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Embarcador.ICMS.AlcadasAlteracaoRegraICMS
{
    public sealed class AprovacaoAlcadaAlteracaoRegraICMS : RegraAutorizacao.AprovacaoAlcada<
        Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.AprovacaoAlcadaAlteracaoRegraICMS,
        Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.RegraAutorizacaoAlteracaoRegraICMS,
        Dominio.Entidades.Embarcador.ICMS.RegraICMS
    >
    {
        #region Construtores

        public AprovacaoAlcadaAlteracaoRegraICMS(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.ICMS.RegraICMS> Consultar(Dominio.ObjetosDeValor.Embarcador.ICMS.FiltroPesquisaAlteracaoRegraICMS filtrosPesquisa)
        {
            var consultaAlteracaoRegraICMS = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.RegraICMS>();

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaAlteracaoRegraICMS = consultaAlteracaoRegraICMS.Where(o => o.DataAlteracao.Value.Date >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaAlteracaoRegraICMS = consultaAlteracaoRegraICMS.Where(o => o.DataAlteracao.Value.Date <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaAlteracaoRegraICMS = consultaAlteracaoRegraICMS.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao) || o.RegraOriginaria.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.SituacaoAlteracao.HasValue)
            {
                consultaAlteracaoRegraICMS = consultaAlteracaoRegraICMS.Where(o => o.SituacaoAlteracao == filtrosPesquisa.SituacaoAlteracao.Value);

                if (filtrosPesquisa.SituacaoAlteracao.Value == SituacaoAlteracaoRegraICMS.SemRegraAprovacao)
                    return consultaAlteracaoRegraICMS;
            }

            var consultaAlcadaAlteracaoRegraICMS = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.ICMS.AlcadasAlteracaoRegraICMS.AprovacaoAlcadaAlteracaoRegraICMS>()
                .Where(o => !o.Bloqueada);

            if (filtrosPesquisa.CodigoUsuario > 0)
                consultaAlcadaAlteracaoRegraICMS = consultaAlcadaAlteracaoRegraICMS.Where(o => o.Usuario.Codigo == filtrosPesquisa.CodigoUsuario);

            if (filtrosPesquisa.SituacaoAlteracao == SituacaoAlteracaoRegraICMS.AguardandoAprovacao)
                consultaAlcadaAlteracaoRegraICMS = consultaAlcadaAlteracaoRegraICMS.Where(o => o.Situacao == SituacaoAlcadaRegra.Pendente);

            if (filtrosPesquisa.SituacaoAlteracao.HasValue)
                return consultaAlteracaoRegraICMS.Where(o => consultaAlcadaAlteracaoRegraICMS.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any());

            return consultaAlteracaoRegraICMS.Where(o =>
                o.SituacaoAlteracao == SituacaoAlteracaoRegraICMS.SemRegraAprovacao ||
                consultaAlcadaAlteracaoRegraICMS.Where(a => a.OrigemAprovacao.Codigo == o.Codigo).Any()
            );
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.ICMS.RegraICMS> Consultar(Dominio.ObjetosDeValor.Embarcador.ICMS.FiltroPesquisaAlteracaoRegraICMS filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaAlteracaoRegraICMS = Consultar(filtrosPesquisa);

            consultaAlteracaoRegraICMS = consultaAlteracaoRegraICMS
                .Fetch(o => o.UFEmitente)
                .Fetch(o => o.UFDestino)
                .Fetch(o => o.UFOrigem)
                .Fetch(o => o.UFTomador)
                .Fetch(o => o.Destinatario)
                .Fetch(o => o.Remetente)
                .Fetch(o => o.Tomador)
                .Fetch(o => o.GrupoDestinatario)
                .Fetch(o => o.GrupoRemetente)
                .Fetch(o => o.GrupoTomador);

            return ObterLista(consultaAlteracaoRegraICMS, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.ICMS.FiltroPesquisaAlteracaoRegraICMS filtrosPesquisa)
        {
            var consultaAlteracaoRegraICMS = Consultar(filtrosPesquisa);

            return consultaAlteracaoRegraICMS.Count();
        }

        #endregion
    }
}

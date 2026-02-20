using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositorio.Auditoria
{
    public class HistoricoIntegracao : RepositorioBase<Dominio.Entidades.Auditoria.HistoricoIntegracao>
    {
        #region Construtores

        public HistoricoIntegracao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Auditoria.HistoricoIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa)
        {
            var consultaHistoricoIntegracao = this.SessionNHiBernate.Query<Dominio.Entidades.Auditoria.HistoricoIntegracao>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CodigoIntegracao))
                consultaHistoricoIntegracao = consultaHistoricoIntegracao.Where(o => o.CodigoIntegracao.Contains(filtrosPesquisa.CodigoIntegracao));

            if (filtrosPesquisa.CodigoIntegradora > 0)
                consultaHistoricoIntegracao = consultaHistoricoIntegracao.Where(o => o.Integradora.Codigo == filtrosPesquisa.CodigoIntegradora);

            if (filtrosPesquisa.OrigemAuditado.HasValue)
                consultaHistoricoIntegracao = consultaHistoricoIntegracao.Where(o => o.Origem == filtrosPesquisa.OrigemAuditado.Value);

            if (filtrosPesquisa.DataInicio.HasValue)
                consultaHistoricoIntegracao = consultaHistoricoIntegracao.Where(o => o.Data >= filtrosPesquisa.DataInicio.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaHistoricoIntegracao = consultaHistoricoIntegracao.Where(o => o.Data <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            return consultaHistoricoIntegracao;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Auditoria.HistoricoIntegracao> Consultar(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaHistoricoIntegracao = Consultar(filtrosPesquisa);

            return ObterLista(consultaHistoricoIntegracao, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaHistoricoObjetoIntegracao filtrosPesquisa)
        {
            var consultaHistoricoIntegracao = Consultar(filtrosPesquisa);

            return consultaHistoricoIntegracao.Count();
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Chamados
{
    public class GrupoMotivoChamado : RepositorioBase<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado>
    {
        public GrupoMotivoChamado(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado> Consultar(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaGrupoMotivoChamado filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta)
        {
            var result = Consultar(filtrosPesquisa);

            return ObterLista(result, parametroConsulta);
        }

        public int ContarConsulta (Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaGrupoMotivoChamado filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado BuscarPorTipoOperacaoERecebeOcorrenciaERP(int tipoOperacaoCodigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado>();

            return query.Where(grupoMotivo => grupoMotivo.RecebeOcorrenciaERP && grupoMotivo.TiposOperacao.Any(tp => tp.Codigo == tipoOperacaoCodigo)).FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado> Consultar(Dominio.ObjetosDeValor.Embarcador.Chamado.FiltroPesquisaGrupoMotivoChamado filtro)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Chamados.GrupoMotivoChamado>();

            if (!string.IsNullOrWhiteSpace(filtro.Descricao))
                query = query.Where(o => o.Descricao.Contains(filtro.Descricao));

            if (!string.IsNullOrWhiteSpace(filtro.CodigoIntegracao))
                query = query.Where(o => o.CodigoIntegracao.Contains(filtro.CodigoIntegracao));

            if (filtro.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Todos)
            {
                if (filtro.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                    query = query.Where(o => o.Situacao);
                else
                    query = query.Where(o => !o.Situacao);
            }

            return query;
        }

        #endregion
    }
}

using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public sealed class OcorrenciaPatioTipo : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo>
    {
        #region Construtores

        public OcorrenciaPatioTipo(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaOcorrenciaPatioTipo filtrosPesquisa)
        {
            var consultaOcorrenciaPatioTipo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo>();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Descricao))
                consultaOcorrenciaPatioTipo = consultaOcorrenciaPatioTipo.Where(o => o.Descricao.Contains(filtrosPesquisa.Descricao));

            if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Ativo)
                consultaOcorrenciaPatioTipo = consultaOcorrenciaPatioTipo.Where(o => o.Ativo);
            else if (filtrosPesquisa.SituacaoAtivo == SituacaoAtivoPesquisa.Inativo)
                consultaOcorrenciaPatioTipo = consultaOcorrenciaPatioTipo.Where(o => !o.Ativo);

            if (filtrosPesquisa.Tipo.HasValue)
                consultaOcorrenciaPatioTipo = consultaOcorrenciaPatioTipo.Where(o => o.Tipo == filtrosPesquisa.Tipo.Value);

            return consultaOcorrenciaPatioTipo;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo BuscarAtivoPorTipo(TipoOcorrenciaPatio tipo)
        {
            var consultaOcorrenciaPatioTipo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo>()
                .Where(o => o.Ativo && o.Tipo == tipo);

            return consultaOcorrenciaPatioTipo.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo> BuscarAtivos()
        {
            var consultaOcorrenciaPatioTipo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo>()
                .Where(o => o.Ativo);

            return consultaOcorrenciaPatioTipo.ToList();
        }

        public Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo BuscarPorCodigo(int codigo)
        {
            var consultaOcorrenciaPatioTipo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo>()
                .Where(o => o.Codigo == codigo);

            return consultaOcorrenciaPatioTipo.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatioTipo> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaOcorrenciaPatioTipo filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaOcorrenciaPatioTipo = Consultar(filtrosPesquisa);

            return ObterLista(consultaOcorrenciaPatioTipo, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaOcorrenciaPatioTipo filtrosPesquisa)
        {
            var consultaOcorrenciaPatioTipo = Consultar(filtrosPesquisa);

            return consultaOcorrenciaPatioTipo.Count();
        }

        #endregion
    }
}

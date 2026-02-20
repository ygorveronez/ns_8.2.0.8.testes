using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.GestaoPatio
{
    public sealed class OcorrenciaPatio : RepositorioBase<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio>
    {
        #region Construtores

        public OcorrenciaPatio(Repositorio.UnitOfWork unitOfWork) : base (unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaOcorrenciaPatio filtrosPesquisa)
        {
            var consultaOcorrenciaPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio>();

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaOcorrenciaPatio = consultaOcorrenciaPatio.Where(o => o.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);

            if (filtrosPesquisa.DataInicial.HasValue)
                consultaOcorrenciaPatio = consultaOcorrenciaPatio.Where(o => o.DataGeracao >= filtrosPesquisa.DataInicial.Value.Date);

            if (filtrosPesquisa.DataLimite.HasValue)
                consultaOcorrenciaPatio = consultaOcorrenciaPatio.Where(o => o.DataGeracao <= filtrosPesquisa.DataLimite.Value.Date.Add(DateTime.MaxValue.TimeOfDay));

            if (filtrosPesquisa.Situacoes?.Count > 0)
                consultaOcorrenciaPatio = consultaOcorrenciaPatio.Where(o => filtrosPesquisa.Situacoes.Contains(o.Situacao));

            if (filtrosPesquisa.TipoLancamento.HasValue)
                consultaOcorrenciaPatio = consultaOcorrenciaPatio.Where(o => o.TipoLancamento == filtrosPesquisa.TipoLancamento.Value);

            return consultaOcorrenciaPatio;
        }

        #endregion

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio BuscarPorCodigo(int codigo)
        {
            var consultaOcorrenciaPatio = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio>()
                .Where(o => o.Codigo == codigo);

            return consultaOcorrenciaPatio.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.GestaoPatio.OcorrenciaPatio> Consultar(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaOcorrenciaPatio filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaOcorrenciaPatio = Consultar(filtrosPesquisa);

            return ObterLista(consultaOcorrenciaPatio, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.GestaoPatio.FiltroPesquisaOcorrenciaPatio filtrosPesquisa)
        {
            var consultaOcorrenciaPatio = Consultar(filtrosPesquisa);

            return consultaOcorrenciaPatio.Count();
        }

        #endregion
    }
}

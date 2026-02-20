using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;

namespace Repositorio.Embarcador.Logistica
{
    public sealed class ManobraTracaoHistorico : RepositorioBase<Dominio.Entidades.Embarcador.Logistica.ManobraTracaoHistorico>
    {
        #region Construtores

        public ManobraTracaoHistorico(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Logistica.ManobraTracaoHistorico> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraTracaoHistorico filtrosPesquisa)
        {
            var consultaManobraHistorico = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Logistica.ManobraTracaoHistorico>();

            if (filtrosPesquisa.CodigoCentroCarregamento > 0)
                consultaManobraHistorico = consultaManobraHistorico.Where(o => o.Manobra.CentroCarregamento.Codigo == filtrosPesquisa.CodigoCentroCarregamento);

            if (filtrosPesquisa.CodigoTracao > 0)
                consultaManobraHistorico = consultaManobraHistorico.Where(o => o.Tracao.Codigo == filtrosPesquisa.CodigoTracao);

            return consultaManobraHistorico;
        }

        #endregion

        #region Métodos Públicos

        public List<Dominio.Entidades.Embarcador.Logistica.ManobraTracaoHistorico> Consultar(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraTracaoHistorico filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaManobraHistorico = Consultar(filtrosPesquisa);

            return ObterLista(consultaManobraHistorico, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaManobraTracaoHistorico filtrosPesquisa)
        {
            var consultaManobraHistorico = Consultar(filtrosPesquisa);

            return consultaManobraHistorico.Count();
        }

        #endregion
    }
}

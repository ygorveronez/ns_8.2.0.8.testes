using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Frota
{
    public class DespesaFrotaPropria : RepositorioBase<Dominio.Entidades.Embarcador.Frota.DespesaFrotaPropria>
    {
        public DespesaFrotaPropria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Globais

        public List<Dominio.Entidades.Embarcador.Frota.DespesaFrotaPropria> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaDespesaFrotaPropria filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consultaDespesaFrotaPropria = Consultar(filtrosPesquisa);

            return ObterLista(consultaDespesaFrotaPropria, parametrosConsulta);
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaDespesaFrotaPropria filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Frota.DespesaFrotaPropria> Consultar(Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaDespesaFrotaPropria filtrosPesquisa)
        {
            var consultaDespesaFrotaPropria = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Frota.DespesaFrotaPropria>();

            if (filtrosPesquisa.CodigoFilial > 0)
                consultaDespesaFrotaPropria = consultaDespesaFrotaPropria.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoFilial);

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                consultaDespesaFrotaPropria = consultaDespesaFrotaPropria.Where(o => o.Data >= filtrosPesquisa.DataInicial);

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                consultaDespesaFrotaPropria = consultaDespesaFrotaPropria.Where(o => o.Data <= filtrosPesquisa.DataFinal);

            return consultaDespesaFrotaPropria;
        }

        #endregion
    }
}

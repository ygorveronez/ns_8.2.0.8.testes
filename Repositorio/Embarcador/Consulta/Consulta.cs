using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace Repositorio.Embarcador.Consulta
{
    public abstract class Consulta<TFiltroPesquisa> where TFiltroPesquisa : class
    {
        #region Atributos Privados Somente Leitura

        protected readonly string _tabela;
        protected readonly bool _somenteRegistrosDistintos;


        #endregion

        #region Construtores

        public Consulta(string tabela) : this(tabela, somenteRegistrosDistintos: false) { }

        public Consulta(string tabela, bool somenteRegistrosDistintos)
        {
            _somenteRegistrosDistintos = somenteRegistrosDistintos;
            _tabela = tabela;
        }

        #endregion

        #region Métodos Protegidos

        protected virtual SQLDinamico ObterSql(TFiltroPesquisa filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, bool somenteContarNumeroRegistros)
        {
            StringBuilder groupBy = new StringBuilder();
            StringBuilder joins = new StringBuilder();
            StringBuilder orderBy = new StringBuilder();
            StringBuilder select = new StringBuilder();
            StringBuilder sql = new StringBuilder();
            StringBuilder where = new StringBuilder();
            List<ParametroSQL> parametrosWhere = new List<ParametroSQL>();


            foreach (Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento propriedade in propriedades)
                SetarSelect(propriedade.Propriedade, propriedade.CodigoDinamico, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);

            SetarOrderBy(parametrosConsulta, select, orderBy, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);
            SetarWhere(filtrosPesquisa, where, joins, groupBy, parametrosWhere);

            string campos = select.ToString().Trim();
            string agrupamentos = groupBy.ToString().Trim();
            string condicoes = where.ToString().Trim();

            if (somenteContarNumeroRegistros)
                sql.Append("select distinct(count(0) over ()) ");
            else
                sql.Append($"select {(_somenteRegistrosDistintos ? "distinct " : "")}{(campos.Length > 0 ? campos.Substring(0, campos.Length - 1) : "")} "); 

            sql.Append($" from {_tabela} ");
            sql.Append(joins.ToString());

            if (condicoes.Length > 0)
                sql.Append($" where {condicoes.Substring(4)} ");

            if (agrupamentos.Length > 0)
                sql.Append($" group by {agrupamentos.Substring(0, agrupamentos.Length - 1)} ");

            if (!somenteContarNumeroRegistros)
            {
                sql.Append($" order by {(orderBy.Length > 0 ? orderBy.ToString() : "1 asc")}");

                if ((parametrosConsulta != null) && ((parametrosConsulta.InicioRegistros > 0) || (parametrosConsulta.LimiteRegistros > 0)))
                    sql.Append($" offset {parametrosConsulta.InicioRegistros} rows fetch next {parametrosConsulta.LimiteRegistros} rows only;");
            }

            return new SQLDinamico(sql.ToString(), parametrosWhere);
        }

        protected void SetarOrderBy(Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, StringBuilder select, StringBuilder orderBy, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, TFiltroPesquisa filtrosPesquisa)
        {
            if (somenteContarNumeroRegistros)
                return;

            if (parametrosConsulta == null)
                return;

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeAgrupar))
            {
                SetarSelect(parametrosConsulta.PropriedadeAgrupar, 0, select, joins, groupBy, somenteContarNumeroRegistros, filtrosPesquisa);

                if (select.Contains(parametrosConsulta.PropriedadeAgrupar))
                    orderBy.Append($"{parametrosConsulta.PropriedadeAgrupar} {parametrosConsulta.DirecaoAgrupar}");
            }

            if (!string.IsNullOrWhiteSpace(parametrosConsulta.PropriedadeOrdenar) && (parametrosConsulta.PropriedadeOrdenar != parametrosConsulta.PropriedadeAgrupar) && select.Contains(parametrosConsulta.PropriedadeOrdenar))
            {
                if (orderBy.Length > 0)
                    orderBy.Append(", ");

                orderBy.Append($"{parametrosConsulta.PropriedadeOrdenar} {parametrosConsulta.DirecaoOrdenar}");
            }
        }

        protected abstract void SetarSelect(string propriedade, int codigoDinamico, StringBuilder select, StringBuilder joins, StringBuilder groupBy, bool somenteContarNumeroRegistros, TFiltroPesquisa filtrosPesquisa);

        protected abstract void SetarWhere(TFiltroPesquisa filtrosPesquisa, StringBuilder where, StringBuilder joins, StringBuilder groupBy, List<Embarcador.Consulta.ParametroSQL> parametros = null);
        



        #endregion

        #region Métodos Públicos

        public SQLDinamico ObterSqlContarPesquisa(TFiltroPesquisa filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            return ObterSql(filtrosPesquisa, parametrosConsulta: null, propriedades: propriedades, somenteContarNumeroRegistros: true);
        }


        public SQLDinamico ObterSqlPesquisa(TFiltroPesquisa filtrosPesquisa, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            return ObterSql(filtrosPesquisa, parametrosConsulta, propriedades, somenteContarNumeroRegistros: false);
        }

        #endregion
    }
}

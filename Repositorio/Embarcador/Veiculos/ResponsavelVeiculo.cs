using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Veiculos
{
    public class ResponsavelVeiculo : RepositorioBase<Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo>
    {
        public ResponsavelVeiculo(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Globais

        public List<Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo> Consultar(int codigoVeiculo, int codigoFuncionarioResponsavel, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(codigoVeiculo, codigoFuncionarioResponsavel);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoVeiculo, int codigoFuncionarioResponsavel)
        {
            var result = Consultar(codigoVeiculo, codigoFuncionarioResponsavel);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo> BuscarResponsaveisVeiculo(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo>();
            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo select obj;
            return result.ToList();
        }
        public Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo BuscarPorVeiculo(int codigoVeiculo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo>();
            var result = from obj in query where obj.Veiculo.Codigo == codigoVeiculo select obj;
            return result.FirstOrDefault();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo> Consultar(int codigoVeiculo, int codigoFuncionarioResponsavel)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Veiculos.ResponsavelVeiculo>();

            var result = from obj in query select obj;

            if (codigoVeiculo > 0)
                result = result.Where(obj => obj.Veiculo.Codigo == codigoVeiculo);

            if (codigoFuncionarioResponsavel > 0)
                result = result.Where(obj => obj.FuncionarioResponsavel.Codigo == codigoFuncionarioResponsavel);

            return result;
        }

        #endregion

        #region Relatório de Responsável por Veículo

        public IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.ResponsavelVeiculo> ConsultarRelatorioResponsavelVeiculo(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioResponsavelVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioResponsavelVeiculo(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Veiculos.ResponsavelVeiculo)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.ResponsavelVeiculo>();
        }

        public int ContarConsultaRelatorioResponsavelVeiculo(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioResponsavelVeiculo filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            string sql = ObterSelectConsultaRelatorioResponsavelVeiculo(filtrosPesquisa, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioResponsavelVeiculo(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioResponsavelVeiculo filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioResponsavelVeiculo(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioResponsavelVeiculo(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioResponsavelVeiculo(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                }
            }

            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += " FROM T_RESPONSAVEL_VEICULO ResponsavelVeiculo ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            // ORDER BY
            if (orderBy.Length > 0)
                query += " ORDER BY " + orderBy;
            else if (!count)
                query += " ORDER BY 1 ASC";

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaRelatorioResponsavelVeiculo(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "ResponsavelVeiculo.REV_CODIGO Codigo, ";
                        groupBy += "ResponsavelVeiculo.REV_CODIGO, ";
                    }
                    break;
                case "DataLancamentoFormatada":
                    if (!select.Contains(" DataLancamento, "))
                    {
                        select += "ResponsavelVeiculo.REV_DATA_LANCAMENTO DataLancamento, ";
                        groupBy += "ResponsavelVeiculo.REV_DATA_LANCAMENTO, ";
                    }
                    break;
                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select += "ResponsavelVeiculo.REV_OBSERVACAO Observacao, ";
                        groupBy += "ResponsavelVeiculo.REV_OBSERVACAO, ";
                    }
                    break;
                case "Placa":
                    if (!select.Contains(" Placa, "))
                    {
                        if (!joins.Contains(" Veiculo "))
                            joins += " LEFT JOIN T_VEICULO Veiculo ON Veiculo.VEI_CODIGO = ResponsavelVeiculo.VEI_CODIGO";

                        select += "Veiculo.VEI_PLACA Placa, ";
                        groupBy += "Veiculo.VEI_PLACA, ";
                    }
                    break;
                case "FuncionarioResponsavel":
                    if (!select.Contains(" FuncionarioResponsavel, "))
                    {
                        if (!joins.Contains(" FuncionarioResponsavel "))
                            joins += " LEFT JOIN T_FUNCIONARIO FuncionarioResponsavel ON FuncionarioResponsavel.FUN_CODIGO = ResponsavelVeiculo.FUN_CODIGO_RESPONSAVEL";

                        select += "FuncionarioResponsavel.FUN_NOME FuncionarioResponsavel, ";
                        groupBy += "FuncionarioResponsavel.FUN_NOME, ";
                    }
                    break;
                case "FuncionarioLancamento":
                    if (!select.Contains(" FuncionarioLancamento, "))
                    {
                        if (!joins.Contains(" FuncionarioLancamento "))
                            joins += " LEFT JOIN T_FUNCIONARIO FuncionarioLancamento ON FuncionarioLancamento.FUN_CODIGO = ResponsavelVeiculo.FUN_CODIGO_LANCAMENTO";

                        select += "FuncionarioLancamento.FUN_NOME FuncionarioLancamento, ";
                        groupBy += "FuncionarioLancamento.FUN_NOME, ";
                    }
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioResponsavelVeiculo(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioResponsavelVeiculo filtrosPesquisa)
        {
            if (filtrosPesquisa.CodigosVeiculo != null && filtrosPesquisa.CodigosVeiculo.Count > 0)
                where += " and ResponsavelVeiculo.VEI_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosVeiculo) + ")";

            if (filtrosPesquisa.CodigosFuncionarioResponsavel != null && filtrosPesquisa.CodigosFuncionarioResponsavel.Count > 0)
                where += " and ResponsavelVeiculo.FUN_CODIGO_RESPONSAVEL in (" + string.Join(", ", filtrosPesquisa.CodigosFuncionarioResponsavel) + ")";
        }

        #endregion
    }
}

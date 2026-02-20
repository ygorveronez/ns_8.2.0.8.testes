using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro.DespesaMensal
{
    public class DespesaMensal : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal>
    {
        public DespesaMensal(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal> Consultar(int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Situacao);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Situacao);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigoEmpresa, string descricao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa status)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.DespesaMensal.DespesaMensal>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Ativo)
                result = result.Where(o => o.Situacao);
            else if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoAtivoPesquisa.Inativo)
                result = result.Where(o => !o.Situacao);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            return result.Count();
        }

        #region Relatório Despesas Mensais

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DespesaMensal> ConsultarRelatorioDespesaMensal(int codigoEmpresa, DateTime dataInicialGeracao, DateTime dataFinalGeracao, DateTime dataInicialPagamento, DateTime dataFinalPagamento, int tipoDespesaFinanceira, double pessoa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioDespesaMensal(codigoEmpresa, dataInicialGeracao, dataFinalGeracao, dataInicialPagamento, dataFinalPagamento, tipoDespesaFinanceira, pessoa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.DespesaMensal)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DespesaMensal>();
        }

        public int ContarConsultaRelatorioDespesaMensal(int codigoEmpresa, DateTime dataInicialGeracao, DateTime dataFinalGeracao, DateTime dataInicialPagamento, DateTime dataFinalPagamento, int tipoDespesaFinanceira, double pessoa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            string sql = ObterSelectConsultaRelatorioDespesaMensal(codigoEmpresa, dataInicialGeracao, dataFinalGeracao, dataInicialPagamento, dataFinalPagamento, tipoDespesaFinanceira, pessoa, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioDespesaMensal(int codigoEmpresa, DateTime dataInicialGeracao, DateTime dataFinalGeracao, DateTime dataInicialPagamento, DateTime dataFinalPagamento, int tipoDespesaFinanceira, double pessoa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioDespesaMensal(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioDespesaMensal(ref where, ref groupBy, ref joins, codigoEmpresa, dataInicialGeracao, dataFinalGeracao, dataInicialPagamento, dataFinalPagamento, tipoDespesaFinanceira, pessoa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioDespesaMensal(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

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
            query += " FROM T_DESPESA_MENSAL DespesaMensal ";

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

        private void SetarSelectRelatorioConsultaRelatorioDespesaMensal(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "DespesaMensal.DME_CODIGO Codigo, ";
                        groupBy += "DespesaMensal.DME_CODIGO, ";
                    }
                    break;
                case "Pessoa":
                    if (!select.Contains(" Pessoa, "))
                    {
                        if (!joins.Contains(" Pessoa "))
                            joins += " LEFT OUTER JOIN T_CLIENTE Pessoa ON Pessoa.CLI_CGCCPF = DespesaMensal.CLI_CGCCPF";

                        select += "Pessoa.CLI_NOME Pessoa, ";
                        groupBy += "Pessoa.CLI_NOME, ";
                    }
                    break;
                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select += "DespesaMensal.DME_DESCRICAO Descricao, ";
                        groupBy += "DespesaMensal.DME_DESCRICAO, ";
                    }
                    break;
                case "DiaProvisao":
                    if (!select.Contains(" DiaProvisao, "))
                    {
                        select += "DespesaMensal.DME_DIA_PROVISAO DiaProvisao, ";
                        groupBy += "DespesaMensal.DME_DIA_PROVISAO, ";
                    }
                    break;
                case "ValorProvisao":
                    if (!select.Contains(" ValorProvisao, "))
                    {
                        select += "SUM(DespesaMensal.DME_VALOR_PROVISAO) ValorProvisao, ";
                    }
                    break;
                case "TipoDespesa":
                    if (!select.Contains(" TipoDespesa, "))
                    {
                        if (!joins.Contains(" TipoDespesa "))
                            joins += " JOIN T_TIPO_DESPESA_FINANCEIRA TipoDespesa ON TipoDespesa.TID_CODIGO = DespesaMensal.TID_CODIGO";

                        select += "TipoDespesa.TID_DESCRICAO TipoDespesa, ";
                        groupBy += "TipoDespesa.TID_DESCRICAO, ";
                    }
                    break;
                case "DataGeracao":
                    if (!select.Contains(" DataGeracao, "))
                    {
                        if (!joins.Contains(" DespesaMensalProcessamentoDespesas "))
                            joins += " LEFT OUTER JOIN T_DESPESA_MENSAL_PROCESSAMENTO_DESPESAS DespesaMensalProcessamentoDespesas ON DespesaMensalProcessamentoDespesas.DME_CODIGO = DespesaMensal.DME_CODIGO";

                        if (!joins.Contains(" DespesaMensalProcessamento "))
                            joins += " LEFT OUTER JOIN T_DESPESA_MENSAL_PROCESSAMENTO DespesaMensalProcessamento ON DespesaMensalProcessamento.DMP_CODIGO = DespesaMensalProcessamentoDespesas.DMP_CODIGO";

                        select += "DespesaMensalProcessamento.DMP_DATA DataGeracao, ";
                        groupBy += "DespesaMensalProcessamento.DMP_DATA, ";
                    }
                    break;
                case "CodigoTitulo":
                    if (!select.Contains(" CodigoTitulo, "))
                    {
                        if (!joins.Contains(" DespesaMensalProcessamentoDespesas "))
                            joins += " LEFT OUTER JOIN T_DESPESA_MENSAL_PROCESSAMENTO_DESPESAS DespesaMensalProcessamentoDespesas ON DespesaMensalProcessamentoDespesas.DME_CODIGO = DespesaMensal.DME_CODIGO";

                        select += "DespesaMensalProcessamentoDespesas.TIT_CODIGO CodigoTitulo, ";
                        groupBy += "DespesaMensalProcessamentoDespesas.TIT_CODIGO, ";
                    }
                    break;
                case "ValorTitulo":
                    if (!select.Contains(" ValorTitulo, "))
                    {
                        if (!joins.Contains(" DespesaMensalProcessamentoDespesas "))
                            joins += " LEFT OUTER JOIN T_DESPESA_MENSAL_PROCESSAMENTO_DESPESAS DespesaMensalProcessamentoDespesas ON DespesaMensalProcessamentoDespesas.DME_CODIGO = DespesaMensal.DME_CODIGO";

                        if (!joins.Contains(" Titulo "))
                            joins += " LEFT OUTER JOIN T_TITULO Titulo ON Titulo.TIT_CODIGO = DespesaMensalProcessamentoDespesas.TIT_CODIGO";

                        select += "SUM(Titulo.TIT_VALOR_ORIGINAL) ValorTitulo, ";
                    }
                    break;
                case "DataPagamento":
                    if (!select.Contains(" DataPagamento, "))
                    {
                        if (!joins.Contains(" DespesaMensalProcessamentoDespesas "))
                            joins += " LEFT OUTER JOIN T_DESPESA_MENSAL_PROCESSAMENTO_DESPESAS DespesaMensalProcessamentoDespesas ON DespesaMensalProcessamentoDespesas.DME_CODIGO = DespesaMensal.DME_CODIGO";

                        if (!joins.Contains(" Titulo "))
                            joins += " LEFT OUTER JOIN T_TITULO Titulo ON Titulo.TIT_CODIGO = DespesaMensalProcessamentoDespesas.TIT_CODIGO";

                        select += "Titulo.TIT_DATA_LIQUIDACAO DataPagamento, ";
                        groupBy += "Titulo.TIT_DATA_LIQUIDACAO, ";
                    }
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioDespesaMensal(ref string where, ref string groupBy, ref string joins, int codigoEmpresa, DateTime dataInicialGeracao, DateTime dataFinalGeracao, DateTime dataInicialPagamento, DateTime dataFinalPagamento, int tipoDespesaFinanceira, double pessoa)
        {
            string pattern = "yyyy-MM-dd";

            if (!joins.Contains(" DespesaMensalProcessamentoDespesas "))
                joins += " LEFT OUTER JOIN T_DESPESA_MENSAL_PROCESSAMENTO_DESPESAS DespesaMensalProcessamentoDespesas ON DespesaMensalProcessamentoDespesas.DME_CODIGO = DespesaMensal.DME_CODIGO";

            if (!joins.Contains(" DespesaMensalProcessamento "))
                joins += " LEFT OUTER JOIN T_DESPESA_MENSAL_PROCESSAMENTO DespesaMensalProcessamento ON DespesaMensalProcessamento.DMP_CODIGO = DespesaMensalProcessamentoDespesas.DMP_CODIGO";

            if (!joins.Contains(" Titulo "))
                joins += " LEFT OUTER JOIN T_TITULO Titulo ON Titulo.TIT_CODIGO = DespesaMensalProcessamentoDespesas.TIT_CODIGO";

            if (codigoEmpresa > 0)
                where += " AND DespesaMensal.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (dataInicialGeracao != DateTime.MinValue)
                where += " AND DespesaMensalProcessamento.DMP_DATA >= '" + dataInicialGeracao.ToString(pattern) + "' ";

            if (dataFinalGeracao != DateTime.MinValue)
                where += " AND DespesaMensalProcessamento.DMP_DATA <= '" + dataFinalGeracao.AddDays(1).ToString(pattern) + "'";

            if (dataInicialPagamento != DateTime.MinValue)
                where += " AND Titulo.TIT_DATA_LIQUIDACAO >= '" + dataInicialPagamento.ToString(pattern) + "' ";

            if (dataFinalPagamento != DateTime.MinValue)
                where += " AND Titulo.TIT_DATA_LIQUIDACAO <= '" + dataFinalPagamento.AddDays(1).ToString(pattern) + "'";

            if (tipoDespesaFinanceira > 0)
                where += " AND DespesaMensal.TID_CODIGO = " + tipoDespesaFinanceira;

            if (pessoa > 0)
                where += " AND DespesaMensal.CLI_CGCCPF = " + pessoa;
        }

        #endregion

    }
}

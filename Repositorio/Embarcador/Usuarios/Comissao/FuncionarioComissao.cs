using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Usuarios.Comissao
{
    public class FuncionarioComissao : RepositorioBase<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao>
    {
        public FuncionarioComissao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public int BuscarProximoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao>();
            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            var resultNumero = result.Select(o => o.Numero);

            int maiorNumero = 0;
            if (resultNumero.Count() > 0)
                maiorNumero = resultNumero.Max();

            return maiorNumero + 1;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao> _Consultar(int codigoEmpresa, DateTime dataInicio, DateTime dataFim, int codigoFuncionario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao>();

            var result = from obj in query select obj;

            // Filtros
            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (codigoFuncionario > 0)
                result = result.Where(o => o.Funcionario.Codigo == codigoFuncionario);

            if (dataInicio != DateTime.MinValue)
                result = result.Where(o => o.DataInicial.Date >= dataInicio);

            if (dataFim != DateTime.MinValue)
                result = result.Where(o => o.DataFinal.Date <= dataFim);

            if (situacao.HasValue)
                result = result.Where(o => o.Situacao == situacao.Value);

            return result;
        }

        public List<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao> Consultar(int codigoEmpresa, DateTime dataInicio, DateTime dataFim, int codigoFuncionario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao? situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoEmpresa, dataInicio, dataFim, codigoFuncionario, situacao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int codigoEmpresa, DateTime dataInicio, DateTime dataFim, int codigoFuncionario, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao? situacao)
        {
            var result = _Consultar(codigoEmpresa, dataInicio, dataFim, codigoFuncionario, situacao);

            return result.Count();
        }

        public bool VerificarTituloComissaoCancelado(int codigoFuncionarioComissao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
            var queryComissao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Usuarios.Comissao.FuncionarioComissao>();

            queryComissao = queryComissao.Where(o => o.Codigo == codigoFuncionarioComissao);
            query = query.Where(o => queryComissao.Select(p => p.Titulo.Codigo).Contains(o.Codigo) && o.StatusTitulo != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Cancelado);

            if (query.Count() == 0)
                return true;
            else
                return false;
        }

        #region Relatório Funcionário Comissão

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pessoas.FuncionarioComissao> ConsultarRelatorioFuncionarioComissao(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int numeroInicial, int numeroFinal, int funcionario, int operador, int titulo, int fatura, bool exibirTitulos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao? situacao, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioFuncionarioComissao(codigoEmpresa, dataInicial, dataFinal, numeroInicial, numeroFinal, funcionario, operador, titulo, fatura, exibirTitulos, situacao, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pessoas.FuncionarioComissao)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.FuncionarioComissao>();
        }

        public int ContarConsultaRelatorioFuncionarioComissao(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int numeroInicial, int numeroFinal, int funcionario, int operador, int titulo, int fatura, bool exibirTitulos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao? situacao, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            string sql = ObterSelectConsultaRelatorioFuncionarioComissao(codigoEmpresa, dataInicial, dataFinal, numeroInicial, numeroFinal, funcionario, operador, titulo, fatura, exibirTitulos, situacao, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioFuncionarioComissao(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int numeroInicial, int numeroFinal, int funcionario, int operador, int titulo, int fatura, bool exibirTitulos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao? situacao, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            if (exibirTitulos)
            {
                select += "FuncionarioComissao.FCO_CODIGO Codigo, ";
                groupBy += "FuncionarioComissao.FCO_CODIGO, ";

                select += "COUNT(FuncionarioComissaoTitulo.FCO_CODIGO) QuantidadeTitulos, ";
            }

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioFuncionarioComissao(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioFuncionarioComissao(ref where, ref groupBy, ref joins, codigoEmpresa, dataInicial, dataFinal, numeroInicial, numeroFinal, funcionario, operador, titulo, fatura, exibirTitulos, situacao);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioFuncionarioComissao(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

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
            query += " FROM T_FUNCIONARIO_COMISSAO FuncionarioComissao ";

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

        private void SetarSelectRelatorioConsultaRelatorioFuncionarioComissao(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select += "FuncionarioComissao.FCO_NUMERO Numero, ";
                        groupBy += "FuncionarioComissao.FCO_NUMERO, ";
                    }
                    break;
                case "DataGeracao":
                    if (!select.Contains(" DataGeracao, "))
                    {
                        select += "FuncionarioComissao.FCO_DATA_GERACAO DataGeracao, ";
                        groupBy += "FuncionarioComissao.FCO_DATA_GERACAO, ";
                    }
                    break;
                case "DataInicial":
                    if (!select.Contains(" DataInicial, "))
                    {
                        select += "FuncionarioComissao.FCO_DATA_INICIAL DataInicial, ";
                        groupBy += "FuncionarioComissao.FCO_DATA_INICIAL, ";
                    }
                    break;
                case "DataFinal":
                    if (!select.Contains(" DataFinal, "))
                    {
                        select += "FuncionarioComissao.FCO_DATA_FINAL DataFinal, ";
                        groupBy += "FuncionarioComissao.FCO_DATA_FINAL, ";
                    }
                    break;
                case "Funcionario":
                    if (!select.Contains(" Funcionario, "))
                    {
                        if (!joins.Contains(" Funcionario "))
                            joins += " LEFT JOIN T_FUNCIONARIO Funcionario ON Funcionario.FUN_CODIGO = FuncionarioComissao.FUN_CODIGO";

                        select += "Funcionario.FUN_NOME Funcionario, ";
                        groupBy += "Funcionario.FUN_NOME, ";
                    }
                    break;
                case "Operador":
                    if (!select.Contains(" Operador, "))
                    {
                        if (!joins.Contains(" Operador "))
                            joins += " LEFT JOIN T_FUNCIONARIO Operador ON Operador.FUN_CODIGO = FuncionarioComissao.FUN_CODIGO_OPERADOR";

                        select += "Operador.FUN_NOME Operador, ";
                        groupBy += "Operador.FUN_NOME, ";
                    }
                    break;
                case "ValorTotalFinal":
                    if (!select.Contains(" ValorTotalFinal, "))
                    {
                        if (!joins.Contains(" FuncionarioComissaoTitulo "))
                            joins += " LEFT JOIN T_FUNCIONARIO_COMISSAO_TITULO FuncionarioComissaoTitulo ON FuncionarioComissaoTitulo.FCO_CODIGO = FuncionarioComissao.FCO_CODIGO";

                        select += "SUM(ISNULL(FuncionarioComissaoTitulo.FCT_VALOR_FINAL, 0)) ValorTotalFinal, ";
                    }
                    break;
                case "PercentualComissao":
                    if (!select.Contains(" PercentualComissao, "))
                    {
                        select += "AVG(FuncionarioComissao.FCO_PERCENTUAL_COMISSAO) PercentualComissao, ";
                    }
                    break;
                case "PercentualComissaoAcrescimo":
                    if (!select.Contains(" PercentualComissaoAcrescimo, "))
                    {
                        select += "AVG(FuncionarioComissao.FCO_PERCENTUAL_COMISSAO_ACRESCIMO) PercentualComissaoAcrescimo, ";
                    }
                    break;
                case "PercentualComissaoTotal":
                    if (!select.Contains(" PercentualComissaoTotal, "))
                    {
                        select += "AVG(FuncionarioComissao.FCO_PERCENTUAL_COMISSAO_TOTAL) PercentualComissaoTotal, ";
                    }
                    break;
                case "ValorComissao":
                    if (!select.Contains(" ValorComissao, "))
                    {
                        select += "AVG(FuncionarioComissao.FCO_VALOR_COMISSAO) ValorComissao, ";
                    }
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioFuncionarioComissao(ref string where, ref string groupBy, ref string joins, int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int numeroInicial, int numeroFinal, int funcionario, int operador, int titulo, int fatura, bool exibirTitulos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFuncionarioComissao? situacao)
        {
            string pattern = "yyyy-MM-dd";

            if (!joins.Contains(" FuncionarioComissaoTitulo "))
                joins += " LEFT JOIN T_FUNCIONARIO_COMISSAO_TITULO FuncionarioComissaoTitulo ON FuncionarioComissaoTitulo.FCO_CODIGO = FuncionarioComissao.FCO_CODIGO";

            if (codigoEmpresa > 0)
                where += " AND FuncionarioComissao.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (numeroInicial > 0)
                where += " AND FuncionarioComissao.FCO_NUMERO >= " + numeroInicial;

            if (numeroFinal > 0)
                where += " AND FuncionarioComissao.FCO_NUMERO <= " + numeroFinal;

            if (dataInicial != DateTime.MinValue)
                where += " AND FuncionarioComissao.FCO_DATA_INICIAL >= '" + dataInicial.ToString(pattern) + "' ";

            if (dataFinal != DateTime.MinValue)
                where += " AND FuncionarioComissao.FCO_DATA_FINAL <= '" + dataFinal.ToString(pattern) + "'";

            if (funcionario > 0)
                where += " AND FuncionarioComissao.FUN_CODIGO = " + funcionario;

            if (operador > 0)
                where += " AND FuncionarioComissao.FUN_CODIGO_OPERADOR = " + operador;

            if (titulo > 0)
                where += " AND FuncionarioComissaoTitulo.TIT_CODIGO = " + titulo;

            if (fatura > 0)
            {
                if (!joins.Contains(" Titulo "))
                    joins += " LEFT JOIN T_TITULO Titulo ON Titulo.TIT_CODIGO = FuncionarioComissaoTitulo.TIT_CODIGO";

                if (!joins.Contains(" FaturaParcela "))
                    joins += " LEFT JOIN T_FATURA_PARCELA FaturaParcela ON FaturaParcela.FAP_CODIGO = Titulo.FAP_CODIGO";

                if (!joins.Contains(" Fatura "))
                    joins += " LEFT JOIN T_FATURA Fatura ON Fatura.FAT_CODIGO = FaturaParcela.FAT_CODIGO";

                where += " AND Fatura.FAT_CODIGO = " + fatura;
            }

            if (situacao.HasValue)
                where += " AND FuncionarioComissao.FCO_SITUACAO = " + situacao.Value.ToString("D");
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Pessoas.FuncionarioComissaoTitulo> ConsultarTitulosRelatorioFuncionarioComissao(List<int> codigosFuncionarioComissao)
        {
            string query = @"   SELECT FCT.FCT_CODIGO Codigo,
                                FCT.FCO_CODIGO CodigoFuncionarioComissao,
                                FCT.TIT_CODIGO CodigoTitulo,
                                C.CLI_NOME Pessoa,
                                C.CLI_CGCCPF CNPJCPFPessoa,
                                C.CLI_FISJUR TipoPessoa,
                                FA.FAT_NUMERO NumeroFatura,
                                T.TIT_DATA_EMISSAO DataEmissao,
                                T.TIT_DATA_VENCIMENTO DataVencimento,
                                T.TIT_DATA_LIQUIDACAO DataLiquidacao,
                                T.TIT_VALOR_ORIGINAL ValorOriginal,
                                T.TIT_VALOR_PAGO ValorPago,
                                FCT.FCT_VALOR_ISS ValorISS,
                                FCT.FCT_VALOR_ICMS ValorICMS,
                                FCT.FCT_VALOR_LIQUIDO ValorLiquido,
                                FCT.FCT_PERCENTUAL_IMPOSTO_FEDERAL PercentualImpostoFederal,
                                FCT.FCT_VALOR_FINAL ValorFinal

                                FROM T_FUNCIONARIO_COMISSAO_TITULO FCT
                                JOIN T_FUNCIONARIO_COMISSAO FC ON FC.FCO_CODIGO = FCT.FCO_CODIGO
                                JOIN T_TITULO T ON T.TIT_CODIGO = FCT.TIT_CODIGO
                                JOIN T_CLIENTE C ON C.CLI_CGCCPF = T.CLI_CGCCPF
                                LEFT OUTER JOIN T_FATURA_PARCELA FP ON FP.FAP_CODIGO = T.FAP_CODIGO
                                LEFT OUTER JOIN T_FATURA FA ON FA.FAT_CODIGO = FP.FAT_CODIGO
                                WHERE 1 = 1 ";

            if (codigosFuncionarioComissao.Count > 0)
                query += " AND FC.FCO_CODIGO IN (" + String.Join(", ", codigosFuncionarioComissao) + ")";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Pessoas.FuncionarioComissaoTitulo)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Pessoas.FuncionarioComissaoTitulo>();
        }

        #endregion


    }
}

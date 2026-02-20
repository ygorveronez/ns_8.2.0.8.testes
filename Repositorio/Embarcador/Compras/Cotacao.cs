using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras
{
    public class Cotacao : RepositorioBase<Dominio.Entidades.Embarcador.Compras.CotacaoCompra>
    {
        public Cotacao(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Compras.CotacaoCompra BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoCompra>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Compras.CotacaoCompra BuscarUltimaCotacao(int codigoEmpresa, int codigoProduto)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoCompra>();
            var result = from obj in query where obj.Produtos.Any(o => o.Produto.Codigo == codigoProduto) && obj.Situacao != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao.Cancelado select obj;
            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            return result.OrderBy("Numero descending").FirstOrDefault();
        }

        public int ProximoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoCompra>();
            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (result.Count() > 0)
                return result.Max(obj => obj.Numero) + 1;
            else
                return 1;
        }

        public List<Dominio.Entidades.Embarcador.Compras.CotacaoCompra> Consultar(int codigoEmpresa, int numero, DateTime dataEmissaoDe, DateTime dataEmissaoAte, double cnpjFornecedor, DateTime dataRetornoDe, DateTime dataRetornoAte, int codigoProduto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao? situacao, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(codigoEmpresa, numero, dataEmissaoDe, dataEmissaoAte, cnpjFornecedor, dataRetornoDe, dataRetornoAte, codigoProduto, situacao);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(int codigoEmpresa, int numero, DateTime dataEmissaoDe, DateTime dataEmissaoAte, double cnpjFornecedor, DateTime dataRetornoDe, DateTime dataRetornoAte, int codigoProduto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao? situacao)
        {
            var result = _Consultar(codigoEmpresa, numero, dataEmissaoDe, dataEmissaoAte, cnpjFornecedor, dataRetornoDe, dataRetornoAte, codigoProduto, situacao);

            return result.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Compras.CotacaoCompraFornecedor> RelatorioCotacaoCompraFornecedor(int codigo)
        {
            string query = @"   select C.COT_NUMERO Numero,
                                C.COT_DATA_EMISSAO DataEmissao,
                                C.COT_DATA_PREVISAO DataPrevisao,
                                C.COT_DESCRICAO Descricao,
                                P.PRO_DESCRICAO Produto,
                                CP.COP_QUANTIDADE Quantidade,
                                CP.COP_VALOR_UNITARIO ValorUnitario,
                                CP.COP_VALOR_TOTAL ValorTotal
                                 from T_COTACAO C
                                JOIN T_COTACAO_PRODUTO CP ON CP.COT_CODIGO = C.COT_CODIGO
                                JOIN T_PRODUTO P ON P.PRO_CODIGO = CP.PRO_CODIGO
                                WHERE 1 = 1";

            if (codigo > 0)
                query += " AND C.COT_CODIGO = " + codigo.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Compras.CotacaoCompraFornecedor)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Compras.CotacaoCompraFornecedor>();
        }

        public List<Dominio.Entidades.Embarcador.Compras.CotacaoCompra> ConsultarCotacoesPendentes(double fornecedor, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _ConsultarCotacoesPendentes(fornecedor);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsultaCotacoesPendentes(double fornecedor)
        {
            var result = _ConsultarCotacoesPendentes(fornecedor);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Compras.CotacaoCompra> _Consultar(int codigoEmpresa, int numero, DateTime dataEmissaoDe, DateTime dataEmissaoAte, double cnpjFornecedor, DateTime dataRetornoDe, DateTime dataRetornoAte, int codigoProduto, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao? situacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoCompra>();

            var result = from obj in query select obj;

            // Filtros

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Empresa.Codigo == codigoEmpresa);

            if (numero > 0)
                result = result.Where(o => o.Numero == numero);

            if (dataEmissaoDe != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao.Value.Date >= dataEmissaoDe);

            if (dataEmissaoAte != DateTime.MinValue)
                result = result.Where(o => o.DataEmissao.Value.Date <= dataEmissaoAte);

            if (dataRetornoDe != DateTime.MinValue)
                result = result.Where(o => o.DataPrevisao.Value.Date >= dataRetornoDe);

            if (dataRetornoAte != DateTime.MinValue)
                result = result.Where(o => o.DataPrevisao.Value.Date <= dataRetornoAte);

            if (codigoProduto > 0)
                result = result.Where(o => o.Produtos.Any(obj => obj.Produto.Codigo == codigoProduto));

            if (situacao.HasValue && (int)situacao > 0)
                result = result.Where(o => o.Situacao == situacao.Value);

            return result;
        }

        private IQueryable<Dominio.Entidades.Embarcador.Compras.CotacaoCompra> _ConsultarCotacoesPendentes(double fornecedor)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.CotacaoCompra>();

            var result = from obj in query
                         where
                         obj.Situacao == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoCotacao.AguardandoRetorno
                         && obj.Fornecedores.Any(f => f.Fornecedor.CPF_CNPJ == fornecedor)
                         select obj;

            return result;
        }

        #endregion

        #region Relatório de Cotação de Compra

        public IList<Dominio.Relatorios.Embarcador.DataSource.Compras.CotacaoCompra> ConsultarRelatorioCotacaoCompra(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioCotacaoCompra filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioCotacaoCompra(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Compras.CotacaoCompra)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Compras.CotacaoCompra>();
        }

        public int ContarConsultaRelatorioCotacaoCompra(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioCotacaoCompra filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sql = ObterSelectConsultaRelatorioCotacaoCompra(filtrosPesquisa, true, propriedades, "", "", "", "", 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioCotacaoCompra(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioCotacaoCompra filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioCotacaoCompra(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioCotacaoCompra(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioCotacaoCompra(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

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
            query += " FROM T_COTACAO_RETORNO_FORNECEDOR_PRODUTO CotacaoRetorno ";

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

        private void SetarSelectRelatorioConsultaRelatorioCotacaoCompra(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "CotacaoRetorno.CRF_CODIGO Codigo, ";
                        groupBy += "CotacaoRetorno.CRF_CODIGO, ";
                    }
                    break;
                case "NumeroCotacao":
                    if (!select.Contains(" NumeroCotacao, "))
                    {
                        select += "Cotacao.COT_NUMERO NumeroCotacao, ";
                        groupBy += "Cotacao.COT_NUMERO, ";

                        SetarJoinsCotacao(ref joins);
                    }
                    break;
                case "DescricaoCotacao":
                    if (!select.Contains(" DescricaoCotacao, "))
                    {
                        select += "Cotacao.COT_DESCRICAO DescricaoCotacao, ";
                        groupBy += "Cotacao.COT_DESCRICAO, ";

                        SetarJoinsCotacao(ref joins);
                    }
                    break;
                case "DataEmissaoFormatada":
                    if (!select.Contains(" DataEmissao, "))
                    {
                        select += "Cotacao.COT_DATA_EMISSAO DataEmissao, ";
                        groupBy += "Cotacao.COT_DATA_EMISSAO, ";

                        SetarJoinsCotacao(ref joins);
                    }
                    break;

                case "Quantidade":
                    if (!select.Contains(" Quantidade, "))
                    {
                        select += "CotacaoProduto.COP_QUANTIDADE Quantidade, ";
                        groupBy += "CotacaoProduto.COP_QUANTIDADE, ";

                        SetarJoinsCotacaoProduto(ref joins);
                    }
                    break;
                case "ValorUnitario":
                    if (!select.Contains(" ValorUnitario, "))
                    {
                        select += "CotacaoProduto.COP_VALOR_UNITARIO ValorUnitario, ";
                        groupBy += "CotacaoProduto.COP_VALOR_UNITARIO, ";

                        SetarJoinsCotacaoProduto(ref joins);
                    }
                    break;
                case "ValorTotal":
                    if (!select.Contains(" ValorTotal, "))
                    {
                        select += "CotacaoProduto.COP_VALOR_TOTAL ValorTotal, ";
                        groupBy += "CotacaoProduto.COP_VALOR_TOTAL, ";

                        SetarJoinsCotacaoProduto(ref joins);
                    }
                    break;

                case "QuantidadeRetornado":
                    if (!select.Contains(" QuantidadeRetornado, "))
                    {
                        select += "CotacaoRetorno.CRF_QUANTIDADE_RETORNO QuantidadeRetornado, ";
                        groupBy += "CotacaoRetorno.CRF_QUANTIDADE_RETORNO, ";
                    }
                    break;
                case "ValorUnitarioRetornado":
                    if (!select.Contains(" ValorUnitarioRetornado, "))
                    {
                        select += "CotacaoRetorno.CRF_VALOR_UNITARIO_RETORNO ValorUnitarioRetornado, ";
                        groupBy += "CotacaoRetorno.CRF_VALOR_UNITARIO_RETORNO, ";
                    }
                    break;
                case "ValorTotalRetornado":
                    if (!select.Contains(" ValorTotalRetornado, "))
                    {
                        select += "CotacaoRetorno.CRF_VALOR_TOTAL_RETORNO ValorTotalRetornado, ";
                        groupBy += "CotacaoRetorno.CRF_VALOR_TOTAL_RETORNO, ";
                    }
                    break;

                case "Fornecedor":
                    if (!select.Contains(" Fornecedor, "))
                    {
                        select += "Fornecedor.CLI_NOME Fornecedor, ";
                        groupBy += "Fornecedor.CLI_NOME, ";

                        SetarJoinsCotacaoFornecedor(ref joins);
                    }
                    break;
                case "Produto":
                    if (!select.Contains(" Produto, "))
                    {
                        select += "Produto.PRO_DESCRICAO Produto, ";
                        groupBy += "Produto.PRO_DESCRICAO, ";

                        SetarJoinsCotacaoProduto(ref joins);
                    }
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioCotacaoCompra(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioCotacaoCompra filtrosPesquisa)
        {
            string datePattern = "yyyy-MM-dd";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                where += " AND CAST(Cotacao.COT_DATA_EMISSAO AS DATE) >= '" + filtrosPesquisa.DataInicial.Date.ToString(datePattern) + "'";
                SetarJoinsCotacao(ref joins);
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                where += " AND CAST(Cotacao.COT_DATA_EMISSAO AS DATE) <= '" + filtrosPesquisa.DataFinal.Date.ToString(datePattern) + "'";
                SetarJoinsCotacao(ref joins);
            }

            if (filtrosPesquisa.CodigosProduto != null && filtrosPesquisa.CodigosProduto.Count > 0)
            {
                where += " and CotacaoProduto.PRO_CODIGO in (" + string.Join(", ", filtrosPesquisa.CodigosProduto) + ")";
                SetarJoinsCotacaoProduto(ref joins);
            }

            if (filtrosPesquisa.CodigosFornecedor != null && filtrosPesquisa.CodigosFornecedor.Count > 0)
            {
                where += " and CotacaoFornecedor.CLI_CGCCPF_FORNECEDOR in (" + string.Join(", ", filtrosPesquisa.CodigosFornecedor) + ")";
                SetarJoinsCotacaoFornecedor(ref joins);
            }
        }

        private void SetarJoinsCotacao(ref string joins)
        {
            if (!joins.Contains(" Cotacao "))
                joins += " JOIN T_COTACAO Cotacao on Cotacao.COT_CODIGO = CotacaoFornecedor.COT_CODIGO and Cotacao.COT_CODIGO = CotacaoProduto.COT_CODIGO ";

            SetarJoinsCotacaoFornecedor(ref joins);
            SetarJoinsCotacaoProduto(ref joins);
        }

        private void SetarJoinsCotacaoFornecedor(ref string joins)
        {
            if (!joins.Contains(" CotacaoFornecedor "))
                joins += " JOIN T_COTACAO_FORNECEDOR CotacaoFornecedor on CotacaoFornecedor.COF_CODIGO = CotacaoRetorno.COF_CODIGO ";

            if (!joins.Contains(" Fornecedor "))
                joins += " JOIN T_CLIENTE Fornecedor on Fornecedor.CLI_CGCCPF = CotacaoFornecedor.CLI_CGCCPF_FORNECEDOR ";
        }

        private void SetarJoinsCotacaoProduto(ref string joins)
        {
            if (!joins.Contains(" CotacaoProduto "))
                joins += " JOIN T_COTACAO_PRODUTO CotacaoProduto on CotacaoProduto.COP_CODIGO = CotacaoRetorno.COP_CODIGO ";

            if (!joins.Contains(" Produto "))
                joins += " JOIN T_PRODUTO Produto on Produto.PRO_CODIGO = CotacaoProduto.PRO_CODIGO ";
        }

        #endregion
    }
}
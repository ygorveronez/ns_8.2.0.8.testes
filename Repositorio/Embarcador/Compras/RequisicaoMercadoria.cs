using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Compras
{
    public class RequisicaoMercadoria : RepositorioBase<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria>
    {
        public RequisicaoMercadoria(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> BuscarPorCodigos(List<int> codigos)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria>();
            var result = from obj in query where codigos.Contains(obj.Codigo) select obj;
            return result.ToList();
        }

        public int BuscarProximoNumero(int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria>();
            var result = from obj in query select obj;

            if (codigoEmpresa > 0)
                result = result.Where(o => o.Filial.Codigo == codigoEmpresa);

            var resultNumero = result.Select(o => o.Numero);

            int maiorNumero = 0;
            if (resultNumero.Count() > 0)
                maiorNumero = resultNumero.Max();

            return maiorNumero + 1;
        }

        public List<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> Consultar(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRequisicaoMercadoria filtrosPesquisa, string propOrdena, string dirOrdena, int inicioRegistros, int maximoRegistros)
        {
            var result = _Consultar(filtrosPesquisa);

            if (!string.IsNullOrWhiteSpace(propOrdena))
                result = result.OrderBy(propOrdena + (dirOrdena == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRequisicaoMercadoria filtrosPesquisa)
        {
            var result = _Consultar(filtrosPesquisa);

            return result.Count();
        }

        #endregion

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria> _Consultar(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRequisicaoMercadoria filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Compras.RequisicaoMercadoria>();

            var result = from obj in query select obj;

            // Filtros
            if (filtrosPesquisa.CodigoEmpresa > 0)
                result = result.Where(o => o.Filial.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.DataInicio != DateTime.MinValue)
                result = result.Where(o => o.Data.Date >= filtrosPesquisa.DataInicio);

            if (filtrosPesquisa.DataFim != DateTime.MinValue)
                result = result.Where(o => o.Data.Date <= filtrosPesquisa.DataFim);

            if (filtrosPesquisa.Filial > 0)
                result = result.Where(o => o.Filial.Codigo == filtrosPesquisa.Filial);

            if (filtrosPesquisa.Motivo > 0)
                result = result.Where(o => o.MotivoCompra.Codigo == filtrosPesquisa.Motivo);

            if (filtrosPesquisa.Situacao.HasValue)
                result = result.Where(o => o.Situacao == filtrosPesquisa.Situacao.Value);

            if (filtrosPesquisa.Modo.HasValue)
                result = result.Where(o => o.Modo == filtrosPesquisa.Modo.Value);

            if (filtrosPesquisa.FuncionarioRequisitado > 0)
                result = result.Where(o => o.FuncionarioRequisitado.Codigo == filtrosPesquisa.FuncionarioRequisitado);

            if (filtrosPesquisa.Veiculo > 0)
                result = result.Where(o => o.Veiculo.Codigo == filtrosPesquisa.Veiculo);

            if (filtrosPesquisa.Numero > 0)
                result = result.Where(o => o.Numero == filtrosPesquisa.Numero);

            return result;
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Compras.PontuacaoComprador> RelatorioPontuacaoComprador(int codigoEmpresa, int codigoComprador, DateTime dataInicial, DateTime dataFinal, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false)
        {
            string query = "";

            query = @"   SELECT 
                ISNULL((SELECT F.FUN_NOME FROM T_ORDEM_COMPRA_REQUISICAO CR JOIN T_ORDEM_COMPRA O ON O.ORC_CODIGO = CR.ORC_CODIGO JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = O.FUN_CODIGO WHERE CR.RME_CODIGO = R.RME_CODIGO)
                ,(SELECT F.FUN_NOME FROM T_COTACAO C JOIN T_ORDEM_COMPRA O ON O.COT_CODIGO = C.COT_CODIGO JOIN T_ORDEM_COMPRA_REQUISICAO CR ON CR.ORC_CODIGO = O.ORC_CODIGO JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = O.FUN_CODIGO WHERE CR.RME_CODIGO = R.RME_CODIGO)) Comprador,
                P.PRO_DESCRICAO Produto,
                R.RME_DATA_APROVACAO DataRequisicao,
                ISNULL((SELECT A.AAA_DATA FROM T_ORDEM_COMPRA_REQUISICAO CR JOIN T_ORDEM_COMPRA O ON O.ORC_CODIGO = CR.ORC_CODIGO JOIN T_AUTORIZACAO_ALCADA_ORDEM_COMPRA A ON A.ORC_CODIGO = O.ORC_CODIGO AND A.AAA_SITUACAO = 1 WHERE CR.RME_CODIGO = R.RME_CODIGO)
                ,(SELECT A.AAA_DATA FROM T_COTACAO C JOIN T_ORDEM_COMPRA O ON O.COT_CODIGO = C.COT_CODIGO JOIN T_ORDEM_COMPRA_REQUISICAO CR ON CR.ORC_CODIGO = O.ORC_CODIGO JOIN T_AUTORIZACAO_ALCADA_ORDEM_COMPRA A ON A.ORC_CODIGO = O.ORC_CODIGO AND A.AAA_SITUACAO = 1 WHERE CR.RME_CODIGO = R.RME_CODIGO)) DataOrdemCompra,
                0 QtdDias 
                FROM T_REQUISICAO_MERCADORIA R
                JOIN T_MERCADORIA M ON M.RME_CODIGO = R.RME_CODIGO
                JOIN T_PRODUTO_ESTOQUE E ON E.PRE_CODIGO = M.PRE_CODIGO
                JOIN T_PRODUTO P ON P.PRO_CODIGO = E.PRO_CODIGO
                WHERE (R.RME_CODIGO IN (SELECT CR.RME_CODIGO FROM T_ORDEM_COMPRA_REQUISICAO CR JOIN T_ORDEM_COMPRA O ON O.ORC_CODIGO = CR.ORC_CODIGO WHERE O.ORC_SITUACAO IN (1,3,4,8)) OR
                R.RME_CODIGO IN (SELECT CR.RME_CODIGO FROM T_COTACAO C JOIN T_ORDEM_COMPRA O ON O.COT_CODIGO = C.COT_CODIGO JOIN T_ORDEM_COMPRA_REQUISICAO CR ON CR.ORC_CODIGO = O.ORC_CODIGO 
                WHERE O.ORC_SITUACAO IN (1,3,4,8)))
                and M.MER_MODO = 2 ";

            if (codigoEmpresa > 0)
                query += @" AND R.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoComprador > 0)
            {
                query += @" AND (R.RME_CODIGO IN (SELECT CR.RME_CODIGO FROM T_ORDEM_COMPRA_REQUISICAO CR JOIN T_ORDEM_COMPRA O ON O.ORC_CODIGO = CR.ORC_CODIGO WHERE O.ORC_SITUACAO IN (1,3,4,8) AND O.FUN_CODIGO = " + codigoComprador.ToString() + @") OR
                    R.RME_CODIGO IN (SELECT CR.RME_CODIGO FROM T_COTACAO C JOIN T_ORDEM_COMPRA O ON O.COT_CODIGO = C.COT_CODIGO JOIN T_ORDEM_COMPRA_REQUISICAO CR ON CR.ORC_CODIGO = O.ORC_CODIGO 
                    WHERE O.ORC_SITUACAO IN (1,3,4,8) AND O.FUN_CODIGO = " + codigoComprador.ToString() + @"))";  // SQL-INJECTION-SAFE
            }
            if (dataInicial != DateTime.MinValue)
                query += " AND R.RME_DATA_APROVACAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

            if (dataFinal != DateTime.MinValue)
                query += " AND R.RME_DATA_APROVACAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                query += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    query += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    query += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            if (maximoRegistros > 0)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Compras.PontuacaoComprador)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Compras.PontuacaoComprador>();
        }

        public int ContarRelatorioPontuacaoComprador(int codigoEmpresa, int codigoComprador, DateTime dataInicial, DateTime dataFinal)
        {
            string query = "";

            query = @"   SELECT  COUNT(0) as CONTADOR 
                FROM T_REQUISICAO_MERCADORIA R
                JOIN T_MERCADORIA M ON M.RME_CODIGO = R.RME_CODIGO
                JOIN T_PRODUTO_ESTOQUE E ON E.PRE_CODIGO = M.PRE_CODIGO
                JOIN T_PRODUTO P ON P.PRO_CODIGO = E.PRO_CODIGO
                WHERE (R.RME_CODIGO IN (SELECT CR.RME_CODIGO FROM T_ORDEM_COMPRA_REQUISICAO CR JOIN T_ORDEM_COMPRA O ON O.ORC_CODIGO = CR.ORC_CODIGO WHERE O.ORC_SITUACAO IN (1,3,4,8)) OR
                R.RME_CODIGO IN (SELECT CR.RME_CODIGO FROM T_COTACAO C JOIN T_ORDEM_COMPRA O ON O.COT_CODIGO = C.COT_CODIGO JOIN T_ORDEM_COMPRA_REQUISICAO CR ON CR.ORC_CODIGO = O.ORC_CODIGO 
                WHERE O.ORC_SITUACAO IN (1,3,4,8)))
                and M.MER_MODO = 2 ";

            if (codigoEmpresa > 0)
                query += @" AND R.EMP_CODIGO = " + codigoEmpresa.ToString();

            if (codigoComprador > 0)
            {
                query += @" AND (R.RME_CODIGO IN (SELECT CR.RME_CODIGO FROM T_ORDEM_COMPRA_REQUISICAO CR JOIN T_ORDEM_COMPRA O ON O.ORC_CODIGO = CR.ORC_CODIGO WHERE O.ORC_SITUACAO IN (1,3,4,8) AND O.FUN_CODIGO = " + /* SQL-INJECTION-SAFE*/ codigoComprador.ToString() + @") OR
                    R.RME_CODIGO IN (SELECT CR.RME_CODIGO FROM T_COTACAO C JOIN T_ORDEM_COMPRA O ON O.COT_CODIGO = C.COT_CODIGO JOIN T_ORDEM_COMPRA_REQUISICAO CR ON CR.ORC_CODIGO = O.ORC_CODIGO 
                    WHERE O.ORC_SITUACAO IN (1,3,4,8) AND O.FUN_CODIGO = " + codigoComprador.ToString() + @"))"; 
            }
            if (dataInicial != DateTime.MinValue)
                query += " AND R.RME_DATA_APROVACAO >= '" + dataInicial.ToString("MM/dd/yyyy") + "'";

            if (dataFinal != DateTime.MinValue)
                query += " AND R.RME_DATA_APROVACAO <= '" + dataFinal.AddDays(1).ToString("MM/dd/yyyy") + "'";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Compras.RequisicaoMercadoria> RelatorioRequisicaoMercadoria(int codigoRequisicaoMercadoria)
        {
            string query = @"   SELECT R.MER_NUMERO Numero,
                                R.FEF_DATA Data,
                                R.RME_OBSERVACAO Observacao,
                                MC.MCO_DESCRICAO Motivo,

                                M.MER_CODIGO CodigoItemBanco,
                                P.PRO_CODIGO CodigoItem,
                                P.PRO_DESCRICAO DescricaoItem,
                                M.MER_QUANTIDADE QuantidadeItem,

                                E.EMP_CNPJ CNPJEmpresa,
                                E.EMP_FANTASIA FantasiaEmpresa,
                                E.EMP_ENDERECO EnderecoEmpresa,
                                E.EMP_NUMERO NumeroEnderecoEmpresa,
                                E.EMP_CEP CEPEmpresa,
                                E.EMP_BAIRRO BairroEmpresa,
                                E.EMP_FONE FoneEmpresa,
                                LEMP.LOC_DESCRICAO CidadeEmpresa,
                                LEMP.UF_SIGLA EstadoEmpresa,

                                F.FUN_NOME NomeColaborador,
                                FR.FUN_NOME NomeFuncionarioRequisitado,

                                S.SET_DESCRICAO Setor,
                                FR.FUN_DATAADMISAO DataAdmisao,
                                LFR.LOC_DESCRICAO CidadeFuncionario,
                                LFR.UF_SIGLA EstadoFuncionario,
                                FR.FUN_ENDERECO EnderecoFuncionario,
                                FR.FUN_CEP CEPFuncionario,
                                FR.FUN_BAIRRO Bairro,
                                FR.FUN_NUMERO_ENDERECO NumeroEnderecoFuncionario,

                                P.PRO_NUMERO_CA NumeroCA,
                                P.PRO_PRODUTO_EPI ProdutoEPI,
                                ISNULL((SELECT COUNT(1) 
                                          FROM T_MERCADORIA RR 
                                          JOIN T_PRODUTO_ESTOQUE EE ON EE.PRE_CODIGO = RR.PRE_CODIGO 
                                          JOIN T_PRODUTO PP ON PP.PRO_CODIGO = EE.PRO_CODIGO 
                                         WHERE RR.RME_CODIGO = R.RME_CODIGO 
                                           AND P.PRO_PRODUTO_EPI = 1), 0) ContemEPI,

                                M.MER_CUSTO_UNITARIO CustoUnitario,
								(M.MER_CUSTO_UNITARIO * M.MER_QUANTIDADE) CustoTotal

                                FROM T_REQUISICAO_MERCADORIA R
                                JOIN T_MOTIVO_COMPRA MC ON MC.MCO_CODIGO = R.MCO_CODIGO
                                LEFT OUTER JOIN T_MERCADORIA M ON M.RME_CODIGO = R.RME_CODIGO
                                LEFT OUTER JOIN T_PRODUTO_ESTOQUE PE ON PE.PRE_CODIGO = M.PRE_CODIGO
                                LEFT OUTER JOIN T_PRODUTO P ON P.PRO_CODIGO = PE.PRO_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = R.EMP_CODIGO
                                JOIN T_LOCALIDADES LEMP ON LEMP.LOC_CODIGO = E.LOC_CODIGO
                                JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = R.FUN_CODIGO
                                LEFT OUTER JOIN T_FUNCIONARIO FR ON FR.FUN_CODIGO = R.FUN_CODIGO_REQUISITADO
                                LEFT OUTER JOIN T_SETOR S ON S.SET_CODIGO = FR.SET_CODIGO
                                LEFT OUTER JOIN T_LOCALIDADES LFR ON LFR.LOC_CODIGO = FR.LOC_CODIGO
                                WHERE R.RME_CODIGO = " + codigoRequisicaoMercadoria.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Compras.RequisicaoMercadoria)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Compras.RequisicaoMercadoria>();
        }

        #endregion

        #region Relatório de Requisição de Mercadoria

        public IList<Dominio.Relatorios.Embarcador.DataSource.Compras.RelatorioRequisicaoMercadoria> ConsultarRelatorioRequisicaoMercadoria(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioRequisicaoMercadoria filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa , string dirAgrupa , string propOrdena, string dirOrdena , int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioRequisicaoMercadoria(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Compras.RelatorioRequisicaoMercadoria)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Compras.RelatorioRequisicaoMercadoria>();
        }

        public int ContarConsultaRelatorioRequisicaoMercadoria(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioRequisicaoMercadoria filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa ="", string dirAgrupa = "", string propOrdena = "", string dirOrdena = "")
        {
            string sql = ObterSelectConsultaRelatorioRequisicaoMercadoria(filtrosPesquisa, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioRequisicaoMercadoria(Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioRequisicaoMercadoria filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioRequisicaoMercadoria(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioRequisicaoMercadoria(ref where, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioRequisicaoMercadoria(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

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
            query += " FROM T_REQUISICAO_MERCADORIA RequisicaoMercadoria ";

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

        private void SetarSelectRelatorioConsultaRelatorioRequisicaoMercadoria(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "RequisicaoMercadoria.RME_CODIGO Codigo, ";
                        groupBy += "RequisicaoMercadoria.RME_CODIGO, ";
                    }
                    break;
                case "Numero":
                    if (!select.Contains(" Numero, "))
                    {
                        select += "RequisicaoMercadoria.MER_NUMERO Numero, ";
                        groupBy += "RequisicaoMercadoria.MER_NUMERO, ";
                    }
                    break;
                case "Data":
                case "DataFormatada":
                    if (!select.Contains(" Data, "))
                    {
                        select += "RequisicaoMercadoria.FEF_DATA Data, ";
                        groupBy += "RequisicaoMercadoria.FEF_DATA, ";
                    }
                    break;
                case "Colaborador":
                    if (!select.Contains(" Colaborador, "))
                    {
                        if (!joins.Contains(" Colaborador "))
                            joins += " JOIN T_FUNCIONARIO Colaborador ON Colaborador.FUN_CODIGO = RequisicaoMercadoria.FUN_CODIGO";

                        select += "Colaborador.FUN_NOME Colaborador, ";
                        groupBy += "Colaborador.FUN_NOME, ";
                    }
                    break;
                case "FuncionarioRequisitado":
                    if (!select.Contains(" FuncionarioRequisitado, "))
                    {
                        if (!joins.Contains(" FuncionarioRequisitado "))
                            joins += " LEFT JOIN T_FUNCIONARIO FuncionarioRequisitado ON FuncionarioRequisitado.FUN_CODIGO = RequisicaoMercadoria.FUN_CODIGO_REQUISITADO";

                        select += "FuncionarioRequisitado.FUN_NOME FuncionarioRequisitado, ";
                        groupBy += "FuncionarioRequisitado.FUN_NOME, ";
                    }
                    break;
                case "Empresa":
                    if (!select.Contains(" Empresa, "))
                    {
                        if (!joins.Contains(" Empresa "))
                            joins += " JOIN T_EMPRESA Empresa ON Empresa.EMP_CODIGO = RequisicaoMercadoria.EMP_CODIGO";

                        select += "Empresa.EMP_RAZAO Empresa, ";
                        groupBy += "Empresa.EMP_RAZAO, ";
                    }
                    break;
                case "Motivo":
                    if (!select.Contains(" Motivo, "))
                    {
                        if (!joins.Contains(" Motivo "))
                            joins += " LEFT JOIN T_MOTIVO_COMPRA Motivo ON Motivo.MCO_CODIGO = RequisicaoMercadoria.MCO_CODIGO";

                        select += "Motivo.MCO_DESCRICAO Motivo, ";
                        groupBy += "Motivo.MCO_DESCRICAO, ";
                    }
                    break;
                case "DescricaoSituacao":
                    if (!select.Contains(" Situacao, "))
                    {
                        select += "RequisicaoMercadoria.RME_SITUACAO Situacao, ";
                        groupBy += "RequisicaoMercadoria.RME_SITUACAO, ";
                    }
                    break;
                case "DescricaoTipo":
                    if (!select.Contains(" Tipo, "))
                    {
                        select += "RequisicaoMercadoria.RME_MODO Tipo, ";
                        groupBy += "RequisicaoMercadoria.RME_MODO, ";
                    }
                    break;
                case "Produto":
                    if (!select.Contains(" Produto, "))
                    {
                        if (!joins.Contains(" Mercadoria "))
                            joins += " LEFT JOIN T_MERCADORIA Mercadoria ON Mercadoria.RME_CODIGO = RequisicaoMercadoria.RME_CODIGO";

                        if (!joins.Contains(" ProdutoEstoque "))
                            joins += " LEFT JOIN T_PRODUTO_ESTOQUE ProdutoEstoque ON ProdutoEstoque.PRE_CODIGO = Mercadoria.PRE_CODIGO";

                        if (!joins.Contains(" Produto "))
                            joins += " LEFT JOIN T_PRODUTO Produto ON Produto.PRO_CODIGO = ProdutoEstoque.PRO_CODIGO";

                        select += "Produto.PRO_DESCRICAO Produto, ";
                        groupBy += "Produto.PRO_DESCRICAO, ";
                    }
                    break;
                case "GrupoProduto":
                    if (!select.Contains(" GrupoProduto, "))
                    {
                        if (!joins.Contains(" Mercadoria "))
                            joins += " LEFT JOIN T_MERCADORIA Mercadoria ON Mercadoria.RME_CODIGO = RequisicaoMercadoria.RME_CODIGO";

                        if (!joins.Contains(" ProdutoEstoque "))
                            joins += " LEFT JOIN T_PRODUTO_ESTOQUE ProdutoEstoque ON ProdutoEstoque.PRE_CODIGO = Mercadoria.PRE_CODIGO";

                        if (!joins.Contains(" Produto "))
                            joins += " LEFT JOIN T_PRODUTO Produto ON Produto.PRO_CODIGO = ProdutoEstoque.PRO_CODIGO";

                        if (!joins.Contains(" GrupoProduto "))
                            joins += " LEFT JOIN T_GRUPO_PRODUTO_TMS GrupoProduto ON GrupoProduto.GPR_CODIGO = Produto.GPR_CODIGO";

                        select += "GrupoProduto.GRP_DESCRICAO GrupoProduto, ";
                        groupBy += "GrupoProduto.GRP_DESCRICAO, ";
                    }
                    break;

                case "Quantidade":
                    if (!select.Contains(" Quantidade, "))
                    {
                        if (!joins.Contains(" Mercadoria "))
                            joins += " LEFT JOIN T_MERCADORIA Mercadoria ON Mercadoria.RME_CODIGO = RequisicaoMercadoria.RME_CODIGO";

                        select += "SUM(Mercadoria.MER_QUANTIDADE) Quantidade, ";
                    }
                    break;

                case "SetorFuncionario":
                    if (!select.Contains(" SetorFuncionario, "))
                    {
                        if (!joins.Contains(" FuncionarioRequisitado "))
                            joins += " LEFT JOIN T_FUNCIONARIO FuncionarioRequisitado ON FuncionarioRequisitado.FUN_CODIGO = RequisicaoMercadoria.FUN_CODIGO_REQUISITADO";

                        if (!joins.Contains(" Setor "))
                            joins += " LEFT JOIN T_SETOR Setor ON Setor.SET_CODIGO = FuncionarioRequisitado.SET_CODIGO ";

                        select += "Setor.SET_DESCRICAO SetorFuncionario, ";
                        groupBy += "Setor.SET_DESCRICAO, ";
                    }              
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioRequisicaoMercadoria(ref string where, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Compras.FiltroPesquisaRelatorioRequisicaoMercadoria filtrosPesquisa)
        {
            string pattern = "yyyy-MM-dd";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                where += " AND RequisicaoMercadoria.EMP_CODIGO = '" + filtrosPesquisa.CodigoEmpresa.ToString() + "' ";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
                where += " AND RequisicaoMercadoria.FEF_DATA >= '" + filtrosPesquisa.DataInicial.ToString(pattern) + "' ";

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
                where += " AND RequisicaoMercadoria.FEF_DATA <= '" + filtrosPesquisa.DataFinal.ToString(pattern) + "'";

            if (filtrosPesquisa.NumeroInicial > 0)
                where += " AND RequisicaoMercadoria.MER_NUMERO >= " + filtrosPesquisa.NumeroInicial;

            if (filtrosPesquisa.NumeroFinal > 0)
                where += " AND RequisicaoMercadoria.MER_NUMERO <= " + filtrosPesquisa.NumeroFinal;

            if (filtrosPesquisa.Produto > 0)
            {
                if (!joins.Contains(" Mercadoria "))
                    joins += " LEFT JOIN T_MERCADORIA Mercadoria ON Mercadoria.RME_CODIGO = RequisicaoMercadoria.RME_CODIGO";

                if (!joins.Contains(" ProdutoEstoque "))
                    joins += " LEFT JOIN T_PRODUTO_ESTOQUE ProdutoEstoque ON ProdutoEstoque.PRE_CODIGO = Mercadoria.PRE_CODIGO";

                where += " AND ProdutoEstoque.PRO_CODIGO = " + filtrosPesquisa.Produto;
            }

            if (filtrosPesquisa.GrupoProduto > 0)
            {
                if (!joins.Contains(" Mercadoria "))
                    joins += " LEFT JOIN T_MERCADORIA Mercadoria ON Mercadoria.RME_CODIGO = RequisicaoMercadoria.RME_CODIGO";

                if (!joins.Contains(" ProdutoEstoque "))
                    joins += " LEFT JOIN T_PRODUTO_ESTOQUE ProdutoEstoque ON ProdutoEstoque.PRE_CODIGO = Mercadoria.PRE_CODIGO";

                if (!joins.Contains(" Produto "))
                    joins += " LEFT JOIN T_PRODUTO Produto ON Produto.PRO_CODIGO = ProdutoEstoque.PRO_CODIGO";

                where += " AND Produto.GPR_CODIGO = " + filtrosPesquisa.GrupoProduto;
            }

            if (filtrosPesquisa.Colaborador > 0)
                where += " AND RequisicaoMercadoria.FUN_CODIGO = " + filtrosPesquisa.Colaborador;

            if (filtrosPesquisa.FuncionarioRequisitado > 0)
                where += " AND RequisicaoMercadoria.FUN_CODIGO_REQUISITADO = " + filtrosPesquisa.FuncionarioRequisitado;

            if (filtrosPesquisa.Motivo > 0)
                where += " AND RequisicaoMercadoria.MCO_CODIGO = " + filtrosPesquisa.Motivo;

            if ((int)filtrosPesquisa.Tipo > 0)
                where += " AND RequiicaoMercadoria.RME_MODO = " + (int)filtrosPesquisa.Tipo;

            if (filtrosPesquisa.Situacao != null && filtrosPesquisa.Situacao.Count > 0)
                where += " AND RequisicaoMercadoria.RME_SITUACAO in (" + string.Join(", ", filtrosPesquisa.Situacao.Select(o => o.ToString("D"))) + ")";

            if(filtrosPesquisa.SetorFuncionario > 0)
            {
                if (!joins.Contains(" FuncionarioRequisitado "))
                    joins += " LEFT JOIN T_FUNCIONARIO FuncionarioRequisitado ON FuncionarioRequisitado.FUN_CODIGO = RequisicaoMercadoria.FUN_CODIGO_REQUISITADO";

                where += $" AND FuncionarioRequisitado.SET_CODIGO = {filtrosPesquisa.SetorFuncionario} ";
            }

                   

        }

        #endregion
    }
}

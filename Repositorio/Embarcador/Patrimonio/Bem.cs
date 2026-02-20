using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Patrimonio
{
    public class Bem : RepositorioBase<Dominio.Entidades.Embarcador.Patrimonio.Bem>
    {
        public Bem(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Patrimonio.Bem BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.Bem>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Patrimonio.Bem> BuscarTodosAtivos()
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.Bem>();
            var result = from obj in query where !obj.DataBaixa.HasValue select obj;
            return result.ToList();
        }

        public List<Dominio.Entidades.Embarcador.Patrimonio.Bem> Consultar(int codigo, int codigoEmpresa, string descricao, string numeroSerie, int codigoFuncionarioAlocado, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.Bem>();

            var result = from obj in query select obj;

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo == codigo);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(numeroSerie))
                result = result.Where(obj => obj.NumeroSerie.Contains(numeroSerie));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (codigoFuncionarioAlocado > 0)
                result = result.Where(obj => obj.FuncionarioAlocado.Codigo == codigoFuncionarioAlocado);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarConsulta(int codigo, int codigoEmpresa, string descricao, string numeroSerie, int codigoFuncionarioAlocado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.Bem>();

            var result = from obj in query select obj;

            if (codigo > 0)
                result = result.Where(obj => obj.Codigo == codigo);

            if (!string.IsNullOrWhiteSpace(descricao))
                result = result.Where(obj => obj.Descricao.Contains(descricao));

            if (!string.IsNullOrWhiteSpace(numeroSerie))
                result = result.Where(obj => obj.NumeroSerie.Contains(numeroSerie));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (codigoFuncionarioAlocado > 0)
                result = result.Where(obj => obj.FuncionarioAlocado.Codigo == codigoFuncionarioAlocado);

            return result.Count();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Titulo> ConsultarFinanceiroBem(int codigoDocumentoEntrada, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            var result = from obj in query where obj.DuplicataDocumentoEntrada.DocumentoEntrada.Codigo == codigoDocumentoEntrada select obj;

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContarFinanceiroBem(int DocumentoEntrada)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();

            var result = from obj in query where obj.DuplicataDocumentoEntrada.DocumentoEntrada.Codigo == DocumentoEntrada select obj;

            return result.Count();
        }

        public bool ContemBemCadastradoPeloDocumentoEntrada(int codigoItemDocumentoEntrada)
        {
            var bem = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Patrimonio.Bem>()
                .Where(o => o.DocumentoEntradaItem.Codigo == codigoItemDocumentoEntrada)
                .FirstOrDefault();

            return bem != null;
        }

        #region Relatórios de Cadastros

        public IList<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoResponsabilidade> RelatorioTermoResponsabilidade(int codigoBem, int codigoTransferencia)
        {
            string queryResponsavel = @" C.CRE_DESCRICAO CentroResultado,
                                        A.AMX_DESCRICAO Almoxarifado,
                                        E.EMP_FANTASIA Responsavel";
            if (codigoTransferencia > 0)
                queryResponsavel = @" 
                                    (SELECT CRE_DESCRICAO FROM T_BEM_TRANSFERENCIA T
                                    JOIN T_CENTRO_RESULTADO C ON C.CRE_CODIGO = T.CRE_CODIGO
                                    WHERE BTR_CODIGO = " + codigoTransferencia.ToString() + @") CentroResultado,

                                    (SELECT AMX_DESCRICAO FROM T_BEM_TRANSFERENCIA T
                                    JOIN T_ALMOXARIFADO A ON A.AMX_CODIGO = T.AMX_CODIGO
                                    WHERE BTR_CODIGO = " + codigoTransferencia.ToString() + @") Almoxarifado,

                                    (SELECT FUN_NOME FROM T_BEM_TRANSFERENCIA T
                                    JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = T.FUN_CODIGO_ENVIO
                                    WHERE BTR_CODIGO = " + codigoTransferencia.ToString() + ") Responsavel";

            string query = @"   SELECT B.BEM_CODIGO Codigo,
                                B.BEM_DESCRICAO Descricao,
                                B.BEM_NUMERO_SERIE NumeroSerie,

                                E.EMP_CNPJ CNPJEmpresa,
                                E.EMP_FANTASIA FantasiaEmpresa,
                                LEMP.LOC_DESCRICAO CidadeEmpresa,
                                LEMP.UF_SIGLA EstadoEmpresa,
                                
                                " + queryResponsavel + @"

                                FROM T_BEM B
                                LEFT OUTER JOIN T_CENTRO_RESULTADO C ON C.CRE_CODIGO = B.CRE_CODIGO
                                LEFT OUTER JOIN T_ALMOXARIFADO A ON A.AMX_CODIGO = B.AMX_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = B.EMP_CODIGO
                                JOIN T_LOCALIDADES LEMP ON LEMP.LOC_CODIGO = E.LOC_CODIGO
                                WHERE 1 = 1 ";

            if (codigoBem > 0)
                query += " AND B.BEM_CODIGO = " + codigoBem.ToString();
            else if (codigoTransferencia > 0)
                query += " AND B.BEM_CODIGO IN (SELECT BEM_CODIGO FROM T_BEM_TRANSFERENCIA_ITEM WHERE BTR_CODIGO = " + codigoTransferencia.ToString() + ")";  // SQL-INJECTION-SAFE

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoResponsabilidade)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoResponsabilidade>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoRecolhimentoMaterial> RelatorioTermoRecolhimentoMaterial(int codigoTransferencia)
        {
            string query = @"   SELECT B.BEM_CODIGO Codigo,
                                B.BEM_DESCRICAO Descricao,
                                B.BEM_NUMERO_SERIE NumeroSerie,

                                E.EMP_CNPJ CNPJEmpresa,
                                E.EMP_FANTASIA FantasiaEmpresa,
                                LEMP.LOC_DESCRICAO CidadeEmpresa,
                                LEMP.UF_SIGLA EstadoEmpresa,
                                
                                C.CRE_DESCRICAO CentroResultado,
                                A.AMX_DESCRICAO Almoxarifado,
                                F.FUN_NOME Responsavel

                                FROM T_BEM B
                                JOIN T_BEM_TRANSFERENCIA_ITEM I ON I.BEM_CODIGO = B.BEM_CODIGO
                                JOIN T_BEM_TRANSFERENCIA T ON T.BTR_CODIGO = I.BTR_CODIGO
                                JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = T.FUN_CODIGO_ENVIO
                                JOIN T_CENTRO_RESULTADO C ON C.CRE_CODIGO = T.CRE_CODIGO
                                JOIN T_ALMOXARIFADO A ON A.AMX_CODIGO = T.AMX_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = B.EMP_CODIGO
                                JOIN T_LOCALIDADES LEMP ON LEMP.LOC_CODIGO = E.LOC_CODIGO
                                WHERE T.BTR_CODIGO = " + codigoTransferencia.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoRecolhimentoMaterial)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoRecolhimentoMaterial>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoBaixaMaterial> RelatorioTermoBaixaMaterial(int codigoBaixa)
        {
            string query = @"   SELECT B.BEM_CODIGO Codigo,
                                B.BEM_DESCRICAO Descricao,
                                B.BEM_NUMERO_SERIE NumeroSerie,

                                E.EMP_CNPJ CNPJEmpresa,
                                E.EMP_FANTASIA FantasiaEmpresa,
                                LEMP.LOC_DESCRICAO CidadeEmpresa,
                                LEMP.UF_SIGLA EstadoEmpresa,
                                
                                C.CRE_DESCRICAO CentroResultado,
                                A.AMX_DESCRICAO Almoxarifado,
                                F.FUN_NOME Responsavel

                                FROM T_BEM B
                                JOIN T_BEM_BAIXA BB ON BB.BBA_CODIGO = B.BEM_CODIGO
                                JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = BB.FUN_CODIGO
                                LEFT OUTER JOIN T_CENTRO_RESULTADO C ON C.CRE_CODIGO = B.CRE_CODIGO
                                LEFT OUTER JOIN T_ALMOXARIFADO A ON A.AMX_CODIGO = B.AMX_CODIGO
                                JOIN T_EMPRESA E ON E.EMP_CODIGO = B.EMP_CODIGO
                                JOIN T_LOCALIDADES LEMP ON LEMP.LOC_CODIGO = E.LOC_CODIGO
                                WHERE BB.BBA_CODIGO = " + codigoBaixa.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoBaixaMaterial)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.TermoBaixaMaterial>();
        }

        #endregion

        #region Relatório de Bens

        public IList<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.Bem> ConsultarRelatorioBem(int codigoEmpresa, DateTime dataAquisicaoInicial, DateTime dataAquisicaoFinal, int codigoBem, int codigoGrupoProduto, int codigoAlmoxarifado, int codigoCentroResultado, DateTime dataAlocadoInicial, DateTime dataAlocadoFinal, int codigoFuncionarioAlocado, int codigoDefeito, double cpfCnpjPessoa, DateTime dataEntrega, DateTime dataRetorno, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioDespesaBem(codigoEmpresa, dataAquisicaoInicial, dataAquisicaoFinal, codigoBem, codigoGrupoProduto, codigoAlmoxarifado, codigoCentroResultado, dataAlocadoInicial, dataAlocadoFinal, codigoFuncionarioAlocado, codigoDefeito, cpfCnpjPessoa, dataEntrega, dataRetorno, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Patrimonio.Bem)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.Bem>();
        }

        public int ContarConsultaRelatorioBem(int codigoEmpresa, DateTime dataAquisicaoInicial, DateTime dataAquisicaoFinal, int codigoBem, int codigoGrupoProduto, int codigoAlmoxarifado, int codigoCentroResultado, DateTime dataAlocadoInicial, DateTime dataAlocadoFinal, int codigoFuncionarioAlocado, int codigoDefeito, double cpfCnpjPessoa, DateTime dataEntrega, DateTime dataRetorno, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            string sql = ObterSelectConsultaRelatorioDespesaBem(codigoEmpresa, dataAquisicaoInicial, dataAquisicaoFinal, codigoBem, codigoGrupoProduto, codigoAlmoxarifado, codigoCentroResultado, dataAlocadoInicial, dataAlocadoFinal, codigoFuncionarioAlocado, codigoDefeito, cpfCnpjPessoa, dataEntrega, dataRetorno, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioDespesaBem(int codigoEmpresa, DateTime dataAquisicaoInicial, DateTime dataAquisicaoFinal, int codigoBem, int codigoGrupoProduto, int codigoAlmoxarifado, int codigoCentroResultado, DateTime dataAlocadoInicial, DateTime dataAlocadoFinal, int codigoFuncionarioAlocado, int codigoDefeito, double cpfCnpjPessoa, DateTime dataEntrega, DateTime dataRetorno, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioBem(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioBem(ref where, ref groupBy, ref joins, codigoEmpresa, dataAquisicaoInicial, dataAquisicaoFinal, codigoBem, codigoGrupoProduto, codigoAlmoxarifado, codigoCentroResultado, dataAlocadoInicial, dataAlocadoFinal, codigoFuncionarioAlocado, codigoDefeito, cpfCnpjPessoa, dataEntrega, dataRetorno);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioBem(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

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
            query += " FROM T_BEM Bem ";

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

        private void SetarSelectRelatorioConsultaRelatorioBem(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "Bem.BEM_CODIGO Codigo, ";
                        groupBy += "Bem.BEM_CODIGO, ";
                    }
                    break;

                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select += "Bem.BEM_DESCRICAO Descricao, ";
                        groupBy += "Bem.BEM_DESCRICAO, ";
                    }
                    break;

                case "NumeroSerie":
                    if (!select.Contains(" NumeroSerie, "))
                    {
                        select += "Bem.BEM_NUMERO_SERIE NumeroSerie, ";
                        groupBy += "Bem.BEM_NUMERO_SERIE, ";
                    }
                    break;

                case "GrupoProduto":
                    if (!select.Contains(" GrupoProduto, "))
                    {
                        if (!joins.Contains(" GrupoProduto "))
                            joins += " LEFT JOIN T_GRUPO_PRODUTO_TMS GrupoProduto ON GrupoProduto.GPR_CODIGO = Bem.GPR_CODIGO";

                        select += "GrupoProduto.GRP_DESCRICAO GrupoProduto, ";
                        groupBy += "GrupoProduto.GRP_DESCRICAO, ";
                    }
                    break;

                case "Almoxarifado":
                    if (!select.Contains(" Almoxarifado, "))
                    {
                        if (!joins.Contains(" Almoxarifado "))
                            joins += " LEFT JOIN T_ALMOXARIFADO Almoxarifado ON Almoxarifado.AMX_CODIGO = Bem.AMX_CODIGO";

                        select += "Almoxarifado.AMX_DESCRICAO Almoxarifado, ";
                        groupBy += "Almoxarifado.AMX_DESCRICAO, ";
                    }
                    break;

                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        if (!joins.Contains(" CentroResultado "))
                            joins += " LEFT JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = Bem.CRE_CODIGO";

                        select += "CentroResultado.CRE_DESCRICAO CentroResultado, ";
                        groupBy += "CentroResultado.CRE_DESCRICAO, ";
                    }
                    break;

                case "DataAquisicao":
                case "DataAquisicaoFormatada":
                    if (!select.Contains(" DataAquisicao, "))
                    {
                        select += "Bem.BEM_DATA_AQUISICAO DataAquisicao, ";
                        groupBy += "Bem.BEM_DATA_AQUISICAO, ";
                    }
                    break;

                case "ValorBem":
                    if (!select.Contains(" ValorBem, "))
                    {
                        select += "SUM(Bem.BEM_VALOR) ValorBem, ";
                    }
                    break;

                case "PercentualDepreciacao":
                    if (!select.Contains(" PercentualDepreciacao, "))
                    {
                        if (!joins.Contains(" GrupoProduto "))
                            joins += " LEFT JOIN T_GRUPO_PRODUTO_TMS GrupoProduto ON GrupoProduto.GPR_CODIGO = Bem.GPR_CODIGO";

                        select += @"CASE WHEN GrupoProduto.GRP_PERCENTUAL_DEPRECIACAO > 0 THEN GrupoProduto.GRP_PERCENTUAL_DEPRECIACAO
                                    ELSE Bem.BEM_PERCENTUAL_DEPRECIACAO
                                    END PercentualDepreciacao, ";
                        groupBy += "Bem.BEM_PERCENTUAL_DEPRECIACAO, GrupoProduto.GRP_PERCENTUAL_DEPRECIACAO, ";
                    }
                    break;
                case "DepreciacaoAcumulada":
                    if (!select.Contains(" DepreciacaoAcumulada, "))
                    {
                        select += "SUM(Bem.BEM_DEPRECIACAO_ACUMULADA) DepreciacaoAcumulada, ";
                    }
                    break;

                case "DataAlocado":
                case "DataAlocadoFormatada":
                    if (!select.Contains(" DataAlocado, "))
                    {
                        select += "Bem.BEM_DATA_ALOCADO DataAlocado, ";
                        groupBy += "Bem.BEM_DATA_ALOCADO, ";
                    }
                    break;

                case "FuncionarioAlocado":
                    if (!select.Contains(" FuncionarioAlocado, "))
                    {
                        if (!joins.Contains(" FuncionarioAlocado "))
                            joins += " LEFT JOIN T_FUNCIONARIO FuncionarioAlocado ON FuncionarioAlocado.FUN_CODIGO = Bem.FUN_CODIGO_ALOCADO";

                        select += "FuncionarioAlocado.FUN_NOME FuncionarioAlocado, ";
                        groupBy += "FuncionarioAlocado.FUN_NOME, ";
                    }
                    break;

                case "DataGarantiaFormatada":
                    if (!select.Contains(" DataGarantia, "))
                    {
                        if (!joins.Contains(" ManutencaoBem "))
                            joins += " LEFT JOIN T_BEM_MANUTENCAO ManutencaoBem ON ManutencaoBem.BEM_CODIGO = Bem.BEM_CODIGO";

                        select += "ManutencaoBem.BMA_DATA_GARANTIA DataGarantia, ";
                        groupBy += "ManutencaoBem.BMA_DATA_GARANTIA, ";
                    }
                    break;

                case "DataEntregaFormatada":
                    if (!select.Contains(" DataEntrega, "))
                    {
                        if (!joins.Contains(" ManutencaoBem "))
                            joins += " LEFT JOIN T_BEM_MANUTENCAO ManutencaoBem ON ManutencaoBem.BEM_CODIGO = Bem.BEM_CODIGO";

                        select += "ManutencaoBem.BMA_DATA_ENTREGA DataEntrega, ";
                        groupBy += "ManutencaoBem.BMA_DATA_ENTREGA, ";
                    }
                    break;

                case "ValorOrcado":
                    if (!select.Contains(" ValorOrcado, "))
                    {
                        if (!joins.Contains(" ManutencaoBem "))
                            joins += " LEFT JOIN T_BEM_MANUTENCAO ManutencaoBem ON ManutencaoBem.BEM_CODIGO = Bem.BEM_CODIGO";

                        select += "ManutencaoBem.BMA_VALOR_ORCADO ValorOrcado, ";
                        groupBy += "ManutencaoBem.BMA_VALOR_ORCADO, ";
                    }
                    break;

                case "ValorPago":
                    if (!select.Contains(" ValorPago, "))
                    {
                        if (!joins.Contains(" ManutencaoBem "))
                            joins += " LEFT JOIN T_BEM_MANUTENCAO ManutencaoBem ON ManutencaoBem.BEM_CODIGO = Bem.BEM_CODIGO";

                        select += "ManutencaoBem.BMA_VALOR_PAGO ValorPago, ";
                        groupBy += "ManutencaoBem.BMA_VALOR_PAGO, ";
                    }
                    break;

                case "Defeito":
                    if (!select.Contains(" Defeito, "))
                    {
                        if (!joins.Contains(" ManutencaoBem "))
                            joins += " LEFT JOIN T_BEM_MANUTENCAO ManutencaoBem ON ManutencaoBem.BEM_CODIGO = Bem.BEM_CODIGO";

                        if (!joins.Contains(" MotivoDefeito "))
                            joins += " LEFT JOIN T_BEM_MOTIVO_DEFEITO MotivoDefeito ON MotivoDefeito.BMD_CODIGO = ManutencaoBem.BMD_CODIGO";

                        select += "MotivoDefeito.BMD_DESCRICAO Defeito, ";
                        groupBy += "MotivoDefeito.BMD_DESCRICAO, ";
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select += "Bem.BEM_OBSERVACAO Observacao, ";
                        groupBy += "Bem.BEM_OBSERVACAO, ";
                    }
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioBem(ref string where, ref string groupBy, ref string joins, int codigoEmpresa, DateTime dataAquisicaoInicial, DateTime dataAquisicaoFinal, int codigoBem, int codigoGrupoProduto, int codigoAlmoxarifado, int codigoCentroResultado, DateTime dataAlocadoInicial, DateTime dataAlocadoFinal, int codigoFuncionarioAlocado, int codigoDefeito, double cpfCnpjPessoa, DateTime dataEntrega, DateTime dataRetorno)
        {
            string pattern = "yyyy-MM-dd";

            if (codigoEmpresa > 0)
                where += " AND Bem.EMP_CODIGO = '" + codigoEmpresa.ToString() + "' ";

            if (dataAquisicaoInicial != DateTime.MinValue)
                where += " AND Bem.BEM_DATA_AQUISICAO >= '" + dataAquisicaoInicial.ToString(pattern) + "' ";

            if (dataAquisicaoFinal != DateTime.MinValue)
                where += " AND Bem.BEM_DATA_AQUISICAO <= '" + dataAquisicaoFinal.ToString(pattern) + "'";

            if (dataAlocadoInicial != DateTime.MinValue)
                where += " AND Bem.BEM_DATA_ALOCADO >= '" + dataAlocadoInicial.ToString(pattern) + "' ";

            if (dataAlocadoFinal != DateTime.MinValue)
                where += " AND Bem.BEM_DATA_ALOCADO <= '" + dataAlocadoFinal.ToString(pattern) + "'";

            if (codigoBem > 0)
                where += " AND Bem.BEM_CODIGO = " + codigoBem;

            if (codigoGrupoProduto > 0)
                where += " AND Bem.GPR_CODIGO = " + codigoGrupoProduto;

            if (codigoAlmoxarifado > 0)
                where += " AND Bem.AMX_CODIGO = " + codigoAlmoxarifado;

            if (codigoCentroResultado > 0)
                where += " AND Bem.CRE_CODIGO = " + codigoCentroResultado;

            if (codigoFuncionarioAlocado > 0)
                where += " AND Bem.FUN_CODIGO_ALOCADO = " + codigoFuncionarioAlocado;

            if (codigoDefeito > 0)
            {
                where += " AND MotivoDefeito.BMD_CODIGO = " + codigoDefeito;

                if (!joins.Contains(" ManutencaoBem "))
                    joins += " LEFT JOIN T_BEM_MANUTENCAO ManutencaoBem ON ManutencaoBem.BEM_CODIGO = Bem.BEM_CODIGO";

                if (!joins.Contains(" MotivoDefeito "))
                    joins += " LEFT JOIN T_BEM_MOTIVO_DEFEITO MotivoDefeito ON MotivoDefeito.BMD_CODIGO = ManutencaoBem.BMD_CODIGO";
            }

            if (cpfCnpjPessoa > 0)
            {
                where += " AND ManutencaoBem.CLI_CGCCPF = " + cpfCnpjPessoa;

                if (!joins.Contains(" ManutencaoBem "))
                    joins += " LEFT JOIN T_BEM_MANUTENCAO ManutencaoBem ON ManutencaoBem.BEM_CODIGO = Bem.BEM_CODIGO";
            }

            if (dataEntrega != DateTime.MinValue)
            {
                where += " AND ManutencaoBem.BMA_DATA_ENTREGA >= '" + dataEntrega.ToString(pattern) + "' ";

                if (!joins.Contains(" ManutencaoBem "))
                    joins += " LEFT JOIN T_BEM_MANUTENCAO ManutencaoBem ON ManutencaoBem.BEM_CODIGO = Bem.BEM_CODIGO";
            }

            if (dataRetorno != DateTime.MinValue)
            {
                where += " AND ManutencaoBem.BMA_DATA_RETORNO <= '" + dataRetorno.ToString(pattern) + "'";

                if (!joins.Contains(" ManutencaoBem "))
                    joins += " LEFT JOIN T_BEM_MANUTENCAO ManutencaoBem ON ManutencaoBem.BEM_CODIGO = Bem.BEM_CODIGO";
            }
        }

        #endregion

        #region Relatório de Mapa de Depreciação

        public IList<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.MapaDepreciacao> ConsultarRelatorioMapaDepreciacao(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int codigoBem, int codigoGrupoProduto, int codigoAlmoxarifado, int codigoCentroResultado, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioDespesaMapaDepreciacao(codigoEmpresa, dataInicial, dataFinal, codigoBem, codigoGrupoProduto, codigoAlmoxarifado, codigoCentroResultado, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Patrimonio.MapaDepreciacao)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Patrimonio.MapaDepreciacao>();
        }

        public int ContarConsultaRelatorioMapaDepreciacao(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int codigoBem, int codigoGrupoProduto, int codigoAlmoxarifado, int codigoCentroResultado, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena)
        {
            string sql = ObterSelectConsultaRelatorioDespesaMapaDepreciacao(codigoEmpresa, dataInicial, dataFinal, codigoBem, codigoGrupoProduto, codigoAlmoxarifado, codigoCentroResultado, true, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, 0, 0);
            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioDespesaMapaDepreciacao(int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int codigoBem, int codigoGrupoProduto, int codigoAlmoxarifado, int codigoCentroResultado, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   orderBy = string.Empty;

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioMapaDepreciacao(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref groupBy, ref joins, count);

            SetarWhereRelatorioConsultaRelatorioMapaDepreciacao(ref where, ref groupBy, ref joins, codigoEmpresa, dataInicial, dataFinal, codigoBem, codigoGrupoProduto, codigoAlmoxarifado, codigoCentroResultado);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioMapaDepreciacao(propAgrupa, 0, ref select, ref groupBy, ref joins, count);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena) && propOrdena != "Codigo")
                        if (propOrdena == "MesAno")
                            orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + "Ano " + dirOrdena + ", Mes " + dirOrdena;
                        else
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
            query += " FROM T_BEM Bem ";

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

        private void SetarSelectRelatorioConsultaRelatorioMapaDepreciacao(string propriedade, int codigoDinamico, ref string select, ref string groupBy, ref string joins, bool count)
        {
            switch (propriedade)
            {
                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "Bem.BEM_CODIGO Codigo, ";
                        groupBy += "Bem.BEM_CODIGO, ";
                    }
                    break;
                case "Descricao":
                    if (!select.Contains(" Descricao, "))
                    {
                        select += "Bem.BEM_DESCRICAO Descricao, ";
                        groupBy += "Bem.BEM_DESCRICAO, ";
                    }
                    break;
                case "NumeroSerie":
                    if (!select.Contains(" NumeroSerie, "))
                    {
                        select += "Bem.BEM_NUMERO_SERIE NumeroSerie, ";
                        groupBy += "Bem.BEM_NUMERO_SERIE, ";
                    }
                    break;
                case "GrupoProduto":
                    if (!select.Contains(" GrupoProduto, "))
                    {
                        if (!joins.Contains(" GrupoProduto "))
                            joins += " LEFT JOIN T_GRUPO_PRODUTO_TMS GrupoProduto ON GrupoProduto.GPR_CODIGO = Bem.GPR_CODIGO";

                        select += "GrupoProduto.GRP_DESCRICAO GrupoProduto, ";
                        groupBy += "GrupoProduto.GRP_DESCRICAO, ";
                    }
                    break;
                case "Almoxarifado":
                    if (!select.Contains(" Almoxarifado, "))
                    {
                        if (!joins.Contains(" Almoxarifado "))
                            joins += " LEFT JOIN T_ALMOXARIFADO Almoxarifado ON Almoxarifado.AMX_CODIGO = Bem.AMX_CODIGO";

                        select += "Almoxarifado.AMX_DESCRICAO Almoxarifado, ";
                        groupBy += "Almoxarifado.AMX_DESCRICAO, ";
                    }
                    break;
                case "CentroResultado":
                    if (!select.Contains(" CentroResultado, "))
                    {
                        if (!joins.Contains(" CentroResultado "))
                            joins += " LEFT JOIN T_CENTRO_RESULTADO CentroResultado ON CentroResultado.CRE_CODIGO = Bem.CRE_CODIGO";

                        select += "CentroResultado.CRE_DESCRICAO CentroResultado, ";
                        groupBy += "CentroResultado.CRE_DESCRICAO, ";
                    }
                    break;
                case "ValorBem":
                    if (!select.Contains(" ValorBem, "))
                    {
                        select += "SUM(Bem.BEM_VALOR) ValorBem, ";
                    }
                    break;
                case "PercentualDepreciacao":
                    if (!select.Contains(" PercentualDepreciacao, "))
                    {
                        if (!joins.Contains(" GrupoProduto "))
                            joins += " LEFT JOIN T_GRUPO_PRODUTO_TMS GrupoProduto ON GrupoProduto.GPR_CODIGO = Bem.GPR_CODIGO";

                        select += @"CASE WHEN GrupoProduto.GRP_PERCENTUAL_DEPRECIACAO > 0 THEN GrupoProduto.GRP_PERCENTUAL_DEPRECIACAO
                                    ELSE Bem.BEM_PERCENTUAL_DEPRECIACAO
                                    END PercentualDepreciacao, ";
                        groupBy += "Bem.BEM_PERCENTUAL_DEPRECIACAO, GrupoProduto.GRP_PERCENTUAL_DEPRECIACAO, ";
                    }
                    break;
                case "MesAno":
                    if (!select.Contains(" MesAno, "))
                    {
                        if (!joins.Contains(" Depreciacao "))
                            joins += " JOIN T_BEM_DEPRECIACAO Depreciacao ON Depreciacao.BEM_CODIGO = Bem.BEM_CODIGO";

                        select += "REPLICATE('0', 2 - LEN(CAST(Depreciacao.BDE_MES AS VARCHAR(2)))) + CAST(Depreciacao.BDE_MES AS VARCHAR(2)) + '-' + CAST(Depreciacao.BDE_ANO AS VARCHAR(4)) MesAno, ";
                        select += "Depreciacao.BDE_ANO Ano, ";
                        select += "Depreciacao.BDE_MES Mes, ";
                        groupBy += "Depreciacao.BDE_MES, Depreciacao.BDE_ANO, ";
                    }
                    break;
                case "ValorDepreciacao":
                    if (!select.Contains(" ValorDepreciacao, "))
                    {
                        if (!joins.Contains(" Depreciacao "))
                            joins += " JOIN T_BEM_DEPRECIACAO Depreciacao ON Depreciacao.BEM_CODIGO = Bem.BEM_CODIGO";

                        select += "SUM(Depreciacao.BDE_VALOR) ValorDepreciacao, ";
                    }
                    break;

                default:
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioMapaDepreciacao(ref string where, ref string groupBy, ref string joins, int codigoEmpresa, DateTime dataInicial, DateTime dataFinal, int codigoBem, int codigoGrupoProduto, int codigoAlmoxarifado, int codigoCentroResultado)
        {
            if (!joins.Contains(" Depreciacao "))
                joins += " JOIN T_BEM_DEPRECIACAO Depreciacao ON Depreciacao.BEM_CODIGO = Bem.BEM_CODIGO";

            if (codigoEmpresa > 0)
                where += " AND Bem.EMP_CODIGO = '" + codigoEmpresa.ToString() + "' ";

            if (dataInicial != DateTime.MinValue)
                where += " AND Depreciacao.BDE_MES >= " + dataInicial.Month + " AND Depreciacao.BDE_ANO >= " + dataInicial.Year;

            if (dataFinal != DateTime.MinValue)
                where += " AND Depreciacao.BDE_MES <= " + dataFinal.Month + " AND Depreciacao.BDE_ANO <= " + dataFinal.Year;

            if (codigoBem > 0)
                where += " AND Bem.BEM_CODIGO = " + codigoBem;

            if (codigoGrupoProduto > 0)
                where += " AND Bem.GPR_CODIGO = " + codigoGrupoProduto;

            if (codigoAlmoxarifado > 0)
                where += " AND Bem.AMX_CODIGO = " + codigoAlmoxarifado;

            if (codigoCentroResultado > 0)
                where += " AND Bem.CRE_CODIGO = " + codigoCentroResultado;
        }

        #endregion

    }
}
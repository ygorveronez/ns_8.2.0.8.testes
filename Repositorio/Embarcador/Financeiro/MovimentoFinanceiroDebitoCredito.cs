using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using System.Globalization;
using Repositorio.Embarcador.Consulta;

namespace Repositorio.Embarcador.Financeiro
{
    public class MovimentoFinanceiroDebitoCredito : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>
    {
        public MovimentoFinanceiroDebitoCredito(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public bool ConteMovimentacaoEmOutraConciliacao(int codigoMovimento, int codigoConciliacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria>();
            var result = from obj in query where obj.Codigo != codigoConciliacao && obj.SituacaoConciliacaoBancaria != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoConciliacaoBancaria.Cancelado && obj.Movimentos.Any(e => e.Codigo == codigoMovimento && e.MovimentoConcolidado == true) select obj;
            return result.Any();
        }

        public bool ContemMovimentacaoConciliacao(string numeroDocumento, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento tipoDocumento, string observacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();
            var result = from obj in query where obj.MovimentoFinanceiro.Observacao.Contains(observacao) && obj.MovimentoConcolidado == true && obj.MovimentoFinanceiro.Documento == numeroDocumento && obj.MovimentoFinanceiro.TipoDocumentoMovimento == tipoDocumento select obj;
            return result.Any();
        }

        public bool MovimentacaoConcilidada(int contaDebito, int contaCredito, decimal valor, string numeroDocumento, int codigoTitulo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento tipoDocumento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();

            query = query.Where(o => o.MovimentoConcolidado == true && o.MovimentoFinanceiro.TipoDocumentoMovimento == tipoDocumento);

            if (contaDebito > 0)
                query = query.Where(o => o.MovimentoFinanceiro.PlanoDeContaDebito.Codigo == contaDebito);
            if (contaCredito > 0)
                query = query.Where(o => o.MovimentoFinanceiro.PlanoDeContaCredito.Codigo == contaCredito);
            if (valor > 0)
                query = query.Where(o => o.MovimentoFinanceiro.Valor == valor);
            if (!string.IsNullOrWhiteSpace(numeroDocumento))
                query = query.Where(o => o.MovimentoFinanceiro.Documento == numeroDocumento);
            if (codigoTitulo > 0)
                query = query.Where(o => o.MovimentoFinanceiro.Titulo.Codigo == codigoTitulo);

            return query.Any();
        }

        public bool MovimentacaoConcilidada(int codigoTituloBaixa, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento tipoDocumento)
        {
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito> query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();
            IQueryable<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado> queryTituloBaixaAgrupado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            queryTituloBaixaAgrupado = queryTituloBaixaAgrupado.Where(o => o.TituloBaixa.Codigo == codigoTituloBaixa);

            query = query.Where(o => o.MovimentoConcolidado && o.MovimentoFinanceiro.TipoDocumentoMovimento == tipoDocumento && queryTituloBaixaAgrupado.Any(t => t.Titulo == o.MovimentoFinanceiro.Titulo));

            return query.Select(o => o.Codigo).Any();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito> BuscarMovimentoParaConciliacaoBancaria(string planoContaSintetico, int codigoPlanoConta, DateTime? dataInicial, DateTime? dataFinal, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();

            if (codigoPlanoConta > 0)
                query = query.Where(obj => obj.PlanoDeConta.Codigo == codigoPlanoConta && obj.MovimentoConcolidado != true);
            else
                query = query.Where(obj => obj.PlanoDeConta.Plano.StartsWith(planoContaSintetico) && obj.PlanoDeConta.AnaliticoSintetico == AnaliticoSintetico.Analitico && obj.MovimentoConcolidado != true);

            if (dataInicial.HasValue)
                query = query.Where(obj => obj.DataMovimento.Date >= dataInicial.Value.Date);
            if (dataFinal.HasValue)
                query = query.Where(obj => obj.DataMovimento.Date <= dataFinal.Value.Date);

            if (codigoEmpresa > 0)
                query = query.Where(obj => obj.MovimentoFinanceiro.Empresa.Codigo == codigoEmpresa);

            var queryConciliacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria>();
            if (codigoEmpresa > 0)
                queryConciliacao = queryConciliacao.Where(obj => obj.Empresa.Codigo == codigoEmpresa);
            query = query.Where(obj => !queryConciliacao.Any(a => a.Movimentos.Any(m => m.Codigo == obj.Codigo && m.MovimentoConcolidado == true && a.SituacaoConciliacaoBancaria != SituacaoConciliacaoBancaria.Cancelado)));

            return query.ToList();
        }

        public Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito BuscarPorMovimentoFinanceiro(int codigo, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito tipo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();
            var result = from obj in query where obj.MovimentoFinanceiro.Codigo == codigo && obj.DebitoCredito == tipo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito> Consultar(int codigoConta, DateTime dataConsulta, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.PlanoDeConta.Codigo == codigoConta && obj.DataMovimento.Date >= dataConsulta.Date);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();

        }

        public int ContarConsulta(int codigoConta, DateTime dataConsulta)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.PlanoDeConta.Codigo == codigoConta && obj.DataMovimento.Date >= dataConsulta.Date);

            return result.Count();
        }

        public void SetarMovimentosConcolidados(List<int> codigos, bool consolidado)
        {
            UnitOfWork.Sessao.CreateQuery("UPDATE MovimentoFinanceiroDebitoCredito SET MovimentoConcolidado = :consolidado WHERE Codigo IN (:codigos)")
                .SetParameterList("codigos", codigos)
                .SetParameter("consolidado", consolidado)
                .ExecuteUpdate();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.BalanceteGerencial> ConsultarRelatorioBalanceteGerencial(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioBalanceteGerencial filtrosPesquisa, string propGrupo, string dirOrdenacaoGrupo, int inicio, int limite)
        {
            string datePatternInicial = "yyyy-MM-dd 00:00:00";
            string datePatternFinal = "yyyy-MM-dd 23:59:59";

            string saldoAnterior = $@" , coalesce((SELECT sum(CASE 
																WHEN MDC_TIPO = 1 and PP.PLA_RECEITA_DESPESA <> 2 THEN MDC_VALOR 
																WHEN PP.PLA_RECEITA_DESPESA = 1 THEN MDC_VALOR 
																WHEN MDC_TIPO = 2 and PP.PLA_RECEITA_DESPESA <> 1 THEN MDC_VALOR * -1 
																WHEN PP.PLA_RECEITA_DESPESA = 2 THEN MDC_VALOR * -1 
																ELSE 0.00 END)
                                                    FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF 
                                                    JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO 
                                                    JOIN T_PLANO_DE_CONTA PP ON PP.PLA_CODIGO = MF.PLA_CODIGO
                                                    WHERE MDC_DATA < '{ filtrosPesquisa.DataInicial.ToString(datePatternInicial) }' AND MF.PLA_CODIGO = xx.PLA_CODIGO";
            string saldoFinal = $@" , coalesce((SELECT sum(CASE 
																WHEN MDC_TIPO = 1 and PP.PLA_RECEITA_DESPESA <> 2 THEN MDC_VALOR 
																WHEN PP.PLA_RECEITA_DESPESA = 1 THEN MDC_VALOR 
																WHEN MDC_TIPO = 2 and PP.PLA_RECEITA_DESPESA <> 1 THEN MDC_VALOR * -1 
																WHEN PP.PLA_RECEITA_DESPESA = 2 THEN MDC_VALOR * -1 
																ELSE 0.00 END)
                                                    FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF 
                                                    JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO 
                                                    JOIN T_PLANO_DE_CONTA PP ON PP.PLA_CODIGO = MF.PLA_CODIGO
                                                    WHERE MDC_DATA <= '{ filtrosPesquisa.DataFinal.ToString(datePatternFinal) }' AND MF.PLA_CODIGO = xx.PLA_CODIGO";

            string centroResultado = $@"SUBSTRING((SELECT DISTINCT ', ' + centroResultado.CRE_DESCRICAO
				                            FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF 
                                            JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO 
                                            JOIN T_CENTRO_RESULTADO centroResultado ON centroResultado.CRE_CODIGO = MM.CRE_CODIGO 
				                            WHERE MF.MDC_DATA >= '{filtrosPesquisa.DataInicial.ToString(datePatternInicial)}' AND MF.MDC_DATA <= '{filtrosPesquisa.DataFinal.ToString(datePatternFinal)}' 
                                                AND MF.PLA_CODIGO = pc.PLA_CODIGO ";

            string planoCentroResultado = $@"SUBSTRING((SELECT DISTINCT ', ' + centroResultado.CRE_PLANO
				                            FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF 
                                            JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO 
                                            JOIN T_CENTRO_RESULTADO centroResultado ON centroResultado.CRE_CODIGO = MM.CRE_CODIGO 
				                            WHERE MF.MDC_DATA >= '{filtrosPesquisa.DataInicial.ToString(datePatternInicial)}' AND MF.MDC_DATA <= '{filtrosPesquisa.DataFinal.ToString(datePatternFinal)}' 
                                                AND MF.PLA_CODIGO = pc.PLA_CODIGO ";

            string condicoesSql = "";
            string sqlPlano = "";

            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                saldoAnterior += " AND MM.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
                saldoFinal += " AND MM.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
                centroResultado += " AND MM.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
                planoCentroResultado += " AND MM.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
                condicoesSql = " AND M.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString() + " ";
                sqlPlano = " AND planoConta.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString() + " ";
            }

            if (filtrosPesquisa.CodigosCentroResultado.Count > 0)
            {
                saldoAnterior += $" AND MM.CRE_CODIGO IN ({ string.Join(", ", filtrosPesquisa.CodigosCentroResultado) })";
                saldoFinal += $" AND MM.CRE_CODIGO IN ({ string.Join(", ", filtrosPesquisa.CodigosCentroResultado) })";
                condicoesSql += $" AND M.CRE_CODIGO IN ({ string.Join(", ", filtrosPesquisa.CodigosCentroResultado) })";
            }

            saldoAnterior += "),0) AS saldo_inicial ";
            saldoFinal += "),0) AS saldo_final ";
            centroResultado += "FOR XML PATH('')), 3, 1000) AS CentroResultado ";
            planoCentroResultado += "FOR XML PATH('')), 3, 1000) AS PlanoCentroResultado ";

            if (!string.IsNullOrWhiteSpace(condicoesSql))
                condicoesSql = " WHERE 1 = 1" + condicoesSql;

            string sqlQuery = $@"SELECT  pc.PLA_PLANO AS Plano
                                    , pc.PLA_DESCRICAO AS Descricao
                                    , pc.PLA_TIPO AS TipoConta
                                    , sum(xx.saldo_inicial) AS SaldoInicial
                                    , sum(xx.entrada) AS Entrada
                                    , sum(xx.saida) AS Saida
                                    , sum(xx.saldo_final) AS SaldoFinal
                                    , { centroResultado }
                                    , { planoCentroResultado }
                                    , pc.PLA_RECEITA_DESPESA ReceitaDespesa
                                    , pc.PLA_CODIGO AS CodigoPlanoConta
                              FROM
                                  (
                                  SELECT xx.PLA_CODIGO
		                               , xx.PLA_PLANO
                                       { saldoAnterior }
                                       , sum(x.entrada) AS entrada
                                       , sum(x.saida) AS saida
                                       { saldoFinal }
                                      FROM (SELECT planoConta.PLA_CODIGO,
				                                planoConta.PLA_PLANO
                                          FROM T_PLANO_DE_CONTA planoConta                                           
                                                 WHERE 1 = 1 { sqlPlano }
                                          ) as xx 
                                          LEFT OUTER JOIN ( 
                                          SELECT planoConta.PLA_CODIGO
				                               , planoConta.PLA_PLANO
				                               , CASE 
											   WHEN mdc.MDC_TIPO = 1 and planoConta.PLA_RECEITA_DESPESA <> 2 THEN
				 			                              mdc.MDC_VALOR				 		                              
													  WHEN planoConta.PLA_RECEITA_DESPESA = 1 THEN
				 			                              mdc.MDC_VALOR
				 		                              ELSE
				 			                              0.00 
				 	                             END AS entrada 
				                               , CASE WHEN mdc.MDC_TIPO = 2 and planoConta.PLA_RECEITA_DESPESA <> 1 THEN
				 			                              mdc.MDC_VALOR
													 WHEN planoConta.PLA_RECEITA_DESPESA = 2 THEN
				 			                              mdc.MDC_VALOR
				 		                              ELSE
				 			                              0.00 
				 	                             END AS saida
                                          FROM T_PLANO_DE_CONTA planoConta 
                                          LEFT JOIN T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO mdc ON mdc.PLA_CODIGO = planoConta.PLA_CODIGO
                                            AND mdc.MDC_DATA >= '{ filtrosPesquisa.DataInicial.ToString(datePatternInicial) }' AND mdc.MDC_DATA <= '{ filtrosPesquisa.DataFinal.ToString(datePatternFinal) }'
                                          LEFT JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = mdc.MOV_CODIGO
                                                { condicoesSql }
                                          ) AS x ON x.PLA_CODIGO = xx.PLA_CODIGO
                                  GROUP BY xx.PLA_CODIGO, xx.PLA_PLANO
                                  ) AS xx
                              JOIN T_PLANO_DE_CONTA pc
                                  ON xx.PLA_PLANO LIKE pc.PLA_PLANO + '%' ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPlanoContaSintetica))
                sqlQuery += "and pc.PLA_PLANO LIKE '" + filtrosPesquisa.NumeroPlanoContaSintetica + "%' ";

            if (filtrosPesquisa.TipoConta.HasValue)
                sqlQuery += "and pc.PLA_TIPO = " + filtrosPesquisa.TipoConta.Value.ToString("d") + " ";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                sqlQuery += "and pc.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            sqlQuery += @"GROUP BY pc.PLA_PLANO, pc.PLA_DESCRICAO, pc.PLA_RECEITA_DESPESA, pc.PLA_CODIGO, pc.PLA_TIPO
                          HAVING sum(xx.saldo_inicial) <> 0 or sum(xx.entrada) <> 0 or sum(xx.saida) <> 0 or sum(xx.saldo_final) <> 0";

            if (!string.IsNullOrWhiteSpace(propGrupo))
                sqlQuery += " ORDER BY " + propGrupo + " " + dirOrdenacaoGrupo + ", Plano ASC";
            else
                sqlQuery += " ORDER BY Plano ASC";

            if (inicio > 0 || limite > 0)
                sqlQuery += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY;";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.BalanceteGerencial)));

            return query.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.BalanceteGerencial>();
        }

        public int ContarConsultaRelatorioBalanceteGerencial(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioBalanceteGerencial filtrosPesquisa)
        {
            string datePatternInicial = "yyyy-MM-dd 00:00:00";
            string datePatternFinal = "yyyy-MM-dd 23:59:59";

            string saldoAnterior = $@" , coalesce((SELECT sum(CASE 
																WHEN MDC_TIPO = 1 and PP.PLA_RECEITA_DESPESA <> 2 THEN MDC_VALOR 
																WHEN PP.PLA_RECEITA_DESPESA = 1 THEN MDC_VALOR 
																WHEN MDC_TIPO = 2 and PP.PLA_RECEITA_DESPESA <> 1 THEN MDC_VALOR * -1 
																WHEN PP.PLA_RECEITA_DESPESA = 2 THEN MDC_VALOR * -1 
																ELSE 0.00 END)
                                                    FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF 
                                                    JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO 
                                                    JOIN T_PLANO_DE_CONTA PP ON PP.PLA_CODIGO = MF.PLA_CODIGO
                                                    WHERE MDC_DATA < '{ filtrosPesquisa.DataInicial.ToString(datePatternInicial) }' AND MF.PLA_CODIGO = xx.PLA_CODIGO";
            string saldoFinal = $@" , coalesce((SELECT sum(CASE 
																WHEN MDC_TIPO = 1 and PP.PLA_RECEITA_DESPESA <> 2 THEN MDC_VALOR 
																WHEN PP.PLA_RECEITA_DESPESA = 1 THEN MDC_VALOR 
																WHEN MDC_TIPO = 2 and PP.PLA_RECEITA_DESPESA <> 1 THEN MDC_VALOR * -1 
																WHEN PP.PLA_RECEITA_DESPESA = 2 THEN MDC_VALOR * -1 
																ELSE 0.00 END)
                                                    FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF 
                                                    JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO 
                                                    JOIN T_PLANO_DE_CONTA PP ON PP.PLA_CODIGO = MF.PLA_CODIGO
                                                    WHERE MDC_DATA <= '{ filtrosPesquisa.DataFinal.ToString(datePatternFinal) }' AND MF.PLA_CODIGO = xx.PLA_CODIGO";

            string condicoesSql = "";
            string sqlPlano = "";

            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                saldoAnterior += " AND MM.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
                saldoFinal += " AND MM.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
                condicoesSql = " AND M.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString() + " ";
                sqlPlano = " AND planoConta.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString() + " ";
            }

            if (filtrosPesquisa.CodigosCentroResultado.Count > 0)
            {
                saldoAnterior += $" AND MM.CRE_CODIGO IN ({ string.Join(", ", filtrosPesquisa.CodigosCentroResultado) })";
                saldoFinal += $" AND MM.CRE_CODIGO IN ({ string.Join(", ", filtrosPesquisa.CodigosCentroResultado) })";
                condicoesSql += $" AND M.CRE_CODIGO IN ({ string.Join(", ", filtrosPesquisa.CodigosCentroResultado) })";
            }

            saldoAnterior += "),0) AS saldo_inicial ";
            saldoFinal += "),0) AS saldo_final ";

            if (!string.IsNullOrWhiteSpace(condicoesSql))
                condicoesSql = " WHERE 1 = 1" + condicoesSql;

            string sqlQuery = $@"SELECT distinct(count(0) over ())
                              FROM
                                  (
                                  SELECT xx.PLA_CODIGO
		                               , xx.PLA_PLANO
                                       { saldoAnterior }
                                       , sum(x.entrada) AS entrada
                                       , sum(x.saida) AS saida
                                       { saldoFinal }
                                      FROM
                                          (SELECT planoConta.PLA_CODIGO,
				                                planoConta.PLA_PLANO
                                          FROM T_PLANO_DE_CONTA planoConta                                           
                                                 WHERE 1 = 1 { sqlPlano }
                                          ) as xx 
                                          LEFT OUTER JOIN (  
                                          SELECT planoConta.PLA_CODIGO
				                               , planoConta.PLA_PLANO
				                               , CASE WHEN mdc.MDC_TIPO = 1 THEN
				 			                              mdc.MDC_VALOR
				 		                              ELSE
				 			                              0.00 
				 	                             END AS entrada 
				                               , CASE WHEN mdc.MDC_TIPO = 2 THEN
				 			                              mdc.MDC_VALOR
				 		                              ELSE
				 			                              0.00 
				 	                             END AS saida
                                          FROM T_PLANO_DE_CONTA planoConta 
                                          LEFT JOIN T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO mdc ON mdc.PLA_CODIGO = planoConta.PLA_CODIGO
                                            AND mdc.MDC_DATA >= '{ filtrosPesquisa.DataInicial.ToString(datePatternInicial) }' AND mdc.MDC_DATA <= '{ filtrosPesquisa.DataFinal.ToString(datePatternFinal) }'
                                          LEFT JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = mdc.MOV_CODIGO 
                                                { condicoesSql }
                                          ) AS x ON x.PLA_CODIGO = xx.PLA_CODIGO
                                  GROUP BY xx.PLA_CODIGO, xx.PLA_PLANO
                                  ) AS xx
                              JOIN T_PLANO_DE_CONTA pc
                                  ON xx.PLA_PLANO LIKE pc.PLA_PLANO + '%' ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroPlanoContaSintetica))
                sqlQuery += "and pc.PLA_PLANO LIKE '" + filtrosPesquisa.NumeroPlanoContaSintetica + "%' ";

            if (filtrosPesquisa.TipoConta.HasValue)
                sqlQuery += "and pc.PLA_TIPO = " + filtrosPesquisa.TipoConta.Value.ToString("d") + " ";

            if (filtrosPesquisa.CodigoEmpresa > 0)
                sqlQuery += "and pc.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            sqlQuery += @"GROUP BY pc.PLA_PLANO, pc.PLA_DESCRICAO, pc.PLA_CODIGO 
                          HAVING sum(xx.saldo_inicial) <> 0 or sum(xx.entrada) <> 0 or sum(xx.saida) <> 0 or sum(xx.saldo_final) <> 0";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            return query.SetTimeout(600).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DRE> ConsultarRelatorioDRE(DateTime dataInicial, DateTime dataFinal, AnaliticoSintetico? tipoConta, int inicio, int limite, int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            string saldoAnterior = @" , coalesce((SELECT sum(CASE 
																WHEN MDC_TIPO = 1 and PP.PLA_RECEITA_DESPESA <> 2 THEN MDC_VALOR 
																WHEN PP.PLA_RECEITA_DESPESA = 1 THEN MDC_VALOR 
																WHEN MDC_TIPO = 2 and PP.PLA_RECEITA_DESPESA <> 1 THEN MDC_VALOR * -1 
																WHEN PP.PLA_RECEITA_DESPESA = 2 THEN MDC_VALOR * -1 
																ELSE 0.00 END) 
										FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF 
										JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO 
										JOIN T_PLANO_DE_CONTA PP ON PP.PLA_CODIGO = MF.PLA_CODIGO
										WHERE MONTH(MDC_DATA) = " + dataInicial.Month.ToString() + @" and YEAR(MDC_DATA) = " + dataInicial.Year.ToString() + @" AND MF.PLA_CODIGO = xx.PLA_CODIGO),0) AS saldo_inicial  ";

            string saldoFinal = @" , coalesce((SELECT sum(CASE 
																WHEN MDC_TIPO = 1 and PP.PLA_RECEITA_DESPESA <> 2 THEN MDC_VALOR 
																WHEN PP.PLA_RECEITA_DESPESA = 1 THEN MDC_VALOR 
																WHEN MDC_TIPO = 2 and PP.PLA_RECEITA_DESPESA <> 1 THEN MDC_VALOR * -1 
																WHEN PP.PLA_RECEITA_DESPESA = 2 THEN MDC_VALOR * -1 
																ELSE 0.00 END) 
										FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF 
										JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO 
										JOIN T_PLANO_DE_CONTA PP ON PP.PLA_CODIGO = MF.PLA_CODIGO
										WHERE MONTH(MDC_DATA) = " + dataFinal.Month.ToString() + @"  AND YEAR(MDC_DATA) = " + dataFinal.Year.ToString() + @" AND MF.PLA_CODIGO = xx.PLA_CODIGO),0) AS saldo_final  ";
            string joinEmpresa = "";
            string sqlPlano = "";
            if (codigoEmpresa > 0)
            {
                saldoAnterior = @" , coalesce((SELECT sum(CASE 
																WHEN MDC_TIPO = 1 and PP.PLA_RECEITA_DESPESA <> 2 THEN MDC_VALOR 
																WHEN PP.PLA_RECEITA_DESPESA = 1 THEN MDC_VALOR 
																WHEN MDC_TIPO = 2 and PP.PLA_RECEITA_DESPESA <> 1 THEN MDC_VALOR * -1 
																WHEN PP.PLA_RECEITA_DESPESA = 2 THEN MDC_VALOR * -1 
																ELSE 0.00 END) 
										FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF 
										JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO 
										JOIN T_PLANO_DE_CONTA PP ON PP.PLA_CODIGO = MF.PLA_CODIGO
										WHERE MONTH(MDC_DATA) = " + dataInicial.Month.ToString() + @" and YEAR(MDC_DATA) = " + dataInicial.Year.ToString() + @" AND MF.PLA_CODIGO = xx.PLA_CODIGO AND MM.EMP_CODIGO = " + codigoEmpresa.ToString() + "),0) AS saldo_inicial  ";

                saldoFinal = @" , coalesce((SELECT sum(CASE 
																WHEN MDC_TIPO = 1 and PP.PLA_RECEITA_DESPESA <> 2 THEN MDC_VALOR 
																WHEN PP.PLA_RECEITA_DESPESA = 1 THEN MDC_VALOR 
																WHEN MDC_TIPO = 2 and PP.PLA_RECEITA_DESPESA <> 1 THEN MDC_VALOR * -1 
																WHEN PP.PLA_RECEITA_DESPESA = 2 THEN MDC_VALOR * -1 
																ELSE 0.00 END) 
										FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF 
										JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO 
										JOIN T_PLANO_DE_CONTA PP ON PP.PLA_CODIGO = MF.PLA_CODIGO
										WHERE MONTH(MDC_DATA) = " + dataFinal.Month.ToString() + @"  AND YEAR(MDC_DATA) = " + dataFinal.Year.ToString() + @" AND MF.PLA_CODIGO = xx.PLA_CODIGO AND MM.EMP_CODIGO = " + codigoEmpresa.ToString() + "),0) AS saldo_final  ";

                joinEmpresa = " JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = MDC.MOV_CODIGO where M.EMP_CODIGO = " + codigoEmpresa.ToString() + " "; ;
                sqlPlano = " AND planoConta.EMP_CODIGO = " + codigoEmpresa.ToString() + " ";
            }

            string sqlQuery = @"SELECT pc.PLA_CODIGO Codigo,  pc.PLA_PLANO AS Plano
                                    , pc.PLA_DESCRICAO AS Descricao
                                    , sum(xx.saldo_inicial) AS SaldoInicial
                                    , sum(xx.saldo_final) AS SaldoAtual
									, 0.0 Variacao
									, pc.PLA_TIPO Tipo 
									, pc.PLA_RECEITA_DESPESA ReceitaDespesa
                              FROM
                                  (
                                  SELECT xx.PLA_CODIGO
		                               , xx.PLA_PLANO
                                       " + saldoAnterior + @"
                                       " + saldoFinal + @"										
                                      FROM
                                          (SELECT planoConta.PLA_CODIGO,
				                                planoConta.PLA_PLANO, planoConta.PLA_RECEITA_DESPESA
                                          FROM T_PLANO_DE_CONTA planoConta                                           
                                                 WHERE 1 = 1 " + sqlPlano + @"	
                                          ) as xx 
										  LEFT OUTER JOIN ( 
                                          SELECT planoConta.PLA_CODIGO
				                               , planoConta.PLA_PLANO
                                              FROM T_PLANO_DE_CONTA planoConta 
                                         LEFT JOIN T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO AS mdc
				                                ON mdc.PLA_CODIGO = planoConta.PLA_CODIGO AND ((MONTH(mdc.MDC_DATA) = " + dataInicial.Month.ToString() + @"  and YEAR(mdc.MDC_DATA) = " + dataInicial.Year.ToString() + @") OR (MONTH(mdc.MDC_DATA) = " + dataFinal.Month.ToString() + @" and YEAR(mdc.MDC_DATA) = " + dataFinal.Year.ToString() + @")) 
                                                " + joinEmpresa + @"
                                          ) AS x ON x.PLA_CODIGO = xx.PLA_CODIGO
                                  GROUP BY xx.PLA_CODIGO, xx.PLA_PLANO
                                  ) AS xx
                              JOIN T_PLANO_DE_CONTA pc
                                  ON xx.PLA_PLANO LIKE pc.PLA_PLANO + '%' AND (pc.PLA_RECEITA_DESPESA = 1 or pc.PLA_RECEITA_DESPESA = 2) ";

            if (tipoConta.HasValue)
                sqlQuery += "and pc.PLA_TIPO = " + tipoConta.Value.ToString("d") + " ";

            if (codigoEmpresa > 0)
                sqlQuery += "and pc.EMP_CODIGO = " + codigoEmpresa.ToString();

            sqlQuery += "GROUP BY pc.PLA_PLANO, pc.PLA_DESCRICAO, pc.PLA_TIPO, pc.PLA_RECEITA_DESPESA, pc.PLA_CODIGO ORDER BY Plano ASC";

            if (inicio > 0 || limite > 0)
                sqlQuery += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY;";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.DRE)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DRE>();
        }

        public int ContarConsultaRelatorioDRE(DateTime dataInicial, DateTime dataFinal, AnaliticoSintetico? tipoConta, int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            string saldoAnterior = @" , coalesce((SELECT sum(CASE MDC_TIPO WHEN 1 THEN MDC_VALOR WHEN 2 THEN MDC_VALOR * -1 ELSE 0.00 END) 
										FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO WHERE MONTH(MDC_DATA) = " + dataInicial.Month.ToString() + @" and YEAR(MDC_DATA) = " + dataInicial.Year.ToString() + @" AND PLA_CODIGO = xx.PLA_CODIGO),0) AS saldo_inicial ";
            string saldoFinal = @" , coalesce((SELECT sum(CASE MDC_TIPO WHEN 1 THEN MDC_VALOR WHEN 2 THEN MDC_VALOR * -1 ELSE 0.00 END) 
										FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO WHERE MONTH(MDC_DATA) = " + dataFinal.Month.ToString() + @"  AND YEAR(MDC_DATA) = " + dataFinal.Year.ToString() + @" AND PLA_CODIGO = xx.PLA_CODIGO),0) AS saldo_final ";
            string joinEmpresa = "";
            string sqlPlano = "";
            if (codigoEmpresa > 0)
            {
                saldoAnterior = @" , coalesce((SELECT sum(CASE MDC_TIPO WHEN 1 THEN MDC_VALOR WHEN 2 THEN MDC_VALOR * -1 ELSE 0.00 END) 
										FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO WHERE MONTH(MDC_DATA) = " + dataInicial.Month.ToString() + @" and YEAR(MDC_DATA) = " + dataInicial.Year.ToString() + @" AND PLA_CODIGO = xx.PLA_CODIGO AND MM.EMP_CODIGO = " + codigoEmpresa.ToString() + "),0) AS saldo_inicial ";

                saldoFinal = @" , coalesce((SELECT sum(CASE MDC_TIPO WHEN 1 THEN MDC_VALOR WHEN 2 THEN MDC_VALOR * -1 ELSE 0.00 END) 
										FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO WHERE MONTH(MDC_DATA) = " + dataFinal.Month.ToString() + @"  AND YEAR(MDC_DATA) = " + dataFinal.Year.ToString() + @" AND PLA_CODIGO = xx.PLA_CODIGO AND MM.EMP_CODIGO = " + codigoEmpresa.ToString() + "),0) AS saldo_final ";
                joinEmpresa = " JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = MDC.MOV_CODIGO where M.EMP_CODIGO = " + codigoEmpresa.ToString() + " "; ;
                sqlPlano = " AND planoConta.EMP_CODIGO = " + codigoEmpresa.ToString() + " ";
            }
            string sqlQuery = @"SELECT distinct(count(0) over ())
                              FROM
                                  (
                                  SELECT xx.PLA_CODIGO
		                               , xx.PLA_PLANO
                                       " + saldoAnterior + @"
                                       " + saldoFinal + @"
                                      FROM
                                          (SELECT planoConta.PLA_CODIGO,
				                                planoConta.PLA_PLANO, planoConta.PLA_RECEITA_DESPESA
                                          FROM T_PLANO_DE_CONTA planoConta                                           
                                                 WHERE 1 = 1 " + sqlPlano + @"	
                                          ) as xx
                                        LEFT OUTER JOIN (
                                          SELECT planoConta.PLA_CODIGO
				                               , planoConta.PLA_PLANO
                                              FROM T_PLANO_DE_CONTA planoConta 
                                         LEFT JOIN T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO AS mdc
				                                ON mdc.PLA_CODIGO = planoConta.PLA_CODIGO AND ((MONTH(mdc.MDC_DATA) = " + dataInicial.Month.ToString() + @"  and YEAR(mdc.MDC_DATA) = " + dataInicial.Year.ToString() + @") OR (MONTH(mdc.MDC_DATA) = " + dataFinal.Month.ToString() + @" and YEAR(mdc.MDC_DATA) = " + dataFinal.Year.ToString() + @"))
                                                " + joinEmpresa + @"
                                          ) AS x ON x.PLA_CODIGO = xx.PLA_CODIGO
                                  GROUP BY xx.PLA_CODIGO, xx.PLA_PLANO
                                  ) AS xx
                              JOIN T_PLANO_DE_CONTA pc
                                  ON xx.PLA_PLANO LIKE pc.PLA_PLANO + '%' AND (pc.PLA_RECEITA_DESPESA = 1 or pc.PLA_RECEITA_DESPESA = 2) ";

            if (tipoConta.HasValue)
                sqlQuery += "and pc.PLA_TIPO = " + tipoConta.Value.ToString("d") + " ";

            if (codigoEmpresa > 0)
                sqlQuery += "and pc.EMP_CODIGO = " + codigoEmpresa.ToString();

            sqlQuery += "GROUP BY pc.PLA_PLANO, pc.PLA_DESCRICAO, pc.PLA_TIPO, pc.PLA_RECEITA_DESPESA, pc.PLA_CODIGO";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            return query.UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DREGerencial> ConsultarRelatorioDREGerencial(DateTime dataInicial, DateTime dataFinal, int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            string saldoAnterior = @" , coalesce((SELECT sum(CASE 
																WHEN MDC_TIPO = 1 and PP.PLA_RECEITA_DESPESA <> 2 THEN MDC_VALOR 
																WHEN PP.PLA_RECEITA_DESPESA = 1 THEN MDC_VALOR 
																WHEN MDC_TIPO = 2 and PP.PLA_RECEITA_DESPESA <> 1 THEN MDC_VALOR * -1 
																WHEN PP.PLA_RECEITA_DESPESA = 2 THEN MDC_VALOR * -1 
																ELSE 0.00 END) 
										FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF 
										JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO 
										JOIN T_PLANO_DE_CONTA PP ON PP.PLA_CODIGO = MF.PLA_CODIGO
										WHERE MONTH(MDC_DATA) = " + dataInicial.Month.ToString() + @" and YEAR(MDC_DATA) = " + dataInicial.Year.ToString() + @" AND MF.PLA_CODIGO = xx.PLA_CODIGO),0) AS saldo_inicial  ";

            string saldoFinal = @" , coalesce((SELECT sum(CASE 
																WHEN MDC_TIPO = 1 and PP.PLA_RECEITA_DESPESA <> 2 THEN MDC_VALOR 
																WHEN PP.PLA_RECEITA_DESPESA = 1 THEN MDC_VALOR 
																WHEN MDC_TIPO = 2 and PP.PLA_RECEITA_DESPESA <> 1 THEN MDC_VALOR * -1 
																WHEN PP.PLA_RECEITA_DESPESA = 2 THEN MDC_VALOR * -1 
																ELSE 0.00 END) 
										FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF 
										JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO 
										JOIN T_PLANO_DE_CONTA PP ON PP.PLA_CODIGO = MF.PLA_CODIGO
										WHERE MONTH(MDC_DATA) = " + dataFinal.Month.ToString() + @"  AND YEAR(MDC_DATA) = " + dataFinal.Year.ToString() + @" AND MF.PLA_CODIGO = xx.PLA_CODIGO),0) AS saldo_final  ";
            string joinEmpresa = "";
            string sqlPlano = "";
            if (codigoEmpresa > 0)
            {
                string ambiente = "";
                if ((int)tipoAmbiente > 0)
                    ambiente = " AND MM.MOV_AMBIENTE = " + Convert.ToString((int)tipoAmbiente);

                saldoAnterior = @" , coalesce((SELECT sum(CASE 
																WHEN MDC_TIPO = 1 and PP.PLA_RECEITA_DESPESA <> 2 THEN MDC_VALOR 
																WHEN PP.PLA_RECEITA_DESPESA = 1 THEN MDC_VALOR 
																WHEN MDC_TIPO = 2 and PP.PLA_RECEITA_DESPESA <> 1 THEN MDC_VALOR * -1 
																WHEN PP.PLA_RECEITA_DESPESA = 2 THEN MDC_VALOR * -1 
																ELSE 0.00 END) 
										FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF 
										JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO 
										JOIN T_PLANO_DE_CONTA PP ON PP.PLA_CODIGO = MF.PLA_CODIGO
										WHERE MONTH(MDC_DATA) = " + dataInicial.Month.ToString() + @" and YEAR(MDC_DATA) = " + dataInicial.Year.ToString() + @" AND MF.PLA_CODIGO = xx.PLA_CODIGO AND MM.EMP_CODIGO = " + codigoEmpresa.ToString() + "),0) AS saldo_inicial  ";

                saldoFinal = @" , coalesce((SELECT sum(CASE 
																WHEN MDC_TIPO = 1 and PP.PLA_RECEITA_DESPESA <> 2 THEN MDC_VALOR 
																WHEN PP.PLA_RECEITA_DESPESA = 1 THEN MDC_VALOR 
																WHEN MDC_TIPO = 2 and PP.PLA_RECEITA_DESPESA <> 1 THEN MDC_VALOR * -1 
																WHEN PP.PLA_RECEITA_DESPESA = 2 THEN MDC_VALOR * -1 
																ELSE 0.00 END) 
										FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF 
										JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO 
										JOIN T_PLANO_DE_CONTA PP ON PP.PLA_CODIGO = MF.PLA_CODIGO
										WHERE MONTH(MDC_DATA) = " + dataFinal.Month.ToString() + @"  AND YEAR(MDC_DATA) = " + dataFinal.Year.ToString() + @" AND MF.PLA_CODIGO = xx.PLA_CODIGO AND MM.EMP_CODIGO = " + codigoEmpresa.ToString() + "),0) AS saldo_final  ";

                if ((int)tipoAmbiente > 0)
                    ambiente = " AND M.MOV_AMBIENTE = " + Convert.ToString((int)tipoAmbiente);

                joinEmpresa = " JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = MDC.MOV_CODIGO where M.EMP_CODIGO = " + codigoEmpresa.ToString() + ambiente + " ";
                sqlPlano = " AND planoConta.EMP_CODIGO = " + codigoEmpresa.ToString() + " ";
            }

            string sqlQuery = @"SELECT pc.PLA_CODIGO Codigo,  pc.PLA_PLANO AS Plano
                                    , pc.PLA_DESCRICAO AS Descricao
                                    , sum(xx.saldo_inicial) AS SaldoInicial
                                    , sum(xx.saldo_final) AS SaldoAtual
									, pc.PLA_TIPO Tipo 
									, pc.PLA_RECEITA_DESPESA ReceitaDespesa
                                    , pc.PLA_GRUPO_DE_RESULTADO GrupoResultado
                                    , CASE 
										WHEN  pc.PLA_GRUPO_DE_RESULTADO = 1 THEN 1
										WHEN  pc.PLA_GRUPO_DE_RESULTADO = 2 THEN 1
										WHEN  pc.PLA_GRUPO_DE_RESULTADO = 3 THEN 2
										WHEN  pc.PLA_GRUPO_DE_RESULTADO = 4 THEN 3
										WHEN  pc.PLA_GRUPO_DE_RESULTADO = 5 THEN 4
										WHEN  pc.PLA_GRUPO_DE_RESULTADO = 6 THEN 4
										WHEN  pc.PLA_GRUPO_DE_RESULTADO = 7 THEN 5
										WHEN  pc.PLA_GRUPO_DE_RESULTADO = 8 THEN 6
									  ELSE 0
									  END GrupoTotalizador
                              FROM
                                  (
                                  SELECT xx.PLA_CODIGO
		                               , xx.PLA_PLANO
                                       " + saldoAnterior + @"
                                       " + saldoFinal + @"										
                                      FROM
                                           (SELECT planoConta.PLA_CODIGO,
				                                planoConta.PLA_PLANO, planoConta.PLA_RECEITA_DESPESA
                                          FROM T_PLANO_DE_CONTA planoConta                                           
                                                 WHERE 1 = 1 " + sqlPlano + @"	
                                          ) as xx 
										  LEFT OUTER JOIN (
                                          SELECT planoConta.PLA_CODIGO
				                               , planoConta.PLA_PLANO
                                              FROM T_PLANO_DE_CONTA planoConta 
                                         LEFT JOIN T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO AS mdc
				                                ON mdc.PLA_CODIGO = planoConta.PLA_CODIGO AND ((MONTH(mdc.MDC_DATA) = " + dataInicial.Month.ToString() + @"  and YEAR(mdc.MDC_DATA) = " + dataInicial.Year.ToString() + @") OR (MONTH(mdc.MDC_DATA) = " + dataFinal.Month.ToString() + @" and YEAR(mdc.MDC_DATA) = " + dataFinal.Year.ToString() + @")) 
                                                " + joinEmpresa + @"
                                          ) AS x ON x.PLA_CODIGO = xx.PLA_CODIGO
                                  GROUP BY xx.PLA_CODIGO, xx.PLA_PLANO
                                  ) AS xx
                              JOIN T_PLANO_DE_CONTA pc
                                  ON xx.PLA_PLANO LIKE pc.PLA_PLANO + '%' AND (pc.PLA_RECEITA_DESPESA = 1 or pc.PLA_RECEITA_DESPESA = 2)  AND pc.PLA_GRUPO_DE_RESULTADO > 0 ";

            if (codigoEmpresa > 0)
                sqlQuery += "and pc.EMP_CODIGO = " + codigoEmpresa.ToString();

            sqlQuery += "GROUP BY pc.PLA_PLANO, pc.PLA_DESCRICAO, pc.PLA_TIPO, pc.PLA_RECEITA_DESPESA, pc.PLA_CODIGO, pc.PLA_GRUPO_DE_RESULTADO ORDER BY Plano ASC";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.DREGerencial)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.DREGerencial>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.MovimentoConciliacaoBancaria> ConsultarMovimentoConcolidados(int codigoConciliacaoBancaria)
        {
            string query = @"SELECT MF.MDC_CODIGO Codigo,
                MF.PLA_CODIGO CodigoPlano,
                MF.MDC_DATA DataMovimento,
                CASE
	                WHEN MF.MDC_TIPO = 2 THEN MF.MDC_VALOR
	                ELSE 0
                END ValorDebito,
                CASE
	                WHEN MF.MDC_TIPO = 1 THEN MF.MDC_VALOR
	                ELSE 0
                END ValorCredito,
                M.MOV_DOCUMENTO Documento,
                M.MOV_OBSERVACAO Observacao,
                M.MOV_TIPO Tipo,
                ISNULL(C.CLI_NOME, G.GRP_DESCRICAO) Pessoa,
                MF.MDC_MOVIMENTO_CONCOLIDADO MovimentoConcolidado,
                CASE 
                WHEN M.MOV_OBSERVACAO LIKE 'COMPENSAÇÃO DO CHEQUE Nº%' THEN M.MOV_OBSERVACAO
                WHEN M.MOV_TIPO = 6 OR M.MOV_TIPO = 7 THEN ISNULL(SUBSTRING((SELECT DISTINCT ', ' + CAST(C.CHQ_NUMERO_CHEQUE AS NVARCHAR(20))
	                                                                    FROM T_TITULO_BAIXA_CHEQUE T
	                                                                    inner join T_CHEQUE C ON C.CHQ_CODIGO = T.CHQ_CODIGO
	                                                                    WHERE CAST(T.TIB_CODIGO AS VARCHAR(20)) = M.MOV_DOCUMENTO FOR XML PATH('')), 3, 1000), '')
				ELSE ''
				END Cheques
                FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF
                JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = MF.MOV_CODIGO
                LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = M.CLI_CGCCPF
                LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = M.GRP_CODIGO
                JOIN T_CONCILIACAO_BANCARIA_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO CC ON CC.MDC_CODIGO = MF.MDC_CODIGO
                WHERE MF.MDC_MOVIMENTO_CONCOLIDADO = 1 AND CC.COB_CODIGO = " + codigoConciliacaoBancaria.ToString("D");


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.MovimentoConciliacaoBancaria)));

            return nhQuery.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Financeiro.MovimentoConciliacaoBancaria>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.MovimentoConciliacaoBancaria> ConsultarMovimentoConciliacaoBancaria(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoBancariaMovimento filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var parametros = new List<ParametroSQL>();

            string queryOrdenacao = "";

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                queryOrdenacao += " order by " + propOrdenacao + " " + dirOrdenacao;

            string query = @"SELECT MF.MDC_CODIGO Codigo,
                MF.PLA_CODIGO CodigoPlano,
                MF.MDC_DATA DataMovimento,
                CASE
	                WHEN MF.MDC_TIPO = 2 THEN MF.MDC_VALOR
	                ELSE 0
                END ValorDebito,
                CASE
	                WHEN MF.MDC_TIPO = 1 THEN MF.MDC_VALOR
	                ELSE 0
                END ValorCredito,
                M.MOV_DOCUMENTO Documento,
                M.MOV_OBSERVACAO Observacao,
                M.MOV_TIPO Tipo,
                ISNULL(C.CLI_NOME, G.GRP_DESCRICAO) Pessoa,
                MF.MDC_MOVIMENTO_CONCOLIDADO MovimentoConcolidado,
                CASE 
                WHEN M.MOV_OBSERVACAO LIKE 'COMPENSAÇÃO DO CHEQUE Nº%' THEN M.MOV_DOCUMENTO
                WHEN M.TIT_CODIGO IS NOT NULL AND M.MOV_TIPO = 6 OR M.MOV_TIPO = 7 THEN ISNULL(SUBSTRING((SELECT DISTINCT ', ' + CAST(C.CHQ_NUMERO_CHEQUE AS NVARCHAR(20))
	                                                                    FROM T_TITULO_BAIXA_CHEQUE T
	                                                                    inner join T_CHEQUE C ON C.CHQ_CODIGO = T.CHQ_CODIGO
	                                                                    WHERE CAST(T.TIB_CODIGO AS VARCHAR(20)) = M.MOV_DOCUMENTO FOR XML PATH('')), 3, 1000), '')
				ELSE ''
				END Cheques
                FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF
                JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = MF.MOV_CODIGO
                LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = M.CLI_CGCCPF
                LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = M.GRP_CODIGO
                JOIN T_CONCILIACAO_BANCARIA_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO CC ON CC.MDC_CODIGO = MF.MDC_CODIGO
                WHERE CC.COB_CODIGO = " + filtrosPesquisa.CodigoConciliacaoBancaria.ToString("D");

            string pattern = "yyyy-MM-dd";
            if (filtrosPesquisa.DataPesquisaMovimento > DateTime.MinValue && filtrosPesquisa.DataAtePesquisaMovimento == DateTime.MinValue)
                query += " AND MF.MDC_DATA > '" + filtrosPesquisa.DataPesquisaMovimento.AddDays(-1).ToString(pattern) + "'";
            else if (filtrosPesquisa.DataPesquisaMovimento == DateTime.MinValue && filtrosPesquisa.DataAtePesquisaMovimento > DateTime.MinValue)
                query += " AND MF.MDC_DATA < '" + filtrosPesquisa.DataAtePesquisaMovimento.AddDays(1).ToString(pattern) + "'";
            else if (filtrosPesquisa.DataPesquisaMovimento > DateTime.MinValue && filtrosPesquisa.DataAtePesquisaMovimento > DateTime.MinValue)
                query += " AND MF.MDC_DATA > '" + filtrosPesquisa.DataPesquisaMovimento.AddDays(-1).ToString(pattern) + "' AND MF.MDC_DATA < '" + filtrosPesquisa.DataAtePesquisaMovimento.AddDays(1).ToString(pattern) + "' ";

            if (filtrosPesquisa.DebitoCreditoMovimento == DebitoCredito.Credito)
                query += " AND MF.MDC_TIPO = 1 ";
            else if (filtrosPesquisa.DebitoCreditoMovimento == DebitoCredito.Debito)
                query += " AND MF.MDC_TIPO = 2 ";

            if (filtrosPesquisa.ValorPesquisaMovimento > 0 && filtrosPesquisa.ValorAtePesquisaMovimento == 0)
                query += " AND MF.MDC_VALOR >= " + String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", filtrosPesquisa.ValorPesquisaMovimento).Replace(",", ".");
            else if (filtrosPesquisa.ValorPesquisaMovimento > 0 && filtrosPesquisa.ValorAtePesquisaMovimento > 0)
                query += " AND MF.MDC_VALOR >= " + String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", filtrosPesquisa.ValorPesquisaMovimento).Replace(",", ".") + " AND MF.MDC_VALOR <= " + String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", filtrosPesquisa.ValorAtePesquisaMovimento).Replace(",", ".");
            else if (filtrosPesquisa.ValorPesquisaMovimento == 0 && filtrosPesquisa.ValorAtePesquisaMovimento > 0)
                query += " AND MF.MDC_VALOR <= " + String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", filtrosPesquisa.ValorAtePesquisaMovimento).Replace(",", ".");
            if (filtrosPesquisa.CnpjPessoa > 0)
                query += " AND M.CLI_CGCCPF = " + filtrosPesquisa.CnpjPessoa;
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumentoPesquisaMovimento))
                query += " AND M.MOV_DOCUMENTO = '" + filtrosPesquisa.NumeroDocumentoPesquisaMovimento + "'";
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.ObservacaoMovimento))
                query += " AND M.MOV_OBSERVACAO LIKE '%" + filtrosPesquisa.ObservacaoMovimento + "%'";
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroChequePesquisaMovimento))
            {
                query += " AND M.MOV_DOCUMENTO IN (SELECT CAST(T.TIB_CODIGO AS VARCHAR(20)) FROM T_TITULO_BAIXA_CHEQUE T inner join T_CHEQUE C ON C.CHQ_CODIGO = T.CHQ_CODIGO WHERE C.CHQ_NUMERO_CHEQUE = :C_CHQ_NUMERO_CHEQUE)";
                parametros.Add(new ParametroSQL("C_CHQ_NUMERO_CHEQUE", filtrosPesquisa.NumeroChequePesquisaMovimento));
            }
            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
                query += " AND G.GRP_CODIGO = " + filtrosPesquisa.CodigoGrupoPessoa;
            if (filtrosPesquisa.CodigoTitulo > 0)
                query += " AND M.TIT_CODIGO = " + filtrosPesquisa.CodigoTitulo;

            query += queryOrdenacao;
            if (maximoRegistros > 0)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var sqlDinamico = new SQLDinamico(query, parametros);

            var nhQuery = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.MovimentoConciliacaoBancaria)));

            return nhQuery.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Financeiro.MovimentoConciliacaoBancaria>();
        }

        public int ContarMovimentoConciliacaoBancaria(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaConciliacaoBancariaMovimento filtrosPesquisa)
        {
            var parametros = new List<ParametroSQL>();

            string query = @"SELECT COUNT(0) as CONTADOR
                FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF
                JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = MF.MOV_CODIGO
                LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = M.CLI_CGCCPF
                LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = M.GRP_CODIGO
                JOIN T_CONCILIACAO_BANCARIA_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO CC ON CC.MDC_CODIGO = MF.MDC_CODIGO
                WHERE CC.COB_CODIGO = " + filtrosPesquisa.CodigoConciliacaoBancaria.ToString("D");

            string pattern = "yyyy-MM-dd";
            if (filtrosPesquisa.DataPesquisaMovimento > DateTime.MinValue && filtrosPesquisa.DataAtePesquisaMovimento == DateTime.MinValue)
                query += " AND MF.MDC_DATA > '" + filtrosPesquisa.DataPesquisaMovimento.AddDays(-1).ToString(pattern) + "'";
            else if (filtrosPesquisa.DataPesquisaMovimento == DateTime.MinValue && filtrosPesquisa.DataAtePesquisaMovimento > DateTime.MinValue)
                query += " AND MF.MDC_DATA < '" + filtrosPesquisa.DataAtePesquisaMovimento.AddDays(1).ToString(pattern) + "'";
            else if (filtrosPesquisa.DataPesquisaMovimento > DateTime.MinValue && filtrosPesquisa.DataAtePesquisaMovimento > DateTime.MinValue)
                query += " AND MF.MDC_DATA > '" + filtrosPesquisa.DataPesquisaMovimento.AddDays(-1).ToString(pattern) + "' AND MF.MDC_DATA < '" + filtrosPesquisa.DataAtePesquisaMovimento.AddDays(1).ToString(pattern) + "' ";

            if (filtrosPesquisa.ValorPesquisaMovimento > 0 && filtrosPesquisa.ValorAtePesquisaMovimento == 0)
                query += " AND MF.MDC_VALOR >= " + String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", filtrosPesquisa.ValorPesquisaMovimento).Replace(",", ".");
            else if (filtrosPesquisa.ValorPesquisaMovimento > 0 && filtrosPesquisa.ValorAtePesquisaMovimento > 0)
                query += " AND MF.MDC_VALOR >= " + String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", filtrosPesquisa.ValorPesquisaMovimento).Replace(",", ".") + " AND MF.MDC_VALOR <= " + String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", filtrosPesquisa.ValorAtePesquisaMovimento).Replace(",", ".");
            else if (filtrosPesquisa.ValorPesquisaMovimento == 0 && filtrosPesquisa.ValorAtePesquisaMovimento > 0)
                query += " AND MF.MDC_VALOR <= " + String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", filtrosPesquisa.ValorAtePesquisaMovimento).Replace(",", ".");
            if (filtrosPesquisa.CnpjPessoa > 0)
                query += " AND M.CLI_CGCCPF = " + filtrosPesquisa.CnpjPessoa;
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumentoPesquisaMovimento))
                query += " AND M.MOV_DOCUMENTO = '" + filtrosPesquisa.NumeroDocumentoPesquisaMovimento + "'";
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.ObservacaoMovimento))
                query += " AND M.MOV_OBSERVACAO LIKE '%" + filtrosPesquisa.ObservacaoMovimento + "%'";
            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroChequePesquisaMovimento))
            {
                query += " AND M.MOV_DOCUMENTO IN (SELECT CAST(T.TIB_CODIGO AS VARCHAR(20)) FROM T_TITULO_BAIXA_CHEQUE T inner join T_CHEQUE C ON C.CHQ_CODIGO = T.CHQ_CODIGO WHERE C.CHQ_NUMERO_CHEQUE = :C_CHQ_NUMERO_CHEQUE)";
                parametros.Add(new ParametroSQL("C_CHQ_NUMERO_CHEQUE", filtrosPesquisa.NumeroChequePesquisaMovimento));
            }
            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
                query += " AND G.GRP_CODIGO = " + filtrosPesquisa.CodigoGrupoPessoa;
            if (filtrosPesquisa.CodigoTitulo > 0)
                query += " AND M.TIT_CODIGO = " + filtrosPesquisa.CodigoTitulo;

            var sqlDinamico = new SQLDinamico(query, parametros);

            var nhQuery = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return nhQuery.SetTimeout(50000).UniqueResult<int>();
        }

        public void DeletarMovimentoEmOutraConciliacao(int codigoMovimento, int codigoConciliacao)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CONCILIACAO_BANCARIA_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO WHERE MDC_CODIGO = " + codigoMovimento + " AND COB_CODIGO <> " + codigoConciliacao).ExecuteUpdate(); // SQL-INJECTION-SAFE
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CONCILIACAO_BANCARIA_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO WHERE MDC_CODIGO = " + codigoMovimento + " AND COB_CODIGO <> " + codigoConciliacao).ExecuteUpdate(); // SQL-INJECTION-SAFE

                        UnitOfWork.CommitChanges();
                    }
                    catch
                    {
                        UnitOfWork.Rollback();
                        throw;
                    }
                }
            }
            catch (NHibernate.Exceptions.GenericADOException ex)
            {
                if (ex.InnerException != null && object.ReferenceEquals(ex.InnerException.GetType(), typeof(System.Data.SqlClient.SqlException)))
                {
                    System.Data.SqlClient.SqlException excecao = (System.Data.SqlClient.SqlException)ex.InnerException;
                    if (excecao.Number == 547)
                    {
                        throw new Exception("O registro possui dependências e não pode ser excluido.", ex);
                    }
                }
                throw;
            }
        }

        public decimal SomaDocumentos(List<int> codigos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito tipo)
        {
            string query = @"SELECT ISNULL(SUM(MDC_VALOR), 0) FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO M
                    WHERE MDC_CODIGO IN (" + string.Join(", ", codigos) + @") AND MDC_TIPO = " + (int)tipo;

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.SetTimeout(50000).UniqueResult<decimal>();
        }

        #endregion
    }
}

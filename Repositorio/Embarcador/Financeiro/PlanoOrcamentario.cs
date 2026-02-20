using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro
{
    public class PlanoOrcamentario : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario>
    {
        public PlanoOrcamentario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        public Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public int ExisteCentroResultadoMesAno(int codigoPlanoOrcamentario, int codigoCentroResultado, DateTime dataBase)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario>();
            var result = from obj in query where obj.Codigo != codigoPlanoOrcamentario && obj.CentroResultado.Codigo == codigoCentroResultado && obj.DataBase.Month == dataBase.Month && obj.DataBase.Year == dataBase.Year select obj;
            return result.Count() > 0 ? result.First().Codigo : 0;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario> Consulta(int codigoCentroResultado, DateTime dataBase, int empresa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario>();

            var result = from obj in query select obj;

            if (dataBase > DateTime.MinValue)
                result = result.Where(obj => obj.DataBase.Month == dataBase.Month && obj.DataBase.Year == dataBase.Year);

            if (codigoCentroResultado > 0)
                result = result.Where(obj => obj.CentroResultado.Codigo == codigoCentroResultado);

            result = result.Where(obj => obj.Empresa.Codigo == empresa);

            return result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending")).Skip(inicioRegistros).Take(maximoRegistros).ToList();
        }

        public int ContaConsulta(int codigoCentroResultado, DateTime dataBase, int empresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario>();

            var result = from obj in query select obj;

            if (dataBase > DateTime.MinValue)
                result = result.Where(obj => obj.DataBase.Month == dataBase.Month && obj.DataBase.Year == dataBase.Year);

            if (codigoCentroResultado > 0)
                result = result.Where(obj => obj.CentroResultado.Codigo == codigoCentroResultado);

            result = result.Where(obj => obj.Empresa.Codigo == empresa);

            return result.Count();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PlanoOrcamentario> ConsultarRelatorioPlanoOrcamentario(DateTime dataInicial, DateTime dataFinal, int codigoCentroResultado, int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            string saldoAnterior = "";
            string saldoFinal = "";
            string saldoAnteriorOrcado = "";
            string saldoFinalOrcado = "";
            string joinEmpresa = "";

            string ambiente = "";
            string centroResultado = "";
            string centroResultadoOrcado = "";

            if ((int)tipoAmbiente > 0)
                ambiente = " AND MM.MOV_AMBIENTE = " + Convert.ToString((int)tipoAmbiente);
            if (codigoCentroResultado > 0)
            {
                centroResultado = " AND MM.CRE_CODIGO = " + Convert.ToString(codigoCentroResultado);
                centroResultadoOrcado = " AND POR.CRE_CODIGO = " + Convert.ToString(codigoCentroResultado);
            }

            saldoAnterior = @" , coalesce((SELECT sum(CASE MDC_TIPO WHEN 1 THEN MDC_VALOR WHEN 2 THEN MDC_VALOR * -1 ELSE 0.00 END) 
									FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO WHERE MONTH(MDC_DATA) = " + dataInicial.Month.ToString() + @" and YEAR(MDC_DATA) = " + dataInicial.Year.ToString() + @" AND PLA_CODIGO = x.PLA_CODIGO AND MM.EMP_CODIGO = " + codigoEmpresa.ToString() + ambiente + centroResultado + "),0) AS saldo_inicial ";

            saldoFinal = @" , coalesce((SELECT sum(CASE MDC_TIPO WHEN 1 THEN MDC_VALOR WHEN 2 THEN MDC_VALOR * -1 ELSE 0.00 END) 
									FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MF JOIN T_MOVIMENTO_FINANCEIRO MM ON MM.MOV_CODIGO = MF.MOV_CODIGO WHERE MONTH(MDC_DATA) = " + dataFinal.Month.ToString() + @"  AND YEAR(MDC_DATA) = " + dataFinal.Year.ToString() + @" AND PLA_CODIGO = x.PLA_CODIGO AND MM.EMP_CODIGO = " + codigoEmpresa.ToString() + ambiente + centroResultado + "),0) AS saldo_final ";

            saldoAnteriorOrcado = @", ISNULL((SELECT SUM(POC_VALOR) FROM T_PLANO_ORCAMENTARIO_CONTA POC JOIN T_PLANO_ORCAMENTARIO POR ON POR.POR_CODIGO = POC.POR_CODIGO
                                    WHERE MONTH(POR_DATA_BASE) = " + dataInicial.Month.ToString() + @" and YEAR(POR_DATA_BASE) = " + dataInicial.Year.ToString() + @" AND PLA_CODIGO = x.PLA_CODIGO AND POR.EMP_CODIGO = " + codigoEmpresa.ToString() + centroResultadoOrcado + "),0) AS saldo_inicial_orcado";

            saldoFinalOrcado = @", ISNULL((SELECT SUM(POC_VALOR) FROM T_PLANO_ORCAMENTARIO_CONTA POC JOIN T_PLANO_ORCAMENTARIO POR ON POR.POR_CODIGO = POC.POR_CODIGO 
									WHERE MONTH(POR_DATA_BASE) = " + dataFinal.Month.ToString() + @" and YEAR(POR_DATA_BASE) = " + dataFinal.Year.ToString() + @" AND PLA_CODIGO = x.PLA_CODIGO AND POR.EMP_CODIGO = " + codigoEmpresa.ToString() + centroResultadoOrcado + "),0) AS saldo_final_orcado";

            if ((int)tipoAmbiente > 0)
                ambiente = " AND M.MOV_AMBIENTE = " + Convert.ToString((int)tipoAmbiente);
            if (codigoCentroResultado > 0)
                centroResultado = " AND M.CRE_CODIGO = " + Convert.ToString(codigoCentroResultado);
            joinEmpresa = " JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = MDC.MOV_CODIGO where M.EMP_CODIGO = " + codigoEmpresa.ToString() + ambiente + centroResultado + " ";

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
                                    , sum(xx.saldo_inicial_orcado) AS SaldoInicialOrcado
                                    , sum(xx.saldo_final_orcado) AS SaldoAtualOrcado
                              FROM
                                  (
                                  SELECT x.PLA_CODIGO
		                               , x.PLA_PLANO
                                       " + saldoAnterior + @"
                                       " + saldoFinal + @"		
                                       ,0 saldo_inicial_orcado
                                       ,0 saldo_final_orcado
                                      FROM
                                          (
                                          SELECT planoConta.PLA_CODIGO
				                               , planoConta.PLA_PLANO
                                              FROM T_PLANO_DE_CONTA planoConta 
                                         LEFT JOIN T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO AS mdc
				                                ON mdc.PLA_CODIGO = planoConta.PLA_CODIGO AND ((MONTH(mdc.MDC_DATA) = " + dataInicial.Month.ToString() + @"  and YEAR(mdc.MDC_DATA) = " + dataInicial.Year.ToString() + @") OR (MONTH(mdc.MDC_DATA) = " + dataFinal.Month.ToString() + @" and YEAR(mdc.MDC_DATA) = " + dataFinal.Year.ToString() + @")) 
                                                " + joinEmpresa + @"
                                          ) AS x
                                  GROUP BY x.PLA_CODIGO, x.PLA_PLANO

                                  union
                                  
                                  SELECT x.PLA_CODIGO
		                               , x.PLA_PLANO
                                        , 0 saldo_inicial 
                                        , 0 saldo_final 		
                                       " + saldoAnteriorOrcado + @"
                                       " + saldoFinalOrcado + @"
                                      FROM
                                          (
                                          SELECT planoConta.PLA_CODIGO
				                               , planoConta.PLA_PLANO
                                              FROM T_PLANO_DE_CONTA planoConta 
											 JOIN T_PLANO_ORCAMENTARIO_CONTA POC ON POC.PLA_CODIGO = planoConta.PLA_CODIGO 
											 JOIN T_PLANO_ORCAMENTARIO POR ON POR.POR_CODIGO = POC.POR_CODIGO 
											 AND ((MONTH(POR_DATA_BASE) = " + dataInicial.Month.ToString() + @"  and YEAR(POR_DATA_BASE) = " + dataInicial.Year.ToString() + @") OR (MONTH(POR_DATA_BASE) = " + dataFinal.Month.ToString() + @" and YEAR(POR_DATA_BASE) = " + dataFinal.Year.ToString() + @")) 
											 where POR.EMP_CODIGO = " + codigoEmpresa.ToString() + centroResultadoOrcado + @"
                                          ) AS x
                                  GROUP BY x.PLA_CODIGO, x.PLA_PLANO

                                  ) AS xx
                              JOIN T_PLANO_DE_CONTA pc
                                  ON xx.PLA_PLANO LIKE pc.PLA_PLANO + '%' AND (pc.PLA_RECEITA_DESPESA = 1 or pc.PLA_RECEITA_DESPESA = 2)  AND pc.PLA_GRUPO_DE_RESULTADO > 0 ";

            sqlQuery += " and pc.EMP_CODIGO = " + codigoEmpresa.ToString();
            sqlQuery += " GROUP BY pc.PLA_PLANO, pc.PLA_DESCRICAO, pc.PLA_TIPO, pc.PLA_RECEITA_DESPESA, pc.PLA_CODIGO, pc.PLA_GRUPO_DE_RESULTADO ORDER BY Plano ASC";

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.PlanoOrcamentario)));

            return query.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.PlanoOrcamentario>();
        }

        public int QtdPlanoOrcamentarioSobreTipoMovimento(int codigoEmpresa, DateTime dataBase, int codigoTipoMovimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario>();
            var queryCentroResultado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado>();

            var resultCentroResultado = from obj in queryCentroResultado where obj.TipoMovimento.Codigo == codigoTipoMovimento select obj.CentroResultado;
            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa && obj.DataBase.Month == dataBase.Month && obj.DataBase.Year == dataBase.Year && resultCentroResultado.Contains(obj.CentroResultado)
                         select obj;

            return result.Count();
        }

        public int QtdPlanoOrcamentarioSobreBaixaTitulo(int codigoEmpresa, int codigoBaixaTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.PlanoOrcamentario>();
            var queryCentroResultado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoCentroResultado>();
            var queryBaixaAgrupado = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TituloBaixaAgrupado>();

            var resultBaixaAgrupado = from obj in queryBaixaAgrupado where obj.TituloBaixa.Codigo == codigoBaixaTitulo select obj.Titulo;
            var resultCentroResultado = from obj in queryCentroResultado where resultBaixaAgrupado.Any(t => t.TipoMovimento.Codigo == obj.TipoMovimento.Codigo) select obj.CentroResultado;
            var result = from obj in query
                         where obj.Empresa.Codigo == codigoEmpresa && resultCentroResultado.Contains(obj.CentroResultado)
                         && resultBaixaAgrupado.Any(t => t.DataEmissao.Value.Month == obj.DataBase.Month && t.DataEmissao.Value.Year == obj.DataBase.Year)
                         select obj;

            return result.Distinct().ToList().Count();
        }
    }
}

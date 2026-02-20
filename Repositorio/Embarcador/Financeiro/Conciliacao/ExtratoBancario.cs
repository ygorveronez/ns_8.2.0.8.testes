using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Repositorio.Embarcador.Financeiro.Conciliacao
{
    public class ExtratoBancario : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario>
    {
        public ExtratoBancario(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario> BuscarMovimentoParaConciliacaoBancaria(string planoContaSintetico, int codigoPlanoConta, DateTime? dataInicial, DateTime? dataFinal, int codigoEmpresa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario>();

            if (codigoPlanoConta > 0)
                query = query.Where(obj => obj.PlanoConta.Codigo == codigoPlanoConta && obj.ExtratoConcolidado != true);
            else
                query = query.Where(obj => obj.PlanoConta.Plano.StartsWith(planoContaSintetico) && obj.PlanoConta.AnaliticoSintetico == Dominio.ObjetosDeValor.Embarcador.Enumeradores.AnaliticoSintetico.Analitico && obj.ExtratoConcolidado != true);

            if (dataInicial.HasValue)
                query = query.Where(obj => obj.DataMovimento.Date >= dataInicial.Value.Date);
            if (dataFinal.HasValue)
                query = query.Where(obj => obj.DataMovimento.Date <= dataFinal.Value.Date);

            if (codigoEmpresa > 0)
                query = query.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            var queryConciliacao = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria>();
            if (codigoEmpresa > 0)
                queryConciliacao = queryConciliacao.Where(obj => obj.Empresa.Codigo == codigoEmpresa);
            query = query.Where(obj => !queryConciliacao.Any(a => a.Extratos.Any(m => m.Codigo == obj.Codigo && m.ExtratoConcolidado == true && a.SituacaoConciliacaoBancaria != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoConciliacaoBancaria.Cancelado)));

            return query.ToList();
        }

        public bool ContemExtratoDuplicado(string observacao, DateTime data, decimal valor, string documento, int codigoPlano)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario>();
            var result = from obj in query where obj.Observacao == observacao && obj.DataMovimento.Date == data.Date && obj.Valor == valor && obj.Documento == documento && obj.PlanoConta.Codigo == codigoPlano select obj;
            return result.Any();
        }

        public bool ContemExtratoEmOutraConciliacao(int codigoExtrato, int codigoConciliacao)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ConciliacaoBancaria>();
            var result = from obj in query where obj.Codigo != codigoConciliacao && obj.SituacaoConciliacaoBancaria != Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoConciliacaoBancaria.Cancelado && obj.Extratos.Any(e => e.Codigo == codigoExtrato && e.ExtratoConcolidado == true) select obj;
            return result.Any();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario> Consultar(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito debitoCredito, int codigoMovimento, int codigoEmpresa, DateTime dataMovimento, decimal valorMovimento, string numeroDocumento, string observacao, int planoConta, bool? extratoConsolidado, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
                result = result.Where(obj => obj.Documento.Contains(numeroDocumento));

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(obj => obj.Observacao.Contains(observacao));

            if (valorMovimento > 0)
                result = result.Where(obj => obj.Valor.Equals(valorMovimento));

            if (codigoMovimento > 0)
                result = result.Where(obj => obj.Codigo.Equals(codigoMovimento));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (dataMovimento > DateTime.MinValue)
                result = result.Where(obj => obj.DataMovimento >= dataMovimento && obj.DataMovimento <= dataMovimento.AddDays(1));

            if (planoConta > 0)
                result = result.Where(obj => obj.PlanoConta.Codigo == planoConta);

            if (debitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito || debitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito)
                result = result.Where(obj => obj.DebitoCredito == debitoCredito);

            if (extratoConsolidado.HasValue)
                result = result.Where(obj => obj.ExtratoConcolidado == extratoConsolidado.Value);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result.ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito debitoCredito, int codigoMovimento, int codigoEmpresa, DateTime dataMovimento, decimal valorMovimento, string numeroDocumento, string observacao, int planoConta, bool? extratoConsolidado)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Conciliacao.ExtratoBancario>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(numeroDocumento))
                result = result.Where(obj => obj.Documento.Contains(numeroDocumento));

            if (!string.IsNullOrWhiteSpace(observacao))
                result = result.Where(obj => obj.Observacao.Contains(observacao));

            if (valorMovimento > 0)
                result = result.Where(obj => obj.Valor.Equals(valorMovimento));

            if (codigoMovimento > 0)
                result = result.Where(obj => obj.Codigo.Equals(codigoMovimento));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (dataMovimento > DateTime.MinValue)
                result = result.Where(obj => obj.DataMovimento >= dataMovimento && obj.DataMovimento <= dataMovimento.AddDays(1));

            if (planoConta > 0)
                result = result.Where(obj => obj.PlanoConta.Codigo == planoConta);

            if (debitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito || debitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito)
                result = result.Where(obj => obj.DebitoCredito == debitoCredito);

            if (extratoConsolidado.HasValue)
                result = result.Where(obj => obj.ExtratoConcolidado == extratoConsolidado.Value);

            return result.Count();
        }

        public void SetarMovimentosConcolidados(List<int> codigos, bool consolidado)
        {
            UnitOfWork.Sessao.CreateQuery("UPDATE ExtratoBancario SET ExtratoConcolidado = :consolidado WHERE Codigo IN (:codigos)")
                .SetParameterList("codigos", codigos)
                .SetParameter("consolidado", consolidado)
                .ExecuteUpdate();
        }

        public decimal SomaDocumentos(List<int> codigos, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito tipo)
        {
            string query = @"SELECT ISNULL(SUM(EXB_VALOR), 0) FROM T_EXTRATO_BANCARIO M
                    WHERE EXB_CODIGO IN (" + string.Join(", ", codigos) + @") AND EXB_DEBITO_CREDITO = " + (int)tipo;

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.SetTimeout(50000).UniqueResult<decimal>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.ExtratoConciliacaoBancaria> ConsultarExtratoConcolidados(int codigoConciliacaoBancaria)
        {
            string query = @"SELECT E.EXB_CODIGO Codigo,
                E.PLA_CODIGO CodigoPlano,
                E.EXB_DATA DataMovimento,
                CASE
	                WHEN E.EXB_DEBITO_CREDITO = 1 THEN E.EXB_VALOR
	                ELSE 0
                END ValorDebito,
                CASE
	                WHEN E.EXB_DEBITO_CREDITO = 2 THEN E.EXB_VALOR
	                ELSE 0
                END ValorCredito,
                E.EXB_DOCUMENTO Documento,
                E.EXB_OBSERVACAO Observacao,
                E.EXB_TIPO Tipo,
                E.EXB_CODIGO_LANCAMENTO + ' - ' + ISNULL(L.ETP_DESCRICAO, '') CodigoLancamento,
                E.MCO_ATIVO ExtratoConcolidado
                FROM T_EXTRATO_BANCARIO E
                LEFT OUTER JOIN T_EXTRATO_BANCARIO_TIPO_LANCAMENTO L ON L.ETP_CODIGO = E.ETP_CODIGO
                JOIN T_CONCILIACAO_BANCARIA_EXTRATO_BANCARIO CC ON CC.EXB_CODIGO = E.EXB_CODIGO
                WHERE E.MCO_ATIVO = 1 AND CC.COB_CODIGO = " + codigoConciliacaoBancaria.ToString("D");

            query = @"SELECT T.Codigo, T.CodigoPlano, T.DataMovimento, T.ValorDebito, T.ValorCredito, T.Documento, T.Observacao, T.Tipo, T.CodigoLancamento, T.ExtratoConcolidado, 
                SUM(T.ValorCredito - T.ValorDebito) OVER (PARTITION BY t.CodigoPlano ORDER BY T.DataMovimento ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS Saldo
                FROM  ( " + query + " ) AS T ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.ExtratoConciliacaoBancaria)));

            return nhQuery.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ExtratoConciliacaoBancaria>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.ExtratoConciliacaoBancaria> ConsultarExtratoConciliacaoBancaria(string observacaoExtrato, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito debitoCreditoExtrato, DateTime dataAtePesquisaExtrato, decimal valorAtePesquisaExtrato, string codigoLancamentoPesquisaExtrato, DateTime dataPesquisaExtrato, decimal valorPesquisaExtrato, string numeroDocumentoPesquisaExtrato, int codigoConciliacaoBancaria, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("eu-ES");
            string queryOrdenacao = "";

            if (!string.IsNullOrWhiteSpace(propOrdenacao))
                queryOrdenacao += " order by " + propOrdenacao + " " + dirOrdenacao;

            string query = @"SELECT E.EXB_CODIGO Codigo,
                E.PLA_CODIGO CodigoPlano,
                E.EXB_DATA DataMovimento,
                CASE
	                WHEN E.EXB_DEBITO_CREDITO = 1 THEN E.EXB_VALOR
	                ELSE 0
                END ValorDebito,
                CASE
	                WHEN E.EXB_DEBITO_CREDITO = 2 THEN E.EXB_VALOR
	                ELSE 0
                END ValorCredito,
                E.EXB_DOCUMENTO Documento,
                E.EXB_OBSERVACAO Observacao,
                E.EXB_TIPO Tipo,
                E.EXB_CODIGO_LANCAMENTO + ' - ' + ISNULL(L.ETP_DESCRICAO, '') CodigoLancamento,
                E.MCO_ATIVO ExtratoConcolidado
                FROM T_EXTRATO_BANCARIO E
                LEFT OUTER JOIN T_EXTRATO_BANCARIO_TIPO_LANCAMENTO L ON L.ETP_CODIGO = E.ETP_CODIGO
                JOIN T_CONCILIACAO_BANCARIA_EXTRATO_BANCARIO CC ON CC.EXB_CODIGO = E.EXB_CODIGO
                WHERE CC.COB_CODIGO = " + codigoConciliacaoBancaria.ToString("D");

            string pattern = "yyyy-MM-dd";
            if (dataPesquisaExtrato > DateTime.MinValue && dataAtePesquisaExtrato == DateTime.MinValue)
                query += " AND CAST(E.EXB_DATA AS DATE) >= '" + dataPesquisaExtrato.ToString(pattern) + "'";
            else if (dataPesquisaExtrato == DateTime.MinValue && dataAtePesquisaExtrato > DateTime.MinValue)
                query += " AND CAST(E.EXB_DATA AS DATE) <= '" + dataAtePesquisaExtrato.ToString(pattern) + "'";
            else if (dataPesquisaExtrato > DateTime.MinValue && dataAtePesquisaExtrato > DateTime.MinValue)
                query += " AND CAST(E.EXB_DATA AS DATE) >= '" + dataPesquisaExtrato.ToString(pattern) + "' AND CAST(E.EXB_DATA AS DATE) <= '" + dataAtePesquisaExtrato.ToString(pattern) + "' ";

            if (debitoCreditoExtrato == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito)
                query += "AND E.EXB_DEBITO_CREDITO = 2 ";
            else if (debitoCreditoExtrato == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito)
                query += "AND E.EXB_DEBITO_CREDITO = 1 ";

            if (valorPesquisaExtrato > 0 && valorAtePesquisaExtrato == 0)
            {
                query += " AND E.EXB_VALOR >= " + String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", valorPesquisaExtrato).Replace(",", ".");
            }
            else if (valorPesquisaExtrato > 0 && valorAtePesquisaExtrato > 0)
            {
                query += " AND E.EXB_VALOR >= " + String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", valorPesquisaExtrato).Replace(",", ".") + " AND E.EXB_VALOR <= " + String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", valorAtePesquisaExtrato).Replace(",", ".");
            }
            else if (valorPesquisaExtrato == 0 && valorAtePesquisaExtrato > 0)
            {
                query += " AND  E.EXB_VALOR <= " + String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", valorAtePesquisaExtrato).Replace(",", ".");
            }
            if (!string.IsNullOrWhiteSpace(observacaoExtrato))
                query += " AND E.EXB_OBSERVACAO LIKE '%" + observacaoExtrato + "%'";

            if (!string.IsNullOrWhiteSpace(numeroDocumentoPesquisaExtrato))
            {
                query += " AND E.EXB_DOCUMENTO = '" + numeroDocumentoPesquisaExtrato + "'";
            }
            if (!string.IsNullOrWhiteSpace(codigoLancamentoPesquisaExtrato))
            {
                query += " AND E.EXB_CODIGO_LANCAMENTO = '" + codigoLancamentoPesquisaExtrato + "'";
            }

            query = @"SELECT T.Codigo, T.CodigoPlano, T.DataMovimento, T.ValorDebito, T.ValorCredito, T.Documento, T.Observacao, T.Tipo, T.CodigoLancamento, T.ExtratoConcolidado, 
                SUM(T.ValorCredito - T.ValorDebito) OVER (PARTITION BY t.CodigoPlano " + queryOrdenacao + @" ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS Saldo
                FROM  ( " + query + " ) AS T ";

            query += queryOrdenacao;
            if (maximoRegistros > 0)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.ExtratoConciliacaoBancaria)));

            return nhQuery.SetTimeout(6000).List<Dominio.ObjetosDeValor.Embarcador.Financeiro.ExtratoConciliacaoBancaria>();
        }

        public int contarExtratoConciliacaoBancaria(string observacaoExtrato, Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito debitoCreditoExtrato, DateTime dataAtePesquisaExtrato, decimal valorAtePesquisaExtrato, string codigoLancamentoPesquisaExtrato, DateTime dataPesquisaExtrato, decimal valorPesquisaExtrato, string numeroDocumentoPesquisaExtrato, int codigoConciliacaoBancaria)
        {
            string query = @"SELECT COUNT(0) as CONTADOR
                FROM T_EXTRATO_BANCARIO E                
                JOIN T_CONCILIACAO_BANCARIA_EXTRATO_BANCARIO CC ON CC.EXB_CODIGO = E.EXB_CODIGO
                WHERE CC.COB_CODIGO = " + codigoConciliacaoBancaria.ToString("D");

            string pattern = "yyyy-MM-dd";
            if (dataPesquisaExtrato > DateTime.MinValue && dataAtePesquisaExtrato == DateTime.MinValue)
                query += " AND CAST(E.EXB_DATA AS DATE) >= '" + dataPesquisaExtrato.ToString(pattern) + "'";
            else if (dataPesquisaExtrato == DateTime.MinValue && dataAtePesquisaExtrato > DateTime.MinValue)
                query += " AND CAST(E.EXB_DATA AS DATE) <= '" + dataAtePesquisaExtrato.ToString(pattern) + "'";
            else if (dataPesquisaExtrato > DateTime.MinValue && dataAtePesquisaExtrato > DateTime.MinValue)
                query += " AND CAST(E.EXB_DATA AS DATE) >= '" + dataPesquisaExtrato.ToString(pattern) + "' AND CAST(E.EXB_DATA AS DATE) <= '" + dataAtePesquisaExtrato.ToString(pattern) + "' ";

            if (debitoCreditoExtrato == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito)
                query += "AND E.EXB_DEBITO_CREDITO = 2 ";
            else if (debitoCreditoExtrato == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito)
                query += "AND E.EXB_DEBITO_CREDITO = 1 ";

            if (valorPesquisaExtrato > 0 && valorAtePesquisaExtrato == 0)
            {
                query += " AND E.EXB_VALOR >= " + String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", valorPesquisaExtrato).Replace(",", ".");
            }
            else if (valorPesquisaExtrato > 0 && valorAtePesquisaExtrato > 0)
            {
                query += " AND E.EXB_VALOR >= " + String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", valorPesquisaExtrato).Replace(",", ".") + " AND E.EXB_VALOR <= " + String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", valorAtePesquisaExtrato).Replace(",", ".");
            }
            else if (valorPesquisaExtrato == 0 && valorAtePesquisaExtrato > 0)
            {
                query += " AND  E.EXB_VALOR <= " + String.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:f}", valorAtePesquisaExtrato).Replace(",", ".");
            }
            if (!string.IsNullOrWhiteSpace(observacaoExtrato))
                query += " AND E.EXB_OBSERVACAO LIKE '%" + observacaoExtrato + "%'";
            if (!string.IsNullOrWhiteSpace(numeroDocumentoPesquisaExtrato))
            {
                query += " AND E.EXB_DOCUMENTO = '" + numeroDocumentoPesquisaExtrato + "'";
            }
            if (!string.IsNullOrWhiteSpace(codigoLancamentoPesquisaExtrato))
            {
                query += " AND E.EXB_CODIGO_LANCAMENTO = '" + codigoLancamentoPesquisaExtrato + "'";
            }

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.SetTimeout(50000).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoBancario> RelatorioExtratoBancario(int codigoEmpresa, int codigoConta, DateTime dataInicial, DateTime dataFinal, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true)
        {
            string sqlData = "";
            string sqlConta = "";

            if (codigoConta > 0)
            {
                sqlConta = " AND PD.PLA_CODIGO = " + codigoConta.ToString();
            }

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
            {
                sqlData = " AND MD.EXB_DATA >= '" + dataInicial.ToString("MM/dd/yyyy") + "' AND MD.EXB_DATA <= '" + dataFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
            }

            if (dataInicial > DateTime.MinValue && dataFinal == DateTime.MinValue)
            {
                sqlData = " AND MD.EXB_DATA >= '" + dataInicial.ToString("MM/dd/yyyy") + "' ";
            }

            if (dataInicial == DateTime.MinValue && dataFinal > DateTime.MinValue)
            {
                sqlData = " AND MD.EXB_DATA <= '" + dataFinal.ToString("MM/dd/yyyy 23:59:59") + "' ";

            }

            string sqlEmpresa = "";
            if (codigoEmpresa > 0)
                sqlEmpresa = " AND (MD.EMP_CODIGO = " + codigoEmpresa.ToString() + ")";

            string queryOrdenacao = "";
            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                queryOrdenacao += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    queryOrdenacao += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    queryOrdenacao += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }

            string sqlSaldoAnterior = " 0.0 SaldoAnterior ";
            if (dataInicial > DateTime.MinValue)
                sqlSaldoAnterior = @"case when ROW_NUMBER() OVER(" + queryOrdenacao + @") = 1 
			                            then (SELECT sum(CASE EXB_DEBITO_CREDITO WHEN 1 THEN EXB_VALOR * -1 WHEN 2 THEN EXB_VALOR ELSE 0.00 END) FROM T_EXTRATO_BANCARIO 
			                            WHERE EXB_DATA < '" + dataInicial.ToString("MM/dd/yyyy") + @"' AND PLA_CODIGO = T.CodigoPlanoConta)
                                    else 0.0 end AS SaldoAnterior ";

            string query = @"SELECT MD.EXB_CODIGO Codigo, " +
                " MD.EXB_DATA DataMovimento, " +
                " MD.EXB_OBSERVACAO Observacao, " +
                " MD.EXB_TIPO TipoDocumentoMovimento, " +
                " MD.EXB_DOCUMENTO Documento, " +
                " PD.PLA_CODIGO CodigoPlanoConta, " +
                " PD.PLA_PLANO PlanoConta, " +
                " PD.PLA_DESCRICAO PlanoContaDescricao, " +
                " (MD.EXB_VALOR * -1) ValorDebito, " +
                " 0.0 ValorCredito, " +
                " MD.EXB_CODIGO_LANCAMENTO + ' - ' + ISNULL(L.ETP_DESCRICAO, '') CodigoLancamento " +
                " FROM T_EXTRATO_BANCARIO MD " +
                " JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = MD.PLA_CODIGO " +
                " LEFT OUTER JOIN T_EXTRATO_BANCARIO_TIPO_LANCAMENTO L ON L.ETP_CODIGO = MD.ETP_CODIGO " +
                " where MD.EXB_DEBITO_CREDITO = 1 " + sqlConta + sqlEmpresa + sqlData +
                "  " +
                " UNION  " +
                "  " +
                " SELECT MD.EXB_CODIGO Codigo, " +
                " MD.EXB_DATA DataMovimento, " +
                " MD.EXB_OBSERVACAO Observacao, " +
                " MD.EXB_TIPO TipoDocumentoMovimento, " +
                " MD.EXB_DOCUMENTO Documento, " +
                " PD.PLA_CODIGO CodigoPlanoConta, " +
                " PD.PLA_PLANO PlanoConta, " +
                " PD.PLA_DESCRICAO PlanoContaDescricao, " +
                " 0.0 ValorDebito, " +
                " (MD.EXB_VALOR) ValorCredito, " +
                " MD.EXB_CODIGO_LANCAMENTO + ' - ' + ISNULL(L.ETP_DESCRICAO, '') CodigoLancamento " +
                " FROM T_EXTRATO_BANCARIO MD " +
                " JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = MD.PLA_CODIGO " +
                " LEFT OUTER JOIN T_EXTRATO_BANCARIO_TIPO_LANCAMENTO L ON L.ETP_CODIGO = MD.ETP_CODIGO " +
                " where MD.EXB_DEBITO_CREDITO = 2 " + sqlConta + sqlEmpresa + sqlData;

            query = @"SELECT T.Codigo, T.DataMovimento, T.Observacao, T.TipoDocumentoMovimento, T.Documento, T.PlanoConta, T.PlanoContaDescricao, T.ValorDebito, T.ValorCredito, T.CodigoPlanoConta, T.CodigoLancamento,
                    " + sqlSaldoAnterior + @"
                    FROM ( " + query + " ) AS T ";

            query = @"SELECT TT.Codigo, TT.DataMovimento, TT.Observacao, TT.TipoDocumentoMovimento, TT.Documento, TT.PlanoConta, TT.PlanoContaDescricao, TT.ValorDebito, TT.ValorCredito, TT.CodigoPlanoConta, TT.CodigoLancamento, TT.SaldoAnterior,
                    SUM(ISNULL(TT.SaldoAnterior, 0) + (TT.ValorDebito - (TT.ValorCredito * -1))) OVER (" + queryOrdenacao + @" ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS Saldo
                    FROM ( " + query + " ) AS TT ";

            query += queryOrdenacao;

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoBancario)));

            return nhQuery.SetTimeout(6000).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoBancario>();
        }

        public int ContarRelatorioExtratoBancario(int codigoEmpresa, int codigoConta, DateTime dataInicial, DateTime dataFinal)
        {
            string sqlData = "";
            string sqlConta = "";

            if (codigoConta > 0)
            {
                sqlConta = " AND PD.PLA_CODIGO = " + codigoConta.ToString();
            }

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
            {
                sqlData = " AND MD.EXB_DATA >= '" + dataInicial.ToString("MM/dd/yyyy") + "' AND MD.EXB_DATA <= '" + dataFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
            }

            if (dataInicial > DateTime.MinValue && dataFinal == DateTime.MinValue)
            {
                sqlData = " AND MD.EXB_DATA >= '" + dataInicial.ToString("MM/dd/yyyy") + "' ";
            }

            if (dataInicial == DateTime.MinValue && dataFinal > DateTime.MinValue)
            {
                sqlData = " AND MD.EXB_DATA <= '" + dataFinal.ToString("MM/dd/yyyy 23:59:59") + "' ";

            }

            string sqlEmpresa = "";
            if (codigoEmpresa > 0)
                sqlEmpresa = " AND (MD.EMP_CODIGO = " + codigoEmpresa.ToString() + ")";

            string query = @"SELECT MD.EXB_CODIGO Codigo, " +
                " MD.EXB_DATA DataMovimento, " +
                " MD.EXB_OBSERVACAO Observacao, " +
                " MD.EXB_TIPO TipoDocumentoMovimento, " +
                " MD.EXB_DOCUMENTO Documento, " +
                " PD.PLA_CODIGO CodigoPlanoConta, " +
                " PD.PLA_PLANO PlanoConta, " +
                " PD.PLA_DESCRICAO PlanoContaDescricao, " +
                " (MD.EXB_VALOR * -1) ValorDebito, " +
                " 0.0 ValorCredito, " +
                " MD.EXB_CODIGO_LANCAMENTO + ' - ' + ISNULL(L.ETP_DESCRICAO, '') CodigoLancamento " +
                " FROM T_EXTRATO_BANCARIO MD " +
                " JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = MD.PLA_CODIGO " +
                " LEFT OUTER JOIN T_EXTRATO_BANCARIO_TIPO_LANCAMENTO L ON L.ETP_CODIGO = MD.ETP_CODIGO " +
                " where MD.EXB_DEBITO_CREDITO = 1 " + sqlConta + sqlEmpresa + sqlData +
                "  " +
                " UNION  " +
                "  " +
                " SELECT MD.EXB_CODIGO Codigo, " +
                " MD.EXB_DATA DataMovimento, " +
                " MD.EXB_OBSERVACAO Observacao, " +
                " MD.EXB_TIPO TipoDocumentoMovimento, " +
                " MD.EXB_DOCUMENTO Documento, " +
                " PD.PLA_CODIGO CodigoPlanoConta, " +
                " PD.PLA_PLANO PlanoConta, " +
                " PD.PLA_DESCRICAO PlanoContaDescricao, " +
                " 0.0 ValorDebito, " +
                " (MD.EXB_VALOR) ValorCredito, " +
                " MD.EXB_CODIGO_LANCAMENTO + ' - ' + ISNULL(L.ETP_DESCRICAO, '') CodigoLancamento " +
                " FROM T_EXTRATO_BANCARIO MD " +
                " JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = MD.PLA_CODIGO " +
                " LEFT OUTER JOIN T_EXTRATO_BANCARIO_TIPO_LANCAMENTO L ON L.ETP_CODIGO = MD.ETP_CODIGO " +
                " where MD.EXB_DEBITO_CREDITO = 2 " + sqlConta + sqlEmpresa + sqlData;

            query = @"SELECT COUNT(0) as CONTADOR
                FROM ( " + query + " ) AS T ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.SetTimeout(6000).UniqueResult<int>();
        }

        public void DeletarExtratoEmOutraConciliacao(int codigoExtrato, int codigoConciliacao)
        {
            try
            {
                if (UnitOfWork.IsActiveTransaction())
                {
                    UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CONCILIACAO_BANCARIA_EXTRATO_BANCARIO WHERE EXB_CODIGO = " + codigoExtrato + " AND COB_CODIGO <> " + codigoConciliacao).ExecuteUpdate(); // SQL-INJECTION-SAFE
                }
                else
                {
                    try
                    {
                        UnitOfWork.Start();

                        UnitOfWork.Sessao.CreateSQLQuery("DELETE FROM T_CONCILIACAO_BANCARIA_EXTRATO_BANCARIO WHERE EXB_CODIGO = " + codigoExtrato + " AND COB_CODIGO <> " + codigoConciliacao).ExecuteUpdate(); // SQL-INJECTION-SAFE

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

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ConciliacaoBancaria> RelatorioConciliacaoBancaria(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioConciliacaoBancaria filtrosPesquisa, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true)
        {
            string sqlExtrato = "", sqlMovimento = "", sqlConta = "";

            if (filtrosPesquisa.CodigoPlano.Count() > 0)
            {
                string codigoPlano = string.Join(", ", filtrosPesquisa.CodigoPlano);
                sqlConta += $" AND PD.PLA_CODIGO IN ({codigoPlano})";
            }

            if (filtrosPesquisa.DataInicial > DateTime.MinValue)
            {
                sqlExtrato += $" AND MD.EXB_DATA >= '{filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy")}' ";
                sqlMovimento += $" AND MD.MDC_DATA >= '{filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy")}' ";
            }

            if (filtrosPesquisa.DataFinal > DateTime.MinValue)
            {
                sqlExtrato += $" AND MD.EXB_DATA < '{filtrosPesquisa.DataFinal.AddDays(1).ToString("MM/dd/yyyy")}' ";
                sqlMovimento += $" AND MD.MDC_DATA < '{filtrosPesquisa.DataFinal.AddDays(1).ToString("MM/dd/yyyy")}' ";
            }

            if (filtrosPesquisa.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito || filtrosPesquisa.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito)
            {
                sqlExtrato += $" AND MD.EXB_DEBITO_CREDITO = {(int)filtrosPesquisa.DebitoCredito} ";
                sqlMovimento += $" AND MD.MDC_TIPO = {(int)filtrosPesquisa.DebitoCredito} ";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Observacao))
            {
                sqlExtrato += $" AND MD.EXB_OBSERVACAO LIKE '%{filtrosPesquisa.Observacao}%' ";
                sqlMovimento += $" AND Movimento.MOV_OBSERVACAO LIKE '%{filtrosPesquisa.Observacao}%' ";
            }

            if (filtrosPesquisa.CodigoTipoMovimento > 0)
            {
                sqlExtrato += " AND 1=0";
                sqlMovimento += $" AND Movimento.TIM_CODIGO = {filtrosPesquisa.CodigoTipoMovimento} ";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumento))
            {
                sqlExtrato += $" AND MD.EXB_DOCUMENTO LIKE '%{filtrosPesquisa.NumeroDocumento}%' ";
                sqlMovimento += $" AND Movimento.MOV_DOCUMENTO LIKE '%{filtrosPesquisa.NumeroDocumento}%' ";
            }

            if (filtrosPesquisa.CodigoMovimento > 0)
            {
                sqlExtrato += " AND 1=0";
                sqlMovimento += $" AND Movimento.MOV_CODIGO = {filtrosPesquisa.CodigoMovimento} ";
            }

            string queryOrdenacao = "";
            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                queryOrdenacao += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                {
                    queryOrdenacao += ", " + propOrdenacao + " " + dirOrdenacao;
                }
                else
                {
                    queryOrdenacao += " order by " + propOrdenacao + " " + dirOrdenacao;
                }
            }


            string query = @"SELECT MD.EXB_CODIGO Codigo, 
                 MD.EXB_DATA Data, 
                 MD.EXB_OBSERVACAO Observacao,                  
                 MD.EXB_DOCUMENTO Documento, 
                 PD.PLA_CODIGO CodigoPlanoConta, 
                 PD.PLA_PLANO PlanoConta, 
                 PD.PLA_DESCRICAO PlanoContaDescricao, 
                 (MD.EXB_VALOR * -1) ValorDebito, 
                 0.0 ValorCredito,
				 'Extrato Bancário' TipoMovimento
                 FROM T_EXTRATO_BANCARIO MD 
                 JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = MD.PLA_CODIGO 
                 LEFT OUTER JOIN T_EXTRATO_BANCARIO_TIPO_LANCAMENTO L ON L.ETP_CODIGO = MD.ETP_CODIGO 
                 where MD.EXB_DEBITO_CREDITO = 1 AND (MD.MCO_ATIVO = 0 OR MD.MCO_ATIVO IS NULL) " + sqlConta + sqlExtrato + @"

                 UNION  
                  
                 SELECT MD.EXB_CODIGO Codigo, 
                 MD.EXB_DATA Data, 
                 MD.EXB_OBSERVACAO Observacao,                  
                 MD.EXB_DOCUMENTO Documento, 
                 PD.PLA_CODIGO CodigoPlanoConta, 
                 PD.PLA_PLANO PlanoConta, 
                 PD.PLA_DESCRICAO PlanoContaDescricao, 
                 0.0 ValorDebito, 
                 (MD.EXB_VALOR) ValorCredito,
				 'Extrato Bancário' TipoMovimento
                 FROM T_EXTRATO_BANCARIO MD 
                 JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = MD.PLA_CODIGO 
                 LEFT OUTER JOIN T_EXTRATO_BANCARIO_TIPO_LANCAMENTO L ON L.ETP_CODIGO = MD.ETP_CODIGO 
                 where MD.EXB_DEBITO_CREDITO = 2  AND (MD.MCO_ATIVO = 0 OR MD.MCO_ATIVO IS NULL) " + sqlConta + sqlExtrato + @"

				 UNION 

				 SELECT MD.MDC_CODIGO Codigo, 
                 MD.MDC_DATA Data, 
                 Movimento.MOV_OBSERVACAO Observacao,                  
                 Movimento.MOV_DOCUMENTO Documento, 
                 MD.PLA_CODIGO CodigoPlanoConta, 
                 PD.PLA_PLANO PlanoConta, 
                 PD.PLA_DESCRICAO PlanoContaDescricao, 
                 0.0 ValorDebito, 
                 (MD.MDC_VALOR) ValorCredito,
				 'Movimento Financeiro' TipoMovimento 
				 FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MD
				 JOIN T_MOVIMENTO_FINANCEIRO Movimento on Movimento.MOV_CODIGO = MD.MOV_CODIGO
				 JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = MD.PLA_CODIGO 
				 WHERE (MD.MDC_MOVIMENTO_CONCOLIDADO = 0 OR MD.MDC_MOVIMENTO_CONCOLIDADO IS NULL)
				 AND MD.MDC_TIPO = 1 " + sqlConta + sqlMovimento + @"

				 UNION 

				 SELECT MD.MDC_CODIGO Codigo, 
                 MD.MDC_DATA Data, 
                 Movimento.MOV_OBSERVACAO Observacao,                  
                 Movimento.MOV_DOCUMENTO Documento, 
                 MD.PLA_CODIGO CodigoPlanoConta, 
                 PD.PLA_PLANO PlanoConta, 
                 PD.PLA_DESCRICAO PlanoContaDescricao, 
                 (MD.MDC_VALOR * -1) ValorDebito, 
                 (0.00) ValorCredito,
				 'Movimento Financeiro' TipoMovimento 
				 FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MD
				 JOIN T_MOVIMENTO_FINANCEIRO Movimento on Movimento.MOV_CODIGO = MD.MOV_CODIGO
				 JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = MD.PLA_CODIGO 
				 WHERE (MD.MDC_MOVIMENTO_CONCOLIDADO = 0 OR MD.MDC_MOVIMENTO_CONCOLIDADO IS NULL)
				 AND MD.MDC_TIPO = 2 " + sqlConta + sqlMovimento;

            query += queryOrdenacao;

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.ConciliacaoBancaria)));

            return nhQuery.SetTimeout(6000).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ConciliacaoBancaria>();
        }

        public int ContarRelatorioConciliacaoBancaria(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioConciliacaoBancaria filtrosPesquisa)
        {
            string sqlExtrato = "", sqlMovimento = "", sqlConta = "";

            if (filtrosPesquisa.CodigoPlano.Count() > 0)
            {
                string codigoPlano = string.Join(", ", filtrosPesquisa.CodigoPlano);
                sqlConta += $" AND PD.PLA_CODIGO IN ({codigoPlano})";
            }
            
            if (filtrosPesquisa.DataInicial > DateTime.MinValue)
            {
                sqlExtrato += $" AND MD.EXB_DATA >= '{filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy")}' ";
                sqlMovimento += $" AND MD.MDC_DATA >= '{filtrosPesquisa.DataInicial.ToString("MM/dd/yyyy")}' ";
            }

            if (filtrosPesquisa.DataFinal > DateTime.MinValue)
            {
                sqlExtrato += $" AND MD.EXB_DATA < '{filtrosPesquisa.DataFinal.AddDays(1).ToString("MM/dd/yyyy")}' ";
                sqlMovimento += $" AND MD.MDC_DATA < '{filtrosPesquisa.DataFinal.AddDays(1).ToString("MM/dd/yyyy")}' ";
            }

            if (filtrosPesquisa.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito || filtrosPesquisa.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito)
            {
                sqlExtrato += $" AND MD.EXB_DEBITO_CREDITO = {(int)filtrosPesquisa.DebitoCredito} ";
                sqlMovimento += $" AND MD.MDC_TIPO = {(int)filtrosPesquisa.DebitoCredito} ";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Observacao))
            {
                sqlExtrato += $" AND MD.EXB_OBSERVACAO LIKE '%{filtrosPesquisa.Observacao}%' ";
                sqlMovimento += $" AND Movimento.MOV_OBSERVACAO LIKE '%{filtrosPesquisa.Observacao}%' ";
            }

            if (filtrosPesquisa.CodigoTipoMovimento > 0)
            {
                sqlExtrato += " AND 1=0";
                sqlMovimento += $" AND Movimento.TIM_CODIGO = {filtrosPesquisa.CodigoTipoMovimento} ";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumento))
            {
                sqlExtrato += $" AND MD.EXB_DOCUMENTO LIKE '%{filtrosPesquisa.NumeroDocumento}%' ";
                sqlMovimento += $" AND Movimento.MOV_DOCUMENTO LIKE '%{filtrosPesquisa.NumeroDocumento}%' ";
            }

            if (filtrosPesquisa.CodigoMovimento > 0)
            {
                sqlExtrato += " AND 1=0";
                sqlMovimento += $" AND Movimento.MOV_CODIGO = {filtrosPesquisa.CodigoMovimento} ";
            }

            string query = @"SELECT MD.EXB_CODIGO Codigo, 
                 MD.EXB_DATA Data, 
                 MD.EXB_OBSERVACAO Observacao,                  
                 MD.EXB_DOCUMENTO Documento, 
                 PD.PLA_CODIGO CodigoPlanoConta, 
                 PD.PLA_PLANO PlanoConta, 
                 PD.PLA_DESCRICAO PlanoContaDescricao, 
                 (MD.EXB_VALOR * -1) ValorDebito, 
                 0.0 ValorCredito,
				 'Extrato Bancário' TipoMovimento
                 FROM T_EXTRATO_BANCARIO MD 
                 JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = MD.PLA_CODIGO 
                 LEFT OUTER JOIN T_EXTRATO_BANCARIO_TIPO_LANCAMENTO L ON L.ETP_CODIGO = MD.ETP_CODIGO 
                 where MD.EXB_DEBITO_CREDITO = 1 AND (MD.MCO_ATIVO = 0 OR MD.MCO_ATIVO IS NULL) " + sqlConta + sqlExtrato + @"

                 UNION  
                  
                 SELECT MD.EXB_CODIGO Codigo, 
                 MD.EXB_DATA Data, 
                 MD.EXB_OBSERVACAO Observacao,                  
                 MD.EXB_DOCUMENTO Documento, 
                 PD.PLA_CODIGO CodigoPlanoConta, 
                 PD.PLA_PLANO PlanoConta, 
                 PD.PLA_DESCRICAO PlanoContaDescricao, 
                 0.0 ValorDebito, 
                 (MD.EXB_VALOR) ValorCredito,
				 'Extrato Bancário' TipoMovimento
                 FROM T_EXTRATO_BANCARIO MD 
                 JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = MD.PLA_CODIGO 
                 LEFT OUTER JOIN T_EXTRATO_BANCARIO_TIPO_LANCAMENTO L ON L.ETP_CODIGO = MD.ETP_CODIGO 
                 where MD.EXB_DEBITO_CREDITO = 2  AND (MD.MCO_ATIVO = 0 OR MD.MCO_ATIVO IS NULL) " + sqlConta + sqlExtrato + @"

				 UNION 

				 SELECT MD.MDC_CODIGO Codigo, 
                 MD.MDC_DATA Data, 
                 Movimento.MOV_OBSERVACAO Observacao,                  
                 Movimento.MOV_DOCUMENTO Documento, 
                 MD.PLA_CODIGO CodigoPlanoConta, 
                 PD.PLA_PLANO PlanoConta, 
                 PD.PLA_DESCRICAO PlanoContaDescricao, 
                 0.0 ValorDebito, 
                 (MD.MDC_VALOR) ValorCredito,
				 'Movimento Financeiro' TipoMovimento 
				 FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MD
				 JOIN T_MOVIMENTO_FINANCEIRO Movimento on Movimento.MOV_CODIGO = MD.MOV_CODIGO
				 JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = MD.PLA_CODIGO 
				 WHERE (MD.MDC_MOVIMENTO_CONCOLIDADO = 0 OR MD.MDC_MOVIMENTO_CONCOLIDADO IS NULL)
				 AND MD.MDC_TIPO = 1 " + sqlConta + sqlMovimento + @"

				 UNION 

				 SELECT MD.MDC_CODIGO Codigo, 
                 MD.MDC_DATA Data, 
                 Movimento.MOV_OBSERVACAO Observacao,                  
                 Movimento.MOV_DOCUMENTO Documento, 
                 MD.PLA_CODIGO CodigoPlanoConta, 
                 PD.PLA_PLANO PlanoConta, 
                 PD.PLA_DESCRICAO PlanoContaDescricao, 
                 (MD.MDC_VALOR * -1) ValorDebito, 
                 (0.00) ValorCredito,
				 'Movimento Financeiro' TipoMovimento 
				 FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MD
				 JOIN T_MOVIMENTO_FINANCEIRO Movimento on Movimento.MOV_CODIGO = MD.MOV_CODIGO
				 JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = MD.PLA_CODIGO 
				 WHERE (MD.MDC_MOVIMENTO_CONCOLIDADO = 0 OR MD.MDC_MOVIMENTO_CONCOLIDADO IS NULL)
				 AND MD.MDC_TIPO = 2 " + sqlConta + sqlMovimento;

            query = @"SELECT COUNT(0) as CONTADOR
                FROM ( " + query + " ) AS T ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.SetTimeout(6000).UniqueResult<int>();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using System.Linq.Dynamic.Core;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Repositorio.Embarcador.Consulta;

namespace Repositorio.Embarcador.Financeiro
{
    public class MovimentoFinanceiro : RepositorioBase<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro>
    {
        public MovimentoFinanceiro(UnitOfWork unitOfWork) : base(unitOfWork) { }

        #region Métodos Públicos

        public Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro BuscarPorCodigo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro>();
            var result = from obj in query where obj.Codigo == codigo select obj;
            return result.FirstOrDefault();
        }

        public Dominio.Entidades.Embarcador.Financeiro.PlanoConta BuscarContaCreditoTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro>();
            var result = from obj in query where obj.Titulo.Codigo == codigo select obj;            
            if (result.Count() > 0)
                return result.OrderBy("Codigo").Select(obj => obj.PlanoDeContaCredito).FirstOrDefault();
            else
                return null;
        }

        public Dominio.Entidades.Embarcador.Financeiro.PlanoConta BuscarContaCreditoTituloContratoFrete(int codigoContratoFrete)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro>();
            var result = from obj in query where obj.Titulo != null && obj.TipoDocumentoMovimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.ContratoFrete select obj;
            if (result.Count() > 0)
            {
                var queryTitulo = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.Titulo>();
                var resultTitulo = from obj in queryTitulo where obj.ContratoFrete.Codigo == codigoContratoFrete select obj;
                var codigoTitulos = resultTitulo.Select(obj => obj.Codigo).ToList();

                return result.Where(obj => codigoTitulos.Contains(obj.Titulo.Codigo)).Select(obj => obj.PlanoDeContaCredito).FirstOrDefault();
            }
            else
                return null;
        }

        public Dominio.Entidades.Embarcador.Financeiro.PlanoConta BuscarContaDebitoTitulo(int codigo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro>();
            var result = from obj in query where obj.Titulo.Codigo == codigo select obj;
            if (result.Count() > 0)
                return result.Select(obj => obj.PlanoDeContaDebito).FirstOrDefault();
            else
                return null;
        }

        public Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro BuscarMovimentoBaixaTituloReceber(int codigoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro>();
            var result = from obj in query where obj.Documento.Equals(codigoTitulo.ToString()) && obj.TipoDocumentoMovimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recebimento select obj;
            if (result.Count() > 0)
                return result.FirstOrDefault();
            else
                return null;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaMovimentoFinanceiro filtrosPesquisa, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var result = Consultar(filtrosPesquisa);

            result = result.OrderBy(propOrdenacao + (dirOrdenacao == "asc" ? " ascending" : " descending"));

            if (inicioRegistros > 0)
                result = result.Skip(inicioRegistros);

            if (maximoRegistros > 0)
                result = result.Take(maximoRegistros);

            return result
                .Fetch(o => o.PlanoDeContaDebito)
                .Fetch(o => o.PlanoDeContaCredito)
                .Fetch(o => o.Pessoa)
                .Fetch(o => o.GrupoPessoas)
                .ToList();
        }

        public int ContarConsulta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaMovimentoFinanceiro filtrosPesquisa)
        {
            var result = Consultar(filtrosPesquisa);

            return result.Count();
        }

        public decimal BuscarSaldoAnteriorConta(int codigoConta, DateTime dataInicial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();

            var result = from obj in query select obj;

            if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataMovimento < dataInicial);
            else
                return 0;

            decimal valorCredito = 0, valorDebito = 0;
            if (codigoConta > 0)
            {
                var resultCredito = result.Where(obj => obj.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito && obj.PlanoDeConta.Codigo == codigoConta);
                if (resultCredito.Count() > 0)
                    valorCredito = resultCredito.Select(obj => obj.Valor).Sum();

                var resultDebito = result.Where(obj => obj.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito && obj.PlanoDeConta.Codigo == codigoConta);
                if (resultDebito.Count() > 0)
                    valorDebito = resultDebito.Select(obj => obj.Valor).Sum();
            }
            return valorDebito - valorCredito;
        }

        public decimal BuscarSaldoEntradas(int codigoConta, DateTime dataInicial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();

            var result = from obj in query select obj;

            if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataMovimento >= dataInicial);
            else
                return 0;

            decimal valorDebito = 0;
            if (codigoConta > 0)
            {
                var resultDebito = result.Where(obj => obj.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito && obj.PlanoDeConta.Codigo == codigoConta);
                if (resultDebito.Count() > 0)
                    valorDebito = resultDebito.Select(obj => obj.Valor).Sum();
            }
            return valorDebito;
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito> BuscarMovimentosFinanceiro(int codigoConta, DateTime dataInicial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();

            var result = from obj in query select obj;

            result = result.Where(obj => obj.DataMovimento >= dataInicial);

            var resultCredito = result.Where(obj => obj.PlanoDeConta.Codigo == codigoConta);

            return resultCredito.ToList();
        }

        public decimal BuscarSaldoSaidas(int codigoConta, DateTime dataInicial)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();

            var result = from obj in query select obj;

            if (dataInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataMovimento >= dataInicial);
            else
                return 0;

            decimal valorCredito = 0;
            if (codigoConta > 0)
            {
                var resultCredito = result.Where(obj => obj.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito && obj.PlanoDeConta.Codigo == codigoConta);
                if (resultCredito.Count() > 0)
                    valorCredito = resultCredito.Select(obj => obj.Valor).Sum();
            }
            return valorCredito;
        }

        public decimal BuscarSaldoContaCliente(int codigoConta, double cnpjCliente)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();

            var result = from obj in query select obj;
            result = result.Where(obj => obj.MovimentoFinanceiro.Pessoa != null && obj.MovimentoFinanceiro.Pessoa.CPF_CNPJ == cnpjCliente);

            decimal valorCredito = 0;
            var resultCredito = result.Where(obj => obj.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito && obj.PlanoDeConta.Codigo == codigoConta);
            if (resultCredito.Count() > 0)
                valorCredito = resultCredito.Select(obj => obj.Valor).Sum();

            decimal valorDebito = 0;
            var resultDebito = result.Where(obj => obj.DebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito && obj.PlanoDeConta.Codigo == codigoConta);
            if (resultDebito.Count() > 0)
                valorDebito = resultDebito.Select(obj => obj.Valor).Sum();

            return valorDebito - valorCredito;
        }

        public decimal BuscarSaldoMotorista(int codigoTipoMovimento, int codigoMotorista)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroDebitoCredito>();
            var queryMotorista = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiroEntidade>();

            var result = from obj in query select obj;
            var resultMotorista = from obj in queryMotorista where obj.Motorista.Codigo == codigoMotorista select obj;

            decimal valorCredito = 0, valorDebito = 0;
            if (codigoMotorista > 0)
            {
                var resultCredito = resultMotorista.Where(obj => obj.TipoMovimentoEntidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Entrada);
                if (codigoTipoMovimento > 0)
                    resultCredito = resultCredito.Where(obj => obj.MovimentoFinanceiro.TipoMovimento.Codigo == codigoTipoMovimento);
                if (resultCredito.Count() > 0)
                    valorCredito = resultCredito.Select(obj => obj.MovimentoFinanceiro.Valor).Sum();

                var resultDebito = resultMotorista.Where(obj => obj.TipoMovimentoEntidade == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoMovimentoEntidade.Saida);
                if (codigoTipoMovimento > 0)
                    resultDebito = resultDebito.Where(obj => obj.MovimentoFinanceiro.TipoMovimento.Codigo == codigoTipoMovimento);
                if (resultDebito.Count() > 0)
                    valorDebito = resultDebito.Select(obj => obj.MovimentoFinanceiro.Valor).Sum();
            }
            return valorCredito - valorDebito;
        }

        public bool ContemMovimentoFinanceiroMesmaDataValorNumeroDocumento(DateTime dataMovimento, decimal valorMovimento, string numeroDocumento, int codigoMovimento)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro>();

            query = query.Where(o => o.DataMovimento.Date == dataMovimento.Date && o.Valor == valorMovimento && o.Documento.Equals(numeroDocumento) && o.Codigo != codigoMovimento);

            return query.Any();
        }

        public Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro BuscarPorBaixaTitulo(int codigoBaixa, DateTime dataBaixa, decimal valorBaixa, decimal valorPagoTitulo, int codigoTitulo)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro>();
            var result = from obj in query
                         where obj.DataMovimento == dataBaixa && obj.Documento.Equals(codigoBaixa.ToString()) && obj.TipoDocumentoMovimento == TipoDocumentoMovimento.Recebimento &&
                                (obj.Valor == valorBaixa || obj.Valor == valorPagoTitulo) && (obj.Titulo == null || obj.Titulo.Codigo == codigoTitulo)
                         select obj;
            return result.FirstOrDefault();
        }

        #endregion

        #region Relatórios

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoAcertoViagem> RelatorioExtratoAcertoViagem(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoAcertoViagem filtrosPesquisa, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool naoDescontarValorSaldoMotorista, bool paginar = true)
        {
            string sqlFiltrosDataAcertoAdiantamentoSaldoAnterior = "";
            string sqlFiltrosDataAcertoDiariaSaldoAnterior = "";
            string sqlFiltrosDataAcertoOutraDespesaSaldoAnterior = "";
            string sqlFiltrosAcertoAdiantamento = "";
            string sqlFiltrosJustificativa = "";
            string sqlFiltroTipoLancamento = "";

            string sqlSaldoAnterior = "";
            string sqlAcertoAdiantamentoSaldoAnterior = "";
            string sqlAcertoDiariaSaldoAnterior = "";
            string sqlAcertoOutraDespesaSaldoAnterior = "";
            string sqlMotoristaCPFSaldoAnterior = " AND MOT.FUN_CPF = T.CPFMotorista), 0.0)";

            string datePatternInicial = "yyyy-MM-dd 00:00:00";
            string datePatternFinal = "yyyy-MM-dd 23:59:59";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                sqlFiltrosDataAcertoAdiantamentoSaldoAnterior += $" AND PA.PAM_DATA_PAGAMENTO < '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "'";
                sqlFiltrosDataAcertoDiariaSaldoAnterior += $" AND AA.ACD_DATA < '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "'";
                sqlFiltrosDataAcertoOutraDespesaSaldoAnterior += $" AND AA.AOD_DATA < '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "'";
            }
            else
            {
                sqlFiltrosDataAcertoAdiantamentoSaldoAnterior += $" AND PA.PAM_DATA_PAGAMENTO < '1987-01-01 00:00:00'";
                sqlFiltrosDataAcertoDiariaSaldoAnterior += $" AND AA.ACD_DATA < '1987-01-01 00:00:00'";
                sqlFiltrosDataAcertoOutraDespesaSaldoAnterior += $" AND AA.AOD_DATA < '1987-01-01 00:00:00'";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoLancamento))
                sqlFiltroTipoLancamento += $" 1 = 1";
            else
                sqlFiltroTipoLancamento += $" 1 = 1";

            if (filtrosPesquisa.Motorista > 0)
                sqlFiltrosAcertoAdiantamento += $" AND MOT.FUN_CODIGO = " + filtrosPesquisa.Motorista.ToString(); // SQL-INJECTION-SAFE

            if (filtrosPesquisa.Veiculo > 0)
                sqlFiltrosAcertoAdiantamento += $" AND AV.ACV_CODIGO IN (SELECT VV.ACV_CODIGO FROM T_ACERTO_VEICULO VV WHERE VV.VEI_CODIGO = " + filtrosPesquisa.Veiculo.ToString() + ")"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.SegmentoVeiculo > 0)
                sqlFiltrosAcertoAdiantamento += $" AND AV.VSE_CODIGO = " + filtrosPesquisa.SegmentoVeiculo.ToString() + ")"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.SituacaoAcerto != null && filtrosPesquisa.SituacaoAcerto.Count > 0)
                sqlFiltrosAcertoAdiantamento += $" AND AV.ACV_SITUACAO IN ( " + string.Join<int>(",", filtrosPesquisa.SituacaoAcerto) + " )"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.Justificativas.Count > 0)
                sqlFiltrosJustificativa += $" AND Justificativa.JUS_CODIGO IN ( " + string.Join<int>(",", filtrosPesquisa.Justificativas) + " )"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.CentroResultado > 0)
                sqlFiltrosAcertoAdiantamento += $" AND ISNULL(AV.CRE_CODIGO, MOT.CRE_CODIGO) = " + filtrosPesquisa.CentroResultado.ToString(); // SQL-INJECTION-SAFE

            string queryOrdenacao = "";
            string queryParti = "";
            var agrup = false;
            if (!string.IsNullOrWhiteSpace(propGrupo))
            {
                agrup = true;
                queryOrdenacao += " order by " + propGrupo + " " + dirOrdenacaoGrupo;
                queryParti = "PARTITION by CPFMotorista ";// + propGrupo + " ";
            }

            if (!string.IsNullOrWhiteSpace(propOrdenacao) && propGrupo != propOrdenacao)
            {
                if (agrup)
                    queryOrdenacao += ", " + propOrdenacao + " " + dirOrdenacao;
                else
                    queryOrdenacao += " order by " + propOrdenacao + " " + dirOrdenacao; 
            }

            string sqlquery =
                @" SELECT " +
                 "      TT.Codigo, " +
                 "      TT.Data, " +
                 "      TT.TipoDocumento, " +
                 "      TT.NumeroDocumento, " +
                 "      TT.ValorSaida, " +
                 "      TT.ValorEntrada, " +
                 "      TT.NumeroFrota, " +
                 "      TT.Motorista, " +
                 "      TT.CPFMotorista, " +
                 "      TT.Observacao, " +
                 "      TT.Veiculos, " +
                 "      TT.Justificativa, " +
                 "      TT.SaldoAnterior, " +
                 "      SUM(TT.SaldoAnterior + (TT.ValorEntrada - (TT.ValorSaida))) " +
                 "          OVER ( " + queryParti + queryOrdenacao + @"  ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS Saldo, " +
                 "      TT.NumeroAcerto " +
                 " FROM ";

            sqlSaldoAnterior +=
                @"  (SELECT " +
                 "        T.Codigo, " +
                 "        T.Data, " +
                 "        T.TipoDocumento, " +
                 "        T.NumeroDocumento, " +
                 "        T.ValorSaida, " +
                 "        T.ValorEntrada, " +
                 "        T.NumeroFrota, " +
                 "        T.Motorista, " +
                 "        T.CPFMotorista, " +
                 "        T.Observacao, " +
                 "        T.Veiculos, " +
                 "        T.Justificativa, " +
                 "        T.NumeroAcerto, " +
                 "        CASE " +
                 "          WHEN ROW_NUMBER() OVER(" + queryParti + queryOrdenacao + @" ) = 1 THEN ( " +
                 " ";

            if (!naoDescontarValorSaldoMotorista)
                sqlAcertoAdiantamentoSaldoAnterior +=
                    @" ISNULL((SELECT SUM(PA.PAM_VALOR - ISNULL(PA.PAM_SALDO_DESCONTADO, 0)) " +
                    "	        FROM T_ACERTO_ADIANTAMENTO AA " +
                    "	        JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = AA.ACV_CODIGO " +
                    "	        JOIN T_PAGAMENTO_MOTORISTA_TMS PA ON PA.PAM_CODIGO = AA.PAM_CODIGO " +
                    "	        JOIN T_FUNCIONARIO MOT ON MOT.FUN_CODIGO = AV.FUN_CODIGO_MOTORISTA " +
                    "               WHERE ";
            else
                sqlAcertoAdiantamentoSaldoAnterior +=
                    @" ISNULL((SELECT SUM(PA.PAM_VALOR) " +
                    "	        FROM T_ACERTO_ADIANTAMENTO AA " +
                    "	        JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = AA.ACV_CODIGO " +
                    "	        JOIN T_PAGAMENTO_MOTORISTA_TMS PA ON PA.PAM_CODIGO = AA.PAM_CODIGO " +
                    "	        JOIN T_FUNCIONARIO MOT ON MOT.FUN_CODIGO = AV.FUN_CODIGO_MOTORISTA " +
                    "               WHERE ";

            sqlAcertoDiariaSaldoAnterior +=
                @" " +
                 "-" +
                 "   ISNULL((SELECT SUM(AA.ACD_VALOR) " +
                 "               FROM T_ACERTO_DIARIA AA " +
                 " 			     JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = AA.ACV_CODIGO " +
                 "               JOIN T_FUNCIONARIO MOT ON MOT.FUN_CODIGO = AV.FUN_CODIGO_MOTORISTA " +
                 "               WHERE ";

            sqlAcertoOutraDespesaSaldoAnterior +=
                @" " +
                 "-" +
                 "   ISNULL((SELECT SUM(CASE WHEN AA.AOD_QUANTIDADE > 1 THEN (AA.AOD_VALOR * AA.AOD_QUANTIDADE) ELSE AA.AOD_VALOR END) " +
                 "               FROM T_ACERTO_OUTRA_DESPESA AA " +
                 "               JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = AA.ACV_CODIGO " +
                 "               JOIN T_FUNCIONARIO MOT ON MOT.FUN_CODIGO = AV.FUN_CODIGO_MOTORISTA " +
                 "               WHERE ";

            sqlSaldoAnterior += sqlAcertoAdiantamentoSaldoAnterior + sqlFiltroTipoLancamento + sqlFiltrosAcertoAdiantamento + sqlFiltrosDataAcertoAdiantamentoSaldoAnterior + sqlMotoristaCPFSaldoAnterior +
                                sqlAcertoDiariaSaldoAnterior + sqlFiltroTipoLancamento + sqlFiltrosAcertoAdiantamento + sqlFiltrosDataAcertoDiariaSaldoAnterior + sqlMotoristaCPFSaldoAnterior +
                                sqlAcertoOutraDespesaSaldoAnterior + sqlFiltroTipoLancamento + sqlFiltrosAcertoAdiantamento + sqlFiltrosDataAcertoOutraDespesaSaldoAnterior + sqlMotoristaCPFSaldoAnterior +
                        " ) ELSE 0.0 " +
                        "END AS SaldoAnterior";

            string sqlFiltrosDataAcertoAdiantamento = "";
            string sqlFiltrosDataAcertoDiaria = "";
            string sqlFiltrosDataAcertoOutraDespesa = "";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                sqlFiltrosDataAcertoAdiantamento = $" AND PA.PAM_DATA_PAGAMENTO >= '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "'";
                sqlFiltrosDataAcertoDiaria = $" AND AA.ACD_DATA >= '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "'";
                sqlFiltrosDataAcertoOutraDespesa = $" AND AA.AOD_DATA >= '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "'";
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                sqlFiltrosDataAcertoAdiantamento += $" AND PA.PAM_DATA_PAGAMENTO <= '" + filtrosPesquisa.DataFinal.ToString(datePatternFinal) + "'";
                sqlFiltrosDataAcertoDiaria += $" AND AA.ACD_DATA <= '" + filtrosPesquisa.DataFinal.ToString(datePatternFinal) + "'";
                sqlFiltrosDataAcertoOutraDespesa += $" AND AA.AOD_DATA <= '" + filtrosPesquisa.DataFinal.ToString(datePatternFinal) + "'";
            }

            string sqlAcertoAdiantamento =
                @" FROM " +
                 "    (SELECT AA.ADI_CODIGO Codigo, " +
                 "               PA.PAM_DATA_PAGAMENTO Data, " +
                 "               'Adiantamento' TipoDocumento, " +
                 "               CAST(PA.PAM_NUMERO AS varchar(20)) NumeroDocumento, " +
                 "               0.0 ValorSaida, " +
                 "               PA.PAM_VALOR - ISNULL(PA.PAM_SALDO_DESCONTADO, 0) ValorEntrada, " +
                 "               AV.VEI_NUMERO_FROTA NumeroFrota, " +
                 "               MOT.FUN_NOME Motorista, " +
                 "               MOT.FUN_CPF CPFMotorista, " +
                 "               '' Observacao, " +
                 "               SUBSTRING((SELECT DISTINCT ', ' + Veiculo.VEI_PLACA " +
                 "               FROM T_VEICULO Veiculo " +
                 "               JOIN T_ACERTO_VEICULO AcertoVeiculo on AcertoVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO " +
                 "               WHERE AcertoVeiculo.ACV_CODIGO = AV.ACV_CODIGO FOR XML PATH('')), 3, 1000) Veiculos, " +
                 "               '' Justificativa, " +
                 "               AV.ACV_NUMERO NumeroAcerto " +
                 "           FROM T_ACERTO_ADIANTAMENTO AA " +
                 "           JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = AA.ACV_CODIGO " +
                 "           JOIN T_PAGAMENTO_MOTORISTA_TMS PA ON PA.PAM_CODIGO = AA.PAM_CODIGO " +
                 "           JOIN T_FUNCIONARIO MOT ON MOT.FUN_CODIGO = AV.FUN_CODIGO_MOTORISTA " +
                 "           WHERE ";

            if (naoDescontarValorSaldoMotorista)
                sqlAcertoAdiantamento =
                @" FROM " +
                 "    (SELECT AA.ADI_CODIGO Codigo, " +
                 "               PA.PAM_DATA_PAGAMENTO Data, " +
                 "               'Adiantamento' TipoDocumento, " +
                 "               CAST(PA.PAM_NUMERO AS varchar(20)) NumeroDocumento, " +
                 "               0.0 ValorSaida, " +
                 "               PA.PAM_VALOR ValorEntrada, " +
                 "               AV.VEI_NUMERO_FROTA NumeroFrota, " +
                 "               MOT.FUN_NOME Motorista, " +
                 "               MOT.FUN_CPF CPFMotorista, " +
                 "               '' Observacao, " +
                 "               SUBSTRING((SELECT DISTINCT ', ' + Veiculo.VEI_PLACA " +
                 "               FROM T_VEICULO Veiculo " +
                 "               JOIN T_ACERTO_VEICULO AcertoVeiculo on AcertoVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO " +
                 "               WHERE AcertoVeiculo.ACV_CODIGO = AV.ACV_CODIGO FOR XML PATH('')), 3, 1000) Veiculos, " +
                 "               '' Justificativa, " +
                 "               AV.ACV_NUMERO NumeroAcerto " +
                 "           FROM T_ACERTO_ADIANTAMENTO AA " +
                 "           JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = AA.ACV_CODIGO " +
                 "           JOIN T_PAGAMENTO_MOTORISTA_TMS PA ON PA.PAM_CODIGO = AA.PAM_CODIGO " +
                 "           JOIN T_FUNCIONARIO MOT ON MOT.FUN_CODIGO = AV.FUN_CODIGO_MOTORISTA " +
                 "           WHERE ";

            if (filtrosPesquisa.Justificativas.Count > 0)
                sqlAcertoAdiantamento += $" 1 = 0 ";
            else
                sqlAcertoAdiantamento += $" 1 = 1 ";

            sqlAcertoAdiantamento += sqlFiltrosAcertoAdiantamento + sqlFiltrosDataAcertoAdiantamento + " UNION ";


            string sqlAcertoDiaria =
                @" SELECT AA.ACD_CODIGO Codigo, " +
                 "               AA.ACD_DATA Data, " +
                 "               'Diária' TipoDocumento, " +
                 "               CAST(AA.ACD_CODIGO AS varchar(20)) NumeroDocumento, " +
                 "               AA.ACD_VALOR ValorSaida, " +
                 "               0.0 ValorEntrada, " +
                 "               AV.VEI_NUMERO_FROTA NumeroFrota, " +
                 "               MOT.FUN_NOME Motorista, " +
                 "               MOT.FUN_CPF CPFMotorista, " +
                 "               AA.ACD_DESCRICAO Observacao, " +
                 "               SUBSTRING((SELECT DISTINCT ', ' + Veiculo.VEI_PLACA " +
                 "           FROM T_VEICULO Veiculo " +
                 "           JOIN T_ACERTO_VEICULO AcertoVeiculo on AcertoVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO " +
                 "           WHERE AcertoVeiculo.ACV_CODIGO = AV.ACV_CODIGO FOR XML PATH('')), 3, 1000) Veiculos, " +
                 "           Justificativa.JUS_DESCRICAO Justificativa, " +
                 "           AV.ACV_NUMERO NumeroAcerto " +
                 "           FROM T_ACERTO_DIARIA AA " +
                 "           JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = AA.ACV_CODIGO " +
                 "           JOIN T_FUNCIONARIO MOT ON MOT.FUN_CODIGO = AV.FUN_CODIGO_MOTORISTA " +
                 "           JOIN T_JUSTIFICATIVA Justificativa ON Justificativa.JUS_CODIGO = AA.JUS_CODIGO " +
                 "           WHERE " +
                 "              AA.ACD_DATA IS NOT NULL AND ";
            sqlAcertoDiaria += sqlFiltroTipoLancamento + sqlFiltrosAcertoAdiantamento + sqlFiltrosDataAcertoDiaria + sqlFiltrosJustificativa + " UNION ";


            string sqlAcertoOutraDespesa =
                @" SELECT AA.AOD_CODIGO Codigo, " +
                 "           AA.AOD_DATA Data, " +
                 "           'Outras Despesas' TipoDocumento, " +
                 "           CAST(AA.AOD_NUMERO_DOCUMENTO AS varchar(20)) NumeroDocumento, " +
                 "           CASE WHEN AA.AOD_QUANTIDADE > 1 THEN (AA.AOD_VALOR * AA.AOD_QUANTIDADE) ELSE AA.AOD_VALOR END ValorSaida, " +
                 "           0.0 ValorEntrada, " +
                 "           AV.VEI_NUMERO_FROTA NumeroFrota, " +
                 "          MOT.FUN_NOME Motorista, " +
                 "           MOT.FUN_CPF CPFMotorista, " +
                 "           AA.AOD_OBSERVACAO Observacao, " +
                 "           SUBSTRING((SELECT DISTINCT ', ' + Veiculo.VEI_PLACA " +
                 "       FROM T_VEICULO Veiculo " +
                 "       JOIN T_ACERTO_VEICULO AcertoVeiculo on AcertoVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO " +
                 "       WHERE AcertoVeiculo.ACV_CODIGO = AV.ACV_CODIGO FOR XML PATH('')), 3, 1000) Veiculos, " +
                 "       Justificativa.JUS_DESCRICAO Justificativa, " +
                 "       AV.ACV_NUMERO NumeroAcerto " +
                 "       FROM T_ACERTO_OUTRA_DESPESA AA " +
                 "       JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = AA.ACV_CODIGO " +
                 "       JOIN T_FUNCIONARIO MOT ON MOT.FUN_CODIGO = AV.FUN_CODIGO_MOTORISTA " +
                 "       JOIN T_JUSTIFICATIVA Justificativa ON Justificativa.JUS_CODIGO = AA.JUS_CODIGO  " +
                 "      WHERE ";
            sqlAcertoOutraDespesa += sqlFiltroTipoLancamento + sqlFiltrosAcertoAdiantamento + sqlFiltrosDataAcertoOutraDespesa + sqlFiltrosJustificativa;

            string query = sqlquery + sqlSaldoAnterior + sqlAcertoAdiantamento + sqlAcertoDiaria + sqlAcertoOutraDespesa + ") AS T) AS TT ";

            query += queryOrdenacao;

            if (paginar && maximoRegistros > 0)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoAcertoViagem)));

            return nhQuery.SetTimeout(6000).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoAcertoViagem>();
        }

        public int ContarRelatorioExtratoAcertoViagem(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoAcertoViagem filtrosPesquisa)
        {
            string sqlFiltrosDataAcertoAdiantamentoSaldoAnterior = "";
            string sqlFiltrosDataAcertoDiariaSaldoAnterior = "";
            string sqlFiltrosDataAcertoOutraDespesaSaldoAnterior = "";
            string sqlFiltrosAcertoAdiantamento = "";
            string sqlFiltrosJustificativa = "";
            string sqlFiltroTipoLancamento = "";

            string sqlSaldoAnterior = "";
            string sqlAcertoAdiantamentoSaldoAnterior = "";
            string sqlAcertoDiariaSaldoAnterior = "";
            string sqlAcertoOutraDespesaSaldoAnterior = "";
            string sqlMotoristaCPFSaldoAnterior = " AND MOT.FUN_CPF = T.CPFMotorista), 0.0)";

            string datePatternInicial = "yyyy-MM-dd 00:00:00";
            string datePatternFinal = "yyyy-MM-dd 23:59:59";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                sqlFiltrosDataAcertoAdiantamentoSaldoAnterior += $" AND PA.PAM_DATA_PAGAMENTO < '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "'";
                sqlFiltrosDataAcertoDiariaSaldoAnterior += $" AND AA.ACD_DATA < '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "'";
                sqlFiltrosDataAcertoOutraDespesaSaldoAnterior += $" AND AA.AOD_DATA < '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "'";
            }

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.TipoLancamento))
                sqlFiltroTipoLancamento += $" 1 = 1";
            else
                sqlFiltroTipoLancamento += $" 1 = 1";

            if (filtrosPesquisa.Motorista > 0)
                sqlFiltrosAcertoAdiantamento += $" AND MOT.FUN_CODIGO = " + filtrosPesquisa.Motorista.ToString();

            if (filtrosPesquisa.Veiculo > 0)
                sqlFiltrosAcertoAdiantamento += $" AND AV.ACV_CODIGO IN (SELECT VV.ACV_CODIGO FROM T_ACERTO_VEICULO VV WHERE VV.VEI_CODIGO = " + filtrosPesquisa.Veiculo.ToString() + ")"; // SQL-INJECTION-SAFE

            if (filtrosPesquisa.SegmentoVeiculo > 0)
                sqlFiltrosAcertoAdiantamento += $" AND AV.VSE_CODIGO = " + filtrosPesquisa.SegmentoVeiculo.ToString() + ")";

            if (filtrosPesquisa.SituacaoAcerto != null && filtrosPesquisa.SituacaoAcerto.Count > 0)
                sqlFiltrosAcertoAdiantamento += $" AND AV.ACV_SITUACAO IN ( " + string.Join<int>(",", filtrosPesquisa.SituacaoAcerto) + " )";

            if (filtrosPesquisa.Justificativas.Count > 0)
                sqlFiltrosJustificativa += $" AND Justificativa.JUS_CODIGO IN (" + String.Join<int>(",", filtrosPesquisa.Justificativas) + ")";


            if (filtrosPesquisa.CentroResultado > 0)
                sqlFiltrosAcertoAdiantamento += $" AND ISNULL(AV.CRE_CODIGO, MOT.CRE_CODIGO) = " + filtrosPesquisa.CentroResultado.ToString();

            string sqlquery =
                @" SELECT " +
                 "      TT.Codigo, " +
                 "      TT.Data, " +
                 "      TT.TipoDocumento, " +
                 "      TT.NumeroDocumento, " +
                 "      TT.ValorSaida, " +
                 "      TT.ValorEntrada, " +
                 "      TT.NumeroFrota, " +
                 "      TT.Motorista, " +
                 "      TT.CPFMotorista, " +
                 "      TT.Observacao, " +
                 "      TT.Veiculos, " +
                 "      TT.Justificativa, " +
                 "      TT.SaldoAnterior, " +
                 "      SUM(TT.SaldoAnterior + (TT.ValorEntrada - (TT.ValorSaida * -1))) " +
                 "          OVER (ORDER BY Codigo ASC ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS Saldo " +
                 " FROM ";

            sqlSaldoAnterior +=
                @"  (SELECT " +
                 "        T.Codigo, " +
                 "        T.Data, " +
                 "        T.TipoDocumento, " +
                 "        T.NumeroDocumento, " +
                 "        T.ValorSaida, " +
                 "        T.ValorEntrada, " +
                 "        T.NumeroFrota, " +
                 "        T.Motorista, " +
                 "        T.CPFMotorista, " +
                 "        T.Observacao, " +
                 "        T.Veiculos, " +
                 "        T.Justificativa, " +
                 "        CASE " +
                 "          WHEN ROW_NUMBER() OVER(ORDER BY Codigo ASC) = 1 THEN ( " +
                 " ";

            sqlAcertoAdiantamentoSaldoAnterior +=
                @" ISNULL((SELECT SUM(PA.PAM_VALOR - ISNULL(PA.PAM_SALDO_DESCONTADO, 0)) " +
                "	        FROM T_ACERTO_ADIANTAMENTO AA " +
                "	        JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = AA.ACV_CODIGO " +
                "	        JOIN T_PAGAMENTO_MOTORISTA_TMS PA ON PA.PAM_CODIGO = AA.PAM_CODIGO " +
                "	        JOIN T_FUNCIONARIO MOT ON MOT.FUN_CODIGO = AV.FUN_CODIGO_MOTORISTA " +
                "               WHERE ";

            sqlAcertoDiariaSaldoAnterior +=
                @" " +
                 "-" +
                 "   ISNULL((SELECT SUM(AA.ACD_VALOR) " +
                 "               FROM T_ACERTO_DIARIA AA " +
                 " 			     JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = AA.ACV_CODIGO " +
                 "               JOIN T_FUNCIONARIO MOT ON MOT.FUN_CODIGO = AV.FUN_CODIGO_MOTORISTA " +
                 "               WHERE ";

            sqlAcertoOutraDespesaSaldoAnterior +=
                @" " +
                 "-" +
                 "   ISNULL((SELECT SUM(CASE WHEN AA.AOD_QUANTIDADE > 1 THEN (AA.AOD_VALOR * AA.AOD_QUANTIDADE) ELSE AA.AOD_VALOR END) " +
                 "               FROM T_ACERTO_OUTRA_DESPESA AA " +
                 "               JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = AA.ACV_CODIGO " +
                 "               JOIN T_FUNCIONARIO MOT ON MOT.FUN_CODIGO = AV.FUN_CODIGO_MOTORISTA " +
                 "               WHERE ";

            sqlSaldoAnterior += sqlAcertoAdiantamentoSaldoAnterior + sqlFiltroTipoLancamento + sqlFiltrosAcertoAdiantamento + sqlFiltrosDataAcertoAdiantamentoSaldoAnterior + sqlMotoristaCPFSaldoAnterior +
                                sqlAcertoDiariaSaldoAnterior + sqlFiltroTipoLancamento + sqlFiltrosAcertoAdiantamento + sqlFiltrosDataAcertoDiariaSaldoAnterior + sqlMotoristaCPFSaldoAnterior +
                                sqlAcertoOutraDespesaSaldoAnterior + sqlFiltroTipoLancamento + sqlFiltrosAcertoAdiantamento + sqlFiltrosDataAcertoOutraDespesaSaldoAnterior + sqlMotoristaCPFSaldoAnterior +
                        " ) ELSE 0.0 " +
                        "END AS SaldoAnterior";

            string sqlFiltrosDataAcertoAdiantamento = "";
            string sqlFiltrosDataAcertoDiaria = "";
            string sqlFiltrosDataAcertoOutraDespesa = "";

            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                sqlFiltrosDataAcertoAdiantamento = $" AND PA.PAM_DATA_PAGAMENTO >= '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "'";
                sqlFiltrosDataAcertoDiaria = $" AND AA.ACD_DATA >= '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "'";
                sqlFiltrosDataAcertoOutraDespesa = $" AND AA.AOD_DATA >= '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "'";
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                sqlFiltrosDataAcertoAdiantamento += $" AND PA.PAM_DATA_PAGAMENTO <= '" + filtrosPesquisa.DataFinal.ToString(datePatternFinal) + "'";
                sqlFiltrosDataAcertoDiaria += $" AND AA.ACD_DATA <= '" + filtrosPesquisa.DataFinal.ToString(datePatternFinal) + "'";
                sqlFiltrosDataAcertoOutraDespesa += $" AND AA.AOD_DATA <= '" + filtrosPesquisa.DataFinal.ToString(datePatternFinal) + "'";
            }

            string sqlAcertoAdiantamento =
                @" FROM " +
                 "    (SELECT AA.ADI_CODIGO Codigo, " +
                 "               PA.PAM_DATA_PAGAMENTO Data, " +
                 "               'Adiantamento' TipoDocumento, " +
                 "               CAST(PA.PAM_NUMERO AS varchar(20)) NumeroDocumento, " +
                 "               0.0 ValorSaida, " +
                 "               PA.PAM_VALOR - ISNULL(PA.PAM_SALDO_DESCONTADO, 0) ValorEntrada, " +
                 "               AV.VEI_NUMERO_FROTA NumeroFrota, " +
                 "               MOT.FUN_NOME Motorista, " +
                 "               MOT.FUN_CPF CPFMotorista, " +
                 "               '' Observacao, " +
                 "               SUBSTRING((SELECT DISTINCT ', ' + Veiculo.VEI_PLACA " +
                 "               FROM T_VEICULO Veiculo " +
                 "               JOIN T_ACERTO_VEICULO AcertoVeiculo on AcertoVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO " +
                 "               WHERE AcertoVeiculo.ACV_CODIGO = AV.ACV_CODIGO FOR XML PATH('')), 3, 1000) Veiculos, " +
                 "               '' Justificativa " +
                 "           FROM T_ACERTO_ADIANTAMENTO AA " +
                 "           JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = AA.ACV_CODIGO " +
                 "           JOIN T_PAGAMENTO_MOTORISTA_TMS PA ON PA.PAM_CODIGO = AA.PAM_CODIGO " +
                 "           JOIN T_FUNCIONARIO MOT ON MOT.FUN_CODIGO = AV.FUN_CODIGO_MOTORISTA " +
                 "           WHERE ";

            if (filtrosPesquisa.Justificativas.Count > 0)
                sqlAcertoAdiantamento += $" 1 = 0 ";
            else
                sqlAcertoAdiantamento += $" 1 = 1 ";

            sqlAcertoAdiantamento += sqlFiltrosAcertoAdiantamento + sqlFiltrosDataAcertoAdiantamento + " UNION ";

            string sqlAcertoDiaria =
                @" SELECT AA.ACD_CODIGO Codigo, " +
                 "               AA.ACD_DATA Data, " +
                 "               'Diária' TipoDocumento, " +
                 "               CAST(AA.ACD_CODIGO AS varchar(20)) NumeroDocumento, " +
                 "               AA.ACD_VALOR ValorSaida, " +
                 "               0.0 ValorEntrada, " +
                 "               AV.VEI_NUMERO_FROTA NumeroFrota, " +
                 "               MOT.FUN_NOME Motorista, " +
                 "               MOT.FUN_CPF CPFMotorista, " +
                 "               AA.ACD_DESCRICAO Observacao, " +
                 "               SUBSTRING((SELECT DISTINCT ', ' + Veiculo.VEI_PLACA " +
                 "           FROM T_VEICULO Veiculo " +
                 "           JOIN T_ACERTO_VEICULO AcertoVeiculo on AcertoVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO " +
                 "           WHERE AcertoVeiculo.ACV_CODIGO = AV.ACV_CODIGO FOR XML PATH('')), 3, 1000) Veiculos, " +
                 "           Justificativa.JUS_DESCRICAO Justificativa " +
                 "           FROM T_ACERTO_DIARIA AA " +
                 "           JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = AA.ACV_CODIGO " +
                 "           JOIN T_FUNCIONARIO MOT ON MOT.FUN_CODIGO = AV.FUN_CODIGO_MOTORISTA " +
                 "           JOIN T_JUSTIFICATIVA Justificativa ON Justificativa.JUS_CODIGO = AA.JUS_CODIGO " +
                 "           WHERE " +
                 "              AA.ACD_DATA IS NOT NULL AND ";
            sqlAcertoDiaria += sqlFiltroTipoLancamento + sqlFiltrosAcertoAdiantamento + sqlFiltrosDataAcertoDiaria + sqlFiltrosJustificativa + " UNION ";


            string sqlAcertoOutraDespesa =
                @" SELECT AA.AOD_CODIGO Codigo, " +
                 "           AA.AOD_DATA Data, " +
                 "           'Outras Despesas' TipoDocumento, " +
                 "           CAST(AA.AOD_NUMERO_DOCUMENTO AS varchar(20)) NumeroDocumento, " +
                 "           CASE WHEN AA.AOD_QUANTIDADE > 1 THEN (AA.AOD_VALOR * AA.AOD_QUANTIDADE) ELSE AA.AOD_VALOR END ValorSaida, " +
                 "           0.0 ValorEntrada, " +
                 "           AV.VEI_NUMERO_FROTA NumeroFrota, " +
                 "          MOT.FUN_NOME Motorista, " +
                 "           MOT.FUN_CPF CPFMotorista, " +
                 "           AA.AOD_OBSERVACAO Observacao, " +
                 "           SUBSTRING((SELECT DISTINCT ', ' + Veiculo.VEI_PLACA " +
                 "       FROM T_VEICULO Veiculo " +
                 "       JOIN T_ACERTO_VEICULO AcertoVeiculo on AcertoVeiculo.VEI_CODIGO = Veiculo.VEI_CODIGO " +
                 "       WHERE AcertoVeiculo.ACV_CODIGO = AV.ACV_CODIGO FOR XML PATH('')), 3, 1000) Veiculos, " +
                 "       Justificativa.JUS_DESCRICAO Justificativa " +
                 "       FROM T_ACERTO_OUTRA_DESPESA AA " +
                 "       JOIN T_ACERTO_DE_VIAGEM AV ON AV.ACV_CODIGO = AA.ACV_CODIGO " +
                 "       JOIN T_FUNCIONARIO MOT ON MOT.FUN_CODIGO = AV.FUN_CODIGO_MOTORISTA " +
                 "       JOIN T_JUSTIFICATIVA Justificativa ON Justificativa.JUS_CODIGO = AA.JUS_CODIGO  " +
                 "      WHERE ";
            sqlAcertoOutraDespesa += sqlFiltroTipoLancamento + sqlFiltrosAcertoAdiantamento + sqlFiltrosDataAcertoOutraDespesa + sqlFiltrosJustificativa;

            string query = sqlquery + sqlSaldoAnterior + sqlAcertoAdiantamento + sqlAcertoDiaria + sqlAcertoOutraDespesa + ") AS T) AS TT ";

            query = @"SELECT COUNT(0) as CONTADOR
                FROM ( " + query + " ) AS A ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.SetTimeout(6000).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoConta> RelatorioExtratoConta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoConta filtrosPesquisa, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros)
        {
            var parametros = new List<ParametroSQL>();  

            string sqlDataDebito = "";
            string sqlDataCredito = "";
            string sqlContaDebito = "";
            string sqlContaCredito = "";
            string sqlColaborador = "";
            string selectColaborador = "";
            string sqlDataBase = "";
            string sqlPessoa = "";
            string sqlGrupoPessoa = "";
            string sqlCodigoMovimento = "";
            string sqlCentroResultado = "";
            string sqlquery = "";

            if (filtrosPesquisa.CodigoMovimento > 0)
                sqlCodigoMovimento = " AND M.MOV_CODIGO = " + filtrosPesquisa.CodigoMovimento.ToString();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CentroResultado))
            {
                if (filtrosPesquisa.CentroResultadoPai)
                    sqlCentroResultado = " AND R.CRE_PLANO LIKE '" + filtrosPesquisa.CentroResultado + ".%'";
                else
                    sqlCentroResultado = " AND R.CRE_PLANO LIKE '" + filtrosPesquisa.CentroResultado + "'";
            }

            if (filtrosPesquisa.PlanosContaAnalitica?.Count > 0)
            {
                sqlContaDebito = $" AND PD.PLA_PLANO in ('{ string.Join("', '", filtrosPesquisa.PlanosContaAnalitica) }')";
                sqlContaCredito = $" AND PC.PLA_PLANO in ('{ string.Join("', '", filtrosPesquisa.PlanosContaAnalitica) }')";
            }

            if (filtrosPesquisa.CodigoColaborador > 0)
            {
                sqlColaborador = " JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = M.FUN_CODIGO AND M.FUN_CODIGO = " + filtrosPesquisa.CodigoColaborador.ToString();
                selectColaborador = ", F.FUN_NOME Colaborador";
            }
            else
            {
                sqlColaborador = " LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = M.FUN_CODIGO ";
                selectColaborador = ", F.FUN_NOME Colaborador";
            }

            string datePatternInicial = "yyyy-MM-dd 00:00:00";
            string datePatternFinal = "yyyy-MM-dd 23:59:59";
            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                sqlDataDebito += " AND MD.MDC_DATA >= '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "'";
                sqlDataCredito += " AND MC.MDC_DATA >= '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "'";
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                sqlDataDebito += " AND MD.MDC_DATA <= '" + filtrosPesquisa.DataFinal.ToString(datePatternFinal) + "'";
                sqlDataCredito += " AND MC.MDC_DATA <= '" + filtrosPesquisa.DataFinal.ToString(datePatternFinal) + "'";
            }

            if (filtrosPesquisa.DataBaseInicial != DateTime.MinValue)
                sqlDataBase += " AND M.MOV_DATA_BASE >= '" + filtrosPesquisa.DataBaseInicial.ToString(datePatternInicial) + "'";

            if (filtrosPesquisa.DataBaseFinal != DateTime.MinValue)
                sqlDataBase += " AND M.MOV_DATA_BASE <= '" + filtrosPesquisa.DataBaseFinal.ToString(datePatternFinal) + "'";

            if (filtrosPesquisa.CodigosGrupoPessoa.Count > 0)
                sqlGrupoPessoa = $" AND M.GRP_CODIGO in ({ string.Join(", ", filtrosPesquisa.CodigosGrupoPessoa) })";

            if (filtrosPesquisa.CnpjPessoa.Count > 0)
                sqlPessoa = $" AND M.CLI_CGCCPF in ({ string.Join(", ", filtrosPesquisa.CnpjPessoa)})";

            string sqlEmpresa = "";
            if (filtrosPesquisa.CodigoEmpresa > 0)
                sqlEmpresa = " AND M.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (filtrosPesquisa.TipoAmbiente != Dominio.Enumeradores.TipoAmbiente.Nenhum)
                sqlEmpresa += " AND M.MOV_AMBIENTE = " + (int)filtrosPesquisa.TipoAmbiente;

            if (filtrosPesquisa.TipoDebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito)
                sqlContaCredito += " AND 1 = 0 ";
            else if (filtrosPesquisa.TipoDebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito)
                sqlContaDebito += " AND 1 = 0 ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumento))
                sqlquery += " and M.MOV_DOCUMENTO like '%" + filtrosPesquisa.NumeroDocumento + "%' ";

            if (filtrosPesquisa.MoedaCotacaoBancoCentral != MoedaCotacaoBancoCentral.Todas)
                sqlquery += " and M.MOV_MOEDA_COTACAO_BANCO_CENTRAL = " + filtrosPesquisa.MoedaCotacaoBancoCentral.ToString("d");

            if (filtrosPesquisa.CodigoPlanoContaSintetica > 0)
            {
                sqlContaDebito += " AND PD.PLA_PLANO in (SELECT DISTINCT PLA_PLANO FROM T_PLANO_DE_CONTA WHERE PLA_TIPO = 1 AND PLA_PLANO LIKE '" + filtrosPesquisa.PlanoContaSintetica + "%')";
                parametros.Add(new ParametroSQL("PLANO_CONTA_SINTETICA_1", filtrosPesquisa.PlanoContaSintetica + "%"));
                sqlContaCredito += " AND PC.PLA_PLANO in (SELECT DISTINCT PLA_PLANO FROM T_PLANO_DE_CONTA WHERE PLA_TIPO = 1 AND PLA_PLANO LIKE '" + filtrosPesquisa.PlanoContaSintetica + "%')";
                parametros.Add(new ParametroSQL("PLANO_CONTA_SINTETICA_2", filtrosPesquisa.PlanoContaSintetica + "%"));
            }

            if (filtrosPesquisa.CodigoPlanoContaContrapartida > 0)
            {
                sqlContaDebito += " AND PC.PLA_CODIGO = " + filtrosPesquisa.CodigoPlanoContaContrapartida;
                sqlContaCredito += " AND PD.PLA_CODIGO = " + filtrosPesquisa.CodigoPlanoContaContrapartida;
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
                    queryOrdenacao += ", " + propOrdenacao + " " + dirOrdenacao;
                else
                    queryOrdenacao += " order by " + propOrdenacao + " " + dirOrdenacao;
            }

            string sqlSaldoAnterior = " 0.0 SaldoAnterior, 0.0 SaldoAnteriorMoedaEstrangeira ";
            if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.CnpjPessoa.Count > 0)
            {
                sqlSaldoAnterior = $@"case when ROW_NUMBER() OVER({ queryOrdenacao }) = 1 
			                            then ISNULL((SELECT sum(CASE MDC_TIPO WHEN 1 THEN MDC_VALOR WHEN 2 THEN MDC_VALOR * -1 ELSE 0.00 END) 
                                                     FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO D 
                                                     JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = D.MOV_CODIGO
			                            WHERE MDC_DATA < '{ filtrosPesquisa.DataInicial.ToString(datePatternInicial) }' AND PLA_CODIGO = T.CodigoPlanoConta  AND M.CLI_CGCCPF in ({ string.Join(", ", filtrosPesquisa.CnpjPessoa)})), 0.0)
                                    else 0.0 end AS SaldoAnterior, ";

                sqlSaldoAnterior += $@"case when ROW_NUMBER() OVER({ queryOrdenacao }) = 1 
			                            then ISNULL((SELECT sum(CASE MDC_TIPO WHEN 1 THEN MOV_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA WHEN 2 THEN MOV_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA * -1 ELSE 0.00 END) 
                                                     FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO D 
                                                     JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = D.MOV_CODIGO
			                            WHERE MDC_DATA < '{ filtrosPesquisa.DataInicial.ToString(datePatternInicial) }' AND PLA_CODIGO = T.CodigoPlanoConta  AND M.CLI_CGCCPF in ({ string.Join(", ", filtrosPesquisa.CnpjPessoa)})), 0.0)
                                    else 0.0 end AS SaldoAnteriorMoedaEstrangeira ";
            }
            else if (filtrosPesquisa.DataInicial > DateTime.MinValue)
            {
                sqlSaldoAnterior = $@"case when ROW_NUMBER() OVER({ queryOrdenacao }) = 1 
			                            then ISNULL((SELECT sum(CASE MDC_TIPO WHEN 1 THEN MDC_VALOR WHEN 2 THEN MDC_VALOR * -1 ELSE 0.00 END) 
                                                     FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO D 
                                                     JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = D.MOV_CODIGO
			                            WHERE MDC_DATA < '{ filtrosPesquisa.DataInicial.ToString(datePatternInicial) }' AND PLA_CODIGO = T.CodigoPlanoConta), 0.0)
                                    else 0.0 end AS SaldoAnterior, ";

                sqlSaldoAnterior += $@"case when ROW_NUMBER() OVER({ queryOrdenacao }) = 1 
			                            then ISNULL((SELECT sum(CASE MDC_TIPO WHEN 1 THEN MOV_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA WHEN 2 THEN MOV_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA * -1 ELSE 0.00 END) 
                                                     FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO D 
                                                     JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = D.MOV_CODIGO
			                            WHERE MDC_DATA < '{ filtrosPesquisa.DataInicial.ToString(datePatternInicial) }' AND PLA_CODIGO = T.CodigoPlanoConta), 0.0)
                                    else 0.0 end AS SaldoAnteriorMoedaEstrangeira ";
            }

            if (filtrosPesquisa.DataBaseInicial > DateTime.MinValue)
            {
                sqlSaldoAnterior = $@"case when ROW_NUMBER() OVER({queryOrdenacao}) = 1 
			                            then ISNULL((SELECT sum(CASE MDC_TIPO WHEN 1 THEN MDC_VALOR WHEN 2 THEN MDC_VALOR * -1 ELSE 0.00 END) 
                                                     FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO D 
                                                     JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = D.MOV_CODIGO
			                            WHERE MOV_DATA_BASE < '{filtrosPesquisa.DataBaseInicial.ToString(datePatternInicial)}' AND PLA_CODIGO = T.CodigoPlanoConta), 0.0)
                                    else 0.0 end AS SaldoAnterior, ";

                sqlSaldoAnterior += $@"case when ROW_NUMBER() OVER({queryOrdenacao}) = 1 
			                            then ISNULL((SELECT sum(CASE MDC_TIPO WHEN 1 THEN MOV_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA WHEN 2 THEN MOV_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA * -1 ELSE 0.00 END) 
                                                     FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO D 
                                                     JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = D.MOV_CODIGO
			                            WHERE MOV_DATA_BASE < '{filtrosPesquisa.DataBaseInicial.ToString(datePatternInicial)}' AND PLA_CODIGO = T.CodigoPlanoConta), 0.0)
                                    else 0.0 end AS SaldoAnteriorMoedaEstrangeira ";
            }

            string query = @"SELECT M.MOV_CODIGO Codigo, " +
                " MD.MDC_DATA Data, " +
                " M.MOV_DATA_BASE Data_Base, " +
                " M.MOV_OBSERVACAO Observacao, " +
                " M.MOV_TIPO TipoDocumento, " +
                " M.MOV_DOCUMENTO NumeroDocumento, " +
                " PD.PLA_PLANO Plano, " +
                " PD.PLA_DESCRICAO PlanoDescricao, " +
                " PC.PLA_PLANO PlanoContraPartida, " +
                " PC.PLA_DESCRICAO PlanoDescricaoContraPartida, " +
                " MD.MDC_VALOR ValorDebito, " +
                " 0.0 ValorCredito, " +
                " PD.PLA_CODIGO CodigoPlanoConta, " +
                " R.CRE_DESCRICAO + ' (' + R.CRE_PLANO + ')' CentroResultado, " +
                " CT.CLI_NOME PessoaTitulo, T.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL NumeroDocumentoTitulo, T.TIT_DATA_VENCIMENTO DataVencimentoTitulo, T.TIT_TIPO_DOCUMENTO_TITULO_ORIGINAL DocumentoTitulo, T.TIT_DESCONTO DescontoTitulo, T.TIT_ACRESCIMO AcrescimoTitulo, T.TIT_VALOR_PAGO ValorPagoTitulo, " +
                " CASE " +
                "     WHEN M.GRP_CODIGO IS NOT NULL THEN G.GRP_DESCRICAO " +
                "     ELSE '' " +
                "  END GrupoFavorecido, " +
                " CASE " +
                "     WHEN M.CLI_CGCCPF IS NOT NULL  THEN C.CLI_NOME " +
                "     ELSE '' " +
                "  END PessoaFavorecido, " +
                " Mot.FUN_NOME Motorista, " +
                " M.MOV_MOEDA_COTACAO_BANCO_CENTRAL MoedaCotacaoBancoCentral, " +
                " M.MOV_VALOR_MOEDA_COTACAO ValorMoedaCotacao, " +
                " M.MOV_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA ValorDebitoMoedaEstrangeira, " +
                " 0.0 ValorCreditoMoedaEstrangeira, " +
                " Configuracao.CEM_UTILIZA_MOEDA_ESTRANGEIRA UtilizaMoedaEstrangeira " +
                    selectColaborador +
                " FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MD " +
                " JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = MD.MOV_CODIGO " +
                " JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = M.PLA_CODIGO_DEBITO " +
                " JOIN T_PLANO_DE_CONTA PC ON PC.PLA_CODIGO = M.PLA_CODIGO_CREDITO " +
                " LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = M.CLI_CGCCPF " +
                " LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = M.GRP_CODIGO " +
                " LEFT OUTER JOIN T_CENTRO_RESULTADO R ON R.CRE_CODIGO = M.CRE_CODIGO " +
                " LEFT OUTER JOIN T_TITULO T ON T.TIT_CODIGO = M.TIT_CODIGO " +
                " LEFT OUTER JOIN T_CLIENTE CT ON CT.CLI_CGCCPF = T.CLI_CGCCPF " +
                " LEFT OUTER JOIN T_MOVIMENTO_FINANCEIRO_ENTIDADE Ent on Ent.MOV_CODIGO = M.MOV_CODIGO  " +
                " LEFT OUTER JOIN T_FUNCIONARIO Mot on Mot.FUN_CODIGO = Ent.FUN_CODIGO " +
                    sqlColaborador + ", T_CONFIGURACAO_EMBARCADOR Configuracao " +
                " where MD.MDC_TIPO = 1 " + sqlContaDebito + sqlDataDebito + sqlEmpresa + sqlDataBase + sqlGrupoPessoa + sqlPessoa + sqlCodigoMovimento + sqlCentroResultado +
                    sqlquery +
                "  " +
                " UNION  " +
                "  " +
                " SELECT M.MOV_CODIGO Codigo, " +
                " MC.MDC_DATA Data, " +
                " M.MOV_DATA_BASE Data_Base, " +
                " M.MOV_OBSERVACAO Observacao, " +
                " M.MOV_TIPO TipoDocumento, " +
                " M.MOV_DOCUMENTO NumeroDocumento, " +
                " PC.PLA_PLANO Plano, " +
                " PC.PLA_DESCRICAO PlanoDescricao, " +
                " PD.PLA_PLANO PlanoContraPartida, " +
                " PD.PLA_DESCRICAO PlanoDescricaoContraPartida, " +
                " 0.0 ValorDebito, " +
                " (MC.MDC_VALOR * -1) ValorCredito, " +
                " PC.PLA_CODIGO CodigoPlanoConta, " +
                " R.CRE_DESCRICAO + ' (' + R.CRE_PLANO + ')' CentroResultado, " +
                " CT.CLI_NOME PessoaTitulo, T.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL NumeroDocumentoTitulo, T.TIT_DATA_VENCIMENTO DataVencimentoTitulo, T.TIT_TIPO_DOCUMENTO_TITULO_ORIGINAL DocumentoTitulo, T.TIT_DESCONTO DescontoTitulo, T.TIT_ACRESCIMO AcrescimoTitulo, T.TIT_VALOR_PAGO ValorPagoTitulo, " +
                " CASE " +
                "     WHEN M.GRP_CODIGO IS NOT NULL THEN G.GRP_DESCRICAO " +
                "     ELSE '' " +
                "  END GrupoFavorecido, " +
                " CASE " +
                "     WHEN M.CLI_CGCCPF IS NOT NULL  THEN C.CLI_NOME " +
                "     ELSE '' " +
                "  END PessoaFavorecido, " +
                " Mot.FUN_NOME Motorista, " +
                " M.MOV_MOEDA_COTACAO_BANCO_CENTRAL MoedaCotacaoBancoCentral, " +
                " M.MOV_VALOR_MOEDA_COTACAO ValorMoedaCotacao, " +
                " 0.0 ValorDebitoMoedaEstrangeira, " +
                " (M.MOV_VALOR_ORIGINAL_MOEDA_ESTRANGEIRA * -1) ValorCreditoMoedaEstrangeira, " +
                " Configuracao.CEM_UTILIZA_MOEDA_ESTRANGEIRA UtilizaMoedaEstrangeira " +
                    selectColaborador +
                " FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MC " +
                " JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = MC.MOV_CODIGO " +
                " JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = M.PLA_CODIGO_DEBITO " +
                " JOIN T_PLANO_DE_CONTA PC ON PC.PLA_CODIGO = M.PLA_CODIGO_CREDITO " +
                " LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = M.CLI_CGCCPF " +
                " LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = M.GRP_CODIGO " +
                " LEFT OUTER JOIN T_CENTRO_RESULTADO R ON R.CRE_CODIGO = M.CRE_CODIGO " +
                " LEFT OUTER JOIN T_TITULO T ON T.TIT_CODIGO = M.TIT_CODIGO " +
                " LEFT OUTER JOIN T_CLIENTE CT ON CT.CLI_CGCCPF = T.CLI_CGCCPF " +
                " LEFT OUTER JOIN T_MOVIMENTO_FINANCEIRO_ENTIDADE Ent on Ent.MOV_CODIGO = M.MOV_CODIGO  " +
                " LEFT OUTER JOIN T_FUNCIONARIO Mot on Mot.FUN_CODIGO = Ent.FUN_CODIGO " +
                    sqlColaborador + ", T_CONFIGURACAO_EMBARCADOR Configuracao " +
                " where MC.MDC_TIPO = 2 " + sqlContaCredito + sqlDataCredito + sqlEmpresa + sqlDataBase + sqlGrupoPessoa + sqlPessoa + sqlCodigoMovimento + sqlCentroResultado +
                    sqlquery;

            query = @"SELECT T.Codigo, T.Data, T.Data_Base, T.Observacao, T.TipoDocumento, T.NumeroDocumento, T.Plano, T.CentroResultado, T.PlanoDescricao, T.PlanoContraPartida, 
                    T.PessoaTitulo, T.NumeroDocumentoTitulo, T.DataVencimentoTitulo, T.DocumentoTitulo, T.DescontoTitulo, T.AcrescimoTitulo, T.ValorPagoTitulo, 
                    T.PlanoDescricaoContraPartida, T.ValorDebito, T.ValorCredito, T.CodigoPlanoConta, T.GrupoFavorecido, T.PessoaFavorecido , T.Colaborador, T.Motorista, 
                    T.MoedaCotacaoBancoCentral, T.ValorMoedaCotacao, T.ValorDebitoMoedaEstrangeira, T.ValorCreditoMoedaEstrangeira, T.UtilizaMoedaEstrangeira,
                    " + sqlSaldoAnterior + @"
                    FROM ( " + query + " ) AS T ";

            query = @"SELECT TT.Codigo, TT.Data, TT.Data_Base, TT.Observacao, TT.TipoDocumento, TT.NumeroDocumento, TT.Plano, TT.CentroResultado, TT.PlanoDescricao, TT.PlanoContraPartida, 
                    TT.PessoaTitulo, TT.NumeroDocumentoTitulo, TT.DataVencimentoTitulo, TT.DocumentoTitulo, TT.DescontoTitulo, TT.AcrescimoTitulo, TT.ValorPagoTitulo,
                    TT.PlanoDescricaoContraPartida, TT.ValorDebito, TT.ValorCredito, TT.CodigoPlanoConta, TT.GrupoFavorecido, TT.PessoaFavorecido , TT.Colaborador, TT.Motorista, TT.SaldoAnterior, 
                    TT.MoedaCotacaoBancoCentral, TT.ValorMoedaCotacao, TT.ValorDebitoMoedaEstrangeira, TT.ValorCreditoMoedaEstrangeira, TT.SaldoAnteriorMoedaEstrangeira, TT.SaldoAnteriorMoedaEstrangeira, TT.UtilizaMoedaEstrangeira, 
                    SUM(TT.SaldoAnterior + (TT.ValorDebito - (TT.ValorCredito * -1))) OVER (" + queryOrdenacao + @" ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS Saldo,
                    SUM(TT.SaldoAnteriorMoedaEstrangeira + (TT.ValorDebitoMoedaEstrangeira - (TT.ValorCreditoMoedaEstrangeira * -1))) OVER (" + queryOrdenacao + @" ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS SaldoMoedaEstrangeira
                    FROM ( " + query + " ) AS TT ";

            query += queryOrdenacao;

            if (maximoRegistros > 0)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";

            var sqlDinamico = new SQLDinamico(query,parametros);

            var nhQuery = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoConta)));

            return nhQuery.SetTimeout(6000).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoConta>();
        }

        public int ContarRelatorioExtratoConta(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoConta filtrosPesquisa)
        {
            var parametros = new List<ParametroSQL>();

            string sqlDataDebito = "";
            string sqlDataCredito = "";
            string sqlContaDebito = "";
            string sqlContaCredito = "";
            string sqlColaborador = "";
            string sqlDataBase = "";
            string sqlPessoa = "";
            string sqlGrupoPessoa = "";
            string sqlCodigoMovimento = "";
            string sqlCentroResultado = "";
            string sqlquery = "";

            if (filtrosPesquisa.CodigoMovimento > 0)
                sqlCodigoMovimento = " AND M.MOV_CODIGO = " + filtrosPesquisa.CodigoMovimento.ToString();

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.CentroResultado))
            {
                if (filtrosPesquisa.CentroResultadoPai)
                    sqlCentroResultado = " AND R.CRE_PLANO LIKE '" + filtrosPesquisa.CentroResultado + ".%'";
                else
                    sqlCentroResultado = " AND R.CRE_PLANO LIKE '" + filtrosPesquisa.CentroResultado + "'";
            }

            if (filtrosPesquisa.PlanosContaAnalitica?.Count > 0)
            {
                sqlContaDebito = $" AND PD.PLA_PLANO in ('{ string.Join("', '", filtrosPesquisa.PlanosContaAnalitica) }')";
                sqlContaCredito = $" AND PC.PLA_PLANO in ('{ string.Join("', '", filtrosPesquisa.PlanosContaAnalitica) }')";
            }

            if (filtrosPesquisa.CodigoColaborador > 0)
                sqlColaborador = " JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = M.FUN_CODIGO AND M.FUN_CODIGO = " + filtrosPesquisa.CodigoColaborador.ToString();
            else
                sqlColaborador = " LEFT OUTER JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = M.FUN_CODIGO ";

            string datePatternInicial = "yyyy-MM-dd 00:00:00";
            string datePatternFinal = "yyyy-MM-dd 23:59:59";
            if (filtrosPesquisa.DataInicial != DateTime.MinValue)
            {
                sqlDataDebito += " AND MD.MDC_DATA >= '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "'";
                sqlDataCredito += " AND MC.MDC_DATA >= '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "'";
            }

            if (filtrosPesquisa.DataFinal != DateTime.MinValue)
            {
                sqlDataDebito += " AND MD.MDC_DATA <= '" + filtrosPesquisa.DataFinal.ToString(datePatternFinal) + "'";
                sqlDataCredito += " AND MC.MDC_DATA <= '" + filtrosPesquisa.DataFinal.ToString(datePatternFinal) + "'";
            }

            if (filtrosPesquisa.DataBaseInicial != DateTime.MinValue)
                sqlDataBase += " AND M.MOV_DATA_BASE >= '" + filtrosPesquisa.DataBaseInicial.ToString(datePatternInicial) + "'";

            if (filtrosPesquisa.DataBaseFinal != DateTime.MinValue)
                sqlDataBase += " AND M.MOV_DATA_BASE <= '" + filtrosPesquisa.DataBaseFinal.ToString(datePatternFinal) + "'";

            if (filtrosPesquisa.CodigosGrupoPessoa.Count > 0)
                sqlGrupoPessoa = $" AND M.GRP_CODIGO in ({ string.Join(", ", filtrosPesquisa.CodigosGrupoPessoa) })";

            if (filtrosPesquisa.CnpjPessoa.Count > 0)
                sqlPessoa = $" AND M.CLI_CGCCPF in ({ string.Join(", ", filtrosPesquisa.CnpjPessoa) })";

            string sqlEmpresa = "";
            if (filtrosPesquisa.CodigoEmpresa > 0)
                sqlEmpresa = " AND M.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

            if (filtrosPesquisa.TipoAmbiente != Dominio.Enumeradores.TipoAmbiente.Nenhum)
                sqlEmpresa += " AND M.MOV_AMBIENTE = " + (int)filtrosPesquisa.TipoAmbiente;

            if (filtrosPesquisa.TipoDebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Debito)
                sqlContaCredito += " AND 1 = 0 ";
            else if (filtrosPesquisa.TipoDebitoCredito == Dominio.ObjetosDeValor.Embarcador.Enumeradores.DebitoCredito.Credito)
                sqlContaDebito += " AND 1 = 0 ";

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumento))
                sqlquery += " and M.MOV_DOCUMENTO like '%" + filtrosPesquisa.NumeroDocumento + "%' ";

            if (filtrosPesquisa.MoedaCotacaoBancoCentral != MoedaCotacaoBancoCentral.Todas)
                sqlquery += " and M.MOV_MOEDA_COTACAO_BANCO_CENTRAL = " + filtrosPesquisa.MoedaCotacaoBancoCentral.ToString("d");

            if (filtrosPesquisa.CodigoPlanoContaSintetica > 0)
            {
                sqlContaDebito += " AND PD.PLA_PLANO in (SELECT DISTINCT PLA_PLANO FROM T_PLANO_DE_CONTA WHERE PLA_TIPO = 1 AND PLA_PLANO LIKE '" + filtrosPesquisa.PlanoContaSintetica + "%')";
                parametros.Add(new ParametroSQL("PLANO_CONTA_SINTETICA_1", filtrosPesquisa.PlanoContaSintetica + "%"));
                sqlContaCredito += " AND PC.PLA_PLANO in (SELECT DISTINCT PLA_PLANO FROM T_PLANO_DE_CONTA WHERE PLA_TIPO = 1 AND PLA_PLANO LIKE '" + filtrosPesquisa.PlanoContaSintetica + "%')"; 
                parametros.Add(new ParametroSQL("PLANO_CONTA_SINTETICA_2", filtrosPesquisa.PlanoContaSintetica + "%"));
            }

            if (filtrosPesquisa.CodigoPlanoContaContrapartida > 0)
            {
                sqlContaDebito += " AND PC.PLA_CODIGO = " + filtrosPesquisa.CodigoPlanoContaContrapartida;
                sqlContaCredito += " AND PD.PLA_CODIGO = " + filtrosPesquisa.CodigoPlanoContaContrapartida;
            }

            string query = @"SELECT M.MOV_CODIGO Codigo 
                FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MD 
                JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = MD.MOV_CODIGO 
                JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = M.PLA_CODIGO_DEBITO 
                JOIN T_PLANO_DE_CONTA PC ON PC.PLA_CODIGO = M.PLA_CODIGO_CREDITO 
                LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = M.CLI_CGCCPF 
                LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = M.GRP_CODIGO 
                LEFT OUTER JOIN T_CENTRO_RESULTADO R ON R.CRE_CODIGO = M.CRE_CODIGO 
                LEFT OUTER JOIN T_TITULO T ON T.TIT_CODIGO = M.TIT_CODIGO 
                LEFT OUTER JOIN T_CLIENTE CT ON CT.CLI_CGCCPF = T.CLI_CGCCPF 
                LEFT OUTER JOIN T_MOVIMENTO_FINANCEIRO_ENTIDADE Ent on Ent.MOV_CODIGO = M.MOV_CODIGO  
                LEFT OUTER JOIN T_FUNCIONARIO Mot on Mot.FUN_CODIGO = Ent.FUN_CODIGO 
                " + sqlColaborador +
                " where MD.MDC_TIPO = 1 " + sqlContaDebito + sqlDataDebito + sqlEmpresa + sqlDataBase + sqlGrupoPessoa + sqlPessoa + sqlCodigoMovimento + sqlCentroResultado +
                    sqlquery +

                @"UNION  
                 
                SELECT M.MOV_CODIGO Codigo 
                FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO MC 
                JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = MC.MOV_CODIGO 
                JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = M.PLA_CODIGO_DEBITO 
                JOIN T_PLANO_DE_CONTA PC ON PC.PLA_CODIGO = M.PLA_CODIGO_CREDITO 
                LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = M.CLI_CGCCPF 
                LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = M.GRP_CODIGO 
                LEFT OUTER JOIN T_CENTRO_RESULTADO R ON R.CRE_CODIGO = M.CRE_CODIGO 
                LEFT OUTER JOIN T_TITULO T ON T.TIT_CODIGO = M.TIT_CODIGO 
                LEFT OUTER JOIN T_CLIENTE CT ON CT.CLI_CGCCPF = T.CLI_CGCCPF 
                LEFT OUTER JOIN T_MOVIMENTO_FINANCEIRO_ENTIDADE Ent on Ent.MOV_CODIGO = M.MOV_CODIGO  
                LEFT OUTER JOIN T_FUNCIONARIO Mot on Mot.FUN_CODIGO = Ent.FUN_CODIGO 
                " + sqlColaborador +
                "where MC.MDC_TIPO = 2 " + sqlContaCredito + sqlDataCredito + sqlEmpresa + sqlDataBase + sqlGrupoPessoa + sqlPessoa + sqlCodigoMovimento + sqlCentroResultado +
                    sqlquery;

            query = @"SELECT COUNT(0) as CONTADOR
                FROM ( " + query + " ) AS T ";

            var sqlDinamico = new SQLDinamico(query,parametros);

            var nhQuery = sqlDinamico.CriarSQLQuery(this.SessionNHiBernate);

            return nhQuery.SetTimeout(6000).UniqueResult<int>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoMotorista> RelatorioExtratoMotoristaNovo(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoMotorista filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string sql = ObterSelectConsultaRelatorioExtratoMotorista(filtrosPesquisa, false, propriedades, propAgrupa, dirAgrupa, propOrdena, dirOrdena, inicio, limite);

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoMotorista)));

            return query.SetTimeout(60000).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoMotorista>();
        }

        public int ContarConsultaRelatorioExtratoMotoristaNovo(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoMotorista filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            string sql = ObterSelectConsultaRelatorioExtratoMotorista(filtrosPesquisa, true, propriedades, "", "", "", "", 0, 0);

            var query = this.SessionNHiBernate.CreateSQLQuery(sql);

            return query.SetTimeout(60000).UniqueResult<int>();
        }

        private string ObterSelectConsultaRelatorioExtratoMotorista(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoMotorista filtrosPesquisa, bool count, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, string propAgrupa, string dirAgrupa, string propOrdena, string dirOrdena, int inicio, int limite)
        {
            string select = string.Empty,
                   groupBy = string.Empty,
                   groupBySub = string.Empty,
                   joins = string.Empty,
                   where = string.Empty,
                   whereSub = string.Empty,
                   orderBy = string.Empty;
            List<string> selectDinamico = new List<string>();

            for (var i = propriedades.Count - 1; i >= 0; i--)
                SetarSelectRelatorioConsultaRelatorioExtratoMotorista(propriedades[i].Propriedade, propriedades[i].CodigoDinamico, ref select, ref whereSub, ref groupBy, ref joins, count, ref selectDinamico);

            SetarWhereRelatorioConsultaRelatorioExtratoMotorista(ref where, ref whereSub, ref groupBySub, ref groupBy, ref joins, filtrosPesquisa);

            if (!count)
            {
                if (!string.IsNullOrWhiteSpace(propAgrupa))
                {
                    SetarSelectRelatorioConsultaRelatorioExtratoMotorista(propAgrupa, 0, ref select, ref whereSub, ref groupBy, ref joins, count, ref selectDinamico);

                    if (select.Contains(propAgrupa))
                        orderBy = propAgrupa + " " + dirAgrupa;
                }

                if (!string.IsNullOrWhiteSpace(propOrdena))
                {
                    if (propOrdena != propAgrupa && select.Contains(propOrdena))//&& propOrdena != "Codigo")
                    {
                        orderBy += (orderBy.Length > 0 ? ", " : string.Empty) + propOrdena + " " + dirOrdena;
                        if (propOrdena.Equals("Data") && propriedades.Any(o => o.Propriedade.Equals("Codigo")))
                            orderBy += ", Codigo " + dirOrdena;
                    }
                }
            }

            // SELECT
            string query = "SELECT ";

            if (count)
                query += "DISTINCT(COUNT(0) OVER())";
            else if (select.Length > 0)
                query += select.Substring(0, select.Length - 2);

            // FROM
            query += @" FROM T_MOVIMENTO_FINANCEIRO MovimentoFinanceiro 
                        JOIN T_MOVIMENTO_FINANCEIRO_ENTIDADE MovimentoFinanceiroEntidade ON MovimentoFinanceiroEntidade.MOV_CODIGO = MovimentoFinanceiro.MOV_CODIGO AND MovimentoFinanceiroEntidade.MOE_TIPO > 0 ";

            // JOIN
            query += joins;

            // WHERE
            query += " WHERE 1 = 1" + where;

            // GROUP BY
            if (groupBy.Length > 0)
                query += " GROUP BY " + groupBy.Substring(0, groupBy.Length - 2);

            if (groupBySub.Length > 0)
            {
                groupBySub = " GROUP BY " + groupBySub.Substring(0, groupBySub.Length - 2);
                groupBySub = string.Empty;
            }

            if (whereSub.Length > 0 || groupBySub.Length > 0)
            {
                if (query.Contains(" Entrada,") || query.Contains(" Saida,"))
                {
                    query = string.Format(query, whereSub, groupBySub);
                }
                else
                    query = string.Format(query, "", "");
            }
            else
                query = string.Format(query, "", "");

            // ORDER BY
            if (orderBy.Length > 0)
                orderBy = " ORDER BY " + orderBy;
            else if (!count)
                orderBy = " ORDER BY 1 ASC";

            if (!count)
            {
                string datePatternInicial = "yyyy-MM-dd 00:00:00";
                string datePatternFinal = "yyyy-MM-dd 23:59:59";

                string sqlSaldoAnterior = " 0.0 SaldoAnterior ";
                if (filtrosPesquisa.DataInicial > DateTime.MinValue)
                {
                    string whereSaldoAnterior = string.Empty;
                    if (filtrosPesquisa.CodigoTipoMovimento > 0)
                        whereSaldoAnterior = " AND M.TIM_CODIGO = " + filtrosPesquisa.CodigoTipoMovimento;

                    sqlSaldoAnterior = $@"case when ROW_NUMBER() OVER(partition by T.CodigoMotorista { orderBy }) = 1 
			                            then ISNULL((SELECT sum(CASE MOE_TIPO WHEN 2 THEN MOV_VALOR WHEN 1 THEN MOV_VALOR * -1 ELSE 0.00 END) 
                                                     FROM T_MOVIMENTO_FINANCEIRO_ENTIDADE D 
                                                     JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = D.MOV_CODIGO
			                            WHERE MOV_DATA < '{ filtrosPesquisa.DataInicial.ToString(datePatternInicial) }' AND D.FUN_CODIGO = T.CodigoMotorista { whereSaldoAnterior }), 0.0)
                                    else 0.0 end AS SaldoAnterior ";
                }

                query = $"SELECT { string.Join(", ", selectDinamico.Select(o => "T." + o)) }, " + 
                    $"{ sqlSaldoAnterior }" +
                    $" FROM ({ query }) AS T ";

                query = $"SELECT { string.Join(", ", selectDinamico.Select(o => "TT." + o)) }, TT.SaldoAnterior, " + 
                    $@"SUM(TT.SaldoAnterior + (TT.Entrada - (TT.Saida * -1))) OVER (partition by TT.CodigoMotorista { orderBy } ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW) AS Saldo
                    FROM ({ query }) AS TT ";

                query += orderBy;
            }

            // LIMIT
            if (!count && limite > 0)
                query += " OFFSET " + inicio.ToString() + " ROWS FETCH NEXT " + limite.ToString() + " ROWS ONLY";

            return query;
        }

        private void SetarSelectRelatorioConsultaRelatorioExtratoMotorista(string propriedade, int codigoDinamico, ref string select, ref string whereSub, ref string groupBy, ref string joins, bool count, ref List<string> selectDinamico)
        {
            switch (propriedade)
            {
                case "DataFormatada":
                    if (!select.Contains(" Data, "))
                    {
                        select += "MovimentoFinanceiro.MOV_DATA Data, ";
                        selectDinamico.Add("Data");
                        groupBy += "MovimentoFinanceiro.MOV_DATA, ";
                        whereSub += " AND M.MOV_DATA = MovimentoFinanceiro.MOV_DATA ";
                    }
                    break;

                case "Observacao":
                    if (!select.Contains(" Observacao, "))
                    {
                        select += "MovimentoFinanceiro.MOV_OBSERVACAO Observacao, ";
                        selectDinamico.Add("Observacao");
                        groupBy += "MovimentoFinanceiro.MOV_OBSERVACAO, ";
                        whereSub += " AND M.MOV_OBSERVACAO = MovimentoFinanceiro.MOV_OBSERVACAO ";
                    }
                    break;

                case "TipoDocumento":
                    if (!select.Contains(" TipoDocumento, "))
                    {
                        select += @"CASE 
	                            WHEN MovimentoFinanceiro.MOV_TIPO = 1 THEN 'Manual'
	                            WHEN MovimentoFinanceiro.MOV_TIPO = 2 THEN 'Nota de Entrada' 
	                            WHEN MovimentoFinanceiro.MOV_TIPO = 3 THEN 'CT-e' 
	                            WHEN MovimentoFinanceiro.MOV_TIPO = 4 THEN 'Faturamento' 
	                            WHEN MovimentoFinanceiro.MOV_TIPO = 5 THEN 'Recibo' 
	                            WHEN MovimentoFinanceiro.MOV_TIPO = 6 THEN 'Pagamento' 
	                            WHEN MovimentoFinanceiro.MOV_TIPO = 7 THEN 'Recebimento' 
	                            WHEN MovimentoFinanceiro.MOV_TIPO = 8 THEN 'Nota de Saída' 
	                            WHEN MovimentoFinanceiro.MOV_TIPO = 9 THEN 'Outros' 
                            ELSE '' 
                            END TipoDocumento, ";
                        selectDinamico.Add("TipoDocumento");
                        groupBy += "MovimentoFinanceiro.MOV_TIPO, ";
                        whereSub += " AND M.MOV_TIPO = MovimentoFinanceiro.MOV_TIPO ";
                    }
                    break;

                case "NumeroDocumento":
                    if (!select.Contains(" NumeroDocumento, "))
                    {
                        select += "MovimentoFinanceiro.MOV_DOCUMENTO NumeroDocumento, ";
                        selectDinamico.Add("NumeroDocumento");
                        groupBy += "MovimentoFinanceiro.MOV_DOCUMENTO, ";
                        whereSub += " AND M.MOV_DOCUMENTO = MovimentoFinanceiro.MOV_DOCUMENTO ";
                    }
                    break;

                case "PlanoDebito":
                    if (!select.Contains(" PlanoDebito, "))
                    {
                        if (!joins.Contains(" PlanoContaDebito "))
                            joins += " LEFT OUTER JOIN T_PLANO_DE_CONTA PlanoContaDebito ON PlanoContaDebito.PLA_CODIGO = MovimentoFinanceiro.PLA_CODIGO_DEBITO";

                        select += "'(' + PlanoContaDebito.PLA_PLANO + ') ' + PlanoContaDebito.PLA_DESCRICAO PlanoDebito, ";
                        selectDinamico.Add("PlanoDebito");
                        groupBy += "PlanoContaDebito.PLA_PLANO, PlanoContaDebito.PLA_DESCRICAO, MovimentoFinanceiro.PLA_CODIGO_DEBITO, ";
                        whereSub += " AND M.PLA_CODIGO_DEBITO = MovimentoFinanceiro.PLA_CODIGO_DEBITO ";
                    }
                    break;

                case "PlanoCredito":
                    if (!select.Contains(" PlanoCredito, "))
                    {
                        if (!joins.Contains(" PlanoContaCredito "))
                            joins += " LEFT OUTER JOIN T_PLANO_DE_CONTA PlanoContaCredito ON PlanoContaCredito.PLA_CODIGO = MovimentoFinanceiro.PLA_CODIGO_CREDITO";

                        select += "'(' + PlanoContaCredito.PLA_PLANO + ') ' + PlanoContaCredito.PLA_DESCRICAO PlanoCredito, ";
                        selectDinamico.Add("PlanoCredito");
                        groupBy += "PlanoContaCredito.PLA_PLANO, PlanoContaCredito.PLA_DESCRICAO, MovimentoFinanceiro.PLA_CODIGO_CREDITO, ";
                        whereSub += " AND M.PLA_CODIGO_CREDITO = MovimentoFinanceiro.PLA_CODIGO_CREDITO ";
                    }
                    break;

                case "Motorista":
                    if (!select.Contains(" Motorista, "))
                    {
                        if (!joins.Contains(" Motorista "))
                            joins += " JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = MovimentoFinanceiroEntidade.FUN_CODIGO";

                        select += "Motorista.FUN_NOME + ' (' + Motorista.FUN_CPF + ')' Motorista, ";
                        if (!select.Contains("CodigoMotorista"))
                            select += "Motorista.FUN_CODIGO CodigoMotorista, ";
                        selectDinamico.Add("Motorista");
                        if (!selectDinamico.Contains("CodigoMotorista"))
                            selectDinamico.Add("CodigoMotorista");
                        groupBy += "Motorista.FUN_CODIGO, ";
                        groupBy += "Motorista.FUN_NOME, Motorista.FUN_CPF, ";
                        whereSub += " AND E.FUN_CODIGO = MovimentoFinanceiroEntidade.FUN_CODIGO ";
                    }
                    break;

                case "CodigoIntegracao":
                    if (!select.Contains(" CodigoIntegracao, "))
                    {
                        if (!joins.Contains(" Motorista "))
                            joins += " JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = MovimentoFinanceiroEntidade.FUN_CODIGO";

                        select += "Motorista.FUN_CODIGO_INTEGRACAO CodigoIntegracao, ";
                        selectDinamico.Add("CodigoIntegracao");
                        groupBy += "Motorista.FUN_CODIGO_INTEGRACAO, ";

                        whereSub += " AND E.FUN_CODIGO = MovimentoFinanceiroEntidade.FUN_CODIGO ";
                    }
                    break;

                case "Despesa":
                    if (!select.Contains(" Despesa, "))
                    {
                        if (!joins.Contains(" Produto "))
                            joins += " LEFT OUTER JOIN T_PRODUTO Produto ON Produto.PRO_CODIGO = MovimentoFinanceiroEntidade.PRO_CODIGO";

                        select += "Produto.PRO_DESCRICAO Despesa, ";
                        selectDinamico.Add("Despesa");
                        groupBy += "Produto.PRO_CODIGO, MovimentoFinanceiroEntidade.PRO_CODIGO, Produto.PRO_DESCRICAO, ";
                    }
                    break;

                case "DataUltimoAcertoFormatada":
                    if (!select.Contains(" DataUltimoAcerto, "))
                    {
                        select += "(SELECT TOP(1) A.ACV_DATA_FINAL FROM T_ACERTO_DE_VIAGEM A WHERE A.FUN_CODIGO_MOTORISTA = Motorista.FUN_CODIGO AND A.ACV_SITUACAO = 2 ORDER BY A.ACV_DATA_FINAL DESC) DataUltimoAcerto, ";
                        selectDinamico.Add("DataUltimoAcerto");
                        groupBy += "Motorista.FUN_CODIGO, ";
                    }
                    break;

                case "Entrada":
                    if (!select.Contains(" Entrada, "))
                    {
                        select += @" ISNULL((SELECT SUM(M.MOV_VALOR) 
                                        FROM T_MOVIMENTO_FINANCEIRO M 
                                        JOIN T_MOVIMENTO_FINANCEIRO_ENTIDADE E ON E.MOV_CODIGO = M.MOV_CODIGO 
                                       WHERE E.MOE_TIPO = 2 
                                         AND E.MOE_TIPO > 0
                                         {0}   
                                         AND E.FUN_CODIGO = MovimentoFinanceiroEntidade.FUN_CODIGO
                                         {1}), 0.0) Entrada, ";
                        selectDinamico.Add("Entrada");
                        groupBy += "MovimentoFinanceiroEntidade.FUN_CODIGO, ";
                    }
                    break;

                case "Saida":
                    if (!select.Contains(" Saida, "))
                    {
                        select += @" ISNULL((SELECT SUM(M.MOV_VALOR) * -1
                                        FROM T_MOVIMENTO_FINANCEIRO M 
                                        JOIN T_MOVIMENTO_FINANCEIRO_ENTIDADE E ON E.MOV_CODIGO = M.MOV_CODIGO 
                                       WHERE E.MOE_TIPO = 1 
                                         AND E.MOE_TIPO > 0 
                                         {0}
                                         AND E.FUN_CODIGO = MovimentoFinanceiroEntidade.FUN_CODIGO
                                         {1}), 0.0) Saida, ";
                        selectDinamico.Add("Saida");
                        groupBy += "MovimentoFinanceiroEntidade.FUN_CODIGO, ";
                    }
                    break;

                case "Saldo":
                    if (!select.Contains(" Saldo, "))
                    {
                        select += "0.0 Saldo, ";
                        if (!selectDinamico.Contains("CodigoMotorista"))
                        {
                            if (!joins.Contains(" Motorista "))
                                joins += " JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = MovimentoFinanceiroEntidade.FUN_CODIGO";

                            selectDinamico.Add("CodigoMotorista");
                            select += "Motorista.FUN_CODIGO CodigoMotorista, ";
                            groupBy += "Motorista.FUN_CODIGO, ";
                        }
                    }
                    break;

                case "SaldoAnterior":
                    if (!select.Contains(" SaldoAnterior, "))
                    {
                        select += "0.0 SaldoAnterior, ";
                        if (!selectDinamico.Contains("CodigoMotorista"))
                        {
                            if (!joins.Contains(" Motorista "))
                                joins += " JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = MovimentoFinanceiroEntidade.FUN_CODIGO";

                            selectDinamico.Add("CodigoMotorista");
                            select += "Motorista.FUN_CODIGO CodigoMotorista, ";
                            groupBy += "Motorista.FUN_CODIGO, ";
                        }
                    }
                    break;

                case "Codigo":
                    if (!select.Contains(" Codigo, "))
                    {
                        select += "MovimentoFinanceiro.MOV_CODIGO Codigo, ";
                        selectDinamico.Add("Codigo");
                        groupBy += "MovimentoFinanceiro.MOV_CODIGO, ";
                        whereSub += " AND M.MOV_CODIGO = MovimentoFinanceiro.MOV_CODIGO ";
                    }
                    break;

                case "TipoMovimento":
                    if (!select.Contains(" TipoMovimento, "))
                    {
                        if (!joins.Contains(" TipoMovimento "))
                            joins += " LEFT JOIN T_TIPO_MOVIMENTO TipoMovimento ON TipoMovimento.TIM_CODIGO = MovimentoFinanceiro.TIM_CODIGO";

                        select += "TipoMovimento.TIM_DESCRICAO TipoMovimento, ";
                        selectDinamico.Add("TipoMovimento");
                        groupBy += "TipoMovimento.TIM_DESCRICAO, ";
                    }
                    break;
            }
        }

        private void SetarWhereRelatorioConsultaRelatorioExtratoMotorista(ref string where, ref string whereSub, ref string groupBySub, ref string groupBy, ref string joins, Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioExtratoMotorista filtrosPesquisa)
        {
            string datePatternInicial = "yyyy-MM-dd 00:00:00";
            string datePatternFinal = "yyyy-MM-dd 23:59:59";

            if (filtrosPesquisa.Codigo > 0)
            {
                where += " AND MovimentoFinanceiro.MOV_CODIGO = " + filtrosPesquisa.Codigo.ToString();
                whereSub += " AND M.MOV_CODIGO = " + filtrosPesquisa.Codigo.ToString();

                groupBySub += "M.MOV_CODIGO, ";
            }

            if (filtrosPesquisa.CodigoTipoMovimento > 0)
            {
                where += " AND MovimentoFinanceiro.TIM_CODIGO =  " + filtrosPesquisa.CodigoTipoMovimento.ToString();
                whereSub += " AND M.TIM_CODIGO = " + filtrosPesquisa.CodigoTipoMovimento.ToString();

                groupBySub += "M.TIM_CODIGO, ";
            }

            if (filtrosPesquisa.CodigoPlanoConta > 0)
            {
                if (!joins.Contains(" PlanoContaCredito "))
                    joins += " LEFT OUTER JOIN T_PLANO_DE_CONTA PlanoContaCredito ON PlanoContaCredito.PLA_CODIGO = MovimentoFinanceiro.PLA_CODIGO_CREDITO";
                if (!joins.Contains(" PlanoContaDebito "))
                    joins += " LEFT OUTER JOIN T_PLANO_DE_CONTA PlanoContaDebito ON PlanoContaDebito.PLA_CODIGO = MovimentoFinanceiro.PLA_CODIGO_DEBITO";

                where += " AND (PlanoContaDebito.PLA_CODIGO = " + filtrosPesquisa.CodigoPlanoConta.ToString();
                where += " OR PlanoContaCredito.PLA_CODIGO = " + filtrosPesquisa.CodigoPlanoConta.ToString() + ")";
                whereSub += " AND (M.PLA_CODIGO_DEBITO = " + filtrosPesquisa.CodigoPlanoConta.ToString();
                whereSub += " OR M.PLA_CODIGO_CREDITO = " + filtrosPesquisa.CodigoPlanoConta.ToString() + ")";

                groupBySub += "M.PLA_CODIGO_DEBITO, M.PLA_CODIGO_CREDITO, ";
            }

            if (filtrosPesquisa.CodigoMotorista > 0)
            {
                if (!joins.Contains(" Motorista "))
                    joins += " JOIN T_FUNCIONARIO Motorista ON Motorista.FUN_CODIGO = MovimentoFinanceiroEntidade.FUN_CODIGO";

                where += " AND Motorista.FUN_CODIGO = " + filtrosPesquisa.CodigoMotorista.ToString();
                whereSub += " AND E.FUN_CODIGO = " + filtrosPesquisa.CodigoMotorista.ToString();

                groupBySub += "E.FUN_CODIGO, ";
            }

            if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.DataFinal > DateTime.MinValue)
            {
                where += " AND MovimentoFinanceiro.MOV_DATA >= '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "' AND MovimentoFinanceiro.MOV_DATA <= '" + filtrosPesquisa.DataFinal.ToString(datePatternFinal) + "'";
                whereSub += " AND M.MOV_DATA >= '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "' AND M.MOV_DATA <= '" + filtrosPesquisa.DataFinal.ToString(datePatternFinal) + "'";

                groupBySub += "M.MOV_DATA, ";
            }

            if (filtrosPesquisa.DataInicial > DateTime.MinValue && filtrosPesquisa.DataFinal == DateTime.MinValue)
            {
                where += " AND MovimentoFinanceiro.MOV_DATA >= '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "' ";
                whereSub += " AND M.MOV_DATA >= '" + filtrosPesquisa.DataInicial.ToString(datePatternInicial) + "' ";

                groupBySub += "M.MOV_DATA, ";
            }

            if (filtrosPesquisa.DataInicial == DateTime.MinValue && filtrosPesquisa.DataFinal > DateTime.MinValue)
            {
                where += " AND MovimentoFinanceiro.MOV_DATA <= '" + filtrosPesquisa.DataFinal.ToString(datePatternFinal) + "' ";
                whereSub += " AND M.MOV_DATA <= '" + filtrosPesquisa.DataFinal.ToString(datePatternFinal) + "' ";

                groupBySub += "M.MOV_DATA, ";
            }

            if (filtrosPesquisa.CodigoEmpresa > 0)
            {
                where += " AND MovimentoFinanceiro.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();
                whereSub += " AND M.EMP_CODIGO = " + filtrosPesquisa.CodigoEmpresa.ToString();

                groupBySub += "M.EMP_CODIGO, ";
            }
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoMotorista> RelatorioExtratoMotorista(bool gerarRelatorioAgrupado, int codigoMovimento, int codigoEmpresa, int codigoConta, DateTime dataInicial, DateTime dataFinal, string propGrupo, string dirOrdenacaoGrupo, string propOrdenacao, string dirOrdenacao, int inicioRegistros, int maximoRegistros, bool paginar = true, bool todosCNPJdaRaizEmbarcador = false, int codigoColaborador = 0)
        {
            string sqlData = "";
            string sqlContaDebito = "";
            string sqlContaCredito = "";
            string sqlColaborador = "";
            string sqlCodigoMovimento = "";

            if (codigoMovimento > 0)
            {
                sqlCodigoMovimento = " AND M.MOV_CODIGO = " + codigoMovimento.ToString();
            }

            if (codigoConta > 0)
            {
                sqlContaDebito = " AND PD.PLA_CODIGO = " + Convert.ToString(codigoConta);
                sqlContaCredito = " AND PC.PLA_CODIGO = " + Convert.ToString(codigoConta);
            }

            if (codigoColaborador > 0)
            {
                sqlColaborador = " AND F.FUN_CODIGO = " + codigoColaborador.ToString();
            }

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
            {
                sqlData = " AND M.MOV_DATA >= '" + dataInicial.ToString("MM/dd/yyyy") + "' AND M.MOV_DATA <= '" + dataFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
            }

            if (dataInicial > DateTime.MinValue && dataFinal == DateTime.MinValue)
            {
                sqlData = " AND M.MOV_DATA >= '" + dataInicial.ToString("MM/dd/yyyy") + "' ";
            }

            if (dataInicial == DateTime.MinValue && dataFinal > DateTime.MinValue)
            {
                sqlData = " AND M.MOV_DATA <= '" + dataFinal.ToString("MM/dd/yyyy") + "' ";
            }

            string sqlEmpresa = "";
            if (codigoEmpresa > 0)
                sqlEmpresa = " AND (M.EMP_CODIGO = " + codigoEmpresa.ToString() + ")";

            string query = @"select M.MOV_CODIGO Codigo,
                M.MOV_DATA Data,
                M.MOV_OBSERVACAO Observacao,
                CASE 
	                WHEN M.MOV_TIPO = 1 THEN 'Manual'
	                WHEN M.MOV_TIPO = 2 THEN 'Nota de Entrada' 
	                WHEN M.MOV_TIPO = 3 THEN 'CT-e' 
	                WHEN M.MOV_TIPO = 4 THEN 'Faturamento' 
	                WHEN M.MOV_TIPO = 5 THEN 'Recibo' 
	                WHEN M.MOV_TIPO = 6 THEN 'Pagamento' 
	                WHEN M.MOV_TIPO = 7 THEN 'Recebimento' 
	                WHEN M.MOV_TIPO = 8 THEN 'Nota de Saída' 
	                WHEN M.MOV_TIPO = 9 THEN 'Outros' 
                ELSE '' 
                END TipoDocumento,
                M.MOV_DOCUMENTO NumeroDocumento,
                '(' + PD.PLA_PLANO + ') ' + PD.PLA_DESCRICAO PlanoDebito,
                '(' + PC.PLA_PLANO + ') ' + PC.PLA_DESCRICAO PlanoCredito,
                F.FUN_NOME + ' (' + F.FUN_CPF + ')' Motorista,
                F.FUN_CODIGO CodigoMotorista,
                CASE
					WHEN ME.MOE_TIPO = 1 THEN 0
					ELSE M.MOV_VALOR
				END Entrada, 
                CASE
					WHEN ME.MOE_TIPO = 1 THEN M.MOV_VALOR * -1
					ELSE 0
				END Saida, 
                0.0 Saldo, 
                0.0 SaldoAnterior
                from  T_MOVIMENTO_FINANCEIRO M                
                JOIN T_MOVIMENTO_FINANCEIRO_ENTIDADE ME ON ME.MOV_CODIGO = M.MOV_CODIGO AND ME.MOE_TIPO > 0
                JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = ME.FUN_CODIGO
                LEFT OUTER JOIN T_PLANO_DE_CONTA PC ON PC.PLA_CODIGO = M.PLA_CODIGO_CREDITO
                LEFT OUTER JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = M.PLA_CODIGO_DEBITO
                WHERE 1 = 1 " + sqlData + sqlContaDebito + sqlContaCredito + sqlColaborador + sqlCodigoMovimento;

            if (gerarRelatorioAgrupado)
            {
                query = @"select min(T.Codigo) Codigo,
                T.Data,
                T.Observacao,
                max(T.TipoDocumento) TipoDocumento,
                T.NumeroDocumento,
                T.PlanoDebito,
                T.PlanoCredito,
                T.Motorista,
                T.CodigoMotorista,
                sum(T.Entrada) Entrada, sum(T.Saida) Saida, sum(T.Saldo) Saldo, sum(T.SaldoAnterior) SaldoAnterior from ( " + query + " ) AS T  " +
                " group by t.PlanoDebito, t.PlanoCredito, t.Motorista, T.CodigoMotorista, T.NumeroDocumento, T.Observacao, T.Data";
            }
            else
            {
                query = @"SELECT Codigo,
                Data,
                Observacao,
                TipoDocumento,
                NumeroDocumento,
                PlanoDebito,
                PlanoCredito,
                Motorista,
                Entrada, Saida, Saldo, SaldoAnterior, CodigoMotorista FROM ( " + query + " ) AS T ";
            }


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

            if (paginar)
                query += " OFFSET " + inicioRegistros + " ROWS FETCH FIRST " + maximoRegistros + " ROWS ONLY";


            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoMotorista)));

            return nhQuery.SetTimeout(50000).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ExtratoMotorista>();
        }

        public int ContarRelatorioExtratoMotorista(bool gerarRelatorioAgrupado, int codigoMovimento, int codigoEmpresa, int codigoConta, DateTime dataInicial, DateTime dataFinal, int codigoColaborador)
        {
            string sqlData = "";
            string sqlContaDebito = "";
            string sqlContaCredito = "";
            string sqlColaborador = "";
            string sqlCodigoMovimento = "";

            if (codigoMovimento > 0)
            {
                sqlCodigoMovimento = " AND M.MOV_CODIGO = " + codigoMovimento.ToString();
            }

            if (codigoConta > 0)
            {
                sqlContaDebito = " AND PD.PLA_CODIGO = " + Convert.ToString(codigoConta);
                sqlContaCredito = " AND PC.PLA_CODIGO = " + Convert.ToString(codigoConta);
            }

            if (codigoColaborador > 0)
            {
                sqlColaborador = " AND F.FUN_CODIGO = " + codigoColaborador.ToString();
            }

            if (dataInicial > DateTime.MinValue && dataFinal > DateTime.MinValue)
            {
                sqlData = " AND M.MOV_DATA >= '" + dataInicial.ToString("MM/dd/yyyy") + "' AND M.MOV_DATA <= '" + dataFinal.ToString("MM/dd/yyyy 23:59:59") + "'";
            }

            if (dataInicial > DateTime.MinValue && dataFinal == DateTime.MinValue)
            {
                sqlData = " AND M.MOV_DATA >= '" + dataInicial.ToString("MM/dd/yyyy") + "' ";
            }

            if (dataInicial == DateTime.MinValue && dataFinal > DateTime.MinValue)
            {
                sqlData = " AND M.MOV_DATA <= '" + dataFinal.ToString("MM/dd/yyyy") + "' ";
            }
            string sqlEmpresa = "";
            if (codigoEmpresa > 0)
                sqlEmpresa = " AND (M.EMP_CODIGO = " + codigoEmpresa.ToString() + ")";

            string query = @"select M.MOV_CODIGO Codigo,
                M.MOV_DATA Data,
                M.MOV_OBSERVACAO Observacao,
                CASE 
	                WHEN M.MOV_TIPO = 1 THEN 'Manual'
	                WHEN M.MOV_TIPO = 2 THEN 'Nota de Entrada' 
	                WHEN M.MOV_TIPO = 3 THEN 'CT-e' 
	                WHEN M.MOV_TIPO = 4 THEN 'Faturamento' 
	                WHEN M.MOV_TIPO = 5 THEN 'Recibo' 
	                WHEN M.MOV_TIPO = 6 THEN 'Pagamento' 
	                WHEN M.MOV_TIPO = 7 THEN 'Recebimento' 
	                WHEN M.MOV_TIPO = 8 THEN 'Nota de Saída' 
	                WHEN M.MOV_TIPO = 9 THEN 'Outros' 
                ELSE '' 
                END TipoDocumento,
                M.MOV_DOCUMENTO NumeroDocumento,
                '(' + PD.PLA_PLANO + ') ' + PD.PLA_DESCRICAO PlanoDebito,
                '(' + PC.PLA_PLANO + ') ' + PC.PLA_DESCRICAO PlanoCredito,
                F.FUN_NOME + ' (' + F.FUN_CPF + ')' Motorista, 
                F.FUN_CODIGO CodigoMotorista,
                CASE
					WHEN ME.MOE_TIPO = 1 THEN 0
					ELSE M.MOV_VALOR
				END Entrada, 
                CASE
					WHEN ME.MOE_TIPO = 1 THEN M.MOV_VALOR * -1
					ELSE 0
				END Saida, 
                0.0 Saldo 
                from  T_MOVIMENTO_FINANCEIRO M                
                JOIN T_MOVIMENTO_FINANCEIRO_ENTIDADE ME ON ME.MOV_CODIGO = M.MOV_CODIGO AND ME.MOE_TIPO > 0
                JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = ME.FUN_CODIGO
                LEFT OUTER JOIN T_PLANO_DE_CONTA PC ON PC.PLA_CODIGO = M.PLA_CODIGO_CREDITO
                LEFT OUTER JOIN T_PLANO_DE_CONTA PD ON PD.PLA_CODIGO = M.PLA_CODIGO_DEBITO             
                WHERE 1 = 1 " + sqlData + sqlContaDebito + sqlContaCredito + sqlColaborador + sqlCodigoMovimento;

            if (gerarRelatorioAgrupado)
            {
                query = @"select min(T.Codigo) Codigo,
                T.Data,
                T.Observacao,
                max(T.TipoDocumento) TipoDocumento,
                T.NumeroDocumento,
                T.PlanoDebito,
                T.PlanoCredito,
                T.Motorista, T.CodigoMotorista,
                sum(T.Entrada) Entrada, sum(T.Saida) Saida, sum(T.Saldo) Saldo from ( " + query + " ) AS T  " +
                    " group by t.PlanoDebito, t.PlanoCredito, t.Motorista, T.NumeroDocumento, T.Observacao, T.Data, T.CodigoMotorista";
            }

            query = "SELECT COUNT(0) as CONTADOR " +
                " FROM ( " + query + " ) AS TT ";

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            return nhQuery.SetTimeout(50000).UniqueResult<int>();
        }

        public IList<Dominio.ObjetosDeValor.Embarcador.Financeiro.GraficoFaturamento> BuscarGraficoFaturamento(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente)
        {
            string queryParameters = "", queryParameters2 = "";
            if ((int)tipoAmbiente > 0)
            {
                queryParameters = " AND T.TIT_AMBIENTE = " + (int)tipoAmbiente;
                queryParameters2 = " AND M.MOV_AMBIENTE = " + (int)tipoAmbiente;
            }

            var sqlQuery = @"SELECT 1 Tipo,
                ISNULL(sum(xx.saldo_final), 0) AS Valor, 0 Quantidade
                FROM
                (
                SELECT x.PLA_CODIGO
	                , x.PLA_PLANO
                    , coalesce((SELECT sum(CASE MDC_TIPO WHEN 1 THEN MDC_VALOR WHEN 2 THEN MDC_VALOR * -1 ELSE 0.00 END) 
                                FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO mdc
                                JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = mdc.MOV_CODIGO
                                WHERE PLA_CODIGO = x.PLA_CODIGO " + queryParameters2 + @"),0) AS saldo_inicial 
                    , sum(x.entrada) AS entrada
                    , sum(x.saida) AS saida
                    , coalesce((SELECT sum(CASE MDC_TIPO WHEN 1 THEN MDC_VALOR WHEN 2 THEN MDC_VALOR * -1 ELSE 0.00 END) 
                                FROM T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO mdc
                                JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = mdc.MOV_CODIGO
                                WHERE PLA_CODIGO = x.PLA_CODIGO " + queryParameters2 + @"),0) AS saldo_final
                    FROM
                        ( 
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
                            JOIN T_MOVIMENTO_FINANCEIRO_DEBITO_CREDITO mdc ON mdc.PLA_CODIGO = planoConta.PLA_CODIGO 
                            JOIN T_MOVIMENTO_FINANCEIRO M ON M.MOV_CODIGO = mdc.MOV_CODIGO
                            WHERE mdc.PLA_CODIGO IN (SELECT T.PLA_CODIGO FROM T_TIPO_PAGAMENTO_RECEBIMENTO T WHERE T.TPR_ATIVO = 1 AND T.EMP_CODIGO = " + codigoEmpresa.ToString() + @") " + queryParameters2 + @"
                        ) AS x
                GROUP BY x.PLA_CODIGO, x.PLA_PLANO
                ) AS xx
                JOIN T_PLANO_DE_CONTA pc
                ON xx.PLA_CODIGO = pc.PLA_CODIGO 
                AND PC.EMP_CODIGO = " + codigoEmpresa.ToString() + @"

                UNION ALL

                SELECT 2 Tipo, ISNULL(SUM(T.TIT_VALOR_ORIGINAL), 0) Valor, 0 Quantidade
                FROM T_TITULO T
                WHERE T.TIT_TIPO = 1 AND T.TIT_STATUS <> 4
                AND T.EMP_CODIGO = " + codigoEmpresa.ToString() + queryParameters + @" 
                AND T.TIT_DATA_EMISSAO >= '" + new DateTime(DateTime.Now.Year, 1, 1).Date.ToString("yyyy-MM-dd") + @"'
                AND T.TIT_DATA_EMISSAO < '" + DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd") + @"'

                UNION ALL

                SELECT 3 Tipo, ISNULL(SUM(T.TIT_VALOR_ORIGINAL), 0) Valor, 0 Quantidade
                FROM T_TITULO T
                WHERE T.TIT_TIPO = 1 AND T.TIT_STATUS = 1
                AND T.EMP_CODIGO = " + codigoEmpresa.ToString() + queryParameters + @"

                UNION ALL

                SELECT 4 Tipo, ISNULL(SUM(T.TIT_VALOR_ORIGINAL), 0) Valor, 0 Quantidade
                FROM T_TITULO T
                WHERE T.TIT_TIPO = 2 AND T.TIT_STATUS = 1
                AND T.EMP_CODIGO = " + codigoEmpresa.ToString() + queryParameters + @"

                UNION ALL

                SELECT 5 Tipo, ISNULL(COUNT(T.TIT_CODIGO), 0) Valor, (SELECT ISNULL(COUNT(T.TIT_CODIGO), 0)
                FROM T_TITULO T
                WHERE T.TIT_TIPO = 1 AND T.TIT_STATUS = 1
                AND T.EMP_CODIGO = " + codigoEmpresa.ToString() + queryParameters + @") Quantidade
                FROM T_TITULO T
                WHERE T.TIT_TIPO = 1 AND T.TIT_STATUS = 1 AND T.TIT_DATA_VENCIMENTO < '" + DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-dd") + @"'
                AND T.EMP_CODIGO = " + codigoEmpresa.ToString() + queryParameters + @"

				UNION ALL

                SELECT 6 Tipo, ISNULL(COUNT(T.TIT_CODIGO), 0) Valor, (SELECT ISNULL(COUNT(T.TIT_CODIGO), 0)
                FROM T_TITULO T
                WHERE T.TIT_TIPO = 2 AND T.TIT_STATUS <> 4
                AND T.EMP_CODIGO = " + codigoEmpresa.ToString() + queryParameters + @") Quantidade
                FROM T_TITULO T
                WHERE T.TIT_TIPO = 2 AND T.TIT_STATUS = 1 
                AND T.EMP_CODIGO = " + codigoEmpresa.ToString() + queryParameters;

            var query = this.SessionNHiBernate.CreateSQLQuery(sqlQuery);

            query.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.ObjetosDeValor.Embarcador.Financeiro.GraficoFaturamento)));

            return query.List<Dominio.ObjetosDeValor.Embarcador.Financeiro.GraficoFaturamento>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro> RelatorioReciboPagamentoMotorista(int codigo, int codigoEmpresa, bool utilizarEmpresaFilialImpressaoReciboPagamentoMotorista)
        {
            string query = @"select 1 Via, PAM_DATA_PAGAMENTO Data, PAM_VALOR ValorTotal, PAM_OBSERVACAO + ' ' + T.PMT_DESCRICAO Observacao, CAST(PAM_NUMERO AS varchar(100)) Documento,
                FM.FUN_NOME Pessoa,
                FM.FUN_CPF CNPJPessoa, 
                'F' TipoPessoa, ";

            if (utilizarEmpresaFilialImpressaoReciboPagamentoMotorista)
            {
                query += @"COALESCE(EMP.EMP_CNPJ,F.FUN_CPF) CNPJEmpresa," +
                        "COALESCE(EMP.EMP_RAZAO,F.FUN_NOME) NomeEmpresa,";
            }
            else
            {
                query += @"F.FUN_CPF CNPJEmpresa," +
                        "F.FUN_NOME NomeEmpresa,";
            }

            query += @" 'F' TipoEmpresa,
                'Pagamento ao Motorista' TipoDocumento,
                M.PAM_VALOR ValorPago,
                1 Parcela,
                0.0 Acrescimo,
                0.0 Desconto,
                FM.FUN_NUMERO_CARTAO NumeroCartao
                from T_PAGAMENTO_MOTORISTA_TMS M
				JOIN T_PAGAMENTO_MOTORISTA_TIPO T ON T.PMT_CODIGO = M.PMT_CODIGO
                JOIN T_FUNCIONARIO FM ON FM.FUN_CODIGO = M.FUN_CODIGO_MOTORISTA                
				JOIN T_FUNCIONARIO F ON F.FUN_CODIGO = M.FUN_CODIGO
                LEFT JOIN T_EMPRESA EMP ON EMP.EMP_CODIGO = M.EMP_CODIGO
                WHERE M.PAM_CODIGO = " + codigo.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro>();
        }


        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro> RelatorioReciboMovimento(int codigo, int codigoEmpresa)
        {
            string query = @"select 1 Via, MOV_DATA Data, MOV_VALOR ValorTotal, MOV_OBSERVACAO Observacao, MOV_DOCUMENTO Documento,
                ISNULL(C.CLI_NOME, G.GRP_DESCRICAO) Pessoa,
                ISNULL(STR(C.CLI_CGCCPF, 14, 0), CAST(G.GRP_CNPJ AS VARCHAR(30))) CNPJPessoa,
                C.CLI_FISJUR TipoPessoa,
                E.EMP_CNPJ CNPJEmpresa, E.EMP_RAZAO NomeEmpresa, E.EMP_TIPO TipoEmpresa,
                LEMP.LOC_DESCRICAO CidadeEmpresa, LEMP.UF_SIGLA EstadoEmpresa,
                CASE 
	                WHEN M.MOV_TIPO = 1 THEN 'Manual'
	                WHEN M.MOV_TIPO = 2 THEN 'Nota de Entrada' 
	                WHEN M.MOV_TIPO = 3 THEN 'CT-e' 
	                WHEN M.MOV_TIPO = 4 THEN 'Faturamento' 
	                WHEN M.MOV_TIPO = 5 THEN 'Recibo' 
	                WHEN M.MOV_TIPO = 6 THEN 'Pagamento' 
	                WHEN M.MOV_TIPO = 7 THEN 'Recebimento' 
	                WHEN M.MOV_TIPO = 8 THEN 'Nota de Saída' 
	                WHEN M.MOV_TIPO = 9 THEN 'Outros' 
	                WHEN M.MOV_TIPO = 10 THEN 'Acerto' 
                    WHEN M.MOV_TIPO = 11 THEN 'Contrato de Free' 
                    WHEN M.MOV_TIPO = 12 THEN 'Adiantamento Motorista' 
                    WHEN M.MOV_TIPO = 13 THEN 'Carga' 
                ELSE '' 
                END TipoDocumento,
                M.MOV_VALOR ValorPago,
                1 Parcela,
                0.0 Acrescimo,
                0.0 Desconto
                from T_MOVIMENTO_FINANCEIRO M
                LEFT OUTER JOIN T_CLIENTE C ON C.CLI_CGCCPF = M.CLI_CGCCPF
                LEFT OUTER JOIN T_GRUPO_PESSOAS G ON G.GRP_CODIGO = M.GRP_CODIGO,
                T_EMPRESA E
                LEFT OUTER JOIN T_LOCALIDADES LEMP ON LEMP.LOC_CODIGO = E.LOC_CODIGO
                WHERE E.EMP_CODIGO = " + codigoEmpresa.ToString() + @"
                AND M.MOV_CODIGO = " + codigo.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro> RelatorioReciboBaixa(int codigo, int codigoEmpresa)
        {
            string query = @"select 1 Via, TIB_DATA_BAIXA Data, TIB_VALOR ValorTotal, TIB_OBSERVACAO Observacao, T.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL Documento,
                C.CLI_NOME Pessoa,
                STR(C.CLI_CGCCPF, 14, 0) CNPJPessoa,
                C.CLI_FISJUR TipoPessoa,
                E.EMP_CNPJ CNPJEmpresa, E.EMP_RAZAO NomeEmpresa, E.EMP_TIPO TipoEmpresa,
                T.TIT_TIPO_DOCUMENTO_TITULO_ORIGINAL TipoDocumento,
                T.TIT_VALOR_PAGO ValorPago,
                T.TIT_SEQUENCIA Parcela,
                T.TIT_VALOR_ACRESCIMO Acrescimo,
                T.TIT_VALOR_DESCONTO Desconto,
                SUBSTRING((SELECT DISTINCT ', ' + TipoPagamentoRecebimento.TPR_DESCRICAO
                    FROM T_TITULO_BAIXA_TIPO_PAGAMENTO_RECEBIMENTO TituloBaixaTipo
					JOIN T_TITULO_BAIXA TituloBaixa ON TituloBaixa.TIB_CODIGO = TituloBaixaTipo.TIB_CODIGO
                    JOIN T_TIPO_PAGAMENTO_RECEBIMENTO TipoPagamentoRecebimento ON TipoPagamentoRecebimento.TPR_CODIGO = TituloBaixaTipo.TPR_CODIGO
                    WHERE TituloBaixa.TIB_CODIGO = B.TIB_CODIGO FOR XML PATH('')), 3, 1000) TipoPagamentos,
                (select count(titulos.TIB_CODIGO) FROM T_TITULO_BAIXA tituloBaixa
			        inner join T_TITULO_BAIXA_AGRUPADO titulos ON titulos.TIB_CODIGO = tituloBaixa.TIB_CODIGO
			        inner join T_TITULO tt on tt.TIT_CODIGO = titulos.TIT_CODIGO
			        inner join T_CLIENTE c on c.CLI_CGCCPF = tt.CLI_CGCCPF
			        WHERE tituloBaixa.TIB_CODIGO = B.TIB_CODIGO) FornecedoresCount
                from T_TITULO_BAIXA B
                JOIN T_TITULO_BAIXA_AGRUPADO A ON A.TIB_CODIGO = B.TIB_CODIGO
                JOIN T_TITULO T ON T.TIT_CODIGO = A.TIT_CODIGO
                JOIN T_CLIENTE C ON C.CLI_CGCCPF = T.CLI_CGCCPF,
                T_EMPRESA E
                WHERE E.EMP_CODIGO = " + codigoEmpresa.ToString() + @"
                AND B.TIB_CODIGO = " + codigo.ToString();

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro)));

            return nhQuery.List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ReciboFinanceiro>();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.MovimentoFinanceiro> ConsultarRelatorio(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioMovimentoFinanceiro filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta)
        {
            var consulta = new ConsultaMovimentoFinanceiro().ObterSqlPesquisa(filtrosPesquisa, parametrosConsulta, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            consulta.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.MovimentoFinanceiro)));

            return consulta.SetTimeout(600).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.MovimentoFinanceiro>();
        }

        public int ContarConsultaRelatorio(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioMovimentoFinanceiro filtrosPesquisa, List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades)
        {
            var consulta = new ConsultaMovimentoFinanceiro().ObterSqlContarPesquisa(filtrosPesquisa, propriedades).CriarSQLQuery(this.SessionNHiBernate);

            return consulta.SetTimeout(600).UniqueResult<int>();
        }

        #endregion

        #region Arquivos Contábeis

        public List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro> BuscarDadosParaArquivoContabil(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro>();

            var result = from obj in query select obj;
            result = result.Where(obj => obj.DataMovimento.Date >= dataInicial.Date && obj.DataMovimento.Date <= dataFinal.Date.AddDays(1));
            //result = result.Where(obj => !string.IsNullOrWhiteSpace(obj.PlanoDeContaDebito.PlanoContabilidade) && !string.IsNullOrWhiteSpace(obj.PlanoDeContaCredito.PlanoContabilidade));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if ((int)tipoAmbiente > 0)
                result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);

            return result.ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.MovimentoFinanceiroArquivoContabil> BuscarDadosParaArquivoContabilEuro(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, DateTime dataInicial, DateTime dataFinal, TipoMovimentoArquivoContabilEuro tipoMovimento, int codigoTipoMovimentoArquivoContabil, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            string queryUnion = @"select distinct  M.MOV_CODIGO Codigo
                  ,M.MOV_DATA Data
                  ,M.MOV_VALOR Valor
                  ,M.MOV_DOCUMENTO Documento
                  ,M.MOV_TIPO TipoDocumentoMovimento
                  ,M.MOV_OBSERVACAO Observacao
                  ,M.TIM_CODIGO CodigoTipoMovimento
                  ,M.PLA_CODIGO_DEBITO CodigoContaDebito
                  ,M.PLA_CODIGO_CREDITO CodigoContaCredito
                  ,M.CRE_CODIGO CodigoCentroResultado
                  ,M.EMP_CODIGO CodigoEmpresa
                  ,M.FUN_CODIGO CodigoFuncionario
                  ,M.MOV_TIPO_GERACAO TipoGeracaoMovimento
                  ,M.MOV_DATA_GERACAO DataGeracao
                  ,M.TIT_CODIGO CodigoTitulo
                  ,M.MOV_DATA_BASE DataBaseSistema
                  ,M.GRP_CODIGO CodigoGrupo
                  ,M.CLI_CGCCPF CNPJPessoa
                  ,M.MOV_AMBIENTE TipoAmbiente
	              ,debito.PLA_PLANO PlanoDeContaDebito
	              ,debito.PLA_PLANO_CONTABILIDADE PlanoDeContaDebitoContabil
	              ,credito.PLA_PLANO PlanoDeContaCredito
	              ,credito.PLA_PLANO_CONTABILIDADE PlanoDeContaCreditoContabil
	              ,null CNPJPessoaTitulo
	              ,null NomePessoa
                  ,titulo.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL NumeroDocumento
                  ,(SELECT TIM_CODIGO_HISTORICO FROM T_TIPO_MOVIMENTO TP WHERE TP.TIM_CODIGO = M.TIM_CODIGO) CodigoHistoricoMovimentoFinanceiro
            from t_movimento_financeiro m
            join T_PLANO_DE_CONTA debito on debito.PLA_CODIGO = m.PLA_CODIGO_DEBITO
            join T_PLANO_DE_CONTA credito on credito.PLA_CODIGO = m.PLA_CODIGO_CREDITO
            left outer join T_TITULO titulo on titulo.TIT_CODIGO = m.TIT_CODIGO";

            string query = @"select distinct  M.MOV_CODIGO Codigo
                  ,M.MOV_DATA Data
                  ,M.MOV_VALOR Valor
                  ,M.MOV_DOCUMENTO Documento
                  ,M.MOV_TIPO TipoDocumentoMovimento
                  ,M.MOV_OBSERVACAO Observacao
                  ,M.TIM_CODIGO CodigoTipoMovimento
                  ,M.PLA_CODIGO_DEBITO CodigoContaDebito
                  ,M.PLA_CODIGO_CREDITO CodigoContaCredito
                  ,M.CRE_CODIGO CodigoCentroResultado
                  ,M.EMP_CODIGO CodigoEmpresa
                  ,M.FUN_CODIGO CodigoFuncionario
                  ,M.MOV_TIPO_GERACAO TipoGeracaoMovimento
                  ,M.MOV_DATA_GERACAO DataGeracao
                  ,M.TIT_CODIGO CodigoTitulo
                  ,M.MOV_DATA_BASE DataBaseSistema
                  ,M.GRP_CODIGO CodigoGrupo
                  ,M.CLI_CGCCPF CNPJPessoa
                  ,M.MOV_AMBIENTE TipoAmbiente
                  ,debito.PLA_PLANO PlanoDeContaDebito
	              ,debito.PLA_PLANO_CONTABILIDADE PlanoDeContaDebitoContabil
	              ,credito.PLA_PLANO PlanoDeContaCredito
	              ,credito.PLA_PLANO_CONTABILIDADE PlanoDeContaCreditoContabil
	              ,c.CLI_CGCCPF CNPJPessoaTitulo
	              ,c.CLI_NOME NomePessoa
                  ,t.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL NumeroDocumento
                  ,(SELECT TIM_CODIGO_HISTORICO FROM T_TIPO_MOVIMENTO TP WHERE TP.TIM_CODIGO = M.TIM_CODIGO) CodigoHistoricoMovimentoFinanceiro
            from t_titulo_baixa b
            join t_titulo_baixa_agrupado a on b.tib_codigo = a.tib_codigo
            join t_titulo t on t.tit_codigo = a.tit_codigo
            join t_movimento_financeiro m on (m.mov_documento = ";

            if (tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasReceber)
            {
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    queryUnion += " and titulo.FAP_CODIGO IS NULL and titulo.tit_tipo_documento_titulo_original not like '%CT-e%' and titulo.tit_tipo_documento_titulo_original not like '%NFS%' and titulo.tit_tipo_documento_titulo_original not like '%Fatura%'";
                }
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                if (tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasPagarNotaFiscal || tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasPagar ||
                    tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasReceberNotaFiscal || tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasReceber)
                    query += " cast(b.tib_codigo as varchar(20))) and t.CLI_CGCCPF = m.CLI_CGCCPF";
                else
                    query += " cast(t.tit_codigo as varchar(20)) or m.TIT_CODIGO = t.TIT_CODIGO) and t.CLI_CGCCPF = m.CLI_CGCCPF";
            }
            else
            {
                if (tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasPagarNotaFiscal || tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasPagar)
                    query += " cast(b.tib_codigo as varchar(20))) and t.CLI_CGCCPF = m.CLI_CGCCPF and m.TIT_CODIGO = t.TIT_CODIGO";
                else
                    query += " cast(t.tit_codigo as varchar(20)) or m.TIT_CODIGO = t.TIT_CODIGO) and t.CLI_CGCCPF = m.CLI_CGCCPF";
            }


            query += @" join T_PLANO_DE_CONTA debito on debito.PLA_CODIGO = m.PLA_CODIGO_DEBITO
                        join T_PLANO_DE_CONTA credito on credito.PLA_CODIGO = m.PLA_CODIGO_CREDITO
                        join T_CLIENTE c on c.CLI_CGCCPF = t.CLI_CGCCPF";

            if (tipoMovimento == TipoMovimentoArquivoContabilEuro.PagamentoMotorista)
                queryUnion += " JOIN T_PAGAMENTO_MOTORISTA_TMS PA on cast(PA.PAM_NUMERO as varchar(20)) = M.MOV_DOCUMENTO AND M.MOV_OBSERVACAO LIKE '%PAGAMENTO MOTORISTA%'";

            string datePattern = "yyyy-MM-dd";

            query += $" WHERE (CAST(M.MOV_DATA AS DATE) BETWEEN '{ dataInicial.ToString(datePattern) } 00:00:00' AND '{ dataFinal.ToString(datePattern) } 23:59:59')";
            queryUnion += $" WHERE (CAST(M.MOV_DATA AS DATE) BETWEEN '{ dataInicial.ToString(datePattern) } 00:00:00' AND '{ dataFinal.ToString(datePattern) } 23:59:59')";

            if (codigoEmpresa > 0)
            {
                query += " AND M.EMP_CODIGO = " + codigoEmpresa.ToString();
                queryUnion += " AND M.EMP_CODIGO = " + codigoEmpresa.ToString();
            }
            if (tipoAmbiente != Dominio.Enumeradores.TipoAmbiente.Nenhum)
            {
                query += " AND M.MOV_AMBIENTE = " + tipoAmbiente.ToString("d");
                queryUnion += " AND M.MOV_AMBIENTE = " + tipoAmbiente.ToString("d");
            }

            if (codigoTipoMovimentoArquivoContabil > 0)
                query += $" AND M.TIM_CODIGO IN (SELECT TIM_CODIGO FROM T_TIPO_MOVIMENTO_ARQUIVO_CONTABIL_TIPOS WHERE TAC_CODIGO = { codigoTipoMovimentoArquivoContabil })"; // SQL-INJECTION-SAFE
            else if (tipoMovimento == TipoMovimentoArquivoContabilEuro.DespesasAcertoViagem)
            {
                queryUnion += @" AND ((M.MOV_TIPO = 10 AND (M.MOV_OBSERVACAO LIKE '%DESPESAS%' OR M.MOV_OBSERVACAO LIKE '%DIARIA%' OR M.MOV_OBSERVACAO LIKE '%DIÁRIA%') AND M.MOV_OBSERVACAO NOT LIKE '%PAGAMENTO DE DIARIA%') 
                                       OR (M.MOV_TIPO = 6 AND M.MOV_OBSERVACAO LIKE '%PAGAMENTO MOTORISTA ADIANTAMENTO%')) ";
            }
            else if (tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasPagarNotaFiscal)
                query += @" and t.TIT_TIPO = 2 and m.mov_tipo = 6 and T.TDD_CODIGO IS NOT NULL";
            else if (tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasPagar)
            {
                query += @" and t.TIT_TIPO = 2 and m.mov_tipo = 6
                    and m.mov_observacao not like '%PAGAMENTO MOTORISTA ADIANTAMENTO%' 
                    and T.TDD_CODIGO IS NULL";
                queryUnion += " AND m.PLA_CODIGO_CREDITO in (select t.PLA_CODIGO from T_TIPO_PAGAMENTO_RECEBIMENTO t) and m.mov_tipo <> 7 and m.mov_tipo <> 6 AND m.mov_tipo <> 3 AND m.mov_tipo <> 2";
            }
            else if (tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasReceberNotaFiscal)
            {
                query += " and t.TIT_TIPO = 1 and m.mov_tipo = 7 ";
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    query += " and (T.FAP_CODIGO IS NOT NULL OR t.tit_tipo_documento_titulo_original like '%CT-e%' OR t.tit_tipo_documento_titulo_original like '%NFS%' OR t.tit_tipo_documento_titulo_original like '%Fatura%')";
            }
            else if (tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasReceber)
            {
                query += " and t.TIT_TIPO = 1 and m.mov_tipo = 7 ";
                queryUnion += " AND m.PLA_CODIGO_DEBITO in (select t.PLA_CODIGO from T_TIPO_PAGAMENTO_RECEBIMENTO t) and m.mov_tipo <> 6 AND m.mov_tipo <> 3 AND m.mov_tipo <> 2 and m.mov_observacao not like 'PAGAMENTO%' and m.mov_observacao not like '%baixa do título%'";

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    query += " and T.FAP_CODIGO IS NULL and t.tit_tipo_documento_titulo_original not like '%CT-e%' and t.tit_tipo_documento_titulo_original not like '%NFS%' and t.tit_tipo_documento_titulo_original not like '%Fatura%'";
                    //queryUnion += " and titulo.FAP_CODIGO IS NULL and titulo.tit_tipo_documento_titulo_original not like '%CT-e%' and titulo.tit_tipo_documento_titulo_original not like '%NFS%' and titulo.tit_tipo_documento_titulo_original not like '%Fatura%'";
                }
            }

            if (tipoMovimento == TipoMovimentoArquivoContabilEuro.DespesasAcertoViagem || tipoMovimento == TipoMovimentoArquivoContabilEuro.PagamentoMotorista)
                query = queryUnion;
            else if (tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasPagar || tipoMovimento == TipoMovimentoArquivoContabilEuro.ContasReceber)
                query = query + " UNION ALL " + queryUnion;

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.MovimentoFinanceiroArquivoContabil)));

            return nhQuery.SetTimeout(6000).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.MovimentoFinanceiroArquivoContabil>();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro> BuscarDadosParaArquivoContabilQuestor(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, DateTime dataInicial, DateTime dataFinal, TipoMovimentoArquivoContabilQuestor tipoMovimento, List<int> codigosTipoMovimento = null, int codigoTipoMovimentoArquivoContabil = 0, int codigoPlanoConta = 0)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro>();

            var result = from obj in query select obj;
            result = result.Where(obj => obj.DataMovimento >= dataInicial.Date && obj.DataMovimento <= dataFinal);

            if (codigosTipoMovimento?.Count > 0)
                result = result.Where(obj => obj.TipoMovimento != null && codigosTipoMovimento.Contains(obj.TipoMovimento.Codigo));

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if (codigoPlanoConta > 0)
                result = result.Where(obj => obj.PlanoDeContaCredito.Codigo == codigoPlanoConta || obj.PlanoDeContaDebito.Codigo == codigoPlanoConta);

            if ((int)tipoAmbiente > 0)
                result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);

            if (codigoTipoMovimentoArquivoContabil > 0)
            {
                var queryTipoMovimentoArquivoContabil = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.TipoMovimentoArquivoContabil>();
                var resultQueryTipoMovimentoArquivoContabil = from obj in queryTipoMovimentoArquivoContabil where obj.Codigo == codigoTipoMovimentoArquivoContabil select obj;

                result = result.Where(obj => resultQueryTipoMovimentoArquivoContabil.Where(a => a.TiposMovimentos.Any(t => t.Codigo == obj.TipoMovimento.Codigo)).Any());
            }
            else if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.ContasPagar)
                result = result.Where(obj => obj.TipoDocumentoMovimento == TipoDocumentoMovimento.Pagamento && !obj.Observacao.Contains("ACRÉSCIMO") && !obj.Observacao.Contains("ACRESCIMO") && !obj.Observacao.Contains("DESCONTO"));
            else if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosPagos)
                result = result.Where(obj => obj.TipoDocumentoMovimento == TipoDocumentoMovimento.Pagamento && (obj.Observacao.Contains("ACRÉSCIMO") || obj.Observacao.Contains("ACRESCIMO") || obj.Observacao.Contains("DESCONTO")));
            else if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.ContasReceber)
                result = result.Where(obj => obj.TipoDocumentoMovimento == TipoDocumentoMovimento.Recebimento && !obj.Observacao.Contains("ACRÉSCIMO") && !obj.Observacao.Contains("ACRESCIMO") && !obj.Observacao.Contains("DESCONTO"));
            else if (tipoMovimento == TipoMovimentoArquivoContabilQuestor.JurosDescontosRecebidos)
                result = result.Where(obj => obj.TipoDocumentoMovimento == TipoDocumentoMovimento.Recebimento && (obj.Observacao.Contains("ACRÉSCIMO") || obj.Observacao.Contains("ACRESCIMO") || obj.Observacao.Contains("DESCONTO")));
            else if (tipoMovimento != TipoMovimentoArquivoContabilQuestor.TipoMovimento)
                result = result.Where(obj => obj.TipoDocumentoMovimento != TipoDocumentoMovimento.Recebimento && obj.TipoDocumentoMovimento != TipoDocumentoMovimento.Recebimento);

            return result
                .Fetch(o => o.Pessoa)
                .Fetch(o => o.Titulo)
                .ToList();
        }

        public List<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro> BuscarDadosParaArquivoContabilJB(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, DateTime dataInicial, DateTime dataFinal)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro>();

            var result = from obj in query select obj;
            result = result.Where(obj => obj.DataMovimento.Date >= dataInicial.Date && obj.DataMovimento.Date <= dataFinal.Date.AddDays(1));
            //result = result.Where(obj => !string.IsNullOrWhiteSpace(obj.PlanoDeContaDebito.PlanoContabilidade) && !string.IsNullOrWhiteSpace(obj.PlanoDeContaCredito.PlanoContabilidade));
            //result = result.Where(obj => obj.Titulo != null);

            if (codigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == codigoEmpresa);

            if ((int)tipoAmbiente > 0)
                result = result.Where(obj => obj.TipoAmbiente == tipoAmbiente);

            result = result.Where(obj => obj.TipoDocumentoMovimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Pagamento || obj.TipoDocumentoMovimento == Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoDocumentoMovimento.Recebimento);

            return result.ToList();
        }

        public IList<Dominio.Relatorios.Embarcador.DataSource.Financeiros.MovimentoFinanceiroArquivoContabil> BuscarDadosParaArquivoContabilPadraoTransben(int codigoEmpresa, Dominio.Enumeradores.TipoAmbiente tipoAmbiente, DateTime dataInicial, DateTime dataFinal, TipoMovimentoArquivoContabilPadraoTransben tipoMovimento, int codigoTipoMovimentoArquivoContabil, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            string queryUnion = @"select distinct  M.MOV_CODIGO Codigo
                                         ,M.MOV_DATA Data
                                         ,M.MOV_VALOR Valor
                                         ,M.MOV_DOCUMENTO Documento
                                         ,M.MOV_TIPO TipoDocumentoMovimento
                                         ,M.MOV_OBSERVACAO Observacao
                                         ,M.TIM_CODIGO CodigoTipoMovimento
                                         ,M.PLA_CODIGO_DEBITO CodigoContaDebito
                                         ,M.PLA_CODIGO_CREDITO CodigoContaCredito
                                         ,M.CRE_CODIGO CodigoCentroResultado
                                         ,M.EMP_CODIGO CodigoEmpresa
                                         ,M.FUN_CODIGO CodigoFuncionario
                                         ,M.MOV_TIPO_GERACAO TipoGeracaoMovimento
                                         ,M.MOV_DATA_GERACAO DataGeracao
                                         ,M.TIT_CODIGO CodigoTitulo
                                         ,M.MOV_DATA_BASE DataBaseSistema
                                         ,M.GRP_CODIGO CodigoGrupo
                                         ,M.CLI_CGCCPF CNPJPessoa
                                         ,M.MOV_AMBIENTE TipoAmbiente
	                                     ,debito.PLA_PLANO PlanoDeContaDebito
	                                     ,debito.PLA_PLANO_CONTABILIDADE PlanoDeContaDebitoContabil
	                                     ,credito.PLA_PLANO PlanoDeContaCredito
	                                     ,credito.PLA_PLANO_CONTABILIDADE PlanoDeContaCreditoContabil
	                                     ,null CNPJPessoaTitulo
	                                     ,null NomePessoa
                                         ,titulo.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL NumeroDocumento
                                         ,(SELECT TIM_CODIGO_HISTORICO FROM T_TIPO_MOVIMENTO TP WHERE TP.TIM_CODIGO = M.TIM_CODIGO) CodigoHistoricoMovimentoFinanceiro
                                    from t_movimento_financeiro m
                                    join T_PLANO_DE_CONTA debito on debito.PLA_CODIGO = m.PLA_CODIGO_DEBITO
                                    join T_PLANO_DE_CONTA credito on credito.PLA_CODIGO = m.PLA_CODIGO_CREDITO
                                    left outer join T_TITULO titulo on titulo.TIT_CODIGO = m.TIT_CODIGO";

            string query = @"select distinct  M.MOV_CODIGO Codigo
                                    ,M.MOV_DATA Data
                                    ,M.MOV_VALOR Valor
                                    ,M.MOV_DOCUMENTO Documento
                                    ,M.MOV_TIPO TipoDocumentoMovimento
                                    ,M.MOV_OBSERVACAO Observacao
                                    ,M.TIM_CODIGO CodigoTipoMovimento
                                    ,M.PLA_CODIGO_DEBITO CodigoContaDebito
                                    ,M.PLA_CODIGO_CREDITO CodigoContaCredito
                                    ,M.CRE_CODIGO CodigoCentroResultado
                                    ,M.EMP_CODIGO CodigoEmpresa
                                    ,M.FUN_CODIGO CodigoFuncionario
                                    ,M.MOV_TIPO_GERACAO TipoGeracaoMovimento
                                    ,M.MOV_DATA_GERACAO DataGeracao
                                    ,M.TIT_CODIGO CodigoTitulo
                                    ,M.MOV_DATA_BASE DataBaseSistema
                                    ,M.GRP_CODIGO CodigoGrupo
                                    ,M.CLI_CGCCPF CNPJPessoa
                                    ,M.MOV_AMBIENTE TipoAmbiente
                                    ,debito.PLA_PLANO PlanoDeContaDebito
	                                ,debito.PLA_PLANO_CONTABILIDADE PlanoDeContaDebitoContabil
	                                ,credito.PLA_PLANO PlanoDeContaCredito
	                                ,credito.PLA_PLANO_CONTABILIDADE PlanoDeContaCreditoContabil
	                                ,c.CLI_CGCCPF CNPJPessoaTitulo
	                                ,c.CLI_NOME NomePessoa
                                    ,t.TIT_NUMERO_DOCUMENTO_TITULO_ORIGINAL NumeroDocumento
                                    ,(SELECT TIM_CODIGO_HISTORICO FROM T_TIPO_MOVIMENTO TP WHERE TP.TIM_CODIGO = M.TIM_CODIGO) CodigoHistoricoMovimentoFinanceiro
                               from t_titulo_baixa b
                               join t_titulo_baixa_agrupado a on b.tib_codigo = a.tib_codigo
                               join t_titulo t on t.tit_codigo = a.tit_codigo
                               join t_movimento_financeiro m on (m.mov_documento = ";


            if (tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.ContasRecebidasDocumento)
            {
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    queryUnion += " and titulo.FAP_CODIGO IS NULL and titulo.tit_tipo_documento_titulo_original not like '%CT-e%' and titulo.tit_tipo_documento_titulo_original not like '%NFS%' and titulo.tit_tipo_documento_titulo_original not like '%Fatura%'";
                }
            }

            if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
            {
                if (tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.ContasPagasDocumento || tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.DemaisPagamentos ||
                    tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.ContasRecebidasDocumento || tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.DemaisRecebimentos)
                    query += " cast(b.tib_codigo as varchar(20))) and t.CLI_CGCCPF = m.CLI_CGCCPF";
            }
            else
            {
                if (tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.ContasPagasDocumento || tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.DemaisPagamentos)
                    query += " cast(b.tib_codigo as varchar(20))) and t.CLI_CGCCPF = m.CLI_CGCCPF and m.TIT_CODIGO = t.TIT_CODIGO";
                else
                    query += " cast(t.tit_codigo as varchar(20)) or m.TIT_CODIGO = t.TIT_CODIGO) and t.CLI_CGCCPF = m.CLI_CGCCPF";
            }


            query += @" join T_PLANO_DE_CONTA debito on debito.PLA_CODIGO = m.PLA_CODIGO_DEBITO
                        join T_PLANO_DE_CONTA credito on credito.PLA_CODIGO = m.PLA_CODIGO_CREDITO
                        join T_CLIENTE c on c.CLI_CGCCPF = t.CLI_CGCCPF";

            string datePattern = "yyyy-MM-dd";

            query += $" WHERE (CAST(M.MOV_DATA AS DATE) BETWEEN '{dataInicial.ToString(datePattern)} 00:00:00' AND '{dataFinal.ToString(datePattern)} 23:59:59')";
            queryUnion += $" WHERE (CAST(M.MOV_DATA AS DATE) BETWEEN '{dataInicial.ToString(datePattern)} 00:00:00' AND '{dataFinal.ToString(datePattern)} 23:59:59')";

            if (codigoEmpresa > 0)
            {
                query += " AND M.EMP_CODIGO = " + codigoEmpresa.ToString();
                queryUnion += " AND M.EMP_CODIGO = " + codigoEmpresa.ToString();
            }
            if (tipoAmbiente != Dominio.Enumeradores.TipoAmbiente.Nenhum)
            {
                query += " AND M.MOV_AMBIENTE = " + tipoAmbiente.ToString("d");
                queryUnion += " AND M.MOV_AMBIENTE = " + tipoAmbiente.ToString("d");
            }

            if (codigoTipoMovimentoArquivoContabil > 0)
                query += $" AND M.TIM_CODIGO IN (SELECT TIM_CODIGO FROM T_TIPO_MOVIMENTO_ARQUIVO_CONTABIL_TIPOS WHERE TAC_CODIGO = {codigoTipoMovimentoArquivoContabil})"; // SQL-INJECTION-SAFE
            else if (tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.ContasPagasDocumento)
                query += @" and t.TIT_TIPO = 2 and m.mov_tipo = 6 and T.TDD_CODIGO IS NOT NULL";
            else if (tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.DemaisPagamentos)
            {
                query += @" and t.TIT_TIPO = 2 and m.mov_tipo = 6
                    and m.mov_observacao not like '%PAGAMENTO MOTORISTA ADIANTAMENTO%' 
                    and T.TDD_CODIGO IS NULL";
                queryUnion += " AND m.PLA_CODIGO_CREDITO in (select t.PLA_CODIGO from T_TIPO_PAGAMENTO_RECEBIMENTO t) and m.mov_tipo <> 7 and m.mov_tipo <> 6 AND m.mov_tipo <> 3 AND m.mov_tipo <> 2";
            }
            else if (tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.ContasRecebidasDocumento)
            {
                query += " and t.TIT_TIPO = 1 and m.mov_tipo = 7 ";
                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                    query += " and (T.FAP_CODIGO IS NOT NULL OR t.tit_tipo_documento_titulo_original like '%CT-e%' OR t.tit_tipo_documento_titulo_original like '%NFS%' OR t.tit_tipo_documento_titulo_original like '%Fatura%')";
            }
            else if (tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.DemaisRecebimentos)
            {
                query += " and t.TIT_TIPO = 1 and m.mov_tipo = 7 ";
                queryUnion += " AND m.PLA_CODIGO_DEBITO in (select t.PLA_CODIGO from T_TIPO_PAGAMENTO_RECEBIMENTO t) and m.mov_tipo <> 6 AND m.mov_tipo <> 3 AND m.mov_tipo <> 2 and m.mov_observacao not like 'PAGAMENTO%' and m.mov_observacao not like '%baixa do título%'";

                if (tipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                {
                    query += " and T.FAP_CODIGO IS NULL and t.tit_tipo_documento_titulo_original not like '%CT-e%' and t.tit_tipo_documento_titulo_original not like '%NFS%' and t.tit_tipo_documento_titulo_original not like '%Fatura%'";
                }
            }

            if (tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.DemaisPagamentos || tipoMovimento == TipoMovimentoArquivoContabilPadraoTransben.DemaisRecebimentos)
                query = query + " UNION ALL " + queryUnion;

            var nhQuery = this.SessionNHiBernate.CreateSQLQuery(query);

            nhQuery.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Dominio.Relatorios.Embarcador.DataSource.Financeiros.MovimentoFinanceiroArquivoContabil)));

            return nhQuery.SetTimeout(6000).List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.MovimentoFinanceiroArquivoContabil>();
        }

        #endregion 

        #region Métodos Privados

        private IQueryable<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro> Consultar(Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaMovimentoFinanceiro filtrosPesquisa)
        {
            var query = this.SessionNHiBernate.Query<Dominio.Entidades.Embarcador.Financeiro.MovimentoFinanceiro>();

            var result = from obj in query select obj;

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.NumeroDocumento))
                result = result.Where(obj => obj.Documento.Contains(filtrosPesquisa.NumeroDocumento));

            if (!string.IsNullOrWhiteSpace(filtrosPesquisa.Observacao))
                result = result.Where(obj => obj.Observacao.Contains(filtrosPesquisa.Observacao));

            if (filtrosPesquisa.ValorMovimento > 0)
                result = result.Where(obj => obj.Valor.Equals(filtrosPesquisa.ValorMovimento));

            if (filtrosPesquisa.CodigoMovimento > 0)
                result = result.Where(obj => obj.Codigo.Equals(filtrosPesquisa.CodigoMovimento));

            if (filtrosPesquisa.CodigoEmpresa > 0)
                result = result.Where(obj => obj.Empresa.Codigo == filtrosPesquisa.CodigoEmpresa);

            if (filtrosPesquisa.DataMovimentoInicial > DateTime.MinValue)
                result = result.Where(obj => obj.DataMovimento.Date >= filtrosPesquisa.DataMovimentoInicial);

            if (filtrosPesquisa.DataMovimentoFinal > DateTime.MinValue)
                result = result.Where(obj => obj.DataMovimento.Date <= filtrosPesquisa.DataMovimentoFinal);

            if (filtrosPesquisa.DataBase > DateTime.MinValue)
                result = result.Where(obj => obj.DataBase.Date == filtrosPesquisa.DataBase);

            if (filtrosPesquisa.TipoDocumento > 0)
                result = result.Where(obj => obj.TipoDocumentoMovimento == filtrosPesquisa.TipoDocumento);

            if (filtrosPesquisa.CodigoGrupoPessoa > 0)
                result = result.Where(obj => obj.GrupoPessoas.Codigo == filtrosPesquisa.CodigoGrupoPessoa);

            if (filtrosPesquisa.CnpjPessoa > 0)
                result = result.Where(obj => obj.Pessoa.CPF_CNPJ == filtrosPesquisa.CnpjPessoa);

            if (filtrosPesquisa.CodigoTipoMovimento > 0)
                result = result.Where(obj => obj.TipoMovimento.Codigo == filtrosPesquisa.CodigoTipoMovimento);

            if (filtrosPesquisa.CodigoCentroResultado > 0)
                result = result.Where(obj => obj.CentroResultado.Codigo == filtrosPesquisa.CodigoCentroResultado);

            if (filtrosPesquisa.CodigoPlanoDebito > 0)
                result = result.Where(obj => obj.PlanoDeContaDebito.Codigo == filtrosPesquisa.CodigoPlanoDebito);

            if (filtrosPesquisa.CodigoPlanoCredito > 0)
                result = result.Where(obj => obj.PlanoDeContaCredito.Codigo == filtrosPesquisa.CodigoPlanoCredito);

            if ((int)filtrosPesquisa.TipoAmbiente > 0)
                result = result.Where(obj => obj.TipoAmbiente == filtrosPesquisa.TipoAmbiente);

            if (filtrosPesquisa.SituacaoMovimento.Value != TipoConsolidacaoMovimentoFinanceiro.Todos)
            {
                bool movimentoConsolidado = filtrosPesquisa.SituacaoMovimento == TipoConsolidacaoMovimentoFinanceiro.Consolidado;
                if (filtrosPesquisa.SituacaoMovimento.Value == TipoConsolidacaoMovimentoFinanceiro.Consolidado)
                    result = result.Where(obj => obj.MovimentosFinanceirosDebitoCredito.Any(M => M.MovimentoConcolidado == true));
                else
                    result = result.Where(obj => !obj.MovimentosFinanceirosDebitoCredito.Any(M => M.MovimentoConcolidado == true));
            }

            if (filtrosPesquisa.MoedaCotacaoBancoCentral != MoedaCotacaoBancoCentral.Todas)
                result = result.Where(obj => obj.MoedaCotacaoBancoCentral == filtrosPesquisa.MoedaCotacaoBancoCentral);

            if (!filtrosPesquisa.VisualizarTitulosPagamentoSalario)
                result = result.Where(obj => obj.FormaTitulo != FormaTitulo.PagamentoSalario);

            return result;
        }

        #endregion
    }
}

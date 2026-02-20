using Newtonsoft.Json;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/Faturamento")]
    public class FaturamentoController : BaseController
    {
		#region Construtores

		public FaturamentoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R047_Faturamento;

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();
            grid.AdicionarCabecalho("Número Fatura", "NumeroFatura", 8, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Nº Parcela", "Sequencia", 5, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissao", 8, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Vencimento", "DataVencimento", 8, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Quitação", "DataQuitacao", 8, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Grupo Pessoa", "Grupo", 20, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Pessoa", "Pessoa", 20, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Valor Fatura", "ValorFatura", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Total Acréscimos", "TotalAcrescimos", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Total Descontos", "TotalDescontos", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Total Fatura", "TotalFatura", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Saldo em Aberto da Fatura", "SaldoAberto", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Satus Financeiro", "StatusFinanceiro", 10, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Cód. Título", "CodigoTitulo", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Valor Pendente", "ValorPendente", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Valor Pago", "ValorPago", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Cód. Fatura", "CodigoFatura", 8, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Base", "DataBaseQuitacao", 8, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("CT-es", "Conhecimentos", 20, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Cargas", "Cargas", 20, Models.Grid.Align.center, false, false);

            return grid;
        }

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Faturamento", "Financeiros", "Faturamento.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroFatura", "asc", "", "", Codigo, unitOfWork, true, false);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);
                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);

                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                int codigoCTe = 0, codigoFatura = 0, codigoGrupoPessoa = 0;
                int.TryParse(Request.Params("ConhecimentoDeTransporte"), out codigoCTe);
                int.TryParse(Request.Params("Fatura"), out codigoFatura);
                int.TryParse(Request.Params("GrupoPessoa"), out codigoGrupoPessoa);

                DateTime dataInicialEmissao, dataFinalEmissao;
                DateTime.TryParse(Request.Params("DataInicialEmissao"), out dataInicialEmissao);
                DateTime.TryParse(Request.Params("DataFinalEmissao"), out dataFinalEmissao);

                DateTime dataInicialVencimento, dataFinalVencimento;
                DateTime.TryParse(Request.Params("DataInicialVencimento"), out dataInicialVencimento);
                DateTime.TryParse(Request.Params("DataFinalVencimento"), out dataFinalVencimento);

                DateTime dataInicialEmissaoCTe, dataFinalEmissaoCTe;
                DateTime.TryParse(Request.Params("DataInicialEmissaoCTe"), out dataInicialEmissaoCTe);
                DateTime.TryParse(Request.Params("DataFinalEmissaoCTe"), out dataFinalEmissaoCTe);

                DateTime dataInicialQuitacao, dataFinalQuitacao;
                DateTime.TryParse(Request.Params("DataInicialQuitacao"), out dataInicialQuitacao);
                DateTime.TryParse(Request.Params("DataFinalQuitacao"), out dataFinalQuitacao);

                DateTime dataInicialEmissaoFatura, dataFinalEmissaoFatura;
                DateTime.TryParse(Request.Params("DataInicialEmissaoFatura"), out dataInicialEmissaoFatura);
                DateTime.TryParse(Request.Params("DataFinalEmissaoFatura"), out dataFinalEmissaoFatura);

                DateTime dataBaseInicial, dataBaseFinal;
                DateTime.TryParse(Request.Params("DataBaseInicial"), out dataBaseInicial);
                DateTime.TryParse(Request.Params("DataBaseFinal"), out dataBaseFinal);

                decimal valorInicial, valorFinal;
                decimal.TryParse(Request.Params("ValorInicial"), out valorInicial);
                decimal.TryParse(Request.Params("ValorFinal"), out valorFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status;
                Enum.TryParse(Request.Params("StatusFinanceiro"), out status);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura situacaoFatura;
                Enum.TryParse(Request.Params("SituacaoFatura"), out situacaoFatura);

                List<int> gruposPessoas = JsonConvert.DeserializeObject<List<int>>(Request.Params("GruposPessoas"));

                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

                var propAgrupa = grid.group.enable ? grid.group.propAgrupa : "";
                string ordenacao = grid.header[grid.indiceColunaOrdena].data;

                IList<Dominio.Relatorios.Embarcador.DataSource.Fatura.Faturamento> listaFaturamento = repFatura.RelatorioFaturamento(dataBaseInicial, dataBaseFinal, dataInicialEmissaoFatura, dataFinalEmissaoFatura, gruposPessoas, dataInicialEmissaoCTe, dataFinalEmissaoCTe, dataInicialQuitacao, dataFinalQuitacao, situacaoFatura, dataInicialVencimento, dataFinalVencimento, valorInicial, valorFinal, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, propAgrupa, grid.group.dirOrdena, ordenacao, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repFatura.ContarFaturamento(dataBaseInicial, dataBaseFinal, dataInicialEmissaoFatura, dataFinalEmissaoFatura, gruposPessoas, dataInicialEmissaoCTe, dataFinalEmissaoCTe, dataInicialQuitacao, dataFinalQuitacao, situacaoFatura, dataInicialVencimento, dataFinalVencimento, valorInicial, valorFinal, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao));

                var lista = (from obj in listaFaturamento
                             select new
                             {
                                 obj.NumeroFatura,
                                 obj.Sequencia,
                                 DataEmissao = obj.DataEmissao != null && obj.DataEmissao > DateTime.MinValue ? obj.DataEmissao.ToString("dd/MM/yyyy") : string.Empty,
                                 DataVencimento = obj.DataVencimento != null && obj.DataVencimento > DateTime.MinValue ? obj.DataVencimento.ToString("dd/MM/yyyy") : string.Empty,
                                 DataQuitacao = obj.DataQuitacao != null && obj.DataQuitacao > DateTime.MinValue ? obj.DataQuitacao.ToString("dd/MM/yyyy") : string.Empty,
                                 obj.Grupo,
                                 obj.Pessoa,
                                 obj.ValorFatura,
                                 obj.TotalAcrescimos,
                                 obj.TotalDescontos,
                                 obj.TotalFatura,
                                 obj.SaldoAberto,
                                 obj.StatusFinanceiro,
                                 obj.CodigoTitulo,
                                 obj.ValorPendente,
                                 obj.ValorPago,
                                 obj.CodigoFatura,
                                 DataBaseQuitacao = obj.DataBaseQuitacao != null && obj.DataBaseQuitacao > DateTime.MinValue ? obj.DataBaseQuitacao.ToString("dd/MM/yyyy") : string.Empty,
                                 obj.Conhecimentos,
                                 obj.Cargas
                             }).ToList();

                grid.AdicionaRows(lista);

                return new JsonpResult(grid);

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                double cnpjPessoa = 0;
                double.TryParse(Request.Params("Pessoa"), out cnpjPessoa);

                int codigoCTe = 0, codigoFatura = 0, codigoGrupoPessoa = 0;
                int.TryParse(Request.Params("ConhecimentoDeTransporte"), out codigoCTe);
                int.TryParse(Request.Params("Fatura"), out codigoFatura);
                int.TryParse(Request.Params("GrupoPessoa"), out codigoGrupoPessoa);

                DateTime dataInicialEmissao, dataFinalEmissao;

                DateTime.TryParse(Request.Params("DataInicialEmissao"), out dataInicialEmissao);
                DateTime.TryParse(Request.Params("DataFinalEmissao"), out dataFinalEmissao);

                DateTime dataInicialVencimento, dataFinalVencimento;
                DateTime.TryParse(Request.Params("DataInicialVencimento"), out dataInicialVencimento);
                DateTime.TryParse(Request.Params("DataFinalVencimento"), out dataFinalVencimento);

                DateTime dataInicialEmissaoCTe, dataFinalEmissaoCTe;
                DateTime.TryParse(Request.Params("DataInicialEmissaoCTe"), out dataInicialEmissaoCTe);
                DateTime.TryParse(Request.Params("DataFinalEmissaoCTe"), out dataFinalEmissaoCTe);

                DateTime dataInicialQuitacao, dataFinalQuitacao;
                DateTime.TryParse(Request.Params("DataInicialQuitacao"), out dataInicialQuitacao);
                DateTime.TryParse(Request.Params("DataFinalQuitacao"), out dataFinalQuitacao);

                DateTime dataInicialEmissaoFatura, dataFinalEmissaoFatura;
                DateTime.TryParse(Request.Params("DataInicialEmissaoFatura"), out dataInicialEmissaoFatura);
                DateTime.TryParse(Request.Params("DataFinalEmissaoFatura"), out dataFinalEmissaoFatura);

                DateTime dataBaseInicial, dataBaseFinal;
                DateTime.TryParse(Request.Params("DataBaseInicial"), out dataBaseInicial);
                DateTime.TryParse(Request.Params("DataBaseFinal"), out dataBaseFinal);

                decimal valorInicial, valorFinal;
                decimal.TryParse(Request.Params("ValorInicial"), out valorInicial);
                decimal.TryParse(Request.Params("ValorFinal"), out valorFinal);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status;
                Enum.TryParse(Request.Params("StatusFinanceiro"), out status);

                Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura situacaoFatura;
                Enum.TryParse(Request.Params("SituacaoFatura"), out situacaoFatura);

                List<int> gruposPessoas = JsonConvert.DeserializeObject<List<int>>(Request.Params("GruposPessoas"));

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);


                string stringConexao = _conexao.StringConexao;
                _ = Task.Factory.StartNew(() => GerarRelatorioFaturamento(dataBaseInicial, dataBaseFinal, dataInicialEmissaoFatura, dataFinalEmissaoFatura, gruposPessoas, dataInicialEmissaoCTe, dataFinalEmissaoCTe, dataInicialQuitacao, dataFinalQuitacao, situacaoFatura, dataInicialVencimento, dataFinalVencimento, valorInicial, valorFinal, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private async Task GerarRelatorioFaturamento(DateTime dataBaseInicial, DateTime dataBaseFinal, DateTime dataInicialEmissaoFatura, DateTime dataFinalEmissaoFatura, List<int> gruposPessoas, DateTime dataInicialEmissaoCTe, DateTime dataFinalEmissaoCTe, DateTime dataInicialQuitacao, DateTime dataFinalQuitacao, Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura situacaoFatura, DateTime dataInicialVencimento, DateTime dataFinalVencimento, decimal valorInicial, decimal valorFinal, Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo status, double cnpjPessoa, int codigoCTe, int codigoFatura, int codigoGrupoPessoa, DateTime dataInicialEmissao, DateTime dataFinalEmissao, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
                Repositorio.Embarcador.Fatura.Fatura repFatura = new Repositorio.Embarcador.Fatura.Fatura(unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Fatura.Faturamento> listaFaturamento = repFatura.RelatorioFaturamento(dataBaseInicial, dataBaseFinal, dataInicialEmissaoFatura, dataFinalEmissaoFatura, gruposPessoas, dataInicialEmissaoCTe, dataFinalEmissaoCTe, dataInicialQuitacao, dataFinalQuitacao, situacaoFatura, dataInicialVencimento, dataFinalVencimento, valorInicial, valorFinal, status, cnpjPessoa, codigoCTe, codigoFatura, codigoGrupoPessoa, dataInicialEmissao, dataFinalEmissao, relatorioTemp.PropriedadeAgrupa, relatorioTemp.OrdemAgrupamento, relatorioTemp.PropriedadeOrdena, relatorioTemp.OrdemOrdenacao, 0, 0, false);

                //var lista = (from obj in listaFaturamento
                //             select new
                //             {
                //                 obj.NumeroFatura,
                //                 obj.Sequencia,
                //                 DataEmissao = obj.DataEmissao != null && obj.DataEmissao > DateTime.MinValue ? obj.DataEmissao.ToString("dd/MM/yyyy") : string.Empty,
                //                 DataVencimento = obj.DataVencimento != null && obj.DataVencimento > DateTime.MinValue ? obj.DataVencimento.ToString("dd/MM/yyyy") : string.Empty,
                //                 DataQuitacao = obj.DataQuitacao != null && obj.DataQuitacao > DateTime.MinValue ? obj.DataQuitacao.ToString("dd/MM/yyyy") : string.Empty,
                //                 obj.Grupo,
                //                 obj.Pessoa,
                //                 obj.ValorFatura,
                //                 obj.TotalAcrescimos,
                //                 obj.TotalDescontos,
                //                 obj.TotalFatura,
                //                 obj.SaldoAberto,
                //                 obj.StatusFinanceiro,
                //                 obj.CodigoTitulo,
                //                 obj.ValorPendente,
                //                 obj.ValorPago,
                //                 obj.CodigoFatura,
                //                 DataBaseQuitacao = obj.DataBaseQuitacao != null && obj.DataBaseQuitacao > DateTime.MinValue ? obj.DataBaseQuitacao.ToString("dd/MM/yyyy") : string.Empty,
                //                 obj.Conhecimentos,
                //                 obj.Cargas
                //             }).ToList();

                Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT identificacaoCamposRPT = new Dominio.Relatorios.Embarcador.ObjetosDeValor.IdentificacaoCamposRPT();
                identificacaoCamposRPT.PrefixoCamposSum = "";
                identificacaoCamposRPT.IndiceSumGroup = "3";
                identificacaoCamposRPT.IndiceSumReport = "4";
                //CrystalDecisions.CrystalReports.Engine.ReportDocument report = serRelatorio.CriarRelatorio(relatorioControleGeracao, relatorioTemp, lista, unitOfWork, identificacaoCamposRPT);
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

                if (codigoCTe > 0)
                {
                    Repositorio.ConhecimentoDeTransporteEletronico repCTe = new Repositorio.ConhecimentoDeTransporteEletronico(unitOfWork);
                    Dominio.Entidades.ConhecimentoDeTransporteEletronico cte = repCTe.BuscarPorCodigo(codigoCTe);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTe", "(" + cte.Numero + ") " + cte.Chave, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("CTe", false));

                if (codigoFatura > 0)
                {
                    Dominio.Entidades.Embarcador.Fatura.Fatura fatura = repFatura.BuscarPorCodigo(codigoFatura);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fatura", "(" + fatura.Codigo + ") " + fatura.Numero, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Fatura", false));

                if (gruposPessoas != null && gruposPessoas.Count > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", "Multiplos grupos", true));
                else if (codigoGrupoPessoa > 0)
                {
                    Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork);
                    Dominio.Entidades.Embarcador.Pessoas.GrupoPessoas grupo = repGrupoPessoas.BuscarPorCodigo(codigoGrupoPessoa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", grupo.Descricao, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoa", false));

                if (cnpjPessoa > 0)
                {
                    Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
                    Dominio.Entidades.Cliente cliente = repCliente.BuscarPorCPFCNPJ(cnpjPessoa);
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", "(" + cliente.CPF_CNPJ_Formatado + ") " + cliente.Nome, true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Pessoa", false));

                if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "De " + dataInicialEmissao.ToString("dd/MM/yyyy") + " até " + dataFinalEmissao.ToString("dd/MM/yyyy"), true));
                else if (dataInicialEmissao > DateTime.MinValue && dataFinalEmissao == DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "De " + dataInicialEmissao.ToString("dd/MM/yyyy"), true));
                else if (dataInicialEmissao == DateTime.MinValue && dataFinalEmissao > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "Até " + dataFinalEmissao.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissao", "Todos", true));

                if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimento", "De " + dataInicialVencimento.ToString("dd/MM/yyyy") + " até " + dataFinalVencimento.ToString("dd/MM/yyyy"), true));
                else if (dataInicialVencimento > DateTime.MinValue && dataFinalVencimento == DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimento", "De " + dataInicialVencimento.ToString("dd/MM/yyyy"), true));
                else if (dataInicialVencimento == DateTime.MinValue && dataFinalVencimento > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimento", "Até " + dataFinalVencimento.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataVencimento", "Todos", true));

                if (valorInicial > 0 && valorFinal > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Valor", "De " + valorInicial.ToString("n2") + " até " + valorFinal.ToString("n2"), true));
                else if (valorInicial > 0 && valorFinal == 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Valor", "De " + valorInicial.ToString("n2"), true));
                else if (valorInicial == 0 && valorFinal > 0)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Valor", "Até " + valorFinal.ToString("n2"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Valor", "Todos", true));

                if (status != Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.Todos)
                {
                    if (status == Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusTitulo.EmAberto)
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusFinanceiro", "Em Aberto", true));
                    else
                        parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusFinanceiro", "Quitado", true));
                }
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("StatusFinanceiro", "Todos", true));

                if (!string.IsNullOrWhiteSpace(relatorioTemp.PropriedadeAgrupa))
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", relatorioTemp.PropriedadeAgrupa, true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Agrupamento", false));

                if ((int)situacaoFatura == -1)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoFatura", "Exceto Canceladas", true));
                else if (situacaoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Cancelado)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoFatura", "Canceladas", true));
                else if (situacaoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.EmAntamento)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoFatura", "Em Andamento", true));
                else if (situacaoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Fechado)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoFatura", "Fechadas", true));
                else if (situacaoFatura == Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoFatura.Liquidado)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoFatura", "Liquidadas", true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SituacaoFatura", "Todas", true));

                if (dataInicialQuitacao > DateTime.MinValue && dataFinalQuitacao > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataQuitacao", "De " + dataInicialQuitacao.ToString("dd/MM/yyyy") + " até " + dataFinalQuitacao.ToString("dd/MM/yyyy"), true));
                else if (dataInicialQuitacao > DateTime.MinValue && dataFinalQuitacao == DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataQuitacao", "De " + dataInicialQuitacao.ToString("dd/MM/yyyy"), true));
                else if (dataInicialQuitacao == DateTime.MinValue && dataFinalQuitacao > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataQuitacao", "Até " + dataFinalQuitacao.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataQuitacao", "Todos", true));

                if (dataInicialEmissaoCTe > DateTime.MinValue && dataFinalEmissaoCTe > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoCTe", "De " + dataInicialEmissaoCTe.ToString("dd/MM/yyyy") + " até " + dataFinalEmissaoCTe.ToString("dd/MM/yyyy"), true));
                else if (dataInicialEmissaoCTe > DateTime.MinValue && dataFinalEmissaoCTe == DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoCTe", "De " + dataInicialEmissaoCTe.ToString("dd/MM/yyyy"), true));
                else if (dataInicialEmissaoCTe == DateTime.MinValue && dataFinalEmissaoCTe > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoCTe", "Até " + dataFinalEmissaoCTe.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoCTe", "Todos", true));

                if (dataInicialEmissaoFatura > DateTime.MinValue && dataFinalEmissaoFatura > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoFatura", "De " + dataInicialEmissaoFatura.ToString("dd/MM/yyyy") + " até " + dataFinalEmissaoFatura.ToString("dd/MM/yyyy"), true));
                else if (dataInicialEmissaoFatura > DateTime.MinValue && dataFinalEmissaoFatura == DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoFatura", "De " + dataInicialEmissaoFatura.ToString("dd/MM/yyyy"), true));
                else if (dataInicialEmissaoFatura == DateTime.MinValue && dataFinalEmissaoFatura > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoFatura", "Até " + dataFinalEmissaoFatura.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataEmissaoFatura", "Todos", true));

                if (dataBaseInicial > DateTime.MinValue && dataBaseFinal > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataBase", "De " + dataBaseInicial.ToString("dd/MM/yyyy") + " até " + dataBaseFinal.ToString("dd/MM/yyyy"), true));
                else if (dataBaseInicial > DateTime.MinValue && dataBaseFinal == DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataBase", "De " + dataBaseInicial.ToString("dd/MM/yyyy"), true));
                else if (dataBaseInicial == DateTime.MinValue && dataBaseFinal > DateTime.MinValue)
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataBase", "Até " + dataBaseFinal.ToString("dd/MM/yyyy"), true));
                else
                    parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataBase", "Todos", true));

                //serRelatorio.PreecherParamentrosFiltro(report, relatorioControleGeracao, relatorioTemp, parametros);

                //serRelatorio.GerarRelatorio(report, relatorioControleGeracao, "Relatorios/Financeiros/Faturamento", unitOfWork);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Financeiros/Faturamento", parametros, relatorioControleGeracao, relatorioTemp, listaFaturamento, unitOfWork, identificacaoCamposRPT);

                await unitOfWork.DisposeAsync();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }
    }
}

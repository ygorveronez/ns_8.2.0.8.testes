using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/ContratoFinanceiro")]
    public class ContratoFinanceiroController : BaseController
    {
		#region Construtores

		public ContratoFinanceiroController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R296_ContratoFinanceiro;

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                int Codigo = Request.GetIntParam("Codigo");

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório Contrato Financeiro", "Financeiros", "ContratoFinanceiro.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Modelo", "asc", "", "", Codigo, unitOfWork, false, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(ex);

                return new JsonpResult(false, "Ocorreu uma falha ao buscar os dados do relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }

        }

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioContratoFinanceiro filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametroConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametroConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.ContratoFinanceiro serContratoFinanceiro = new Servicos.Embarcador.Relatorios.Financeiros.ContratoFinanceiro(unitOfWork, TipoServicoMultisoftware, Cliente);

                serContratoFinanceiro.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.ContratoFinanceiro> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametroConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
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
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioContratoFinanceiro filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, servicoException.Message);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioContratoFinanceiro ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioContratoFinanceiro()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                NumeroDocumento = Request.GetStringParam("NumeroDocumento"),
                NumeroDocumentoEntrada = Request.GetStringParam("NumeroDocumentoEntrada"),
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                CpfCnpjFornecedor = Request.GetDoubleParam("Fornecedor"),
                CodigosVeiculos = Request.GetListParam<int>("Veiculo"),
                Situacoes = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoContratoFinanciamento>("Situacao")
            };
        }

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            decimal TamanhoColunasMedia = 6;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequeno = 4;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Data Emissão", "DataEmissaoFormatada", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Número Documento", "NumeroDocumento", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Fornecedor", "Fornecedor", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Empresa", "Empresa", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Veículo", "Veiculo", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Número Documento Entrada", "NumeroDocumentoEntrada", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Quantidades Parcelas", "QuantidadeParcela", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Capital", "ValorCapital", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Juros", "ValorJuros", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Pago Capital", "ValorPagoCapital", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Pago Juros", "ValorPagoJuros", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Situação", "SituacaoFormatada", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Pago Parcela", "ValorPagoParcela", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Acréscimo", "ValorAcrescimo", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor Acréscimo Titulo", "ValorAcrescimoTitulo", TamanhoColunasPequeno, Models.Grid.Align.right, false, false, false, false, false);

            return grid;
        }

        #endregion
    }
}

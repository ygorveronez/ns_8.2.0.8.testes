using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.PagamentosMotoristas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/PagamentosMotoristas/PendenciaMotorista")]
    public class PendenciaMotoristaController : BaseController
    {
		#region Construtores

		public PendenciaMotoristaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R320_PendenciaMotorista;

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Pendência de Motorista", "PagamentosMotoristas", "PendenciaMotorista.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(dadosRelatorio);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
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

                Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPendenciaMotorista filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.PagamentosMotoristas.PendenciaMotorista servicoRelatorioPendenciaMotorista = new Servicos.Embarcador.Relatorios.PagamentosMotoristas.PendenciaMotorista(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioPendenciaMotorista.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.PagamentoMotorista.PendenciaMotorista> listaPendenciaMotorista, out int totalRegistros, filtrosPesquisa, propriedades, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaPendenciaMotorista);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
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

                Servicos.Embarcador.Relatorios.Relatorio svcRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = svcRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPendenciaMotorista filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = svcRelatorio.AdicionarRelatorioParaGeracao(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPendenciaMotorista ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPendenciaMotorista filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.PagamentosMotoristas.FiltroPesquisaPendenciaMotorista
            {
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                Situacao = Request.GetEnumParam<SituacaoPendenciaMotorista>("Situacao"),
                ValorInicial = Request.GetDecimalParam("ValorInicial"),
                ValorFinal = Request.GetDecimalParam("ValorFinal"),
                CodigoMotorista = Request.GetIntParam("Motorista"),

            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Data", "DataFormatada", 10, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Situação", "SituacaoDescricao", 10, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Motorista", "Motorista", 20, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Pendência", "DescricaoPendencia", 20, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Justificativa", "DescricaoJustificativa", 20, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Valor", "Valor", 7, Models.Grid.Align.left, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Pendência", "ValorPendencia", 7, Models.Grid.Align.left, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Observação", "Observacao", 15, Models.Grid.Align.left, false, false, false, false, true);


            return grid;
        }

        #endregion
    }
}

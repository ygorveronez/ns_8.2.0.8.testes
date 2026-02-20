using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Logisitca
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Logistica/ControleTempoViagem")]
    public class ControleTempoViagemController : BaseController
    {
		#region Construtores

		public ControleTempoViagemController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R264_ControleTempoViagem;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Controle de Tempo de Viagem", "Logistica", "ControleTempoViagem.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "RazaoSocial", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioControleTempoViagem filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Logistica.ControleTempoViagem servicoRelatorioControleTempoViagem = new Servicos.Embarcador.Relatorios.Logistica.ControleTempoViagem(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioControleTempoViagem.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Logistica.ControleTempoViagem> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaAcerto);

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

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, parametrosConsulta.PropriedadeAgrupar);
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioControleTempoViagem filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
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

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            decimal TamanhoColunaPequena = 2.25m;
            decimal TamanhoColunaGrande = 5m;
            decimal TamanhoColunaMedia = 3.5m;

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Nº NF", "NumeroNF", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Fatura", "DataFaturaFormatada", TamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Tempo de Viagem (dias)", "TempoViagem", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Tempo de Viagem (horas)", "TempoViagemEmHoras", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Prev. de Entrega", "PrevisaoEntregaFormatada", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Entrega Real", "DataEntregaRealFormatada", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Performance", "Performance", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Retorno do Comprovante", "RetornoComprovanteFormatada", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Dias p/ Retorno do Comprovante", "DiasRetornoComprovante", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Documento Venda", "DocumentoVenda", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Razão Social Destinatário", "RazaoSocialDestinatario", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Carga", "Carga", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Destino", "Destino", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Valor da Nota", "ValorNota", TamanhoColunaPequena, Models.Grid.Align.right, false, false, false, true, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioControleTempoViagem ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioControleTempoViagem()
            {
                CodigosCargas = Request.GetListParam<int>("Carga"),
                NumerosNota = Request.GetListParam<int>("NumeroNota"),
                Destinos = Request.GetListParam<int>("Destino"),
                Transportadores = Request.GetListParam<int>("Transportador"),
                DataFaturaInicial = Request.GetDateTimeParam("DataFaturaInicial"),
                DataFaturaFinal = Request.GetDateTimeParam("DataFaturaFinal"),
                PrevisaoEntregaInicial = Request.GetDateTimeParam("PrevisaoEntregaInicial"),
                PrevisaoEntregaFinal = Request.GetDateTimeParam("PrevisaoEntregaFinal"),
                DataEntregaRealInicial = Request.GetDateTimeParam("DataEntregaRealInicial"),
                DataEntregaRealFinal = Request.GetDateTimeParam("DataEntregaRealFinal"),
                Performance = Request.GetIntParam("Performance"),
                DataRetornoComprovanteInicial = Request.GetDateTimeParam("DataRetornoComprovanteInicial"),
                DataRetornoComprovanteFinal = Request.GetDateTimeParam("DataRetornoComprovanteFinal"),
                DocumentoVenda = Request.GetStringParam("DocumentoVenda"),
                RazaoSocialDestinatario = Request.GetStringParam("RazaoSocialDestinatario"),
                ValorNotaInicial = Request.GetDecimalParam("ValorNotaInicial"),
                ValorNotaFinal = Request.GetDecimalParam("ValorNotaFinal"),
                DiasRetornoComprovante = Request.GetIntParam("DiasRetornoComprovante")
            };
        }

        #endregion
    }
}

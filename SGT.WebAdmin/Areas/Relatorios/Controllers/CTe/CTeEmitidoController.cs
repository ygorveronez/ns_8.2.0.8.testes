using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/CTe/CTeEmitido")]
    public class CTeEmitidoController : BaseController
    {
		#region Construtores

		public CTeEmitidoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R002_CTESEmitidosPorEmbarcador;

        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaGrande = 5.50m;
        private decimal TamanhoColunaMedia = 3m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork,cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório CT-e(s) Emitidos", "CTe", "CTeEmitidoEmbarcador.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", Codigo, unitOfWork, true, true);
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
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        public async Task<IActionResult> PesquisaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTeEmitido filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.CTes.CTeEmitido servicoRelatorioCteEmitido = new Servicos.Embarcador.Relatorios.CTes.CTeEmitido(unitOfWork, TipoServicoMultisoftware, Cliente, cancellationToken);

                servicoRelatorioCteEmitido.ExecutarPesquisa(out List<Dominio.ObjetosDeValor.Relatorios.RelatorioCTesEmitidosPorEmbarcador> listaAcerto, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaAcerto);

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

        public async Task<IActionResult> GerarRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTeEmitido filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
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

        #endregion

        #region Métodos Privados

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", "Codigo", 2, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Número", "Numero", 2, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Série", "Serie", 2, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("Nº NF-e", "NumeroNotaFiscal", 2, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Status", "DescricaoStatus", 3, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Emissão", "DataEmissao", 3, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("Autorização", "DataAutorizacao", 3, Models.Grid.Align.center, false, false);
            grid.AdicionarCabecalho("CNPJ Remetente", "CPFCNPJRemetente", 4, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Remetente", "Remetente", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("UF Remetente", "UFRemetente", 4, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ Destinatário", "CPFCNPJDestinatario", 4, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("UF Destinatário", "UFDestinatario", 4, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("CNPJ Transportador", "CNPJTransportador", (decimal)3.5, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", (decimal)10.5, Models.Grid.Align.left, true, false, false, true);
            grid.AdicionarCabecalho("UF Transportador", "UFTransportador", 4, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Valor ICMS", "ValorICMS", 3, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Pedágio", "ValorPedagio", 3, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Frete", "ValorFrete", 3, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor a Receber", "ValorAReceber", 3, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Placa", "PlacaVeiculo", 3, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("CNF", "CNF", 3, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Log", "Log", 8, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Observação", "Observacao", 15, Models.Grid.Align.left, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTeEmitido ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCTeEmitido()
            {
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoEmpresaPai = this.Empresa.Codigo,
                CpfCnpjEmbarcador = Request.GetDoubleParam("Embarcador"),
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                DataAutorizacaoInicial = Request.GetDateTimeParam("DataAutorizacaoInicial"),
                DataAutorizacaoFinal = Request.GetDateTimeParam("DataAutorizacaoFinal"),
            };
        }

        #endregion
    }
}

using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/CTe/AFRMMControl")]
    public class AFRMMControlController : BaseController
    {
		#region Construtores

		public AFRMMControlController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R201_AFRMM;

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                int.TryParse(Request.Params("Codigo"), out int codigoRelatorio);

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "CT-e Ativos para o AFRMM-Control", "CTe", "AFRMMControl.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true, 10, "Arial", false, 0);
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

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControl filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.CTes.AFRMMControl servicoRelatorioAFRMControl = new Servicos.Embarcador.Relatorios.CTes.AFRMMControl(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioAFRMControl.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.CTe.AFRMMControl> listaAFRMControl, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaAFRMControl);

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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControl filtrosPesquisa = ObterFiltrosPesquisa();
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

        private Models.Grid.Grid GridPadrao()
        {
            decimal TamanhoColunasMedia = 5;
            decimal TamanhoColunasDescritivos = 10;
            decimal TamanhoColunasPequena = 3;

            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.Prop("FilialFormatada").Nome("FILIAL").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Agr(true);
            grid.Prop("CodigoFilial").Nome("CODIGO").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("Navio").Nome("NAVIO").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("Viagem").Nome("VIAGEM").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Agr(true);
            grid.Prop("Direcao").Nome("DIRECAO").Tamanho(TamanhoColunasPequena).Align(Models.Grid.Align.left);
            grid.Prop("POL").Nome("POL").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Agr(true);
            grid.Prop("POLCodigo").Nome("CODIGO").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("POD").Nome("POD").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Agr(true);
            grid.Prop("PODCodigo").Nome("CODIGO").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("CTAC").Nome("CTAC").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("NumeroFiscal").Nome("Nº_FISCAL").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left);
            grid.Prop("ChaveCTe").Nome("CHAVE_CTE").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("DataEmissaoFormatada").Nome("DATA_EMISSAO").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.center);
            grid.Prop("CTM").Nome("CTM").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ChaveCTM").Nome("CHAVE_CTM").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ChaveCTeCliente").Nome("Chave CTe Cliente").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ChaveNFe").Nome("CHAVE_NFE").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("TipoDocumento").Nome("Tipo").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("StatusCTe").Nome("Status").Tamanho(TamanhoColunasMedia).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("ObservacaoComplementar").Nome("Descrição").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);
            grid.Prop("MotivoCancelamento").Nome("Motivo Cancelamento").Tamanho(TamanhoColunasDescritivos).Align(Models.Grid.Align.left).Visibilidade(false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControl ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioAFRMMControl()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal")
            };
        }

        #endregion
    }
}

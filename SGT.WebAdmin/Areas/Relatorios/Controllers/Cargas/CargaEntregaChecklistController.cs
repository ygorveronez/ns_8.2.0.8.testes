using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/Cargas/CargaEntregaChecklist")]
    public class CargaEntregaChecklistController : BaseController
    {
		#region Construtores

		public CargaEntregaChecklistController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R289_CargaEntregaChecklist;
        private readonly int _limitePergustasRespostas = 60;

        #endregion Atributos

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await servicoRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Checklist de Coleta/Entrega", "Cargas", "CargaEntregaChecklist.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, false, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(await ObterGridPadraoAsync(unitOfWork, cancellationToken), relatorio, buscarCabecalhoPorCodigoDinamico: true);

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

        public async Task<IActionResult> PesquisaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioCargaEntregaChecklist filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedadesAgrupamento = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.ControleEntrega.CargaEntregaChecklist servicoRelatorioCargaEntregaChecklist = new Servicos.Embarcador.Relatorios.ControleEntrega.CargaEntregaChecklist(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioCargaEntregaChecklist.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.CargaEntregaChecklist> listaChecklist, out int totalRegistros, filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaChecklist);

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

        public async Task<IActionResult> GerarRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioCargaEntregaChecklist filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedadesAgrupamento = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, propriedadesAgrupamento, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);
                string stringConexao = _conexao.StringConexao;

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

        #endregion Métodos Globais

        #region Métodos Privados

        private async Task<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioCargaEntregaChecklist> ObterFiltrosPesquisaAsync(Repositorio.UnitOfWork unitOfWork,
            CancellationToken cancellationToken)
        {
            int codigoFilial = Request.GetIntParam("Filial");

            return new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRelatorioCargaEntregaChecklist()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                CodigoCheckListTipo = Request.GetIntParam("CheckListTipo"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                DataCarregamentoInicial = Request.GetNullableDateTimeParam("DataCarregamentoInicial"),
                DataCarregamentoFinal = Request.GetNullableDateTimeParam("DataCarregamentoFinal"),
                CodigoFilial = codigoFilial,
                Filiais = codigoFilial == 0 ? await ObterListaCodigoFilialPermitidasOperadorLogisticaAsync(unitOfWork, cancellationToken) : new List<int>() { codigoFilial },
                Recebedores = await ObterListaCnpjCpfRecebedorPermitidosOperadorLogisticaAsync(unitOfWork, cancellationToken),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigosMotoristas = Request.GetListParam<int>("Motorista"),
                CodigoRemetentePecuarista = Request.GetIntParam("RemetentePecuaria"),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                DataCargaInicial = Request.GetNullableDateTimeParam("DataCargaInicial"),
                DataCargaFinal = Request.GetNullableDateTimeParam("DataCargaFinal")
            };
        }

        private async Task<Models.Grid.Grid> ObterGridPadraoAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Carga", "Carga", 6, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Placa", "Placa", 4, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data de Criação da carga", "DataCriacaoCarga", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data de Embarque", "DataDeEmbarque", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Nome do Motorista", "NomeMotorista", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("KM Planejado", "KMPlanejados", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("KM Realizados", "KMRealizados", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data e Horário de Inicio", "InicioViagem", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Local", "Local", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Fun. Resp. Entrega Manual", "FuncionarioResponsavelEntrega", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Data Entrega", "DataEntrega", 10, Models.Grid.Align.left, true, false, false, true, false);

            Repositorio.Embarcador.GestaoPatio.CheckListOpcoes repositorioChecklistOpcoes = new Repositorio.Embarcador.GestaoPatio.CheckListOpcoes(unitOfWork);
            List<Dominio.Entidades.Embarcador.GestaoPatio.CheckListOpcoes> listaPerguntas = await repositorioChecklistOpcoes.BuscarPerguntasPorCheckListTipoAsync(Request.GetIntParam("CheckListTipo"), cancellationToken);
            int totalPerguntas = Math.Min(listaPerguntas.Count, _limitePergustasRespostas);

            for (int i = 0; i < totalPerguntas; i++)
                grid.AdicionarCabecalho(listaPerguntas[i].Descricao, $"ColunaDinamica{i}", 10, Models.Grid.Align.left, false, false, false, false, TipoSumarizacao.nenhum, CodigoDinamico: listaPerguntas[i].Codigo);

            return grid;
        }

        #endregion Métodos Privados
    }
}

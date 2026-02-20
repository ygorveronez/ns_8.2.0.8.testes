using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Frota
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Frota/PneuHistorico")]
    public class PneuHistoricoController : BaseController
    {
		#region Construtores

		public PneuHistoricoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R156_PneuHistorico;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await servicoRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Histórico de Movimentação de Pneus", "Frota", "PneuHistorico.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);
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

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Frota.PneuHistorico servicoRelatorioPneuHistorico = new Servicos.Embarcador.Relatorios.Frota.PneuHistorico(unitOfWork, TipoServicoMultisoftware, Cliente);

                var lista = await servicoRelatorioPneuHistorico.ConsultarRegistrosAsync(filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(lista.Count);
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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

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
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatório.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaPneuHistorico()
            {
                CodigoBandaRodagem = Request.GetIntParam("BandaRodagem"),
                CodigoDimensao = Request.GetIntParam("Dimensao"),
                CodigoMarca = Request.GetIntParam("Marca"),
                CodigoModelo = Request.GetIntParam("Modelo"),
                CodigoPneu = Request.GetIntParam("Pneu"),
                DataInicio = Request.GetNullableDateTimeParam("DataInicio"),
                DataLimite = Request.GetNullableDateTimeParam("DataLimite"),
                Vida = Request.GetNullableEnumParam<VidaPneu>("Vida"),
                SituacaoPneu = Request.GetNullableEnumParam<SituacaoPneu>("SituacaoPneu"),
                SomenteSucata = Request.GetBoolParam("SomenteSucata"),
                CodigoServico = Request.GetIntParam("Servico"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                DTO = Request.GetStringParam("DTO"),
                CodigoUsuarioOperador = Request.GetIntParam("UsuarioOperador"),
                CodigoMotivoSucata = Request.GetListParam<int>("MotivoSucata"),
                CodigoAlmoxarifado = Request.GetListParam<int>("Almoxarifado"),
                TiposAquisicao = Request.GetListEnumParam<TipoAquisicaoPneu>("TipoAquisicao")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe)
                filtrosPesquisa.CodigoEmpresa = this.Usuario.Empresa?.Codigo ?? 0;

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Número de Fogo", "NumeroFogo", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data", "DataFormatada", 10, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 26, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Banda de Rodagem", "BandaRodagem", 12, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Banda de Rodagem no Histórico", "BandaRodagemHistorico", 12, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Dimensão", "Dimensao", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Marca", "Marca", 12, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Modelo", "Modelo", 12, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Vida", "VidaDescricao", 12, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Observação Sucata", "ObservacaoSucata", 12, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Serviços", "Servicos", 12, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("KM Rodado", "KmAtualRodado", 12, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("DOT", "DTO", 12, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Custo estimado serviço", "CustoEstimadoReforma", 10, Models.Grid.Align.right, true, false, false, false, false);                                      
            grid.AdicionarCabecalho("Data Movimentação", "DataMovimentacaoFormatada", 10, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Usuário/Operador", "UsuarioOperador", 10, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Motivo de sucatemento", "MotivoSucata", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Almoxarifado", "Almoxarifado", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Tipo de Aquisição", "TipoAquisicaoDescricao", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Sulco Anterior", "SulcoAnterior", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Sulco", "Sulco", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Sulco Gasto", "SulcoGasto", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor RECAPE", "ValorResidualAtualPneu", 8, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Observação", "ObservacaoPneu", 8, Models.Grid.Align.center, true, false, false, false, false);

            return grid;
        }

        #endregion
    }
}

using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Veiculos
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Veiculos/Manutencao")]
    public class ManutencaoController : BaseController
    {
		#region Construtores

		public ManutencaoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R052_Manutencao;

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, Localization.Resources.Relatorios.Veiculos.Veiculo.RelatorioManutencao, "Veiculos", "Manutencao.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Placa", "asc", "", "", Codigo, unitOfWork, true, false);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);
                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Relatorios.Veiculos.Veiculo.OcorreuUmaFalhaAoBuscarOsDadosDoRelatorio );
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

                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioManutencao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Veiculos.Manutencao servicoRelatorioManutencao = new Servicos.Embarcador.Relatorios.Veiculos.Manutencao(unitOfWork, TipoServicoMultisoftware, Cliente, cancellationToken);

                var lista = await servicoRelatorioManutencao.ConsultarRegistrosAsync(filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(lista.Count);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Relatorios.Veiculos.Veiculo.OcorreuUmaFalhaConsultar);
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
                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioManutencao filtrosPesquisa = ObterFiltrosPesquisa();

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
                return new JsonpResult(false, Localization.Resources.Relatorios.Veiculos.Veiculo.OcorreuUmaFalhaGerarRelatorio);
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
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.DataEmissao, "DataEmissaoFormatada", 8, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Fornecedor, "Fornecedor", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Placa, "Placa", 8, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Segmento, "Segmento", 8, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.TipoMovimento, "TipoMovimento", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.NaturezaOperacao, "NaturezaOperacao", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Produto, "Produto", 20, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Quantidade, "Quantidade", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.ValorUnitario, "ValorUnitario", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.ValorTotal, "ValorTotal", 8, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Equipamento, "Equipamento", 8, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.CustoUnitario, "ValorCustoUnitario", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.CustoTotal, "ValorCustoTotal", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.CentroResultado, "CentroResultado", 8, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.NumeroDocumento, "NumeroDocumento", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.SerieDocumento, "SerieDocumento", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.CNPJFornecedor, "CNPJFornecedor", 8, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.KMVeiculo, "QuilometragemVeiculo", 8, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Horimetro, "Horimetro", 6, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.SituacaoLanDocEntrada, "DescricaoSituacaoLancDocEntrada", 10, Models.Grid.Align.center, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioManutencao ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioManutencao()
            {
                CodigoTipoMovimento = Request.GetIntParam("TipoMovimento"),
                CodigosNaturezaOperacao = Request.GetListParam<int>("NaturezaOperacao"),
                CodigosVeiculo = Request.GetListParam<int>("Veiculo"),
                CodigosSegmento = Request.GetListParam<int>("Segmento"),
                CodigosEquipamento = Request.GetListParam<int>("Equipamento"),
                CodigoPlacas = Request.GetListParam<int>("Placas"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                CnpjCpfFornecedor = Request.GetDoubleParam("Fornecedor"),
                CentrosResultado = Request.GetListParam<int>("CentroResultado"),
                Produtos = Request.GetListParam<int>("Produto"),
                ExibirApenasComVeiculoOuEquipamento = Request.GetNullableBoolParam("ExibirApenasComVeiculoOuEquipamento"),
                SituacaoLancDocEntrada = Request.GetNullableEnumParam<SituacaoDocumentoEntrada>("SituacaoLancDocEntrada")
            };
        }
    
        #endregion
    }
}

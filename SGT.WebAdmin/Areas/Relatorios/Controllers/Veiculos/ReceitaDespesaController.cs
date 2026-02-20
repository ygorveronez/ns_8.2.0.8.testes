using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Veiculos
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Veiculos/ReceitaDespesa")]
    public class ReceitaDespesaController : BaseController
    {
		#region Construtores

		public ReceitaDespesaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R162_ReceitaDespesaVeiculo;

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await servicoRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, Localization.Resources.Relatorios.Veiculos.Veiculo.RelatorioReceitasDespesasVeiculo, "Veiculos", "VeiculoReceitaDespesa.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "PlacaVeiculo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dadosRelatorio = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(dadosRelatorio);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Relatorios.Veiculos.Veiculo.OcorreuUmaFalhaAoBuscarOsDadosDoRelatorio);
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

                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaReceitaDespesa filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);

                Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa repVeiculoReceitaDespesa = new Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa(unitOfWork, cancellationToken);

                int totalRegistros = await repVeiculoReceitaDespesa.ContarConsultaRelatorio(filtrosPesquisa, propriedades);

                IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.ReceitaDespesa> listaReceitasDespesas = totalRegistros > 0 ? await repVeiculoReceitaDespesa.ConsultarRelatorioAsync(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.ReceitaDespesa>();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaReceitasDespesas);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Relatorios.Veiculos.Veiculo.OcorreuUmaFalhaAoConsultar);
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
                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaReceitaDespesa filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio svcRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = svcRelatorio.AdicionarRelatorioParaGeracao(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = svcRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemporario.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);

                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Relatorios.Veiculos.Veiculo.OcorreuUmaFalhaGerarRelatorio);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private async Task GerarRelatorio(
            Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaReceitaDespesa filtrosPesquisa,
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades,
            Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta,
            Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, 
            Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario,
            string stringConexao,
            CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa repVeiculoReceitaDespesa = new Repositorio.Embarcador.Veiculos.VeiculoReceitaDespesa(unitOfWork, cancellationToken);

                IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.ReceitaDespesa> dsReceitaDespesa = await repVeiculoReceitaDespesa.ConsultarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta);

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = ObterParametrosRelatorio(unitOfWork, filtrosPesquisa);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Veiculos/ReceitaDespesa", parametros,relatorioControleGeracao, relatorioTemporario, dsReceitaDespesa, unitOfWork, null, null, true, TipoServicoMultisoftware);
            }
            catch (Exception excecao)
            {
               await servicoRelatorio.RegistrarFalhaGeracaoRelatorioAsync(relatorioControleGeracao, unitOfWork, excecao, cancellationToken);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaReceitaDespesa ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaReceitaDespesa filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaReceitaDespesa()
            {
                CodigoModeloVeicular = Request.GetIntParam("ModeloVeicular"),
                CodigoSegmentoVeiculo = Request.GetIntParam("SegmentoVeiculo"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial")
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.DescricaoVeiculos, "PlacaVeiculo", 2, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.ModeloVeicular, "ModeloVeicular", 6, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Segmento, "SegmentoVeiculo", 6, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.CTe, "ResultadoCTe", 2, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Abastecimento, "ResultadoAbastecimento", 2, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Pedagio, "ResultadoPedagio", 2, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Titulo, "ResultadoTitulo", 2, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.DocumentoEntrada, "ResultadoDocumentoEntrada", 2, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.OrdemServico, "ResultadoOrdemServico", 2, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.AcertoViagem, "ResultadoAcertoViagem", 2, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Pneu, "ResultadoPneu", 2, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Outros, "ResultadoOutros", 2, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.TotalGeral, "ResultadoGeral", 2, Models.Grid.Align.right, true, false, false, true, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);

            return grid;
        }

        private List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> ObterParametrosRelatorio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaReceitaDespesa filtrosPesquisa)
        {
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.ModeloVeicularCarga repModeloVeicularCarga = new Repositorio.Embarcador.Cargas.ModeloVeicularCarga(unitOfWork);
            Repositorio.Embarcador.Veiculos.SegmentoVeiculo repSegmentoVeiculo = new Repositorio.Embarcador.Veiculos.SegmentoVeiculo(unitOfWork);

            Dominio.Entidades.Embarcador.Veiculos.SegmentoVeiculo segmentoVeiculo = filtrosPesquisa.CodigoSegmentoVeiculo > 0 ? repSegmentoVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoSegmentoVeiculo) : null;
            Dominio.Entidades.Embarcador.Cargas.ModeloVeicularCarga modeloVeicularCarga = filtrosPesquisa.CodigoModeloVeicular > 0 ? repModeloVeicularCarga.BuscarPorCodigo(filtrosPesquisa.CodigoModeloVeicular) : null;
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;

            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataInicial", filtrosPesquisa.DataInicial));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("DataFinal", filtrosPesquisa.DataFinal));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Veiculo", veiculo?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("ModeloVeicularCarga", modeloVeicularCarga?.Descricao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("SegmentoVeiculo", segmentoVeiculo?.Descricao));
            
            return parametros;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar.Contains("Abastecimento"))
                return "(ISNULL(ReceitaAbastecimento, 0) - ISNULL(DespesaAbastecimento, 0))";
            else if (propriedadeOrdenar.Contains("Titulo"))
                return "(ISNULL(ReceitaTitulo, 0) - ISNULL(DespesaTitulo, 0))";
            else if (propriedadeOrdenar.Contains("Pneu"))
                return "(ISNULL(ReceitaPneu, 0) - ISNULL(DespesaPneu, 0))";
            else if (propriedadeOrdenar.Contains("DocumentoEntrada"))
                return "(ISNULL(ReceitaDocumentoEntrada, 0) - ISNULL(DespesaDocumentoEntrada, 0))";
            else if (propriedadeOrdenar.Contains("CTe"))
                return "(ISNULL(ReceitaCTe, 0) - ISNULL(DespesaCTe, 0))";
            else if (propriedadeOrdenar.Contains("Pedagio"))
                return "(ISNULL(ReceitaPedagio, 0) - ISNULL(DespesaPedagio, 0))";
            else if (propriedadeOrdenar.Contains("OrdemServico"))
                return "(ISNULL(ReceitaOrdemServico, 0) - ISNULL(DespesaOrdemServico, 0))";
            else if (propriedadeOrdenar.Contains("AcertoViagem"))
                return "(ISNULL(ReceitaAcertoViagem, 0) - ISNULL(DespesaAcertoViagem, 0))";
            else if (propriedadeOrdenar.Contains("Outros"))
                return "(ISNULL(ReceitaOutros, 0) - ISNULL(DespesaOutros, 0))";
            else if (propriedadeOrdenar == "ResultadoGeral")
                return @"((ISNULL(ReceitaAbastecimento, 0) - ISNULL(DespesaAbastecimento, 0)) + 
                          (ISNULL(ReceitaTitulo, 0) - ISNULL(DespesaTitulo, 0)) + 
                          (ISNULL(ReceitaPneu, 0) - ISNULL(DespesaPneu, 0)) +
                          (ISNULL(ReceitaDocumentoEntrada, 0) - ISNULL(DespesaDocumentoEntrada, 0)) +
                          (ISNULL(ReceitaCTe, 0) - ISNULL(DespesaCTe, 0)) +
                          (ISNULL(ReceitaPedagio, 0) - ISNULL(DespesaPedagio, 0)) +
                          (ISNULL(ReceitaOrdemServico, 0) - ISNULL(DespesaOrdemServico, 0)) +
                          (ISNULL(ReceitaAcertoViagem, 0) - ISNULL(DespesaAcertoViagem, 0)) +
                          (ISNULL(ReceitaOutros, 0) - ISNULL(DespesaOutros, 0)))";

            return propriedadeOrdenar;
        }

        #endregion
    }
}

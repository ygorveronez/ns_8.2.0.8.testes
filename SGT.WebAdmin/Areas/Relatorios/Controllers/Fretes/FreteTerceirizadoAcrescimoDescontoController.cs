using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Fretes
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Fretes/FreteTerceirizadoAcrescimoDesconto")]
    public class FreteTerceirizadoAcrescimoDescontoController : BaseController
    {
		#region Construtores

		public FreteTerceirizadoAcrescimoDescontoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R267_FreteTerceirizadoAcrescimoDesconto;

        private readonly decimal TamanhoColunaPequena = 1.75m;
        private readonly decimal TamanhoColunaGrande = 5.50m;
        private readonly decimal TamanhoColunaMedia = 3m;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Fretes Terceirizados com Acréscimos/Descontos", "Fretes", "FreteTerceirizadoAcrescimoDesconto.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "ContratoFrete", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

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

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoAcrescimoDesconto filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Fretes.FreteTerceirizadoAcrescimoDesconto servicoRelatorioFreteTerceirizado = new Servicos.Embarcador.Relatorios.Fretes.FreteTerceirizadoAcrescimoDesconto(unitOfWork, TipoServicoMultisoftware, Cliente);

                var lista = await servicoRelatorioFreteTerceirizado.ConsultarRegistrosAsync(filtrosPesquisa, agrupamentos, parametrosConsulta);

                //servicoRelatorioFreteTerceirizado.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Fretes.FreteTerceirizadoAcrescimoDesconto> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoAcrescimoDesconto filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException servicoException)
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

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Contrato de Frete", "ContratoFrete", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissaoFormatada", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CIOT", "NumeroCIOT", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);

            grid.AdicionarCabecalho("CPF/CNPJ Terceiro", "CPFCNPJTerceiroFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Terceiro", "Terceiro", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Veículo", "Veiculo", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Motorista", "Motorista", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, false);

            grid.AdicionarCabecalho("Valor Acréscimo", "ValorAcrescimo", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor Desconto", "ValorDesconto", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Justificativa", "Justificativa", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Taxa Terceiro", "TaxaTerceiro", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoAcrescimoDesconto ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaRelatorioFreteTerceirizadoAcrescimoDesconto()
            {
                CpfCnpjTerceiro = Request.GetDoubleParam("Terceiro"),
                DataEmissaoContratoInicial = Request.GetDateTimeParam("DataEmissaoContratoInicial"),
                DataEmissaoContratoFinal = Request.GetDateTimeParam("DataEmissaoContratoFinal"),
                Veiculo = Request.GetIntParam("Veiculo"),
                NumeroContrato = Request.GetIntParam("NumeroContrato"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                Situacao = Request.GetListEnumParam<SituacaoContratoFrete>("Situacao")
            };
        }

        #endregion
    }
}

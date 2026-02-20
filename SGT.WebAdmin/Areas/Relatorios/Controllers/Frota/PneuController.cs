using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Frota
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Frota/Pneu")]
    public class PneuController : BaseController
    {
		#region Construtores

		public PneuController(Conexao conexao) : base(conexao) { }

		#endregion


        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R290_Pneu;

        #endregion

        #region Propriedades

        private decimal _tamanhoColunaPequena = 1.75m;
        private decimal _tamanhoColunaGrande = 5.50m;
        private decimal _tamanhoColunaMedia = 3m;

        #endregion

        #region Métodos Publicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int codigoRelatorio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Pneus", "Frota", "Pneu.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "NumeroFogo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
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

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneu filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Frota.Pneu servicoPneu = new Servicos.Embarcador.Relatorios.Frota.Pneu(unitOfWork, TipoServicoMultisoftware, Cliente);

                var lista = await servicoPneu.ConsultarRegistrosAsync(filtrosPesquisa, agrupamentos, parametrosConsulta);

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
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneu filtrosPesquisa = ObterFiltrosPesquisa();
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
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o reltároio.");
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
            grid.AdicionarCabecalho("Numero de fogo", "NumeroFogo", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Frota", "Frota", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Veículo", "Placa", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Modelo veicular", "ModeloVeicular", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Modelo do pneu", "ModeloPneu", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Marca do pneu", "MarcaPneu", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Tipo da banda", "TipoBandaRodagemDescricao", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Status do pneu", "StatusPneuDescricao", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data de entrada", "DataEntradaFormatada", _tamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Banda de rodagem", "BandaRodagem", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Almoxarifado atual", "AlmoxarifadoAtual", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Movimentação", "Movimentacao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Km Rodado", "KmRodado", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Vida Útil", "VidaUtilDescricao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("TP. Aquisição", "TPAquisicaoDescricao", _tamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneu ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroPesquisaRelatorioPneu()
            {
                CodigoPneu = Request.GetIntParam("Pneu"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoModeloPneu = Request.GetIntParam("ModeloPneu"),
                CodigoMarcaPneu = Request.GetIntParam("MarcaPneu"),
                TiposBandaRodagem = Request.GetListEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoBandaRodagemPneu>("TipoBandaRodagem"),
                StatusPneu = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoPneu>("StatusPneu"),
                Movimentacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SimNao>("Movimentacao"),
                VidaUtil = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.VidaPneu>("VidaUtil"),
            };
        }

        #endregion

    }
}

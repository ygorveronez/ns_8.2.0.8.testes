using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.ConfiguracaoContabil
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/ConfiguracaoContabil/ConfiguracaoCentroResultado")]
    public class ConfiguracaoCentroResultadoController : BaseController
    {
		#region Construtores

		public ConfiguracaoCentroResultadoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R298_ConfiguracaoCentroResultado;

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Configuração Centro de Resultado", "ConfiguracaoContabil", "ConfiguracaoCentroResultado.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(unitOfWork), relatorio);

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

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaConfiguracaoCentroResultado filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.ConfiguracaoContabil.ConfiguracaoCentroResultado servicoRelatorioConfiguracaoCentroResultado = new Servicos.Embarcador.Relatorios.ConfiguracaoContabil.ConfiguracaoCentroResultado(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioConfiguracaoCentroResultado.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.ConfiguracaoContabil.ConfiguracaoCentroResultado> listaConfiguracaoCentroResultado, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaConfiguracaoCentroResultado);

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

        [AllowAuthenticate]
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
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaConfiguracaoCentroResultado filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
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

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Remetente", "RemetenteFormatado", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Destinatário", "DestinatarioFormatado", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Tomador", "TomadorFormatado", 8, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", 8, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Tipo Ocorrência", "TipoOcorrencia", 8, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Grupo Produto", "GrupoProduto", 8, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Rota", "RotaFrete", 8, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação", "SituacaoDescricao", 8, Models.Grid.Align.right, false, false, false, false, true);
            grid.AdicionarCabecalho("Centro de Resultado Contabilização", "CentroResultadoContabilizacao", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Centro de Resultado Escrituração", "CentroResultadoEscrituracao", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Centro de Resultado ICMS", "CentroResultadoICMS", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Centro de Resultado PIS", "CentroResultadoPIS", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Centro de Resultado COFINS", "CentroResultadoCOFINS", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Item do Serviço", "ItemServico", 10, Models.Grid.Align.left, false, false, false, false, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaConfiguracaoCentroResultado ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.ConfiguracaoContabil.FiltroPesquisaConfiguracaoCentroResultado()
            {
                Remetente = Request.GetIntParam("Remetente"),
                Destinatario = Request.GetIntParam("Destinatario"),
                Tomador = Request.GetIntParam("Tomador"),
                Transportador = Request.GetIntParam("Transportador"),
                TipoOperacao = Request.GetIntParam("TipoOperacao"),
                TipoOcorrencia = Request.GetIntParam("TipoOcorrencia"),
                GrupoProduto = Request.GetIntParam("GrupoProduto"),
                RotaFrete = Request.GetIntParam("RotaFrete"),
                Situacao = Request.GetNullableBoolParam("Situacao"),
                CentroResultadoContabilizacao = Request.GetIntParam("CentroResultadoContabilizacao"),
                CentroResultadoEscrituracao = Request.GetIntParam("CentroResultadoEscrituracao"),
                CentroResultadoICMS = Request.GetIntParam("CentroResultadoICMS"),
                CentroResultadoPIS = Request.GetIntParam("CentroResultadoPIS"),
                CentroResultadoCOFINS = Request.GetIntParam("CentroResultadoCOFINS"),
            };
        }

        #endregion
    }
}

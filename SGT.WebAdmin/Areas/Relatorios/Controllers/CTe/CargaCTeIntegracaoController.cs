using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.CTe
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/CTe/CargaCTeIntegracao")]
    public class CargaCTeIntegracaoController : BaseController
    {
		#region Construtores

		public CargaCTeIntegracaoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R097_CargaCTeIntegracao;
        private decimal TamanhoColumaExtraPequena = 1m;
        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaGrande = 5.50m;
        private decimal TamanhoColunaMedia = 3m;

        #endregion

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio;
                int.TryParse(Request.Params("Codigo"), out codigoRelatorio);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Integrações de CT-e", "CTe", "CargaCTeIntegracao.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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

                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCargaCTeIntegracao filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.CTes.CargaCTeIntegracao servicoRelatorioCargaCTeIntegracao = new Servicos.Embarcador.Relatorios.CTes.CargaCTeIntegracao(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioCargaCTeIntegracao.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.CTe.Integracao.CargaCTeIntegracao> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
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
                Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCargaCTeIntegracao filtrosPesquisa = ObterFiltrosPesquisa();

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

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Nº CT-e", "NumeroCTe", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Série CT-e", "SerieCTe", TamanhoColumaExtraPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Emissão", "DataEmissao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Integracão", "DataIntegracao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo Integracão", "TipoIntegracao", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Retorno Integracão", "Mensagem", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação Integração", "DescricaoSituacao", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Tentativas", "Tentativas", TamanhoColumaExtraPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Documento Transporte", "NumeroDocumentoTransporte", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("% ICMS", "AliquotaICMS", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Valor do ICMS", "ValorICMS", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor do Frete", "ValorFrete", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);
            grid.AdicionarCabecalho("Valor a Receber", "ValorAReceber", TamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoSumarizacao.sum);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCargaCTeIntegracao ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCargaCTeIntegracao filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.CTe.FiltroPesquisaRelatorioCargaCTeIntegracao()
            {
                DataEmissaoInicial = Request.GetDateTimeParam("DataEmissaoInicial"),
                DataEmissaoFinal = Request.GetDateTimeParam("DataEmissaoFinal"),
                DataIntegracaoInicial = Request.GetDateTimeParam("DataIntegracaoInicial"),
                DataIntegracaoFinal = Request.GetDateTimeParam("DataIntegracaoFinal"),
                GrupoPessoas = Request.GetIntParam("GrupoPessoas"),
                CTe = Request.GetIntParam("CTe"),
                Carga = Request.GetIntParam("Carga"),
                TipoIntegracao = Request.GetIntParam("TipoIntegracao"),
            };

            return filtrosPesquisa;
        }

        #endregion
    }
}

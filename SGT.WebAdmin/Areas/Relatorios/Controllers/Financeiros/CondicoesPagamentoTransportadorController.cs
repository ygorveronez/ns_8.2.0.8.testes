using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Financeiros/CondicoesPagamentoTransportador")]
    public class CondicoesPagamentoTransportadorController : BaseController
    {
		#region Construtores

		public CondicoesPagamentoTransportadorController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R322_CondicoesPagamentoTransportador;

        private decimal TamanhoColunaExtraPequena = 1m;
        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaMedia = 3m;
        private decimal TamanhoColunaGrande = 5.50m;

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int Codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Condições de Pagamento por Transportador", "Financeiros", "CondicoesPagamentoTransportador.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, true);

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

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCondicoesPagamentoTransportador filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.CondicoesPagamentoTransportador servicoRelatorioCondicoesPagamentoTransportador = new Servicos.Embarcador.Relatorios.Financeiros.CondicoesPagamentoTransportador(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioCondicoesPagamentoTransportador.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.RelatorioCondicoesPagamentoTransportador> listaPagamentos, out int totalRegistros, filtrosPesquisa, propriedades, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaPagamentos);

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

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio svcRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = svcRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCondicoesPagamentoTransportador filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = svcRelatorio.AdicionarRelatorioParaGeracao(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

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

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCondicoesPagamentoTransportador ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCondicoesPagamentoTransportador filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaCondicoesPagamentoTransportador
            {
                CodigoIntegracao = Request.GetStringParam("CodigoIntegracao"),
                Estado = Request.GetStringParam("Estado") == "0" ? String.Empty : Request.GetStringParam("Estado"),
                CodigoEmpresa = Request.GetIntParam("Empresa"),
                CodigoTipoCarga = Request.GetIntParam("TipoCarga"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Razão Social", "RazaoSocial", TamanhoColunaGrande, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Código de Integração", "CodigoIntegracao", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("CNPJ", "CNPJFormatado", TamanhoColunaPequena, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("UF", "SiglaUF", TamanhoColunaExtraPequena, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Dia da Semana", "DiaSemanaDescricao", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho("Dia do Mês", "DiaMesFormatado", TamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Dia de Emissão Limite", "DiaEmissaoLimiteFormatado", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Vencimento Fora do Mês", "VencimentoForaMesFormatado", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Tipo de Prazo", "TipoPrazoPagamentoDescricao", TamanhoColunaGrande, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Dias de Prazo", "DiasDePrazoPagamento", TamanhoColunaPequena, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Tipo de Carga", "TipoCargaDescricao", TamanhoColunaGrande, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacaoDescricao", TamanhoColunaGrande, Models.Grid.Align.center, false, false, false, false, false);

            return grid;
        }

        #endregion
    }
}

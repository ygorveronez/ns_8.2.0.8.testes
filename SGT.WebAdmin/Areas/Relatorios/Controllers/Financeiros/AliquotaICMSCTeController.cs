using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Financeiros
{
	[Area("Relatorios")]
	public class AliquotaICMSCTeController : BaseController
    {
		#region Construtores

		public AliquotaICMSCTeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R045_AliquotaICMSCTe;

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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Alíquotas de ICMS para Emissão de CT-e", "Financeiros", "AliquotaICMSCTe.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "EstadoEmpresa", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

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

        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioAliquotaICMSCTe filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Financeiros.AliquotaICMSCTe servicoRelatorioAliquota = new Servicos.Embarcador.Relatorios.Financeiros.AliquotaICMSCTe(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioAliquota.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Financeiros.AliquotaICMSCTe> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioAliquotaICMSCTe filtrosPesquisa = ObterFiltrosPesquisa();
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

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("UF da Empresa", "EstadoEmpresa", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("UF de Origem", "EstadoOrigem", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("UF de Destino", "EstadoDestino", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CST", "CST", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Alíquota", "Aliquota", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Atividade Tomador", "Atividade", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Atividade Destinatário", "AtividadeDestinatario", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("CFOP", "CFOP", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);

            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            if (configuracaoTMS.ExibirCFOPCompra)
                grid.AdicionarCabecalho("CFOP de Compra", "CFOPCompra", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);


            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioAliquotaICMSCTe ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioAliquotaICMSCTe filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Financeiro.FiltroPesquisaRelatorioAliquotaICMSCTe()
            {
                EstadoDestino = Request.GetStringParam("EstadoDestino"),
                EstadoEmpresa = Request.GetStringParam("EstadoEmpresa"),
                EstadoOrigem = Request.GetStringParam("EstadoOrigem"),
            };

            return filtrosPesquisa;
        }

        #endregion
    }
}

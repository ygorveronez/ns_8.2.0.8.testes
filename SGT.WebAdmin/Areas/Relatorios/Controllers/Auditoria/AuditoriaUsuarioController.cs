using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using System.Linq;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Auditoria
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Auditoria/Auditoria")]
    public class AuditoriaUsuarioController : BaseController
    {
		#region Construtores

		public AuditoriaUsuarioController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R257_AuditoriaUsuario;
        private readonly decimal _tamanhoColumaExtraPequena = 1m;
        private readonly decimal _tamanhoColunaGrande = 5.50m;
        private readonly decimal _tamanhoColunaMedia = 3m;
        private readonly decimal _tamanhoColunaPequena = 1.75m;

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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await servicoRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório Auditoria Usuários", "Auditoria", "AuditoriaUsuario.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Data", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
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
                Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaRelatorioAuditoria filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Auditoria.AuditoriaUsuario servicoRelatorioAuditoriaUsuario = new Servicos.Embarcador.Relatorios.Auditoria.AuditoriaUsuario(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioAuditoriaUsuario.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Auditoria.AuditoriaUsuario> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao consultar.");
            }
        }

        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                string stringConexao = _conexao.StringConexao;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Repositorio.Auditoria.HistoricoObjeto repAuditoria = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio svcRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = svcRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaRelatorioAuditoria filtrosPesquisa = ObterFiltrosPesquisa();
                int totalRegistros = repAuditoria.ContarConsultaRelatorioAuditoriaUsuario(filtrosPesquisa, null); // TODO (ct-reports): Repassar CT

                if (totalRegistros == 0)
                    return new JsonpResult(false, $@"Não é possível gerar um relatório de auditoria do usuário sem registros.");

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await svcRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, servicoException.Message);
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

        public async Task<IActionResult> PesquisaMenu(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
           {
                string descricao = Request.GetStringParam("Descricao");
                Models.Grid.Grid grid = ObterGridMenu();
                Repositorio.Auditoria.HistoricoObjeto repositorioHistoricoObjeto = new Repositorio.Auditoria.HistoricoObjeto(unitOfWork, cancellationToken);
                IList<Dominio.Relatorios.Embarcador.DataSource.Auditoria.AuditoriaObjeto> listaHistoricoObjeto = repositorioHistoricoObjeto.ConsultarMenus(descricao, grid.header[grid.indiceColunaOrdena].data, grid.dirOrdena, grid.inicio, grid.limite);
                grid.setarQuantidadeTotal(repositorioHistoricoObjeto.ContarConsultaMenus(descricao)); // TODO (ct-reports): Repassar CT

                var retorno = (
                    from obj in listaHistoricoObjeto
                    select new
                    {
                        obj.Codigo,
                        obj.Descricao,
                    }
                ).ToList();

                grid.AdicionaRows(retorno);

                return new JsonpResult(grid);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoConsultar);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaRelatorioAuditoria ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaRelatorioAuditoria filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Auditoria.FiltroPesquisaRelatorioAuditoria()
            {
                CodigosUsuario = Request.GetNullableListParam<int>("Usuario"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                Menus = Request.GetNullableListParam<string>("Menu"),
                AcaoRealizada = Request.GetNullableStringParam("AcaoRealizada")
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Usuário", "Usuario", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Objeto", "Menu", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Ação", "Acao", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Horário/Data", "Data", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Propriedade", "Propriedade", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Valor Anterior", "ValorAntigo", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, true);
            grid.AdicionarCabecalho("Valor Novo", "ValorNovo", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, true, true);

            return grid;
        }

        private Models.Grid.Grid ObterGridMenu()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid(Request)
            {
                header = new List<Models.Grid.Head>()
            };
            grid.AdicionarCabecalho("Codigo", "Codigo", 40, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Nome do Menu:", "Descricao", 40, Models.Grid.Align.left, true);

            return grid;
        }

        #endregion
    }
}

using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.NotasFiscais
{
    [Area("Relatorios")]
	[CustomAuthorize("Relatorios/NotasFiscais/ItemNaoConformidade")]
    public class ItemNaoConformidadeController : BaseController
    {
		#region Construtores

		public ItemNaoConformidadeController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R325_ItemNaoConformidade;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorioAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Itens Não Conformidade", "NotasFiscais", "ItemNaoConformidade.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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
                grid.scrollHorizontal = IsLayoutClienteAtivo(unitOfWork);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaRelatorioItemNaoConformidade filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.NotasFiscais.ItemNaoConformidade servicoRelatorioItemNaoConformidade = new Servicos.Embarcador.Relatorios.NotasFiscais.ItemNaoConformidade(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioItemNaoConformidade.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.NotasFiscais.ItemNaoConformidade> listaItemNaoConformidade, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(countRegistros);
                grid.AdicionaRows(listaItemNaoConformidade);

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
                Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaRelatorioItemNaoConformidade filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
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

        private Models.Grid.Grid GridPadrao(Repositorio.UnitOfWork unidadeDeTrabalho)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);

            grid.AdicionarCabecalho("Descrição", "Descricao", 10, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Código Integração", "CodigoIntegracao", 10, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Situação", "SituacaoFormatada", 10, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Nota Fiscal", "NotaFiscal", 10, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Grupo", "GrupoFormatado", 10, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("SubGrupo", "SubGrupoFormatado", 10, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Area", "AreaFormatada", 10, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Irrelevante Não Conformidade", "IrrelevanteNaoConformidadeFormatado", 10, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Permite Contingência", "PermiteContingenciaFormatado", 10, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Tipo de Regra", "TipoRegraFormatado", 10, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("ParticipanteFormatado", false);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", 10, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Filial", "Filial", 10, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 10, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("CFOP", "CFOP", 10, Models.Grid.Align.left, true, false, false, true, true);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaRelatorioItemNaoConformidade ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaRelatorioItemNaoConformidade filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.NotaFiscal.FiltroPesquisaRelatorioItemNaoConformidade()
            {
                Descricao = Request.GetStringParam("Descricao"),
                Situacao = Request.GetNullableBoolParam("Situacao"),
                Grupo = Request.GetNullableEnumParam<GrupoNC>("Grupo"),
                SubGrupo = Request.GetNullableEnumParam<SubGrupoNC>("SubGrupo"),
                Area = Request.GetNullableEnumParam<AreaNC>("Area"),
                IrrelevanteParaNC = Request.GetNullableBoolParam("IrrelevanteNaoConformidade"),
                PermiteContingencia = Request.GetNullableBoolParam("PermiteContingencia")
            };

            return filtrosPesquisa;
        }

        #endregion
    }
}

using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Ocorrencias/RegrasAutorizacaoOcorrencia")]
    public class RegrasAutorizacaoOcorrenciaController : BaseController
    {
        #region Construtores

        public RegrasAutorizacaoOcorrenciaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R192_RegrasAutorizacaoOcorrencia;

        private const decimal TamanhoColunaMedia = 10m;

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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Regras de Autorização de Ocorrências", "Ocorrencias", "RegrasAutorizacaoOcorrencia.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio relatorioRetornar = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(relatorioRetornar);
            }
            catch (Dominio.Excecoes.Embarcador.BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
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

                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioRegrasAutorizacaoOcorrencia filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Servicos.Embarcador.Relatorios.Ocorrencias.RegrasAutorizacaoOcorrencia servicoRegrasAutorizacaoOcorrencia = new Servicos.Embarcador.Relatorios.Ocorrencias.RegrasAutorizacaoOcorrencia(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRegrasAutorizacaoOcorrencia.ExecutarPesquisa(
                    out List<Dominio.Relatorios.Embarcador.DataSource.Ocorrencias.RegrasAutorizacaoOcorrencia>
                    listaRegrasAutorizacaoOcorrencia,
                    out int countRegistros,
                    filtrosPesquisa,
                    propriedades,
                    parametrosConsulta);

                grid.AdicionaRows(listaRegrasAutorizacaoOcorrencia);
                grid.setarQuantidadeTotal(countRegistros);

                return new JsonpResult(grid);
            }
            catch (Dominio.Excecoes.Embarcador.BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
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
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();

                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioRegrasAutorizacaoOcorrencia filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);

                await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Dominio.Excecoes.Embarcador.BaseException excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, excecao.Message);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        #endregion

        #region Métodos Privados
        private Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioRegrasAutorizacaoOcorrencia ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Ocorrencia.FiltroPesquisaRelatorioRegrasAutorizacaoOcorrencia()
            {
                Ativo = Request.GetEnumParam("Ativo", SituacaoAtivoPesquisa.Todos),
                CodigoAprovador = Request.GetIntParam("Aprovador"),
                DataVgenciaInicial = Request.GetNullableDateTimeParam("DataVgenciaInicial"),
                DataVigenciaLimite = Request.GetNullableDateTimeParam("DataVgenciaLimite"),
                Descricao = Request.GetStringParam("Descricao"),
                EtapaAutorizacao = Request.GetNullableEnumParam<EtapaAutorizacaoOcorrencia>("EtapaAutorizacao"),
                ExibirAlcadas = Request.GetBoolParam("ExibirAlcadas"),
                CodigosFiliais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork),
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork)
            };
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Vigência", "DataVigenciaFormatada", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Etapa", "EtapaAutorizacaoDescricao", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Dias Prazo", "DiasPrazoAprovacaoFormatado", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("N° Aprovadores", "NumeroAprovadores", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("N° Reprovadores", "NumeroReprovadores", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Prioridade", "PrioridadeAprovacao", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação", "AtivoDescricao", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Observação", "Observacao", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Tipo de Ocorrências", "TipoOcorrencia", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Valor da Ocorrência", "ValorOcorrencia", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de Operação", "TipoOperacao", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Aprovadores", "Aprovadores", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        #endregion
    }
}

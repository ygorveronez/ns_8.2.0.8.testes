using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Fretes
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Fretes/ConfiguracaoTabelaFrete")]
    public class ConfiguracaoTabelaFreteController : BaseController
    {
		#region Construtores

		public ConfiguracaoTabelaFreteController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R195_ConfiguracaoTabelaFrete;

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Configurações de Tabela de Frete", "Fretes", "ConfiguracaoTabelaFrete.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio relatorioRetornar = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(relatorioRetornar);
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

                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoTabelaFrete filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);

                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                parametrosConsulta.PropriedadeAgrupar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeAgrupar);

                Repositorio.Embarcador.Frete.TabelaFrete repositorio = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork, cancellationToken);
                
                int totalRegistros = await repositorio.ContarConsultaRelatorioConfiguracao(filtrosPesquisa, propriedades);
                
                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoTabelaFrete> listaRegras = totalRegistros > 0 ? await repositorio.ConsultarRelatorioConfiguracaoAsync(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoTabelaFrete>();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaRegras);

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
                Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoTabelaFrete filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemporario.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);

                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                parametrosConsulta.PropriedadeAgrupar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeAgrupar);

                string stringConexao = _conexao.StringConexao;                

                _ = Task.Factory.StartNew(() => GerarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));                

                return new JsonpResult(true);
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

        private async Task GerarRelatorio(
            Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoTabelaFrete filtrosPesquisa,
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
                Repositorio.Embarcador.Frete.TabelaFrete repTabelaFrete = new Repositorio.Embarcador.Frete.TabelaFrete(unitOfWork, cancellationToken);

                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = await ObterParametrosRelatorio(unitOfWork, filtrosPesquisa, cancellationToken);
                IList<Dominio.Relatorios.Embarcador.DataSource.Fretes.ConfiguracaoTabelaFrete> dataSourceTabelaFrete = await repTabelaFrete.ConsultarRelatorioConfiguracaoAsync(filtrosPesquisa, propriedades, parametrosConsulta);
                
                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Fretes/ConfiguracaoTabelaFrete", parametros,relatorioControleGeracao, relatorioTemporario, dataSourceTabelaFrete, unitOfWork, null, null, true, TipoServicoMultisoftware);
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

        private Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoTabelaFrete ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoTabelaFrete()
            {
                Situacao = Request.GetNullableBoolParam("Situacao"),
                CodigoGrupoPessoas = Request.GetListParam<int>("GrupoPessoas"),
                DataFinal = Request.GetNullableDateTimeParam("DataFinal"),
                DataInicial = Request.GetNullableDateTimeParam("DataInicial"),
                TabelasVigentes = Request.GetNullableBoolParam("TabelasVigentes")
            };
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Código", "CodigoIntegracao", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Descrição", "Descricao", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", 15, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Inicial", "DescricaoDataInicial", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Final", "DescricaoDataFinal", 5, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Situação", "DescricaoSituacao", 5, Models.Grid.Align.left, true, false, false, true, true);
            
            return grid;
        }

        private async Task<List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>> ObterParametrosRelatorio(
            Repositorio.UnitOfWork unitOfWork,
            Dominio.ObjetosDeValor.Embarcador.Frete.FiltroPesquisaConfiguracaoTabelaFrete filtrosPesquisa,
            CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Pessoas.GrupoPessoas repGrupoPessoas = new Repositorio.Embarcador.Pessoas.GrupoPessoas(unitOfWork, cancellationToken);
            
            List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro> parametros = new List<Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro>();
            
            IList<string> grupoPessoas = filtrosPesquisa.CodigoGrupoPessoas?.Count() > 0 ? await repGrupoPessoas.BuscarDescricaoPorCodigoAsync(filtrosPesquisa.CodigoGrupoPessoas) : new List<string>();

            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Ativo", filtrosPesquisa.Situacao));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("TabelasVigentes", filtrosPesquisa.TabelasVigentes));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("GrupoPessoas", string.Join(", ", grupoPessoas)));
            parametros.Add(new Dominio.Relatorios.Embarcador.ObjetosDeValor.Parametro("Vigencia", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));

            
            return parametros;
        }

        private string ObterPropriedadeOrdenar(string prop)
        {
            if (prop != "Descricao" && prop.StartsWith("Descricao"))
                prop = prop.Replace("Descricao", "");

            return prop;
        }

        #endregion
    }
}

using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Cargas/ModeloVeicularCarga")]
    public class ModeloVeicularCargaController : BaseController
    {
        #region Construtores

        public ModeloVeicularCargaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R354_ModeloVeicularCarga;
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

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await servicoRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Modelo Veicular de Carga", "Cargas", "ModeloVeicularCarga.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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
                Models.Grid.Relatorio modeloRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaModeloVeicularCarga filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = modeloRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Cargas.ModeloVeicularCarga servicoRelatorioModeloVeicular = new Servicos.Embarcador.Relatorios.Cargas.ModeloVeicularCarga(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioModeloVeicular.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ModeloVeicularCarga.ModeloVeicularCarga> lista, out int totalRegistros, filtrosPesquisa, propriedades, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(lista);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Relatorios.Veiculos.Veiculo.OcorreuUmaFalhaAoConsultar);
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
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynamicRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynamicRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynamicRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynamicRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaModeloVeicularCarga filtrosPesquisa = ObterFiltrosPesquisa();

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynamicRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (Exception excecao)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(excecao);
                return new JsonpResult(false, Localization.Resources.Relatorios.Veiculos.Veiculo.OcorreuUmaFalhaAoGerarRelatorio);
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

            grid.AdicionarCabecalho("Descrição", "Descricao", TamanhoColunaGrande, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Código de Integração", "CodigoIntegracao", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Código Gerenciadora de Risco", "CodigoIntegracaoGerenciadoraRisco", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Número Eixos", "NumeroEixos", TamanhoColunaPequena, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Padrão de Eixos", "PadraoEixosFormatado", TamanhoColunaPequena, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Número Eixos Suspensos", "NumeroEixosSuspensos", TamanhoColunaPequena, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Tipo", "TipoFormatado", TamanhoColunaMedia, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Número Reboque", "NumeroReboques", TamanhoColunaPequena, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Dias Proximo Checklist", "DiasRealizarProximoChecklist", TamanhoColunaPequena, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Situação", "AtivoDescricao", TamanhoColunaMedia, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Grupo", "GrupoModeloVeicular", TamanhoColunaGrande, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Fator de emissão de CO2", "FatorEmissaoCO2", TamanhoColunaMedia, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Código Tipo Carga ANTT", "CodigoTipoCargaANTT", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Velocidade Média", "VelocidadeMedia", TamanhoColunaPequena, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Unidade Capacidade", "UnidadeCapacidadeFormatado", TamanhoColunaPequena, Models.Grid.Align.right, false, true);
            grid.AdicionarCabecalho("Capacidade Carregamento", "CapacidadePesoTransporte", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Tolerância Mínima Carregamento", "ToleranciaPesoMenor", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Quantidade extra", "ToleranciaPesoExtra", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Tolerância Mínima Pallets", "ToleranciaMinimaPaletes", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Ocupação Cúbica Pallets", "OcupacaoCubicaPaletes", TamanhoColunaMedia, Models.Grid.Align.left, false, true);
            grid.AdicionarCabecalho("Códigos para integração", "CodigosIntegracao", TamanhoColunaGrande, Models.Grid.Align.left, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaModeloVeicularCarga ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaModeloVeicularCarga()
            {
                ModeloVeicular = Request.GetListParam<int>("ModeloVeicular"),
                Ativo = Request.GetNullableBoolParam("Ativo"),
                Tipo = Request.GetListEnumParam<TipoModeloVeicularCarga>("Tipo")
            };
        }

        #endregion
    }
}

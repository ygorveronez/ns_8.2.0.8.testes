using Dominio.Relatorios.Embarcador.Enumeradores;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Veiculos
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Veiculos/HistoricoVeiculoVinculo")]
    public class HistoricoVeiculoVinculoController : BaseController
    {
		#region Construtores

		public HistoricoVeiculoVinculoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        private readonly CodigoControleRelatorios _codigoControleRelatorio = CodigoControleRelatorios.R251_HistoricoVeiculoVinculo;
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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, Localization.Resources.Relatorios.Veiculos.Veiculo.RelatorioHistoricoVinculosVeiculos, "Veiculos", "HistoricoVeiculoVinculo.rpt", OrientacaoRelatorio.Retrato, "Placa", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(unitOfWork), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, Localization.Resources.Relatorios.Veiculos.Veiculo.OcorreuUmaFalhaBuscarDadosRelatorio);
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
                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoVeiculo filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo repositorio = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo(unitOfWork);
                int totalRegistros = repositorio.ContarConsultaRelatorioHistoricoVeiculoVinculo(filtrosPesquisa, propriedades);
                IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculoVinculo> listaVeiculo = totalRegistros > 0 ? repositorio.ConsultarRelatorioHistoricoVeiculoVinculo(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculoVinculo>();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaVeiculo);

                return new JsonpResult(grid);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao);

                return new JsonpResult(false, Localization.Resources.Relatorios.Veiculos.Veiculo.OcorreuUmaFalhaConsultar);
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
                Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoVeiculo filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemporario.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);
                parametrosConsulta.PropriedadeOrdenar = ObterPropriedadeOrdenar(parametrosConsulta.PropriedadeOrdenar);
                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarRelatorio(filtrosPesquisa, propriedades, parametrosConsulta, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

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

        private async Task GerarRelatorio(Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoVeiculo filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo repositorio = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculo(unitOfWork);
                Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoReboque repReboque = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoReboque(unitOfWork);
                Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoEquipamento repEquipamento = new Repositorio.Embarcador.Veiculos.HistoricoVeiculoVinculoEquipamento(unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculoVinculo> dataSource = repositorio.ConsultarRelatorioHistoricoVeiculoVinculo(filtrosPesquisa, propriedades, parametrosConsulta);
                IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculoVinculoReboques> listaReboques = new List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculoVinculoReboques>();
                IList<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculoVinculoEquipamentos> listaEquipamentos = new List<Dominio.Relatorios.Embarcador.DataSource.Veiculos.HistoricoVeiculoVinculoEquipamentos>();
                if (filtrosPesquisa.VisualizarVinculosSubRelatorio)
                {
                    if (dataSource.Count() > 0)
                    {
                        List<int> codigosVinculo = dataSource.Select(o => o.CodigoVinculoVeiculo).ToList();

                        if (codigosVinculo.Count() > 0)
                        {
                            for (int i = 0; i < codigosVinculo.Count(); i++)
                            {
                                listaReboques = repReboque.ConsultarReboquesVinculo(codigosVinculo[i]);
                                listaEquipamentos = repEquipamento.ConsultarEquipamentosVinculo(codigosVinculo[i]);
                            }
                        }
                    }
                }
                List<Parametro> parametros = ObterParametrosRelatorio(unitOfWork, filtrosPesquisa);
                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Veiculos/HistoricoVeiculoVinculo", parametros,relatorioControleGeracao, relatorioTemporario, dataSource, unitOfWork, null,
                    new List<KeyValuePair<string, dynamic>>()
                    {
                        new KeyValuePair<string, dynamic>("Reboques", listaReboques),
                        new KeyValuePair<string, dynamic>("Equipamentos", listaEquipamentos)
                    },
                    true, TipoServicoMultisoftware);

            }
            catch (Exception excecao)
            {
                servicoRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, excecao);
            }
            finally
            {
                await unitOfWork.DisposeAsync();
            }
        }

        private Models.Grid.Grid ObterGridPadrao(Repositorio.UnitOfWork unitOfWork)
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.CodigoVinculo, "CodigoVinculoVeiculo", _tamanhoColunaPequena, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.DescricaoVeiculos, "Veiculo", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.NumeroFrota, "NumeroFrota", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Marca, "Marca", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.TipoDaPropriedade, "TipoPropriedade", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Reboque, "Reboques", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.Equipamento, "Equipamentos", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Motorista, "Motorista", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.DataHoraVinculo, "DataHoraVinculoFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.QuantidadeDiasVinculo, "QtdDiasVinculo", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.KMRodadoVinculo, "KMRodadoVinculo", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.KMVeiculoRealizarVinculo, "KMVeiculoRealizarVinculo", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Usuario, "Usuario", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.CentroResultado, "CentroResultado", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Veiculos.Veiculo.DataHoraVinculoCentroResultado, "DataHoraCentroResultadoFormatada", _tamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, true);

            return grid;
        }

        private List<Parametro> ObterParametrosRelatorio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoVeiculo filtrosPesquisa)
        {
            List<Parametro> parametros = new List<Parametro>();

            Repositorio.Usuario repUsuario = new Repositorio.Usuario(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Veiculos.Equipamento repEquipamento = new Repositorio.Embarcador.Veiculos.Equipamento(unitOfWork);

            Dominio.Entidades.Usuario usuario = filtrosPesquisa.CodigoMotorista > 0 ? repUsuario.BuscarPorCodigo(filtrosPesquisa.CodigoMotorista) : null;
            Dominio.Entidades.Veiculo veiculo = filtrosPesquisa.CodigoVeiculo > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoVeiculo) : null;
            Dominio.Entidades.Embarcador.Veiculos.Equipamento equipamento = filtrosPesquisa.CodigoEquipamento > 0 ? repEquipamento.BuscarPorCodigo(filtrosPesquisa.CodigoEquipamento) : null;
            Dominio.Entidades.Veiculo reboque = filtrosPesquisa.CodigoReboque > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigoReboque) : null;

            parametros.Add(new Parametro("Veiculo", veiculo != null ? veiculo.Descricao : string.Empty));
            parametros.Add(new Parametro("Motorista", usuario != null ? usuario.Nome : string.Empty));
            parametros.Add(new Parametro("Equipamento", equipamento != null ? equipamento.Descricao : string.Empty));
            parametros.Add(new Parametro("Reboque", reboque != null ? reboque.Descricao : string.Empty));
            parametros.Add(new Parametro("VisualizarVinculosSubRelatorio", (bool?)filtrosPesquisa.VisualizarVinculosSubRelatorio));

            return parametros;
        }

        private Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoVeiculo ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Veiculos.FiltroPesquisaRelatorioHistoricoVeiculo()
            {
                DataHoraVinculoInicialHistoricoVeiculo = Request.GetDateTimeParam("DataHoraVinculoInicialHistoricoVeiculo"),
                DataHoraVinculoFinalHistoricoVeiculo = Request.GetDateTimeParam("DataHoraVinculoFinalHistoricoVeiculo"),
                DataInicialVinculoCentroResultado = Request.GetDateTimeParam("DataInicialVinculoCentroResultado"),
                DataFinalVinculoCentroResultado = Request.GetDateTimeParam("DataFinalVinculoCentroResultado"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoReboque = Request.GetIntParam("Reboque"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoEquipamento = Request.GetIntParam("Equipamento"),
                VisualizarVinculosSubRelatorio = Request.GetBoolParam("VisualizarVinculosSubRelatorio")
            };
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        #endregion
    }
}

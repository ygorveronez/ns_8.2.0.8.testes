using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.Excecoes.Embarcador;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Frotas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Frotas/ManutencaoVeiculo")]
    public class ManutencaoVeiculoController : BaseController
    {
		#region Construtores

		public ManutencaoVeiculoController(Conexao conexao) : base(conexao) { }

		#endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R163_ManutencaoVeiculo;

        #region Métodos Públicos

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigo = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Manutenções de Veículo", "Frota", "ManutencaoVeiculo.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "CodigoServico", "desc", "PlacaVeiculo", "desc", codigo, unitOfWork, true, true);
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

                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioManutencaoVeiculo filtrosPesquisa = await ObterFiltrosPesquisa(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Frota.ManutencaoVeiculo servicoRelatorioManuntencaoVeiculo = new Servicos.Embarcador.Relatorios.Frota.ManutencaoVeiculo(unitOfWork, TipoServicoMultisoftware, Cliente);
                var lista = await servicoRelatorioManuntencaoVeiculo.ConsultarRegistrosAsync(filtrosPesquisa, agrupamentos, parametrosConsulta);
                grid.setarQuantidadeTotal(lista.Count);
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
                Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioManutencaoVeiculo filtrosPesquisa = await ObterFiltrosPesquisa(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException servicoException)
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

            grid.AdicionarCabecalho("Cód. Serviço", "CodigoServico", 5, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Serviço", "DescricaoServico", 10, Models.Grid.Align.left, true, false, false, true);
            grid.AdicionarCabecalho("OBS Serviço", "ObservacaoServico", 15, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Motivo", "DescricaoMotivoServico", 7, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Tipo", "DescricaoTipoServico", 7, Models.Grid.Align.left, true, true);
            grid.AdicionarCabecalho("Validade KM", "ValidadeKM", 7, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Tolerância KM", "ToleranciaKM", 7, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Validade Dias", "ValidadeDias", 7, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Tolerância DIas", "ToleranciaDias", 7, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Veículo", "PlacaVeiculo", 10, Models.Grid.Align.left, true, false, false, true);
            grid.AdicionarCabecalho("KM Atual", "KmAtualVeiculo", 10, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("Última OS Exec.", "NumeroOS", 7, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("OBS OS", "ObservacaoOS", 15, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Data Última Exec.", "DescricaoDataUltimaExecucao", 10, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho("KM Última Exec.", "QuilometragemUltimaExecucao", 7, Models.Grid.Align.right, true, true);
            grid.AdicionarCabecalho("KM Próxima Troca", "KMProximaTroca", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Equipamento", "Equipamento", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("KM Restante", "KMRestante", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Horímetro Restante", "HorimetroRestante", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Horímetro Atual", "HorimetroAtual", 10, Models.Grid.Align.right, false, false);
            grid.AdicionarCabecalho("Situação Última OS", "DescricaoSituacaoUltimaOS", 10, Models.Grid.Align.left, false, false);
            grid.AdicionarCabecalho("Horímetro Última Exec.", "HorimetroUltimaExecucao", 7, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Horímetro Próxima Troca", "HorimetroProximaTroca", 7, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Data Próxima Troca", "DescricaoDataProximaTroca", 10, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Dias Restante", "DiaRestante", 7, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Validade Horímetro", "ValidadeHorimetro", 7, Models.Grid.Align.right, true, false);
            grid.AdicionarCabecalho("Marca do Veículo", "MarcaVeiculo", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Modelo do Veículo", "ModeloVeiculo", 10, Models.Grid.Align.left, true, false);
            grid.AdicionarCabecalho("Segmento do Veículo", "SegmentoVeiculo", 10, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Centro de Resultado", "CentroResultado", 10, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Func. Responsável Veic.", "ResponsavelVeiculo", 10, Models.Grid.Align.left, false, false, false, true, false);


            return grid;
        }

        private async Task<Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioManutencaoVeiculo> ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioManutencaoVeiculo filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Frota.FiltroRelatorioManutencaoVeiculo()
            {
                CodigosServico = Request.GetListParam<int>("Servico"),
                TiposManutencao = Request.GetListEnumParam<TipoManutencaoServicoVeiculo>("TipoManutencao"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CodigoEquipamento = Request.GetIntParam("Equipamento"),
                CodigoEmpresa = TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiNFe ? this.Usuario.Empresa.Codigo : 0,
                VisualizarServicosPendentesManutencao = Request.GetBoolParam("VisualizarServicosPendentesManutencao"),
                CnpjCpfLocalManutencao = Request.GetDoubleParam("LocalManutencao"),
                VisualizarSomenteServicosExecutadosAnteriormente = Request.GetBoolParam("VisualizarSomenteServicosExecutadosAnteriormente"),
                KMAtual = Request.GetIntParam("KMAtual"),
                HorimetroAtual = Request.GetIntParam("HorimetroAtual"),
                UtilizarValidadeServicoPeloGrupoServicoOrdemServico = ConfiguracaoEmbarcador.UtilizarValidadeServicoPeloGrupoServicoOrdemServico,
                VisualizarVeiculosEquipamentosAcoplados = Request.GetBoolParam("VisualizarVeiculosEquipamentosAcoplados"),
                CodigoModeloVeiculo = Request.GetIntParam("ModeloVeiculo"),
                CodigoMarcaVeiculo = Request.GetIntParam("MarcaVeiculo"),
                CodigoModeloEquipamento = Request.GetIntParam("ModeloEquipamento"),
                CodigoMarcaEquipamento = Request.GetIntParam("MarcaEquipamento"),
                CodigosSegmentoVeiculo = Request.GetListParam<int>("SegmentoVeiculo"),
                CodigoFuncionarioResponsavel = Request.GetIntParam("FuncionarioResponsavel"),
                CodigoCentroResultado = Request.GetIntParam("CentroResultado"),
                VisualizarSomenteVeiculosAtivos = Request.GetBoolParam("VisualizarSomenteVeiculosAtivos")
            };

            if (filtrosPesquisa.VisualizarVeiculosEquipamentosAcoplados && filtrosPesquisa.CodigoVeiculo > 0)
            {
                Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork, cancellationToken);

                Dominio.Entidades.Veiculo veiculo = await repVeiculo.BuscarPorCodigoAsync(filtrosPesquisa.CodigoVeiculo);
                List<Dominio.Entidades.Veiculo> reboques = veiculo.VeiculosVinculados.ToList();

                filtrosPesquisa.CodigosEquipamentosAcoplados = veiculo.Equipamentos.Select(o => o.Codigo).ToList();
                if (reboques.Count > 0)
                {
                    filtrosPesquisa.CodigosReboques = new List<int>();
                    foreach (Dominio.Entidades.Veiculo reboque in reboques)
                    {
                        filtrosPesquisa.CodigosReboques.Add(reboque.Codigo);
                        filtrosPesquisa.CodigosEquipamentosAcoplados.AddRange(reboque.Equipamentos.Select(o => o.Codigo).ToList());
                    }
                }
            }

            return filtrosPesquisa;
        }

        #endregion
    }
}

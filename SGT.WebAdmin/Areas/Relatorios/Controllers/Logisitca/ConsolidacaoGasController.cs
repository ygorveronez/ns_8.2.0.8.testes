using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Logisitca
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Logistica/ConsolidacaoGas")]
    public class ConsolidacaoGasController : BaseController
    {
		#region Construtores

		public ConsolidacaoGasController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        private Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R253_ConsolidacaoGas;
        private decimal TamanhoColunasValores = (decimal)1.75;
        private decimal TamanhoColunasDescricao = (decimal)2.75;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio;
                int.TryParse(Request.Params("Codigo"), out codigoRelatorio);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Consolidações de Abastecimento de Gás", "Logistica", "ConsolidacaoGas.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                var retorno = gridRelatorio.RetornoGridPadraoRelatorio(ObterGridPadrao(), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioConsolidacaoGas filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Logistica.ConsolidacaoGas servicoRelatorioConsolidacaoGas = new Servicos.Embarcador.Relatorios.Logistica.ConsolidacaoGas(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioConsolidacaoGas.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Logistica.ConsolidacaoGas> solicitacoesGas, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.AdicionaRows(solicitacoesGas);
                grid.setarQuantidadeTotal(countRegistros);

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

                int codigoEmpresa = Empresa?.Codigo ?? 0;

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio svcRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = svcRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioConsolidacaoGas filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = svcRelatorio.AdicionarRelatorioParaGeracao(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

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

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioConsolidacaoGas ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioConsolidacaoGas()
            {
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                Bases = Request.GetListParam<double>("Base"),
                DisponibilidadeTransferencia = Request.GetEnumParam<SimNao>("DisponibilidadeTransferencia"),
                VolumeRodoviario = Request.GetEnumParam<SimNao>("VolumeRodoviario"),
                TipoFilial = Request.GetEnumParam<TipoFilialAbastecimentoGas>("TipoFilial")
            };
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);

            grid.AdicionarCabecalho("Data Medição", "DataMedicaoFormatada", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Abertura", "Abertura", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("% Abertura", "PorcentagemAbertura", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Capacidade", "Capacidade", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Lastro", "Lastro", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Estoque Mínimo", "EstoqueMinimo", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Estoque Máximo", "EstoqueMaximo", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Cliente Base", "ClienteBaseDescricao", TamanhoColunasDescricao, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Produto", "Produto", TamanhoColunasDescricao, Models.Grid.Align.left, false, false, false, true, true);
            grid.AdicionarCabecalho("Saldo de disponibilidade para transferência", "SaldoDisponibilidadeTransferencia", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Previsão de Bombeio", "PrevisaoBombeio", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Previsão de transferência recebida", "PrevisaoTransferenciaRecebida", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Previsão de demanda empresarial", "PrevisaoDemandaEmpresarial", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Previsão de demanda domiciliar", "PrevisaoDemandaDomiciliar", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Estoque no UltraSystem", "EstoqueUltrasystem", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Previsão de Transferência Enviada (Transferência Programada)", "PrevisaoTransferenciaEnviada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Densidade da Abertura do Dia", "DensidadeAberturaDia", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Previsão de fechamento", "PrevisaoFechamento", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Volume Rodoviário Para carregamento no próximo dia", "VolumeRodoviarioCarregamentoProximoDia", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Previsão de Bombeio para o próximo dia", "PrevisaoBombeioProximoDia", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Disponibilidade de Transferência para o próximo dia", "DisponibilidadeTransferenciaProximoDia", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false).AdicionarAoAgrupamentoQuandoInvisivel(true);
            grid.AdicionarCabecalho("Veículos planejados", "VeiculosPlanejados", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Demanda planejada", "DemandaPlanejada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false).AdicionarAoAgrupamentoQuandoInvisivel(true);
            grid.AdicionarCabecalho("Demanda a planejar", "DemandaPlanejar", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Cliente Supridor", "ClienteSupridorDescricao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de carga", "TipoDeCarga", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo de operação", "TipoOperacao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Modelo veicular", "ModeloVeicular", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Observação", "Observacao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Usuário", "Usuario", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data última alteração", "DataUltimaAlteracaoFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Usuário Adição Quantidade", "UsuarioAdicaoQuantidade", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data Adição Quantidade", "DataAdicaoQuantidadeFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Adicional Vol. Rodoviário Carregamento Próx. Dia", "AdicionalVolumeRodoviarioCarregamentoProximoDia", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Adicional Disp. Transf. Prox. Dia", "AdicionalDisponibilidadeTransferenciaProximoDia", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        #endregion
    }
}

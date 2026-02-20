using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
	[Area("Relatorios")]
	public class CargaIntegracaoDadosTransportesController : BaseController
    {
		#region Construtores

		public CargaIntegracaoDadosTransportesController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R301_CargaIntegracaoDadosTransportes;

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

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Dados de Integrações de Cargas", "Cargas", "CargaIntegracaoDadosTransportes.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, false, true);

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

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Carga.CargaIntegracaoDadosTransportes servicoCargaIntegracaoDadosTransportes = new Servicos.Embarcador.Relatorios.Carga.CargaIntegracaoDadosTransportes(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoCargaIntegracaoDadosTransportes.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Integracao.CargaIntegracaoDadosTransportes> lista, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes filtrosPesquisa = ObterFiltrosPesquisa();

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

        private Models.Grid.Grid GridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Grupo de Pessoas", "GrupoPessoas", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.AdicionarCabecalho("Veículos", "Veiculos", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Motoristas", "Motoristas", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("CPF Motoristas", "CPFMotoristas", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tipo Operação", "TipoOperacao", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Data Carga", "DataCargaFormatada", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Tipo Integracão", "TipoIntegracao", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho("Tentativas", "Tentativas", TamanhoColumaExtraPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Integracão", "DataIntegracao", TamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Retorno Integracão", "Mensagem", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação Integração", "DescricaoSituacao", TamanhoColunaGrande, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Protocolo", "Protocolo", TamanhoColunaPequena, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Situação Carga", "DescricaoSituacaoCarga", TamanhoColunaMedia, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Origem", "Origem", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Destino", "Destino", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("CNPJ Filial", "CNPJFilialFormatado", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.AdicionarCabecalho("Filial", "Filial", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false).Ocultar(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.AdicionarCabecalho("Tipo Carga", "TipoCarga", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS ? "Empresa/Filial" : "Transportador", "Transportador", TamanhoColunaGrande, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Número Rastreador Veículos", "NumeroRastreadorVeiculos", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Tecnologia Rastreador Veículos", "TecnologiaRastreadorVeiculos", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Data encerramento", "DataEncerramentoFormatada", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaRelatorioCargaIntegracaoDadosTransportes()
            {
                DataInicialCarga = Request.GetDateTimeParam("DataInicialCarga"),
                DataFinalCarga = Request.GetDateTimeParam("DataFinalCarga"),
                DataInicioIntegracao = Request.GetDateTimeParam("DataInicioIntegracao"),
                DataFinalIntegracao = Request.GetDateTimeParam("DataFinalIntegracao"),
                DataInicioEncerramento = Request.GetDateTimeParam("DataInicioEncerramento"),
                DataFinalEncerramento = Request.GetDateTimeParam("DataFinalEncerramento"),
                CodigoCarga = Request.GetIntParam("Carga"),
                TipoIntegracao = Request.GetEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoIntegracao>("TipoIntegracao"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                CodigoTransportador = Request.GetIntParam("Transportador"),
                CodigoFilial = Request.GetIntParam("Filial"),
                Situacao = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao>("Situacao")
            };

            return filtrosPesquisa;
        }

        #endregion
    }
}

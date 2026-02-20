using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Logisitca
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Logistica/MonitoramentoNivelServico")]
    public class MonitoramentoNivelServicoController : BaseController
    {
		#region Construtores

		public MonitoramentoNivelServicoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R179_MonitoramentoNivelServico;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            var unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = servicoRelatorio.BuscarConfiguracaoPadrao(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Nivel de Serviço", "Logistica", "MonitoramentoNivelServico.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "asc", "", "", codigoRelatorio, unitOfWork, true, true);
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

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoNivelServico filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Repositorio.Embarcador.Logistica.Monitoramento repositorio = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoNivelServico> listaMonitoramentoNivelServicos = repositorio.ConsultarRelatorioNivelServico(filtrosPesquisa, propriedades, parametrosConsulta);

                grid.setarQuantidadeTotal(repositorio.ContarConsultaRelatorioNivelServico(filtrosPesquisa, propriedades));
                grid.AdicionaRows(listaMonitoramentoNivelServicos);

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
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoNivelServico filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
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

        private async Task GerarRelatorio(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoNivelServico filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Embarcador.Logistica.Monitoramento repositorioMonitoramento = new Repositorio.Embarcador.Logistica.Monitoramento(unitOfWork);
                IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.MonitoramentoNivelServico> dataSourceMonitoramentoNivelServico = repositorioMonitoramento.ConsultarRelatorioNivelServico(filtrosPesquisa, propriedades, parametrosConsulta);
                List<Parametro> parametros = ObterParametrosRelatorio(unitOfWork, filtrosPesquisa);
                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Logistica/MonitoramentoNivelServico",parametros, relatorioControleGeracao, relatorioTemporario, dataSourceMonitoramentoNivelServico, unitOfWork, null, null, true, TipoServicoMultisoftware);
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

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoNivelServico ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoNivelServico filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoNivelServico()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("CodigoCargaEmbarcador"),
                PlacaVeiculo = Request.GetStringParam("PlacaVeiculo"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                DataConfirmacaoDocumentosInicial = Request.GetDateTimeParam("DataConfirmacaoDocumentosInicial"),
                DataConfirmacaoDocumentosFinal = Request.GetDateTimeParam("DataConfirmacaoDocumentosFinal"),
                Filiais = ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork),
                Recebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                FiltrarCargasPorParteDoNumero = ConfiguracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Data da Carga", "DataDaCargaFormatada", 12, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("SM", "SM", 12, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Carga", "Carga", 12, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Placa", "PlacaVeiculo", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Modelo Veicular", "ModeloVeicular", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Tipo Rodado", "DescricaoTipoRodado", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("CD", "CDDescricao", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("UF Origem", "UFOrigem", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Loja", "LojaDescricao", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("UF Destino", "UFDestino", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Região", "Regiao", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Data Saída CD", "DataSaidaCDFormatada", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Data Entrada Loja", "DataEntradaLojaFormatada", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Data Saída Loja", "DataSaidaLojaFormatada", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Permanência", "Permanencia", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Tipo de transporte", "TipoDeTransporte", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", 8, Models.Grid.Align.center, false, false, false, true, true);
            grid.AdicionarCabecalho("NFs", "NFS", 8, Models.Grid.Align.center, false, false, false, true, false);
            grid.AdicionarCabecalho("Tipo da Carga", "TipoCarga", 8, Models.Grid.Align.left, false, false, false, true, false);
            grid.AdicionarCabecalho("Motorista", "NomeMotorista", 8, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("Janela Descarga", "JanelaDescarga", 8, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho("Nº Transporte", "NumeroTransporte", 8, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho("Confirmação Documentos", "DataConfirmacaoDocumentoFormatada", 8, Models.Grid.Align.center, false, false, false, false, false);

            return grid;
        }

        private List<Parametro> ObterParametrosRelatorio(Repositorio.UnitOfWork unitOfWork, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaMonitoramentoNivelServico filtrosPesquisa)
        {
            List<Parametro> parametros = new List<Parametro>();

            parametros.Add(new Parametro("NumeroCarga", filtrosPesquisa.CodigoCargaEmbarcador));
            parametros.Add(new Parametro("PlacaVeiculo", filtrosPesquisa.PlacaVeiculo));
            parametros.Add(new Parametro("Data", filtrosPesquisa.DataInicial, filtrosPesquisa.DataFinal));
            parametros.Add(new Parametro("PeriodoConfirmacaoDocumentos", filtrosPesquisa.DataConfirmacaoDocumentosInicial, filtrosPesquisa.DataConfirmacaoDocumentosFinal));

            return parametros;
        }

        #endregion
    }
}

using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using System.Threading.Tasks;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Logistica/AgendaCancelada")]
    public class AgendaCanceladaController : BaseController
    {
		#region Construtores

		public AgendaCanceladaController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R241_AgendaCancelada;

        #endregion

        #region Métodos Globais

        [AllowAuthenticate]
        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao); ;
            try
            {
                await unitOfWork.StartAsync(cancellationToken);
                int Codigo = int.Parse(Request.Params("Codigo"));

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Agendas Canceladas", "Logistica", "AgendaCancelada.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", Codigo, unitOfWork, true, true);
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
        }

        [AllowAuthenticate]
        public async Task<IActionResult> Pesquisa(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendaCancelada filtrosPesquisa = ObterFiltrosPesquisa();

                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta(ObterPropriedadeOrdenar);
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);
                int totalRegistros = repositorioAgendamentoColeta.ContarConsultaRelatorioAgendaCancelada(filtrosPesquisa, propriedades);
                IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.AgendaCancelada> listaJanelas = (totalRegistros > 0) ? repositorioAgendamentoColeta.ConsultarRelatorioAgendaCancelada(filtrosPesquisa, propriedades, parametrosConsulta) : new List<Dominio.Relatorios.Embarcador.DataSource.Logistica.AgendaCancelada>();
                
                grid.AdicionaRows(listaJanelas);
                grid.setarQuantidadeTotal(totalRegistros);

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

        [AllowAuthenticate]
        public async Task<IActionResult> GerarRelatorio(CancellationToken cancellationToken)
        {

            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);
            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendaCancelada filtrosPesquisa = ObterFiltrosPesquisa();

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);


                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, this.Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);
                await unitOfWork.CommitChangesAsync(cancellationToken);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemp.ObterParametrosConsulta();
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemp.PropriedadeAgrupa);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemp);

                string stringConexao = _conexao.StringConexao;
                _ = Task.Factory.StartNew(() => GerarRelatorioAgendaCancelada(propriedades, filtrosPesquisa, relatorioControleGeracao, relatorioTemp, stringConexao, CancellationToken.None));

                return new JsonpResult(true);
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                Servicos.Log.TratarErro(ex);
                return new JsonpResult(false, "Ocorreu uma falha ao gerar o relatorio.");
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendaCancelada ObterFiltrosPesquisa()
        {
            return new Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendaCancelada()
            {
                Fornecedor = Request.GetDoubleParam("Fornecedor"),
                TipoDeCarga = Request.GetIntParam("TipoDeCarga"),
                Destinatario = Request.GetDoubleParam("Destinatario"),
                DataInicio = Request.GetDateTimeParam("DataInicio"),
                DataFim = Request.GetDateTimeParam("DataFim"),
                Filial = Request.GetIntParam("Filial"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                Pedido = Request.GetStringParam("Pedido"),
                Senha = Request.GetStringParam("Senha"),
                SituacaoAgendamento = Request.GetListEnumParam<SituacaoAgendamentoColeta>("SituacaoAgendamento"),
            };
        }

        private async Task GerarRelatorioAgendaCancelada(List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendaCancelada filtrosPesquisa, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemp, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);

            Repositorio.Embarcador.Relatorios.RelatorioControleGeracao repRelatorioControleGeracao = new Repositorio.Embarcador.Relatorios.RelatorioControleGeracao(unitOfWork);
            Repositorio.Embarcador.Logistica.AgendamentoColeta repositorioAgendamentoColeta = new Repositorio.Embarcador.Logistica.AgendamentoColeta(unitOfWork);

            Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = new Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta()
                {
                    DirecaoOrdenar = relatorioTemp.OrdemOrdenacao,
                    PropriedadeOrdenar = relatorioTemp.PropriedadeOrdena
                };

                List<Parametro> parametros = ObterParametros(filtrosPesquisa, unitOfWork);

                IList<Dominio.Relatorios.Embarcador.DataSource.Logistica.AgendaCancelada> listaJanelas = repositorioAgendamentoColeta.ConsultarRelatorioAgendaCancelada(filtrosPesquisa, propriedades, parametrosConsulta);

                serRelatorio.GerarRelatorioDinamico("Relatorios/Logistica/AgendaCancelada",parametros,relatorioControleGeracao, relatorioTemp, listaJanelas, unitOfWork);

                await unitOfWork.DisposeAsync();
            }
            catch (Exception ex)
            {
                serRelatorio.RegistrarFalhaGeracaoRelatorio(relatorioControleGeracao, unitOfWork, ex);
            }
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();
            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false, true);
            grid.AdicionarCabecalho("Data Agenda", "DataAgenda", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Senha", "Senha", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Número Pedido", "NumeroPedido", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Fornecedor", "Fornecedor", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Destinatário", "Destinatario", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Quantidade Caixas", "QuantidadeCaixas", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Data Cancelamento", "DataCancelamento", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Usuário Solicitante", "Solicitante", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Carga", "Carga", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Motivo Cancelamento", "MotivoCancelamento", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação Agendamento", "DescricaoSituacaoAgendamento", 15, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Situação Janela", "SituacaoJanelaFormatado", 15, Models.Grid.Align.left, false, false, false, false, true);

            return grid;
        }

        private string ObterPropriedadeOrdenar(string propriedadeOrdenar)
        {
            return propriedadeOrdenar;
        }

        private List<Parametro> ObterParametros(Dominio.ObjetosDeValor.Embarcador.Logistica.FiltroPesquisaRelatorioAgendaCancelada filtrosPesquisa, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.TipoDeCarga repositorioTipoDeCarga = new Repositorio.Embarcador.Cargas.TipoDeCarga(unitOfWork);
            Repositorio.Cliente repositorioCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Embarcador.Filiais.Filial repositorioFilial = new Repositorio.Embarcador.Filiais.Filial(unitOfWork);

            List<Parametro> parametros = new List<Parametro>
            {
                new Parametro("Fornecedor", repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Fornecedor)?.Descricao ?? ""),
                new Parametro("TipoDeCarga", repositorioTipoDeCarga.BuscarPorCodigo(filtrosPesquisa.TipoDeCarga)?.Descricao ?? ""),
                new Parametro("Data", filtrosPesquisa.DataInicio, filtrosPesquisa.DataFim),
                new Parametro("Destinatario", repositorioCliente.BuscarPorCPFCNPJ(filtrosPesquisa.Destinatario)?.Descricao ?? ""),
                new Parametro("Filial", repositorioFilial.BuscarPorCodigo(filtrosPesquisa.Filial)?.Descricao ?? ""),
                new Parametro("Carga", filtrosPesquisa.NumeroCarga),
                new Parametro("Senha", filtrosPesquisa.Senha),
                new Parametro("Pedido", filtrosPesquisa.Pedido)
            };

            return parametros;
        }

        #endregion
    }
}

using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
	[Area("Relatorios")]
	[CustomAuthorize("Relatorios/Cargas/AvaliacaoEntregaPedido")]
    public class AvaliacaoEntregaPedidoController : BaseController
    {
		#region Construtores

		public AvaliacaoEntregaPedidoController(Conexao conexao) : base(conexao) { }

		#endregion

        #region Atributos

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R235_AvaliacaoEntregaPedido;

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

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Avaliação de Entrega", "Cargas", "AvaliacaoEntregaPedido.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "DataAvaliacao", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

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

        public async Task<IActionResult> PesquisaAsync(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                Models.Grid.Grid grid = new Models.Grid.Grid(Request);
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAvaliacaoEntregaPedido filtrosPesquisa = ObterFiltrosPesquisa();
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                parametrosConsulta.PropriedadeOrdenar = ObterParametroOrdenacao(parametrosConsulta.PropriedadeOrdenar);
                parametrosConsulta.PropriedadeAgrupar = ObterParametroOrdenacao(parametrosConsulta.PropriedadeAgrupar);

                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repControleEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork, cancellationToken);
                int totalRegistros = await repControleEntrega.ContarConsultaRelatorioAvaliacaoEntregaPedidoAsync(filtrosPesquisa, agrupamentos, cancellationToken);
                var listaCargas = (totalRegistros > 0) ? await repControleEntrega.ConsultarRelatorioAvaliacaoEntregaPedidoAsync(filtrosPesquisa, agrupamentos, parametrosConsulta, cancellationToken) : new List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Carga.AvaliacaoEntregaPedido>();

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaCargas);

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
                Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAvaliacaoEntregaPedido filtrosPesquisa = ObterFiltrosPesquisa();
                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);
                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                await unitOfWork.StartAsync(cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);
                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, Usuario, dynRelatorio.TipoArquivoRelatorio, unitOfWork);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = servicoRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = relatorioTemporario.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> propriedades = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, relatorioTemporario.PropriedadeAgrupa);
                parametrosConsulta.PropriedadeOrdenar = ObterParametroOrdenacao(parametrosConsulta.PropriedadeOrdenar);
                parametrosConsulta.PropriedadeAgrupar = ObterParametroOrdenacao(parametrosConsulta.PropriedadeAgrupar);
                string stringConexao = _conexao.StringConexao;

                _ = Task.Factory.StartNew(() => GerarArquivoRelatorioAsync(filtrosPesquisa, propriedades, parametrosConsulta, relatorioControleGeracao, relatorioTemporario, stringConexao, CancellationToken.None));

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

        private async Task GerarArquivoRelatorioAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAvaliacaoEntregaPedido filtrosPesquisa, List<PropriedadeAgrupamento> propriedades, Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta, Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao, Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario, string stringConexao, CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(stringConexao);
            Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

            try
            {
                Repositorio.Empresa repositorioEmpresa = new Repositorio.Empresa(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega repControleEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntrega(unitOfWork, cancellationToken);
                Dominio.Entidades.Empresa empresaRelatorio = await repositorioEmpresa.BuscarPorCodigoAsync(Empresa.Codigo);
                var listaRelatorio = await repControleEntrega.ConsultarRelatorioAvaliacaoEntregaPedidoAsync(filtrosPesquisa, propriedades, parametrosConsulta, cancellationToken);

                List<Parametro> parametros = await ObterParametrosAsync(filtrosPesquisa, unitOfWork);

                servicoRelatorio.GerarRelatorioDinamico("Relatorios/Cargas/AvaliacaoEntregaPedido", parametros, relatorioControleGeracao, relatorioTemporario, listaRelatorio, unitOfWork, null, null, true, TipoServicoMultisoftware, empresaRelatorio.CaminhoLogoDacte);
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

        private Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAvaliacaoEntregaPedido ObterFiltrosPesquisa()
        {
            Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAvaliacaoEntregaPedido filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAvaliacaoEntregaPedido()
            {
                DataAvaliacaoInicial = Request.GetNullableDateTimeParam("DataInicial"),
                DataAvaliacaoFinal = Request.GetNullableDateTimeParam("DataFinal"),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                CnpjsDestinatarios = Request.GetListParam<double>("Destinatarios"),
                CodigosTransportadores = Request.GetListParam<int>("Transportadores"),
                CodigosMotivos = Request.GetListParam<int>("Motivos"),
                CodigosVeiculos = Request.GetListParam<int>("Veiculos"),
            };

            return filtrosPesquisa;
        }

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Número da Carga", "NumeroCarga", 7, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Número do Pedido", "NumeroPedido", 7, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Data Avaliação", "DataAvaliacao", 10, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Transportador", "TransportadorFormatado", 12, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Veículos", "Placas", 7, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Motivo", "Motivo", 10, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Nota", "NotasFiscais", 7, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Feedback", "Feedback", 20, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Cliente", "DestinatarioFormatado", 10, Models.Grid.Align.left, true, false, false, false, true);

            return grid;
        }

        private async Task<List<Parametro>> ObterParametrosAsync(Dominio.ObjetosDeValor.Embarcador.Carga.FiltroPesquisaAvaliacaoEntregaPedido filtrosPesquisa,
            Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Cliente repCliente = new Repositorio.Cliente(unitOfWork);
            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Veiculo repVeiculo = new Repositorio.Veiculo(unitOfWork);
            Repositorio.Embarcador.Cargas.MotivoAvaliacao repMotivoAvaliacao = new Repositorio.Embarcador.Cargas.MotivoAvaliacao(unitOfWork);

            List<Dominio.Entidades.Cliente> destinatarios = filtrosPesquisa.CnpjsDestinatarios?.Count > 0 ? await repCliente.BuscarPorCPFCNPJsAsync(filtrosPesquisa.CnpjsDestinatarios) : null;
            List<Dominio.Entidades.Empresa> transportadores = filtrosPesquisa.CodigosTransportadores?.Count > 0 ? await repEmpresa.BuscarPorCodigosAsync(filtrosPesquisa.CodigosTransportadores) : null;
            List<Dominio.Entidades.Veiculo> veiculos = filtrosPesquisa.CodigosVeiculos?.Count > 0 ? repVeiculo.BuscarPorCodigo(filtrosPesquisa.CodigosVeiculos) : null;
            List<Dominio.Entidades.Embarcador.Cargas.MotivoAvaliacao> motivos = filtrosPesquisa.CodigosMotivos?.Count > 0 ? await repMotivoAvaliacao.BuscarPorCodigosAsync(filtrosPesquisa.CodigosMotivos) : null;

            List<Parametro> parametros = new List<Parametro>
            {
                new Parametro("NumeroCarga", filtrosPesquisa.NumeroCarga),
                new Parametro("DataInicial", filtrosPesquisa.DataAvaliacaoInicial),
                new Parametro("DataFinal", filtrosPesquisa.DataAvaliacaoFinal),
                new Parametro("Destinatarios", destinatarios != null ? string.Join(", ", destinatarios.Select(o => o.Descricao)) : null),
                new Parametro("Transportadores", transportadores != null ? string.Join(", ", transportadores.Select(o => o.Descricao)) : null),
                new Parametro("Veiculos", veiculos != null ? string.Join(", ", veiculos.Select(o => o.Placa)) : null),
                new Parametro("Motivos", motivos != null ? string.Join(", ", motivos.Select(o => o.Descricao)) : null),
            };

            return parametros;
        }

        private string ObterParametroOrdenacao(string propriedadeOrdenar)
        {
            if (propriedadeOrdenar == "TransportadorFormatado")
                return "TransportadorRazao";

            if (propriedadeOrdenar == "DestinatarioFormatado")
                return "DestinatarioNome";

            return propriedadeOrdenar;
        }

        #endregion
    }
}


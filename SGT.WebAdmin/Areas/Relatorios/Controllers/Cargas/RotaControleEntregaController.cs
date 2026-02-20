using Dominio.Relatorios.Embarcador.Enumeradores;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Cargas/RotaControleEntrega")]
    public class RotaControleEntregaController : BaseController
    {
        #region Construtores

        public RotaControleEntregaController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados

        private readonly CodigoControleRelatorios _codigoControleRelatorio = CodigoControleRelatorios.R256_RotaControleEntrega;
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

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Rota Contole Entrega", "Cargas", "RotaControleEntrega.rpt", OrientacaoRelatorio.Retrato, "Filial", "asc", "", "", codigoRelatorio, unitOfWork, true, true);

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

                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRotaControleEntrega filtrosPesquisa = await ObterFiltrosPesquisa(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.ControleEntrega.RotaControleEntrega servicoRelatorioRotaControleEntrega = new Servicos.Embarcador.Relatorios.ControleEntrega.RotaControleEntrega(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioRotaControleEntrega.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Cargas.ControleEntrega.RotaControleEntrega> listaRotas, out int countRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(countRegistros);
                grid.AdicionaRows(listaRotas);

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

                Repositorio.Embarcador.Relatorios.Relatorio repRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = Newtonsoft.Json.JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<Dominio.Relatorios.Embarcador.ObjetosDeValor.PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRotaControleEntrega filtrosPesquisa = await ObterFiltrosPesquisa(unitOfWork, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.RelatorioControleGeracao relatorioControleGeracao = await serRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

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

        private Models.Grid.Grid ObterGridPadrao()
        {
            Models.Grid.Grid grid = new Models.Grid.Grid();

            grid.header = new List<Models.Grid.Head>();

            grid.AdicionarCabecalho("Codigo", false);
            grid.AdicionarCabecalho("Filial", "DescricaoFilial", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Carga", "NumeroCarga", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Placa", "VeiculoPlaca", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Transportador", "Transportador", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Status", "StatusFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Peso Entregue", "PesoEntregue", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Peso Devolvido", "PesoDevolvido", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Ent. Realizadas", "EntregasRealizadas", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Reentregas", "Reentregas", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("KM Realizados", "KMRealizados", _tamanhoColunaPequena, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho("Cód. Cliente", "CodigoCliente", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Cliente", "RazaoSocial", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Cidade", "Cidade", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("UF", "UF", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Apontamento no Raio", "ApontamentoNoRaio", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Sequência Realizada", "SequenciaRealizada", _tamanhoColunaMedia, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho("Início Realizado", "InicioRealizadoFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Fim Realizado", "FimRealizadoFormatada", _tamanhoColunaMedia, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Chegada Realizada", "ChegadaRealizadaFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Início Descarga", "InicioDescargaFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Saída Realizada", "SaidaRealizadaFormatada", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("Latitude", "Latitude", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Longitude", "Longitude", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho("Reversão", "Reversao", _tamanhoColunaPequena, Models.Grid.Align.left, true, false, false, false, false);

            return grid;
        }

        async Task<Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRotaControleEntrega> ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            List<int> filiais = Request.GetListParam<int>("Filial");
            return new Dominio.ObjetosDeValor.Embarcador.Carga.ControleEntrega.FiltroPesquisaRotaControleEntrega()
            {
                CodigoCargaEmbarcador = Request.GetStringParam("Carga"),
                CodigosTransportador = Request.GetListParam<int>("Transportador"),
                CodigosVeiculo = Request.GetListParam<int>("Veiculos"),
                CpfCnpjDestinatarios = Request.GetListParam<double>("Destinatario"),
                CpfCnpjEmitentes = Request.GetListParam<double>("Emitente"),
                NumerosPedido = Request.GetListParam<int>("Pedidos"),
                NumeroNotasFiscais = Request.GetListParam<int>("NumerosNotaFiscal"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                DataEntregaPedidoInicial = Request.GetDateTimeParam("DataEntregaPedidoInicial"),
                DataEntregaPedidoFinal = Request.GetDateTimeParam("DataEntregaPedidoFinal"),
                CodigosFilial = filiais.Count == 0 ? await ObterListaCodigoFilialPermitidasOperadorLogisticaAsync(unitOfWork, cancellationToken) : filiais,
                DataPrevisaoEntregaPedidoInicial = Request.GetDateTimeParam("DataPrevisaoEntregaPedidoInicial"),
                DataPrevisaoEntregaPedidoFinal = Request.GetDateTimeParam("DataPrevisaoEntregaPedidoFinal"),
                Recebedores = await ObterListaCnpjCpfRecebedorPermitidosOperadorLogisticaAsync(unitOfWork, cancellationToken)

            };
        }

        #endregion
    }
}

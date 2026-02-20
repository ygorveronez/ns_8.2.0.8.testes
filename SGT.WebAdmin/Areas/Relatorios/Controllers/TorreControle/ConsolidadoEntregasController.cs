using Dominio.Excecoes.Embarcador;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;


namespace SGT.WebAdmin.Areas.Relatorios.Controllers.TorreControle
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/TorreControle/ConsolidadoEntregas")]
    public class ConsolidadoEntregasController : BaseController
    {
        #region Construtores

        public ConsolidadoEntregasController(Conexao conexao) : base(conexao) { }

        #endregion

        Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios CodigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R317_ConsolidadoEntregas;

        private decimal TamanhoColumaExtraPequena = 1m;
        private decimal TamanhoColunaPequena = 1.75m;
        private decimal TamanhoColunaGrande = 5.50m;
        private decimal TamanhoColunaMedia = 3m;

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Servicos.Embarcador.Relatorios.Relatorio serRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await serRelatorio.BuscarConfiguracaoPadraoAsync(CodigoControleRelatorio, TipoServicoMultisoftware, "Relatório de Consolidação de Entregas", "TorreControle", "ConsolidadoEntregas.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Retrato, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = gridRelatorio.RetornoGridPadraoRelatorio(GridPadrao(), relatorio);

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

                Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsolidadoEntregas filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.TorreControle.ConsolidadoEntregas servicoConsolidadoEntregas = new Servicos.Embarcador.Relatorios.TorreControle.ConsolidadoEntregas(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoConsolidadoEntregas.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.TorreControle.ConsolidadoEntregas> listaConsolidadoEntregas, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaConsolidadoEntregas);

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
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo, cancellationToken);

                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioTemporario = serRelatorio.ObterRelatorioTemporario(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Relatorio mdlRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                gridRelatorio.SetarRelatorioPelaGrid(dynRelatorio.Grid, relatorioTemporario);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = serRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsolidadoEntregas filtrosPesquisa = ObterFiltrosPesquisa(unitOfWork);
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
                return new JsonpResult(false, Localization.Resources.Gerais.Geral.OcorreuFalhaAoGerar);
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
            grid.AdicionarCabecalho("Transporte", "CodigoCargaEmbarcador", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Fornecimento", "NumeroPedido", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Nota Fiscal", "NumeroNotaFiscal", TamanhoColunaMedia, Models.Grid.Align.left, false, false, false, false, true);
            grid.AdicionarCabecalho("Peso", "PesoTotalPedido", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data/Hora Criação Transporte", "DataCriacaoCarga", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data/Hora Faturamento", "DataInicioViagemPrevista", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data/Hora Início Viagem", "DataInicioViagemRealizada", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data/Hora Previsão Inicial", "DataPrevisaoEntrega", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data/Hora Previsão de Entrega Reprogramada", "DataEntregaReprogramada", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data/Hora Previsão de Entrega Ajustada", "DataPrevisaoEntregaAjustada", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data/Hora Início da Entrega", "DataInicioEntrega", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data/Hora Chegada no Alvo", "DataEntradaRaio", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data/Hora Saída do Alvo", "DataSaidaRaio", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data/Hora Confirmada Entrega", "DataConfirmacao", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Status Prazo de Entrega Inicial", "StatusPrazoEntregaFormatado", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Tempo de Atraso (Horas)", "TempoAtraso", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Ocorrência ", "MotivoUltimaOcorrencia", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data/Hora de Abertura Ocorrência", "DataUltimaOcorrencia", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data/Hora Ação Início de Viagem", "DataInicioViagem", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Data/Hora Ação Fim de Viagem", "DataFimEntrega", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Canhoto Anexo?", "PossuiCanhoto", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Usuário Anexo Canhoto", "UsuarioCanhoto", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Data/Hora Anexo Canhoto", "DataCanhoto", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Transportadora ", "Transportador", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Motorista", "Motoristas", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Placa Cavalo", "PlacaTracaoFormatada", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Placa Carreta", "PlacasReboques", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Data/Hora Última Posição Registrada", "DataUltimaPosicao", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Status Posicionamento Atual", "StatusPosicaoVeiculoFormatado", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Localização Atual", "CoordenadasLocalizacaoAtual", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Tipo de Interação Início Viagem", "TipoInteracaoInicioViagemFormatado", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Tipo de Interação Chegada", "TipoInteracaoChegadaViagemFormatado", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Tipo de Interação Fim Entrega", "TipoInteracaoFimEntregaFormatado", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("% Rastreabilidade no percuso", "PercentualViagem", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Cód. Local de Organização do Transporte", "CodigoFilial", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Local de Organização do Transporte", "NomeFilial", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Origem", "Origem", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Código Origem", "CodigoOrigem", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Cidade/UF Origem", "CidadeUFOrigem", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Endereço Origem", "EnderecoOrigem", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Coordenadas Origem", "CoordenadasOrigem", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Cliente", "Cliente", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Cidade/UF Cliente", "CidadeUFCliente", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Endereço Cliente", "EnderecoCliente", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Coordenadas Cliente", "CoordenadasCliente", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Ordem de Entregas Prevista", "OrdemEntregaPrevista", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Ordem de Entregas Realizada", "OrdemEntregaRealizada", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("KM Rota", "DistanciaPrevista", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("Distância Atual do Alvo (KM)", "DistanciaAteDestino", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho("% Conclusão Entrega", "PercentualCargaEntregue", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Cód. Transporte", "CodigoTipoOperacao", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Cód. Material", "CodigoProduto", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Descrição Material", "DescricaoProduto", TamanhoColunaMedia, Models.Grid.Align.center, false, false, false, false, true);
            grid.AdicionarCabecalho("Cód. Tp. Expedição", "CodigoModeloVeicular", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho("Tipo Ocorrência", "TipoOcorrencia", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Relatorio.SituacaoControleEntregas, "SituacaoViagemFormatada", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Escritorio Venda", "EscritorioVenda", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Canal Venda", "CanalVenda", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Tipo Mercado", "TipoMercado", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Equipe Vendas", "EquipeVendas", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Canal Entrega", "CanalEntrega", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho("Carga Critica", "CargaCriticaFormatado", TamanhoColunaMedia, Models.Grid.Align.center, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Relatorio.PedidoCritico, "PedidoCriticoFormatado", TamanhoColunaPequena, Models.Grid.Align.center, true, false, false, true, false);

            return grid;
        }

        private Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsolidadoEntregas ObterFiltrosPesquisa(Repositorio.UnitOfWork unitOfWork)
        {
            Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsolidadoEntregas filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.TorreControle.FiltroPesquisaRelatorioConsolidadoEntregas()
            {
                CodigoCarga = Request.GetListParam<int>("Carga"),
                DataInicioViagemPrevistaInicial = Request.GetNullableDateTimeParam("DataInicioViagemPrevistaInicial"),
                DataInicioViagemPrevistaFinal = Request.GetNullableDateTimeParam("DataInicioViagemPrevistaFinal"),
                DataInicioViagemRealizadaInicial = Request.GetNullableDateTimeParam("DataInicioViagemRealizadaInicial"),
                DataInicioViagemRealizadaFinal = Request.GetNullableDateTimeParam("DataInicioViagemRealizadaFinal"),
                DataConfirmacaoInicial = Request.GetNullableDateTimeParam("DataConfirmacaoInicial"),
                DataConfirmacaoFinal = Request.GetNullableDateTimeParam("DataConfirmacaoFinal"),
                CodigoPedido = Request.GetIntParam("Pedido"),
                CodigoNotaFiscal = Request.GetIntParam("NotaFiscal"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CpfCnpjClienteOrigem = Request.GetDoubleParam("ClienteOrigem"),
                CpfCnpjClienteDestino = Request.GetDoubleParam("ClienteDestino"),
                CodigoCidadeOrigem = Request.GetIntParam("CidadeOrigem"),
                CodigoCidadeDestino = Request.GetIntParam("CidadeDestino"),
                CodigoTipoOperacao = Request.GetIntParam("TipoOperacao"),
                TipoInteracaoInicioViagem = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoInteracaoEntrega>("TipoInteracaoInicioViagem"),
                TipoInteracaoChegadaViagem = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoInteracaoEntrega>("TipoInteracaoChegadaViagem"),
                StatusViagem = Request.GetNullableEnumParam<Dominio.ObjetosDeValor.Embarcador.Enumeradores.StatusViagemControleEntrega>("StatusViagem"),
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (!Empresa.Matriz.Any())
                {
                    Repositorio.Empresa empresa = new Repositorio.Empresa(unitOfWork);
                    List<int> codigosEmpresa = empresa.BuscarCodigoMatrizEFiliais(Usuario.Empresa?.CNPJ_SemFormato);
                    filtrosPesquisa.CodigoTransportador = codigosEmpresa?.Count > 0 ? codigosEmpresa : null;
                }
                else
                    filtrosPesquisa.CodigoTransportador = new List<int>() { Usuario.Empresa?.Codigo ?? 0 };
            }
            else
                filtrosPesquisa.CodigoTransportador = Request.GetListParam<int>("Transportador");


            return filtrosPesquisa;
        }

        #endregion
    }
}

using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.Relatorios.Embarcador.ObjetosDeValor;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SGT.WebAdmin.Controllers;
using SGTAdmin.Controllers;

namespace SGT.WebAdmin.Areas.Relatorios.Controllers.Cargas
{
    [Area("Relatorios")]
    [CustomAuthorize("Relatorios/Cargas/Pedido")]
    public class PedidoController : BaseController
    {
        #region Construtores

        public PedidoController(Conexao conexao) : base(conexao) { }

        #endregion

        #region Atributos Privados Somente Leitura

        private readonly Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios _codigoControleRelatorio = Dominio.Relatorios.Embarcador.Enumeradores.CodigoControleRelatorios.R091_Pedidos;
        private readonly int _numeroMaximoComplementos = 30;

        #endregion

        #region Métodos Globais

        public async Task<IActionResult> BuscarDadosRelatorio(CancellationToken cancellationToken)
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(_conexao.StringConexao);

            try
            {
                await unitOfWork.StartAsync(cancellationToken);

                int codigoRelatorio = Request.GetIntParam("Codigo");

                Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido repositorioConfiguracaoPedido = new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork, cancellationToken);
                Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoPedido configuracaoPedido = await repositorioConfiguracaoPedido.BuscarConfiguracaoPadraoAsync();

                string tituloRelatorio = configuracaoPedido.UtilizarRelatorioPedidoComoStatusEntrega ? "Relatório de Status de Entrega" : "Relatório de Pedidos";

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork);
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorio = await servicoRelatorio.BuscarConfiguracaoPadraoAsync(_codigoControleRelatorio, TipoServicoMultisoftware, tituloRelatorio, "Cargas", "Pedidos.rpt", Dominio.Relatorios.Embarcador.Enumeradores.OrientacaoRelatorio.Paisagem, "Codigo", "desc", "", "", codigoRelatorio, unitOfWork, true, true);
                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio retorno = gridRelatorio.RetornoGridPadraoRelatorio(await ObterGridPadraoAsync(unitOfWork, cancellationToken), relatorio);

                await unitOfWork.CommitChangesAsync(cancellationToken);

                return new JsonpResult(retorno);
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

                Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = grid.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = mdlRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Servicos.Embarcador.Relatorios.Carga.Pedido servicoRelatorioPedido = new Servicos.Embarcador.Relatorios.Carga.Pedido(unitOfWork, TipoServicoMultisoftware, Cliente);

                servicoRelatorioPedido.ExecutarPesquisa(out List<Dominio.Relatorios.Embarcador.DataSource.Cargas.Pedido.RelatorioPedido> listaPedidos, out int totalRegistros, filtrosPesquisa, agrupamentos, parametrosConsulta);

                grid.setarQuantidadeTotal(totalRegistros);
                grid.AdicionaRows(listaPedidos);

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

                Repositorio.Embarcador.Relatorios.Relatorio repositorioRelatorio = new Repositorio.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Servicos.Embarcador.Relatorios.Relatorio servicoRelatorio = new Servicos.Embarcador.Relatorios.Relatorio(unitOfWork, cancellationToken);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio dynRelatorio = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Relatorios.Relatorio>(Request.Params("Relatorio"));
                Dominio.Entidades.Embarcador.Relatorios.Relatorio relatorioOrigem = await repositorioRelatorio.BuscarPorCodigoAsync(dynRelatorio.Codigo);

                Models.Grid.Relatorio gridRelatorio = new Models.Grid.Relatorio();
                Models.Grid.Grid grid = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.Grid.Grid>(dynRelatorio.Grid);

                Dominio.ObjetosDeValor.Embarcador.Relatorios.ConfiguracaoRelatorio configuracaoRelatorio = servicoRelatorio.ObterConfiguracaoRelatorio(dynRelatorio, relatorioOrigem, TipoServicoMultisoftware, gridRelatorio.ObterColunasRelatorio(grid));
                Dominio.ObjetosDeValor.Embarcador.Consulta.ParametroConsulta parametrosConsulta = configuracaoRelatorio.ObterParametrosConsulta();
                List<PropriedadeAgrupamento> agrupamentos = gridRelatorio.VerificarAgrupamentosParaConsulta(grid.header, parametrosConsulta, string.Empty);
                Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa = await ObterFiltrosPesquisaAsync(unitOfWork, cancellationToken);

                await servicoRelatorio.AdicionarRelatorioParaGeracaoAsync(relatorioOrigem, configuracaoRelatorio, Usuario, Empresa, filtrosPesquisa, agrupamentos, parametrosConsulta, dynRelatorio.TipoArquivoRelatorio, null, unitOfWork, TipoServicoMultisoftware);

                return new JsonpResult(true);
            }
            catch (ServicoException servicoException)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return new JsonpResult(false, true, servicoException.Message);
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

        private async Task<Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido> ObterFiltrosPesquisaAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repositorioConfiguracaoEmbarcador = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoEmbarcador = await repositorioConfiguracaoEmbarcador.BuscarConfiguracaoPadraoAsync();

            List<int> codigosFiliais = Request.GetListParam<int>("Filial");
            List<int> codigosTiposCargas = Request.GetListParam<int>("TipoCarga");
            List<int> codigosTiposOperacoes = Request.GetListParam<int>("TipoOperacao");

            Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido filtrosPesquisa = new Dominio.ObjetosDeValor.Embarcador.Carga.Pedido.FiltroPesquisaRelatorioPedido()
            {
                CodigosFilial = codigosFiliais.Count == 0 ? ObterListaCodigoFilialPermitidasOperadorLogistica(unitOfWork) : codigosFiliais,
                CodigosRecebedores = ObterListaCnpjCpfRecebedorPermitidosOperadorLogistica(unitOfWork),
                CodigosGruposPessoas = Request.GetListParam<int>("GrupoPessoas"),
                CodigosModelosVeiculos = Request.GetListParam<int>("ModeloVeiculo"),
                CodigosRotaFrete = Request.GetListParam<int>("RotaFrete"),
                CodigosRestricoes = Request.GetListParam<int>("Restricao"),
                CodigoMotorista = Request.GetIntParam("Motorista"),
                CodigosOrigem = Request.GetListParam<int>("Origem"),
                CodigosDestino = Request.GetListParam<int>("Destino"),
                DataInclusaoBookingInicial = Request.GetNullableDateTimeParam("DataInclusaoBookingInicial"),
                DataInclusaoBookingLimite = Request.GetNullableDateTimeParam("DataInclusaoBookingLimite"),
                DataInclusaoPCPInicial = Request.GetNullableDateTimeParam("DataInclusaoPCPInicial"),
                DataInclusaoPCPLimite = Request.GetNullableDateTimeParam("DataInclusaoPCPLimite"),
                DeliveryTerm = Request.GetStringParam("DeliveryTerm"),
                IdAutorizacao = Request.GetStringParam("IdAutorizacao"),
                SiglasOrigem = Request.GetListParam<string>("UFOrigem"),
                SiglasDestino = Request.GetListParam<string>("UFDestino"),
                CodigosTipoCarga = codigosTiposCargas.Count == 0 ? ObterListaCodigoTipoCargaPermitidosOperadorLogistica(unitOfWork) : codigosTiposCargas,
                CodigosTipoOperacao = codigosTiposOperacoes.Count == 0 ? ObterListaCodigoTipoOperacaoPermitidosOperadorLogistica(unitOfWork) : codigosTiposOperacoes,
                CodigosPedido = Request.GetListParam<int>("Pedido"),
                CodigoVeiculo = Request.GetIntParam("Veiculo"),
                CpfCnpjsDestinatario = Request.GetListParam<double>("Destinatario"),
                CpfCnpjsRemetente = Request.GetListParam<double>("Remetente"),
                DataFinal = Request.GetDateTimeParam("DataFinal"),
                DataInicial = Request.GetDateTimeParam("DataInicial"),
                ExibirProdutos = Request.GetBoolParam("ExibirProdutos"),
                PedidosSemCarga = Request.GetBoolParam("PedidosSemCarga"),
                Situacoes = JsonConvert.DeserializeObject<List<SituacaoCarga>>(Request.Params("Situacoes")),
                SituacoesCargaMercante = Request.GetListEnumParam<SituacaoCargaMercante>("SituacaoCargaMercante"),
                SomenteComReserva = Request.GetBoolParam("SomenteComReserva"),
                SomentePedidosCanceladosAposVincularCarga = Request.GetBoolParam("SomentePedidosCanceladosAposVincularCarga"),
                TipoLocalPrestacao = Request.GetEnumParam<TipoLocalPrestacao>("TipoLocalPrestacao"),
                SituacoesPedido = JsonConvert.DeserializeObject<List<SituacaoPedido>>(Request.Params("SituacoesPedido")),
                DataCarregamentoInicial = Request.GetDateTimeParam("DataCarregamentoInicial"),
                DataCarregamentoFinal = Request.GetDateTimeParam("DataCarregamentoFinal"),
                DataCriacaoPedidoInicial = Request.GetDateTimeParam("DataCriacaoPedidoInicial"),
                DataCriacaoPedidoFinal = Request.GetDateTimeParam("DataCriacaoPedidoFinal"),
                UtilizarDadosDasCargasAgrupadas = Request.GetBoolParam("UtilizarDadosDasCargasAgrupadas"),
                UtilizarDadosDosPedidos = !(configuracaoEmbarcador?.UtilizarDadosCargaRelatorioPedido ?? false),
                NumeroCarga = Request.GetStringParam("NumeroCarga"),
                CodigoGerente = Request.GetIntParam("Gerente"),
                CodigoSupervisor = Request.GetIntParam("Supervisor"),
                CodigoVendedor = Request.GetIntParam("Vendedor"),
                DataInicioViagemInicial = Request.GetDateTimeParam("DataInicioViagemInicial"),
                DataInicioViagemFinal = Request.GetDateTimeParam("DataInicioViagemFinal"),
                DataEntregaInicial = Request.GetDateTimeParam("DataEntregaInicial"),
                DataEntregaFinal = Request.GetDateTimeParam("DataEntregaFinal"),
                PrevisaoEntregaPedidoDataInicial = Request.GetDateTimeParam("PrevisaoEntregaPedidoDataInicial"),
                PrevisaoEntregaPedidoDataFinal = Request.GetDateTimeParam("PrevisaoEntregaPedidoDataFinal"),
                PrevisaoDataInicial = Request.GetNullableDateTimeParam("PrevisaoDataInicial"),
                PrevisaoDataFinal = Request.GetNullableDateTimeParam("PrevisaoDataFinal"),
                NumeroPedidoCliente = Request.GetStringParam("NumeroPedidoCliente"),
                SituacoesEntrega = Request.GetListEnumParam<SituacaoEntrega>("SituacoesEntrega"),
                NumeroPedido = Request.GetStringParam("NumeroPedido"),
                CpfCnpjsExpedidor = Request.GetListParam<double>("Expedidor"),
                PossuiExpedidor = Request.GetNullableBoolParam("PossuiExpedidor"),
                PossuiRecebedor = Request.GetNullableBoolParam("PossuiRecebedor"),
                ExibirCargasAgrupadas = Request.GetBoolParam("ExibirCargasAgrupadas"),
                TipoPropostaMultiModal = Request.GetListEnumParam<TipoPropostaMultimodal>("TipoPropostaMultiModal"),
                CodigoNumeroViagemNavio = Request.GetIntParam("NumeroViagemNavio"),
                CodigoPortoOrigem = Request.GetIntParam("PortoOrigem"),
                CodigoPortoDestino = Request.GetIntParam("PortoDestino"),
                DataETAPortoOrigemInicial = Request.GetDateTimeParam("DataETAPortoOrigemInicial"),
                DataETAPortoOrigemFinal = Request.GetDateTimeParam("DataETAPortoOrigemFinal"),
                DataETSPortoOrigemInicial = Request.GetDateTimeParam("DataETSPortoOrigemInicial"),
                DataETSPortoOrigemFinal = Request.GetDateTimeParam("DataETSPortoOrigemFinal"),
                DataETAPortoDestinoInicial = Request.GetDateTimeParam("DataETAPortoDestinoInicial"),
                DataETAPortoDestinoFinal = Request.GetDateTimeParam("DataETAPortoDestinoFinal"),
                DataETSPortoDestinoInicial = Request.GetDateTimeParam("DataETSPortoDestinoInicial"),
                DataETSPortoDestinoFinal = Request.GetDateTimeParam("DataETSPortoDestinoFinal"),
                DataInclusaoPedidoInicial = Request.GetDateTimeParam("DataInclusaoPedidoInicial"),
                DataInclusaoPedidoFinal = Request.GetDateTimeParam("DataInclusaoPedidoFinal"),
                CodigoOperadorPedido = Request.GetListParam<int>("OperadorPedido"),
                CodigoCentroResultado = Request.GetListParam<int>("CentroResultado"),
                DataInicioJanela = Request.GetDateTimeParam("DataInicioJanela"),
                FiltrarCargasPorParteDoNumero = configuracaoEmbarcador?.FiltrarCargasPorParteDoNumero ?? false,
                AguardandoIntegracao = Request.GetNullableEnumParam<AguardandoIntegracao>("AguardandoIntegracao"),
                SomentePedidosDeIntegracao = Request.GetBoolParam("SomentePedidosDeIntegracao"),
                CentroDeCustoViagemDescricao = Request.GetStringParam("CentroDeCustoViagemCodigo"),
                CentroDeCustoViagemCodigo = Request.GetIntParam("CentroDeCustoViagemCodigo")
            };

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                if (!Empresa.Matriz.Any())
                {
                    Repositorio.Empresa empresa = new Repositorio.Empresa(unitOfWork, cancellationToken);
                    List<int> codigosEmpresa = await empresa.BuscarCodigoMatrizEFiliaisAsync(Usuario.Empresa?.CNPJ_SemFormato);
                    filtrosPesquisa.CodigosTransportadores = codigosEmpresa?.Count > 0 ? codigosEmpresa : null;
                }
                else
                    filtrosPesquisa.CodigosTransportadores = Empresa != null ? new List<int>() { Empresa.Codigo } : null;
            }
            else
                filtrosPesquisa.CodigosTransportadores = Request.GetListParam<int>("Transportador");

            return filtrosPesquisa;
        }

        private async Task<Models.Grid.Grid> ObterGridPadraoAsync(Repositorio.UnitOfWork unitOfWork, CancellationToken cancellationToken)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService repositorioConfiguracaoWebService = new Repositorio.Embarcador.Configuracoes.ConfiguracaoWebService(unitOfWork, cancellationToken);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoWebService configuracaoWebService = await repositorioConfiguracaoWebService.BuscarConfiguracaoPadraoAsync();

            decimal TamanhoColunasValores = 3;
            decimal TamanhoColunasInformativo = (decimal)3.75;
            decimal TamanhoColunasDescritivo = (decimal)4.50;
            Repositorio.Embarcador.Frete.ComponenteFrete repComponenteFrete = new Repositorio.Embarcador.Frete.ComponenteFrete(unitOfWork, cancellationToken);
            Models.Grid.Grid grid = new Models.Grid.Grid()
            {
                header = new List<Models.Grid.Head>()
            };

            grid.AdicionarCabecalho("Codigo", false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
            {
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NumeroPedido, "NumeroPedido", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NumeroPedidoInterno, "NumeroPedidoInterno", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, true);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataCadastro, "DataCadastro", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            }
            else
            {
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NumeroPedido, "NumeroPedido", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, true);
                grid.AdicionarCabecalho("NumeroPedidoInterno", false);
                grid.AdicionarCabecalho("DataCadastro", false);
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataCriacaoCarga, "DataCriacaoCarga", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, false);
            }

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador
                || TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Filial, "Filial", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, true, false);
            else
                grid.AdicionarCabecalho("Filial", false);

            if (TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CodigoPedidoCliente, "CodigoPedidoCliente", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, true);
            else
                grid.AdicionarCabecalho("CodigoPedidoCliente", false);

            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CNPJRemetenteFormatado, "CNPJRemetenteFormatado", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Remetente, "Remetente", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.GrupoRemetente, "GrupoRemetente", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CategoriaRemetente, "CategoriaRemetente", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CNPJDestinatarioFormatado, "CNPJDestinatarioFormatado", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Destinatario, "Destinatario", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.GrupoDestinatario, "GrupoDestinatario", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CategoriaDestinatario, "CategoriaDestinatario", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PrevisaoSaidaString, "PrevisaoSaidaString", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PrevisaoSaidaAnteriorString, "PrevisaoSaidaAnteriorString", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PrevisaoSaidaAnteriorResponsavelDescricao, "PrevisaoSaidaAnteriorResponsavelDescricao", TamanhoColunasInformativo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PrevisaoSaidaAnteriorObservacao, "PrevisaoSaidaAnteriorObservacao", TamanhoColunasInformativo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PrevisaoEntregaPedidoFormatada, "PrevisaoEntregaPedidoFormatada", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataPrevistaFormatada, "DataPrevistaFormatada", TamanhoColunasInformativo, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PrevisaoEntregaAnteriorString, "PrevisaoEntregaAnteriorString", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PrevisaoEntregaAnteriorResponsavelDescricao, "PrevisaoEntregaAnteriorResponsavelDescricao", TamanhoColunasInformativo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PrevisaoEntregaAnteriorObservacao, "PrevisaoEntregaAnteriorObservacao", TamanhoColunasInformativo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataInicioEmissaoDocumentosFormatada, "DataInicioEmissaoDocumentosFormatada", TamanhoColunasInformativo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataFimEmissaoDocumentos, "DataFimEmissaoDocumentos", TamanhoColunasInformativo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Origem, "Origem", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.UFOrigem, "UFOrigem", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PaisOrigem, "PaisOrigem", TamanhoColunasDescritivo, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Destino, "Destino", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.UFDestino, "UFDestino", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PaisDestino, "PaisDestino", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CNPJExpedidorFormatado, "CNPJExpedidorFormatado", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Expedidor, "Expedidor", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ExpedidorNomeFantasia, "ExpedidorNomeFantasia", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CNPJRecebedorFormatado, "CNPJRecebedorFormatado", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Recebedor, "Recebedor", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CNPJTomadorFormatado, "CNPJTomadorFormatado", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Tomador, "Tomador", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Peso, "Peso", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CubagemPedido, "CubagemPedido", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PesoPedido, "PesoPedido", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NumeroCarga, "NumeroCarga", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataCarregamentoString, "DataCarregamentoString", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataColetaString, "DataColetaString", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Transportador, "Transportador", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, true, false).Ocultar(TipoServicoMultisoftware == AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.TipoCarga, "TipoCarga", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.TipoOperacao, "TipoOperacao", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.GrupoPessoas, "GrupoPessoas", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Veiculos, "Veiculos", TamanhoColunasInformativo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Gerais.Geral.Motorista, "Motoristas", TamanhoColunasInformativo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CPFMotoristasFormatado, "CPFMotoristasFormatado", TamanhoColunasInformativo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ModeloVeicular, "ModeloVeicular", TamanhoColunasInformativo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CapacidadePesoModeloVeicular, "CapacidadePesoModeloVeicular", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.RotaFrete, "RotaFrete", TamanhoColunasInformativo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Restricoes, "Restricoes", TamanhoColunasInformativo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Ordem, "Ordem", TamanhoColunasInformativo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Paletes, "Paletes", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.VolumeNF, "VolumeNF", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.QuantidadeTotalProduto, "QuantidadeTotalProduto", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NotasFiscais, "NotasFiscais", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, true);
            grid.AdicionarCabecalho("CT-es", "CTes", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, true);

            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS)
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Fatura, "Fatura", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, true);

            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Transbordo, "Transbordo", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("ISS", "ISS", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.FreteLiquido, "FreteLiquido", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);

            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CSTIBSCBS, "CSTIBSCBS", TamanhoColunasDescritivo, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ClassifTribIBSCBS, "ClassificacaoTributariaIBSCBS", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.BaseCalculoIBSCBS, "BaseCalculoIBSCBS", TamanhoColunasDescritivo, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.AliquotaIBSEstadual, "AliquotaIBSEstadual", TamanhoColunasDescritivo, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PercentualReducaoIBSEstadual, "PercentualReducaoIBSEstadual", TamanhoColunasDescritivo, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ValorReducaoIBSEstadual, "ValorReducaoIBSEstadual", TamanhoColunasDescritivo, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ValorIBSEstadual, "ValorIBSEstadual", TamanhoColunasDescritivo, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.AliquotaIBSMunicipal, "AliquotaIBSMunicipal", TamanhoColunasDescritivo, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PercentualReducaoIBSMunicipal, "PercentualReducaoIBSMunicipal", TamanhoColunasDescritivo, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ValorReducaoIBSMunicipal, "ValorReducaoIBSMunicipal", TamanhoColunasDescritivo, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ValorIBSMunicipal, "ValorIBSMunicipal", TamanhoColunasDescritivo, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.AliquotaCBS, "AliquotaCBS", TamanhoColunasDescritivo, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PercentualReducaoCBS, "PercentualReducaoCBS", TamanhoColunasDescritivo, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ValorReducaoCBS, "ValorReducaoCBS", TamanhoColunasDescritivo, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ValorCBS, "ValorCBS", TamanhoColunasDescritivo, Models.Grid.Align.right, false, false, false, false, false);

            grid.AdicionarCabecalho("ICMS", "ICMS", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.TotalReceber, "TotalReceber", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Volume, "Volume", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ValorMercadoria, "ValorMercadoria", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataChipFormatada, "DataChipFormatada", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataCancelFormatada, "DataCancelFormatada", TamanhoColunasInformativo, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ObservacaoCTe, "ObservacaoCTe", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CanalEntrega, "CanalEntrega", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NumeroBooking, "NumeroBooking", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DiaSemana, "DiaSemana", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataEmbarqueFormatada, "DataEmbarqueFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PortoChegada, "PortoChegada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PortoSaida, "PortoSaida", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Temperatura, "Temperatura", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.TipoEmbarque, "TipoEmbarque", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Companhia, "Companhia", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NumeroNavio, "NumeroNavio", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Resumo, "Resumo", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Reserva, "Reserva", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho("ETA", "DataETA", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataRetiradaCtrn, "DataRetiradaCtrn", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DeliveryTerm, "DeliveryTerm", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.IdAutorizacao, "IdAutorizacao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataInclusaoBooking, "DataInclusaoBooking", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataInclusaoPCP, "DataInclusaoPCP", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataTerminoCarregamento, "DataTerminoCarregamento", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataAgendamento, "DataAgendamento", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.SenhaAgendamento, "SenhaAgendamento", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.SenhaAgendamentoCliente, "SenhaAgendamentoCliente", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.TipoSeparacao, "TipoSeparacao", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.QtdeItensProdutos, "QtdeItensProdutos", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ObservacaoInterna, "ObservacaoInterna", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ObservacaoCarga, "ObservacaoCarga", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DescricaoSituacaoCarga, "DescricaoSituacaoCarga", TamanhoColunasInformativo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DescricaoSituacaoEntrega, "DescricaoSituacaoEntrega", TamanhoColunasInformativo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DescricaoSituacaoPedido, "DescricaoSituacaoPedido", TamanhoColunasInformativo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataInicioViagemFormatada, "DataInicioViagemFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataEntregaFormatada, "DataEntregaFormatada", TamanhoColunasDescritivo, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Saldo, "Saldo", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PedidoComAgenda, "PedidoComAgenda", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataFimJanelaFormatada, "DataFimJanelaFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Gerente, "Gerente", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Vendedor, "Vendedor", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Supervisor, "Supervisor", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PrevisaoEntregaTransportador, "PrevisaoEntregaTransportador", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ValorCustoFrete, "ValorCustoFrete", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false).Sumarizar(TipoSumarizacao.sum);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NumeroPedidoCliente, "NumeroPedidoCliente", TamanhoColunasInformativo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.GrossSales, "GrossSales", TamanhoColunasInformativo, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PossuiIscaFormatada, "PossuiIscaFormatada", TamanhoColunasInformativo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Observacao, "Observacao", TamanhoColunasInformativo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NumeroOrdem, "NumeroOrdem", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PossuiEtiquetagemFormatada, "PossuiEtiquetagemFormatada", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataAlocacaoPedidoFormatada, "DataAlocacaoPedidoFormatada", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataAlocacaoISISFormatada, "DataAlocacaoISISFormatada", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ValorNota, "ValorNota", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Carregamento, "Carregamento", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.OrdemCarregamento, "OrdemCarregamento", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CodIntegracaoDestinatarioPedido, "CodIntegracaoDestinatarioPedido", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CodIntegracaoTipoCargaPedido, "CodIntegracaoTipoCargaPedido", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CodIntegracaoProdutoEmbarcadorPedido, "CodIntegracaoProdutoEmbarcadorPedido", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DescricaoProdutoEmbarcadorPedido, "DescricaoProdutoEmbarcadorPedido", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.QtdProdutoPedidoFormatada, "QtdProdutoPedidoFormatada", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PesoProdutoPedidoFormatada, "PesoProdutoPedidoFormatada", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ValorFreteNegociado, "ValorFreteNegociado", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false).Ocultar(ConfiguracaoEmbarcador.SolicitarValorFretePorTonelada);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ValorFreteTransportadorTerceiro, "ValorFreteTransportadorTerceiro", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false).Ocultar(ConfiguracaoEmbarcador.SolicitarValorFretePorTonelada);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ValorFreteToneladaNegociado, "ValorFreteToneladaNegociado", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.SolicitarValorFretePorTonelada);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ValorFreteToneladaTerceiro, "ValorFreteToneladaTerceiro", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false).Ocultar(!ConfiguracaoEmbarcador.SolicitarValorFretePorTonelada);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ValorPedagioRota, "ValorPedagioRota", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiTMS);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ValorTotalPedido, "ValorTotalPedido", TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataSalvamentoDadosTransporteFormatada, "DataSalvamentoDadosTransporteFormatada", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataConfirmacaoEnvioDocumentosFormatada, "DataConfirmacaoEnvioDocumentosFormatada", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataConfirmacaoValorFreteFormatada, "DataConfirmacaoValorFreteFormatada", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataEnvioCTeOcorrenciaFormatada, "DataEnvioCTeOcorrenciaFormatada", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PrevisaoEntregaReprogramadaFormatada, "PrevisaoEntregaReprogramadaFormatada", TamanhoColunasValores, Models.Grid.Align.center, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.IdAgrupadorDescricao, "IdAgrupadorDescricao", TamanhoColunasValores, Models.Grid.Align.right, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CodigoSAP, "CodigoSAP", TamanhoColunasInformativo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.GerenteRegional, "GerenteRegional", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.EnderecoPrincipalDestinatario, "EnderecoPrincipalDestinatario", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.BairroDestinatario, "BairroDestinatario", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NumeroDestinatario, "NumeroDestinatario", TamanhoColunasInformativo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ComplementoDestinatario, "ComplementoDestinatario", TamanhoColunasInformativo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.OrdemEmbarque, "OrdemEmbarque", TamanhoColunasInformativo, Models.Grid.Align.center, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NumeroEXP, "NumeroEXP", TamanhoColunasInformativo, Models.Grid.Align.center, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataEstufagemFormatada, "DataEstufagemFormatada", TamanhoColunasInformativo, Models.Grid.Align.center, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.SemanaEstufagem, "SemanaEstufagem", TamanhoColunasInformativo, Models.Grid.Align.center, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PagamentoMaritimoDescricao, "PagamentoMaritimoDescricao", TamanhoColunasInformativo, Models.Grid.Align.center, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NomeNavio, "NomeNavio", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NomeNavioTransbordo, "NomeNavioTransbordo", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataDeadLineDrafFormatada, "DataDeadLineDrafFormatada", TamanhoColunasInformativo, Models.Grid.Align.center, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataDeadLineCargaFormatada, "DataDeadLineCargaFormatada", TamanhoColunasInformativo, Models.Grid.Align.center, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataETATransbordoFormatada, "DataETATransbordoFormatada", TamanhoColunasInformativo, Models.Grid.Align.center, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataETSFormatada, "DataETSFormatada", TamanhoColunasInformativo, Models.Grid.Align.center, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CodigoArmador, "CodigoArmador", TamanhoColunasInformativo, Models.Grid.Align.center, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NumeroLacre, "NumeroLacre", TamanhoColunasInformativo, Models.Grid.Align.center, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NumeroContainer, "NumeroContainer", TamanhoColunasInformativo, Models.Grid.Align.center, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.TerminalOrigem, "TerminalOrigem", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Despachante, "Despachante", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PossuiGensetDescricao, "PossuiGensetDescricao", TamanhoColunasValores, Models.Grid.Align.center, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho("Incoterm", "Incoterm", TamanhoColunasInformativo, Models.Grid.Align.center, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho("In Land", "InLand", TamanhoColunasInformativo, Models.Grid.Align.center, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PortoViagemOrigem, "PortoViagemOrigem", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PaisPortoViagemOrigem, "PaisPortoViagemOrigem", TamanhoColunasInformativo, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PortoViagemDestino, "PortoViagemDestino", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PaisPortoViagemDestino, "PaisPortoViagemDestino", TamanhoColunasInformativo, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.TipoContainer, "TipoContainer", TamanhoColunasInformativo, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ViaTransporte, "ViaTransporte", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false).Ocultar(TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiEmbarcador);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.TipoPropostaMultiModalDescricao, "TipoPropostaMultiModalDescricao", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.ViagemNavio, "ViagemNavio", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PortoOrigem, "PortoOrigem", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PortoDestino, "PortoDestino", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataETAPortoOrigemFormatada, "DataETAPortoOrigemFormatada", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataETSPortoOrigemFormatada, "DataETSPortoOrigemFormatada", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataETAPortoDestinoFormatada, "DataETAPortoDestinoFormatada", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataETSPortoDestinoFormatada, "DataETSPortoDestinoFormatada", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataInclusaoPedidoFormatada, "DataInclusaoPedidoFormatada", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NumeroRedespachoFormatado, "NumeroRedespachoFormatado", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataEntradaNoRaio, "DataEntradaNoRaio", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.OperadorPedido, "OperadorPedido", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CentroResultado, "CentroResultado", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NaturezaOP, "NaturezaOP", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.EmailSolicitante, "EmailSolicitante", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataInicioJanelaFormatada, "DataInicioJanelaFormatada", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CodigoIntegracao, "CodigoIntegracao", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.TipoServicoPedido, "TipoServicoPedido", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NumeroCarregamento, "NumeroCarregamento", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CodIntegracaoRemetente, "CodIntegracaoRemetente", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CodIntegracaoDestinatario, "CodIntegracaoDestinatario", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NotasFiscaisPedido, "NotasFiscaisPedido", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.AguardandoIntegracaoDescricao, "AguardandoIntegracaoDescricao", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.QtdPalletsCarregado, "QtdPalletsCarregado", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CargaPossuiAnexo, "CargaPossuiAnexosFormatada", TamanhoColunasDescritivo, Models.Grid.Align.center, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.LinhaSeparacao, "LinhaSeparacao", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PacotesIntegrados, "QuantidadePacotes", TamanhoColunasDescritivo, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PacotesColetados, "QuantidadePacotesColetados", TamanhoColunasDescritivo, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Pedidos.Pedido.CustoFrete, "CustoFrete", TamanhoColunasValores, Models.Grid.Align.left, true, false, false, false, false);

            if (configuracaoWebService?.PermiteReceberDataCriacaoPedidoERP ?? false)
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataCriacaoPedidoERP, "DataDeCriacaoPedidoERPFormatada", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false);

            if (new Repositorio.Embarcador.Configuracoes.ConfiguracaoPedido(unitOfWork)?.BuscarPrimeiroRegistro()?.PermiteInformarPedidoDeSubstituicao ?? false)
                grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.SubstituicaoDescricao, "SubstituicaoDescricao", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);

            //Colunas montadas dinamicamente
            if (TipoServicoMultisoftware != AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware.MultiCTe)
            {
                int i;
                List<Dominio.Entidades.Embarcador.Frete.ComponenteFrete> componentes = await repComponenteFrete.BuscarTodosAtivosAsync();
                componentes = componentes.Take(_numeroMaximoComplementos).ToList();
                // 0 ~ componentes.Count
                for (i = 0; i < componentes.Count; i++)
                    grid.AdicionarCabecalho(componentes[i].Descricao, "ValorComponente" + (i + 1).ToString(), TamanhoColunasValores, Models.Grid.Align.right, true, false, false, false, TipoSumarizacao.sum, componentes[i].Codigo);
                // componentes.Count ~ Maximo
                for (; i < _numeroMaximoComplementos; i++)
                    grid.AdicionarCabecalho("ValorComponente" + (i + 1).ToString(), false);
            }

            /*
            Km da rota (Informação da Rota inserida no pedido)
            Autorizado (sim/não)
            Autorizador (Usuário)
            Motivo do pedido
            Motivo da autorização do pedido
             */

            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.KmRota, "KmRota", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Autorizado, "Autorizado", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Autorizador, "Autorizador", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.MotivoAutorizacaoPedido, "MotivoAutorizacaoPedido", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.MotivoPedido, "MotivoPedido", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataEntregaOcorrenciaPedido, "DataEntregaOcorrenciaPedidoFormatada", TamanhoColunasDescritivo, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.Protocolo, "Protocolo", TamanhoColunasValores, Models.Grid.Align.left, false, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Relatorios.Cargas.Carga.CentroDeCustoViagem, "CentroDeCustoViagemDescricao", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.PedidoBloqueado, "DescricaoPedidoBloqueado", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.NovaDataAgendamento, "NovaDataAgendamento", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, true, false);
            grid.AdicionarCabecalho("Motivo Cancelamento", "MotivoCancelamento", TamanhoColunasDescritivo, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho("Usuario Cancelamento", "UsuarioCancelamento", TamanhoColunasDescritivo, Models.Grid.Align.center, true, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.EscritorioVenda, "EscritorioVenda", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CanalVenda, "CanalVenda", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.TipoMercado, "TipoMercado", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.EquipeVendas, "EquipeVendas", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.CargaCritica, "CargaCriticaFormatado", TamanhoColunasDescritivo, Models.Grid.Align.left, true, false, false, false, false);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataVinculoTracao, "DataVinculoTracaoFormatada", TamanhoColunasDescritivo, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataVinculoReboque, "DataVinculoReboqueFormatada", TamanhoColunasDescritivo, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.DataVinculoMotorista, "DataVinculoMotoristaFormatada", TamanhoColunasDescritivo, Models.Grid.Align.center, true, true);
            grid.AdicionarCabecalho(Localization.Resources.Cargas.Pedido.LocalVinculo, "DescricaoLocalVinculo", TamanhoColunasDescritivo, Models.Grid.Align.center, true, true);
            return grid;
        }

        #endregion
    }
}

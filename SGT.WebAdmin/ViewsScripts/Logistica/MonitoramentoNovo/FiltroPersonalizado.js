// #region Objetos Globais do Arquivo
var _criarFiltroPersonalizadoMonitoramentoNovo;
// #endregion Objetos Globais do Arquivo

// #region Classes
var CriarFiltroPersonalizadoMonitoramentoNovo = function () {
    this.CheckCodigoCargaEmbarcador = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Carga, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "CodigoCargaEmbarcador" });
    this.CheckVeiculo = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Veiculos, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "Veiculo" });
    this.CheckMonitoramentoStatus = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.SituacaoMonitoramento, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "MonitoramentoStatus" });
    this.CheckStatusViagem = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EtapaMonitoramento, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "StatusViagem" });
    this.CheckTransportador = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Transportador, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "Transportador" });
    this.CheckGrupoPessoa = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.GrupoPessoa, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "GrupoPessoa" });
    this.CheckNumeroPedido = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Pedido, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "NumeroPedido" });
    this.CheckNumeroNotaFiscal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.NotaFiscal, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "NumeroNotaFiscal" });
    this.CheckDataInicial = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicioCriacaoCarga, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "DataInicial" });
    this.CheckDataFinal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFimCriacaoCarga, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "DataFinal" });
    this.CheckSomenteRastreados = PropertyEntity({ text: Localization.Resources.Gerais.Geral.SomenteVeiculosComRastreadorOnline, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "SomenteRastreados" });
    this.CheckCliente = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Cliente, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "Cliente" });
    this.CheckFiltroCliente = PropertyEntity({ text: Localization.Resources.Gerais.Geral.FiltrarClientes, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "FiltroCliente" });
    this.CheckCategoriaPessoa = PropertyEntity({ text: Localization.Resources.Gerais.Geral.Categoria, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "CategoriaPessoa" });
    this.CheckFuncionarioVendedor = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Vendedor, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "FuncionarioVendedor" });
    this.CheckNumeroEXP = PropertyEntity({ text: Localization.Resources.Gerais.Geral.NumeroEXP, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "NumeroEXP" });
    this.CheckDataEntregaPedidoInicio = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataEntregaPedidoInicial, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "DataEntregaPedidoInicio" });
    this.CheckDataEntregaPedidoFinal = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataEntregaPedidoFinal, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "DataEntregaPedidoFinal" });
    this.CheckPrevisaoEntregaInicio = PropertyEntity({ text: Localization.Resources.Gerais.Geral.PrevisaoEntregaPlanejadaInicio, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "PrevisaoEntregaInicio" });
    this.CheckPrevisaoEntregaFinal = PropertyEntity({ text: Localization.Resources.Gerais.Geral.PrevisaoEntregaPlanejadaFinal, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "PrevisaoEntregaFinal" });
    this.CheckExpedidor = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Expedidor, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "Expedidor" });
    this.CheckVeiculosComContratoDeFrete = PropertyEntity({ text: Localization.Resources.Gerais.Geral.ApenasVeiculosQuePossuemContratoDeFrete, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "VeiculosComContratoDeFrete" });
    this.CheckOrigem = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Origem, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "Origem" });
    this.CheckDestino = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Destino, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "Destino" });
    this.CheckEstadoOrigem = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EstadoOrigem, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "EstadoOrigem" });
    this.CheckEstadoDestino = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EstadoDestino, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "EstadoDestino" });
    this.CheckNaoExibirResumosAlerta = PropertyEntity({ val: ko.observable(false), text: Localization.Resources.Logistica.Monitoramento.NaoExibirGraficoDeAlertas, getType: typesKnockout.bool, visible: ko.observable(false), nomeFiltro: "NaoExibirResumosAlerta" });
    this.CheckDataEmissaoNFeFim = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataFimEmissaoNFe, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "DataEmissaoNFeFim" });
    this.CheckDataEmissaoNFeInicio = PropertyEntity({ text: Localization.Resources.Gerais.Geral.DataInicioEmissaoNFe, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "DataEmissaoNFeInicio" });
    this.CheckResponsavelVeiculo = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.ResponsavelPeloVeiculo, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "ResponsavelVeiculo" });
    this.CheckCentroResultado = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.CentroDeResultado, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "CentroResultado" });
    this.CheckFronteiraRotaFrete = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.FronteiraRotaFrete, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "FronteiraRotaFrete" });
    this.CheckPaisDestino = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.PaisDestino, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "PaisDestino" });
    this.CheckPaisOrigem = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.PaisOrigem, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "PaisOrigem" });
    this.CheckTipoOperacao = PropertyEntity({ text: Localization.Resources.Gerais.Geral.TipoDeOperacao, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "TipoOperacao" });
    this.CheckApenasMonitoramentosCriticos = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.ApenasMonitoramentosCriticos, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "ApenasMonitoramentosCriticos" });
    this.CheckVeiculosEmLocaisTracking = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.VeiculosEmLocais, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "VeiculosEmLocaisTracking" });
    this.CheckLocaisTracking = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.LocaisTracking, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "LocaisTracking" });
    this.CheckDataInicioCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicioCarregamento, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "DataInicioCarregamento" });
    this.CheckDataFimCarregamento = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFimCarregamento, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "DataFimCarregamento" });
    this.CheckPossuiRecebedor = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.PossuiRecebedor, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "PossuiRecebedor" });
    this.CheckPossuiExpedidor = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.PossuiExpedidor, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "PossuiExpedidor" });
    this.CheckDestinatario = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Destinatario, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "Destinatario" });
    this.CheckRecebedores = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Recebedor, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "Recebedores" });
    this.CheckCodigoCargaEmbarcadorMulti = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.MultiplosNumerosCarga, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "CodigoCargaEmbarcadorMulti" });
    this.CheckTipoTrecho = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.TipoTrecho, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "TipoTrecho" });
    this.CheckInicioViagemPrevistaInicial = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataInicialDaPrevisaoDeInicioDeViagem, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "InicioViagemPrevistaInicial" });
    this.CheckInicioViagemPrevistaFinal = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataFinalDaPrevisaoDeInicioDeViagem, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "InicioViagemPrevistaFinal" });
    this.CheckCodigoMotorista = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Motorista, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "CodigoMotorista" });
    this.CheckRemetente = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Remetente, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "Remetente" });
    this.CheckTipoCarga = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.TipoCarga, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "TipoCarga" });
    this.CheckProdutos = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Produtos, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "Produtos" });
    this.CheckDataRealEntrega = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.DataRealEntrega, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "DataRealEntrega" });
    this.CheckCanalVenda = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.CanalVenda, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "CanalEntrega" });
    this.CheckMesoregiao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Mesoregiao, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "Mesoregiao" });
    this.CheckRegiao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.Regiao, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "Regiao" });
    this.CheckTipoCobrancaMultimodal = PropertyEntity({ val: ko.observable(EnumTipoCobrancaMultimodal.Nenhum), options: EnumTipoCobrancaMultimodal.obterOpcoes(), text: Localization.Resources.Ocorrencias.TipoOcorrencia.ModalTransporte, def: EnumTipoCobrancaMultimodal.Nenhum, enable: ko.observable(true), required: ko.observable(false), visible: ko.observable(true), nomeFiltro: "TipoCobrancaMultimodal" });
    this.CheckVendedor = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Vendedor, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "Vendedor" });
    this.CheckSupervisor = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Supervisor, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "Supervisor" });
    this.CheckColetaNoPrazo = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.ColetaNoPrazo, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "ColetaNoPrazo" });
    this.CheckEntregaNoPrazo = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.EntregaNoPrazo, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "EntregaNoPrazo" });
    this.CheckTendenciaProximaColeta = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.TendenciaColeta, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "TendenciaProximaColeta" });
    this.CheckTendenciaProximaEntrega = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.TendenciaEntrega, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "TendenciaProximaEntrega" });
    this.CheckTipoMercadoria = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.TipoMercadoria, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "TipoMercadoria" });
    this.CheckRotaFrete = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.RotaFrete, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "RotaFrete" });
    this.CheckParqueada = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.Parqueada, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "Parqueada" });
    this.CheckSituacaoIntegracaoSM = PropertyEntity({ text: Localization.Resources.Gerais.Geral.SituacaoIntegracao, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "SituacaoIntegracaoSM" });
    this.CheckSomenteUltimoPorCarga = PropertyEntity({ text: Localization.Resources.Gerais.Geral.SomenteUltimoPorCarga, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "SomenteUltimoPorCarga" });
    this.CheckVeiculoNoRaio = PropertyEntity({ text: Localization.Resources.Logistica.Monitoramento.VeiculoNoRaio, val: ko.observable(false), def: false, getType: typesKnockout.bool, nomeFiltro: "VeiculoNoRaio" });
   
    this.SalvarFiltros = PropertyEntity({ eventClick: salvarFiltrosClick, type: types.event, text: ko.observable(Localization.Resources.Gerais.Geral.Salvar), idGrid: guid(), visible: ko.observable(true), enable: ko.observable(true) });
    this.RemoverAbaFiltros = PropertyEntity({ eventClick: deletarAbaFiltrosClick, type: types.event, text: ko.observable("Remover Aba"), idGrid: guid(), visible: ko.observable(false), enable: ko.observable(true) });
    this.NomeAba = PropertyEntity({ type: types.text, text: ko.observable("Nome da Aba"), visible: ko.observable(true), val: ko.observable("") });
    this.Habilitar = PropertyEntity({ val: ko.observable(true), def: false, getType: typesKnockout.bool, visible: ko.observable(true), nomeFiltro: "" })


};
// #endregion Classes

// #region Funções de Inicialização
function loadFiltrosPersonalizadosMonitoramentoNovo() {
    _criarFiltroPersonalizadoMonitoramentoNovo = new CriarFiltroPersonalizadoMonitoramentoNovo();
    KoBindings(_criarFiltroPersonalizadoMonitoramentoNovo, "knockoutCriarFiltrosPersonalizadosMonitoramentoNovo");
    AdicionarObservables();
}
// #endregion Funções de Inicialização

// #region Funções Associadas a Eventos
function salvarFiltrosClick() {
    var filtro = _criarFiltroPersonalizadoMonitoramentoNovo;
    var listaFiltros = new Array();

    for (var prop in filtro) {
        if (filtro.hasOwnProperty(prop)) {

            var property = filtro[prop];
            var nome = property.nomeFiltro;
            if (property.val() == true) {
                listaFiltros.push(nome);
            }
        }
    }
    executarReST("FiltroPesquisaPersonalizado/AdicionarFiltrosPesquisa", { NomeAba: _criarFiltroPersonalizadoMonitoramentoNovo.NomeAba.val(), FiltrosSelecionados: JSON.stringify(listaFiltros) }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Salvo com sucesso.");
                controlarVisibilidadeFiltrosPersonalizados();
                buscarFiltrosPesquisaPersonalizado();

                if (_criarFiltroPersonalizadoMonitoramentoNovo.CheckStatusViagem.val() && _pesquisaMonitoramentoMapa.PersonalizadoStatusViagem.val())
                    buscaStatusViagemMonitoramentoNovo(null, _pesquisaMonitoramentoMapa.StatusViagem, _pesquisaMonitoramentoMapa.PersonalizadoStatusViagem);

                Global.fecharModal('divModalCriarFiltroPersonalizado');

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function deletarAbaFiltrosClick() {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a aba de Filtros?", function () {
        executarReST("FiltroPesquisaPersonalizado/RemoverAbaFiltrosPesquisa", null, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Gerais.Geral.Sucesso, "Removido com sucesso.");
                    LimparCampos(_criarFiltroPersonalizadoMonitoramentoNovo);
                    $('#liTabPersonalizada').hide();
                    $('#liTabDatas').addClass('active');
                    $('#tabDatas').addClass('active');
                    $('#tabDatas').addClass('show');
                    $('#aTabPersonalizada').removeClass('active');
                    $('#tabPersonalizada').removeClass('active');
                    $('#tabPersonalizada').removeClass('show');

                    _criarFiltroPersonalizadoMonitoramentoNovo.RemoverAbaFiltros.visible(false);
                    _criarFiltroPersonalizadoMonitoramentoNovo.Habilitar.val(true);

                    Global.fecharModal('divModalCriarFiltroPersonalizado');
                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
            }
        });
    });
}

// #endregion Funções Associadas a Eventos

// #region Funções Públicas
function buscarFiltrosPesquisaPersonalizado() {
    executarReST("FiltroPesquisaPersonalizado/ObterFiltroPesquisaPadrao", null, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                var filtro = _criarFiltroPersonalizadoMonitoramentoNovo;

                if (arg.Data.Aba.Codigo != null) {

                    _pesquisaMonitoramentoMapa.AbaPersonalizada.text(arg.Data.Aba.Descricao);
                    _criarFiltroPersonalizadoMonitoramentoNovo.NomeAba.val(arg.Data.Aba.Descricao);
                    _criarFiltroPersonalizadoMonitoramentoNovo.RemoverAbaFiltros.visible(true);

                    exibirAbaFiltrosPersonalizados();
                } else {
                    $('#liTabPersonalizada').hide();
                }

                for (var prop in filtro) {
                    if (filtro.hasOwnProperty(prop)) {

                        var property = filtro[prop];
                        var nome = property.nomeFiltro;

                        for (var data in arg.Data.NomeFiltros)
                            if (arg.Data.NomeFiltros[data].NomeFiltro == nome) {
                                property.val(true);
                            }
                    }
                }
                controlarVisibilidadeFiltrosPersonalizados();

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}
// #endregion Funções Públicas

// #region Funções Privadas
function controlarVisibilidadeFiltrosPersonalizados() {
    var filtro = _criarFiltroPersonalizadoMonitoramentoNovo;

    for (var prop in filtro) {
        if (filtro.hasOwnProperty(prop)) {

            var property = filtro[prop];

            if (property.val()) {
                var propFiltro = prop.replace("Check", "Personalizado");
                var propFiltroOriginal = prop.replace("Check", "");

                if (_pesquisaMonitoramentoMapa[propFiltro]) {
                    if (typeof _pesquisaMonitoramentoMapa[propFiltro].visible == 'function')
                        _pesquisaMonitoramentoMapa[propFiltro].visible(true);
                    else {
                        _pesquisaMonitoramentoMapa[propFiltro].visible = true;
                    }
                }

                if (_pesquisaMonitoramentoMapa[propFiltroOriginal]) {
                    if (typeof _pesquisaMonitoramentoMapa[propFiltroOriginal].visible == 'function')
                        _pesquisaMonitoramentoMapa[propFiltroOriginal].visible(false);
                    else {
                        _pesquisaMonitoramentoMapa[propFiltroOriginal].visible = false;
                    }
                }
            }

            if (!property.val()) {
                var propFiltro = prop.replace("Check", "Personalizado");
                var propFiltroOriginal = prop.replace("Check", "");

                if (_pesquisaMonitoramentoMapa[propFiltro]) {
                    if (typeof _pesquisaMonitoramentoMapa[propFiltro].visible == 'function')
                        _pesquisaMonitoramentoMapa[propFiltro].visible(false);
                    else {
                        _pesquisaMonitoramentoMapa[propFiltro].visible = false;
                    }
                }
                if (_pesquisaMonitoramentoMapa[propFiltroOriginal]) {
                    if (typeof _pesquisaMonitoramentoMapa[propFiltroOriginal].visible == 'function')
                        _pesquisaMonitoramentoMapa[propFiltroOriginal].visible(true);
                    else {
                        _pesquisaMonitoramentoMapa[propFiltroOriginal].visible = true;
                    }
                }
            }
        }
    }
}

function VerificarLimiteCheckBoxes() {
    var quantidadeLimite = 17;
    var quantidadeMenorQueLimite = true;
    var quantidadeOpcoesAtivas = 0;
    var filtro = _criarFiltroPersonalizadoMonitoramentoNovo;

    for (var prop in filtro) {
        if (filtro.hasOwnProperty(prop)) {
            var ativo = filtro[prop].val();

            if (prop != "Habilitar") {
                quantidadeOpcoesAtivas = ativo ? quantidadeOpcoesAtivas + 1 : quantidadeOpcoesAtivas;

                if (quantidadeOpcoesAtivas == 0 && _criarFiltroPersonalizadoMonitoramentoNovo.NomeAba.val() == "")
                    _criarFiltroPersonalizadoMonitoramentoNovo.SalvarFiltros.enable(true);

                if (quantidadeOpcoesAtivas >= quantidadeLimite) {
                    quantidadeMenorQueLimite = false;
                    break;
                }
            }
        }
    }
    _criarFiltroPersonalizadoMonitoramentoNovo.Habilitar.val(quantidadeMenorQueLimite);
}

function AdicionarObservables() {
    var filtro = _criarFiltroPersonalizadoMonitoramentoNovo;

    for (var prop in filtro) {
        if (filtro.hasOwnProperty(prop) && prop != "Habilitar") {
            filtro[prop].val.subscribe(VerificarLimiteCheckBoxes);
        }
    }
}

function exibirAbaFiltrosPersonalizados() {
    if ($('#liTabDatas').hasClass('active')) {
        $('#liTabDatas').removeClass('active');
        $('#tabDatas').removeClass('active');
        $('#tabDatas').removeClass('show');
    }
    if ($('#liTabMonitoramento').hasClass('active')) {
        $('#liTabMonitoramento').removeClass('active');
        $('#tabMonitoramento').removeClass('active');
        $('#tabMonitoramento').removeClass('show');
    }

    if ($('#liTabVeiculo').hasClass('active')) {
        $('#liTabVeiculo').removeClass('active');
        $('#tabVeiculo').removeClass('active');
        $('#tabVeiculo').removeClass('show');
    }

    if ($('#liTabCarga').hasClass('active')) {
        $('#liTabCarga').removeClass('active');
        $('#tabCarga').removeClass('active');
        $('#tabCarga').removeClass('show');
    }

    $('#liTabPersonalizada').show();
    $('#aTabPersonalizada').addClass('active');
    $('#tabPersonalizada').addClass('active');
    $('#tabPersonalizada').addClass('show');

}
// #endregion Funções Privadas

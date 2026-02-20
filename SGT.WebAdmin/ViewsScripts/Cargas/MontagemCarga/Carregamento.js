/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Globais.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../Consultas/Carregamento.js" />
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/PreCarga.js" />
/// <reference path="../../Consultas/TipoSeparacao.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCarregamento.js" />
/// <reference path="../../Enumeradores/EnumTipoCondicaoPagamento.js" />
/// <reference path="Bloco.js" />
/// <reference path="CapacidadeJanelaCarregamento.js" />
/// <reference path="Carga.js" />
/// <reference path="CarregamentoCarga.js" />
/// <reference path="CarregamentoFilial.js" />
/// <reference path="CarregamentoPedido.js" />
/// <reference path="Carregamentos.js" />
/// <reference path="CarregamentoTransporte.js" />
/// <reference path="DirecoesGoogleMaps.js" />
/// <reference path="Distancia.js" />
/// <reference path="GoogleMaps.js" />
/// <reference path="MontagemCarga.js" />
/// <reference path="OrigemDestino.js" />
/// <reference path="Pedido.js" />
/// <reference path="PedidoProduto.js" />
/// <reference path="PedidosMapa.js" />
/// <reference path="Roteirizador.js" />
/// <reference path="SimulacaoFrete.js" />
/// <reference path="Importacao.js" />

//*******MAPEAMENTO KNOUCKOUT*******


var _carregamento;
var _crudCarregamento;
var EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = false;
var _gerarCargasDeColeta = false;
var _buscaPeriodoCarregamento;
var _preenchendoDadosCarregamento = false;
var ocultar = false
var _precisarSetarPedidosSelecionadosTabelaMontagemCarga = true;
var _VALOR_MINIMO_VALIDADO = false;

var Carregamento = function () {
    var self = this;

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.CarregamentoRedespacho = PropertyEntity({ val: ko.observable(false), def: 0, getType: typesKnockout.bool });
    this.Carregamento = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.NumeroCarregamento.getRequiredFieldDescription()), idBtnSearch: guid(), enable: ko.observable(false) });
    this.InformarPeriodoCarregamento = PropertyEntity({ val: ko.observable(_CONFIGURACAO_TMS.InformarPeriodoCarregamentoMontagemCarga), def: _CONFIGURACAO_TMS.InformarPeriodoCarregamentoMontagemCarga, getType: typesKnockout.bool });

    this.ModeloVeicularCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (!_CONFIGURACAO_TMS.ModeloVeicularCargaNaoObrigatorioMontagemCarga ? "*" : "") + Localization.Resources.Cargas.MontagemCarga.ModeloVeicularCarga.getFieldDescription(), issue: 44, required: !_CONFIGURACAO_TMS.ModeloVeicularCargaNaoObrigatorioMontagemCarga, idBtnSearch: guid(), eventChange: modeloVeicularCargaBlur, enable: ko.observable(true), visible: ko.observable(true), numeroReboques: 0, exigirDefinicaoReboquePedido: false, OcupacaoCubicaPaletes: "0" });
    this.TipoSeparacao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TipoSeparacao.getFieldDescription()), issue: 0, required: ko.observable(false), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(_CONFIGURACAO_TMS.ExigirTipoSeparacaoMontagemCarga) });
    this.Observacao = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Observacao.getFieldDescription()), val: ko.observable(""), def: "", maxlength: 2000, visible: ko.observable(true) });
    this.TipoCondicaoPagamento = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TipoCondicaoPagamento.getFieldDescription()), val: ko.observable(EnumTipoCondicaoPagamento.Todos), options: EnumTipoCondicaoPagamento.ObterOpcoes(), def: EnumTipoCondicaoPagamento.Todos, visible: ko.observable(_CONFIGURACAO_TMS.InformarTipoCondicaoPagamentoMontagemCarga), required: ko.observable(_CONFIGURACAO_TMS.InformarTipoCondicaoPagamentoMontagemCarga) });
    this.ValorFreteManual = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ValorFreteManual.getFieldDescription()), getType: typesKnockout.decimal, maxlength: 10, visible: ko.observable(false) });
    this.TipoMontagemCarga = PropertyEntity({ val: ko.observable(EnumTipoMontagemCarga.NovaCarga), options: EnumTipoMontagemCarga.obterOpcoes(), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TipoMontagemCarga.getFieldDescription()), issue: 1141, def: EnumTipoMontagemCarga.NovaCarga, eventChange: tipoMontagemCargaChange, enable: ko.observable(true), visible: ko.observable(true) });
    this.Situacao = PropertyEntity({ val: ko.observable(EnumSituacaoCarregamento.EmMontagem), def: EnumSituacaoCarregamento.EmMontagem });
    this.PreCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PreCarga.getFieldDescription()), idBtnSearch: guid(), enable: ko.observable(true), visible: ko.observable(false), required: ko.observable(false) });
    this.Recebedor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Recebedor.getFieldDescription()), idBtnSearch: guid(), issue: 0, visible: ko.observable(false), required: ko.observable(false), val: ko.observable("") });
    this.Expedidor = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Expedidor.getFieldDescription()), idBtnSearch: guid(), issue: 0, visible: ko.observable(true), required: ko.observable(false) });
    this.PedidoViagemNavio = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PedidoViagemNavio.getFieldDescription()), idBtnSearch: guid(), required: false, visible: ko.observable(_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal), enable: ko.observable(true) });
    this.TipoDeCarga = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: (_CONFIGURACAO_TMS.TipoCargaObrigatorioMontagemCarga ? "*" : "") + Localization.Resources.Cargas.MontagemCarga.TipoDeCarga.getFieldDescription(), idBtnSearch: guid(), enable: ko.observable(true), required: _CONFIGURACAO_TMS.TipoCargaObrigatorioMontagemCarga, eventChange: tipoCargaCarregamentoBlur, visible: _CONFIGURACAO_TMS.ExibirTipoDeCargaNaAbaCarregamentoNaMontagemCarga, Paletizado: false });
    this.DataInicioViagemPrevista = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.DataPrevisaoInicioViagem.getFieldDescription()), required: ko.observable(false), getType: typesKnockout.dateTime, enable: ko.observable(true), visible: ko.observable(false) });

    //this.Destino = PropertyEntity({ text: "Destino: ", val: ko.observable("") });
    //this.NumeroCarregamento = PropertyEntity({ text: "NumeroCarregamento: ", val: ko.observable("") });

    this.Peso = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Peso.getFieldDescription()), val: ko.observable("0,0000"), def: "0,0000", visible: ko.observable(true) });
    this.CapacidadePeso = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CapacidadePeso.getFieldDescription()), val: ko.observable("0,0000"), def: "0,0000", visible: ko.observable(false) });
    this.ToleranciaPesoMenor = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ToleranciaPesoMenor.getFieldDescription()), val: ko.observable("0,0000"), def: "0,0000", visible: ko.observable(false) });
    this.LotacaoPeso = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.LotacaoPeso.getFieldDescription()), val: ko.observable("0,0000"), def: "0,0000", visible: ko.observable(false) });

    this.Pallets = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.TotalPallets.getFieldDescription()), val: ko.observable("0,0000"), def: "0,000", visible: ko.observable(true) });
    this.CapacidadePallets = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CapacidadePallets.getFieldDescription()), val: ko.observable("0,000"), def: "0,000", visible: ko.observable(false) });
    this.ToleranciaMinimaPaletes = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ToleranciaMinimaPaletes.getFieldDescription()), val: ko.observable("0,000"), def: "0,000", visible: ko.observable(false) });
    this.LotacaoPallets = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.LotacaoPallets.getFieldDescription()), val: ko.observable("0,00"), def: "0,00", visible: ko.observable(false) });

    this.CubagemPaletes = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CubagemPaletes.getFieldDescription()), val: ko.observable("0,00"), def: "0,00", getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: true, allowNegative: false } });
    this.Cubagem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Cubagem.getFieldDescription()), val: ko.observable("0,0000"), def: "0,00", visible: ko.observable(true) });
    this.CapacidadeCubagem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CapacidadeCubagem.getFieldDescription()), val: ko.observable("0,00"), def: "0,00", visible: ko.observable(false) });
    this.ToleranciaMinimaCubagem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ToleranciaMinimaCubagem.getFieldDescription()), val: ko.observable("0,00"), def: "0,00", visible: ko.observable(false) });
    this.LotacaoCubagem = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.LotacaoCubagem.getFieldDescription()), val: ko.observable("0,00"), def: "0,00", visible: ko.observable(false) });

    this.Unidade = PropertyEntity({ text: ko.observable("Unidade:"), val: ko.observable("0,0000"), def: "0,0000", visible: ko.observable(true) });
    this.CapacidadeUnidade = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.CapacidadePeso.getFieldDescription()), val: ko.observable("0,0000"), def: "0,0000", visible: ko.observable(false) });
    this.ToleranciaUnidadeMenor = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.ToleranciaPesoMenor.getFieldDescription()), val: ko.observable("0,0000"), def: "0,0000", visible: ko.observable(false) });
    this.LotacaoUnidade = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.LotacaoPeso.getFieldDescription()), val: ko.observable("0,0000"), def: "0,0000", visible: ko.observable(false) });

    this.PesoCarregamento = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PesoCarregamento.getFieldDescription()), val: ko.observable(0), visible: ko.observable(false), getType: typesKnockout.decimal, configDecimal: { precision: 4, allowZero: false, allowNegative: false } });
    this.PalletCarregamento = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.PalletCarregamento.getFieldDescription()), val: ko.observable(0), visible: ko.observable(false), getType: typesKnockout.decimal, configDecimal: { precision: 2, allowZero: false, allowNegative: false } });

    this.Pedidos = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), visible: ko.observable(true) });
    this.Cargas = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), visible: ko.observable(true) });
    this.ListaDadosPorFilial = ko.observableArray([]);

    this.Valor = PropertyEntity({ text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Valor.getFieldDescription()), val: ko.observable("0,00"), def: "0,00", visible: ko.observable(true) });

    this.DataPrevisaoSaida = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.DataPrevisaoSaida.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });
    this.DataPrevisaoRetorno = PropertyEntity({ text: Localization.Resources.Cargas.MontagemCarga.DataPrevisaoEntrega2.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.dateTime, visible: ko.observable(true) });

    this.PreCarga.required.subscribe(function (campoObrigatorio) {
        self.PreCarga.text(campoObrigatorio ? Localization.Resources.Cargas.MontagemCarga.PreCarga.getRequiredFieldDescription() : Localization.Resources.Cargas.MontagemCarga.PreCarga.getFieldDescription());
        self.PreCarga.visible(campoObrigatorio);
    });

    this.PesoCarregamento.val.subscribe(function (novoValor) {
        pesoCarregamentoChange(novoValor);
    });

    this.PalletCarregamento.val.subscribe(function (novoValor) {
        palletCarregamentoChange(novoValor);
    });
};

var CRUDCarregamento = function () {
    // #9395, vamos usar mesmo atributo pois é o mesmo cliente
    // o mesmo qquer todos os bot~ões visiveis desde o inicio.
    var visible = _CONFIGURACAO_TMS.LimparTelaAoSalvarMontagemCarga;

    this.DownloadEDI = PropertyEntity({ eventClick: downloadEDIClick, type: types.event, text: "Download EDI", visible: ko.observable(false), enable: ko.observable(true) });
    this.Bloco = PropertyEntity({ eventClick: abrirGerarBlocoClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Bloco.getFieldDescription()), visible: ko.observable(visible), enable: ko.observable(true) });
    this.SimularFrete = PropertyEntity({ eventClick: simularFreteClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.SimularFrete.getFieldDescription()), visible: ko.observable(visible), enable: ko.observable(true) });
    this.Roteirizacao = PropertyEntity({ eventClick: roteirizarCargaClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Roteirizacao.getFieldDescription()), visible: ko.observable(visible), enable: ko.observable(true) });
    this.GerarCarga = PropertyEntity({ eventClick: gerarCargaClick, type: types.event, text: (ocultaGerarCarregamentosMontagemCarga() ? Localization.Resources.Cargas.MontagemCarga.Agendar : Localization.Resources.Cargas.MontagemCarga.GerarCarga), visible: ko.observable(visible), enable: ko.observable(true) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarCarregamentoClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Cancelar), visible: ko.observable(visible), enable: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarCarregamentoClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Atualizar), visible: ko.observable(true), enable: ko.observable(true) });
    this.Limpar = PropertyEntity({ eventClick: iniciarNovoCarregamentoClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Limpar), visible: ko.observable(true), enable: ko.observable(true) });
    this.AutorizarVeiculo = PropertyEntity({ eventClick: AutorizarVeiculoClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.AutorizarVeiculo), visible: ko.observable(false), enable: ko.observable(true) });
    this.Imprimir = PropertyEntity({ eventClick: ImprimirClick, type: types.event, text: ko.observable(Localization.Resources.Cargas.MontagemCarga.Imprimir), visible: ko.observable(false), enable: ko.observable(true) });
};

function ocultaGerarCarregamentosMontagemCarga() {
    return _CONFIGURACAO_TMS.OcultaGerarCarregamentosMontagemCarga;
}

//*******EVENTOS*******

function retornoTipoCargaCarregamento(registroSelecionado) {
    _carregamento.TipoDeCarga.codEntity(registroSelecionado.Codigo);
    _carregamento.TipoDeCarga.entityDescription(registroSelecionado.Descricao);
    _carregamento.TipoDeCarga.val(registroSelecionado.Descricao);
    _carregamento.TipoDeCarga.Paletizado = registroSelecionado.Paletizado;

    obterPesosEAjustarCapacidade();
    buscarCapacidadeJanelaCarregamento(buscarPeriodoCarregamento);
}

function tipoCargaCarregamentoBlur() {
    if (_carregamento.TipoDeCarga.val() == "")
        limparDadosTipoDeCarga();
}

function pesoCarregamentoChange(valor) {
    if (_carregamento.ModeloVeicularCarga.val() == "") {
        reiniciarCapacidadesCarregamento();
    }
    _carregamento.Peso.val(_carregamento.PesoCarregamento.val());
    obterPesosEAjustarCapacidade();
    VerificarVisibilidadeBuscaSugestaoPedido();
}

function palletCarregamentoChange(valor) {
    if (_carregamento.ModeloVeicularCarga.val() == "") {
        reiniciarCapacidadesCarregamento();
    }
    _carregamento.Pallets.val(_carregamento.PalletCarregamento.val());
    obterPesosEAjustarCapacidade();
    VerificarVisibilidadeBuscaSugestaoPedido();
}

function roteirizarAutomaticamenteAoAdicionarRemoverPedido() {
    if (_roteirizadorCarregamento && _CONFIGURACAO_TMS.MontagemCarga.RoteirizarAutomaticamenteAposRoteirizadoAoAdicionarRemoverPedido === true && _carregamento.Carregamento.codEntity() > 0) {
        //Verificando se o carregamento já está "Roteirizado"
        var polilinhaRoteirizacao = _roteirizadorCarregamento.PolilinhaRota.val();
        //Se roteirizado e está configurado para roteirizar automaticamente.
        if (!string.IsNullOrWhiteSpace(polilinhaRoteirizacao)) {
            atualizarCarregamentoClick(function () {
                roteirizarCarregamentoSemModal(null, true);
                return;
            });
        }
    }
}

function roteirizarCarregamentoSemModal(callback, atualizarRoteirizacao) {
    //Vamos obter os pontos..
    // vamos chamar o buscar rota..
    // Salvar a roteirização.. 
    carregarRoteiroCarregamento(false, atualizarRoteirizacao, function (result) {
        if (result) {
            //Gerando a roteirização do carregamento...
            gerarRoteirizacaoGoogleMapsOSM(false, function (respostaOrdenada) {

                finalizarControleManualRequisicao();
                // Vamos salvar a roteirização
                salvarRotaCarregamento(callback);

            });
        }
    });
}

function roteirizarCargaClick(e) {
    var codigo = _carregamento.Carregamento.codEntity();
    if (codigo == 0) {
        atualizarCarregamentoClick(roteirizarCargaClick);
    } else {
        carregarRoteiroCarregamento(true, false);
    }
}

function tipoMontagemCargaChange(e) {
    buscarInformacoesTipoMontagem();
    PesquisarCargas();
    PesquisarPedidos();
}

function loadCarregamento() {
    _carregamento = new Carregamento();
    KoBindings(_carregamento, "knoutCarregamento");

    _crudCarregamento = new CRUDCarregamento();
    KoBindings(_crudCarregamento, "knoutCRUDCarregamento");

    HeaderAuditoria("Carregamento", _carregamento);

    BuscarPedidoViagemNavio(_carregamento.PedidoViagemNavio);
    BuscarModelosVeicularesCarga(_carregamento.ModeloVeicularCarga, retornoModeloVeicular, null, (_CONFIGURACAO_TMS.LimparTelaAoSalvarMontagemCarga == true ? _carregamento.TipoDeCarga : null), null, null, null, null, null, null, _CONFIGURACAO_TMS.LimparTelaAoSalvarMontagemCarga);
    BuscarClientes(_carregamento.Recebedor);
    BuscarClientes(_carregamento.Expedidor);

    if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.AgruparCargas)
        BuscarCarregamento(_carregamento.Carregamento, retornoCarregamento, EnumSituacaoCarregamento.obterSituacoesEmMontagem(), null, _CONFIGURACAO_TMS.TipoMontagemCargaPadrao);
    else
        BuscarCarregamento(_carregamento.Carregamento, retornoCarregamento, EnumSituacaoCarregamento.obterSituacoesEmMontagem());

    //Tarefa MARFRIG, #9391 vamos filtrar somente os tipos de carga da filial.
    //if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador)
    //    new BuscarTiposdeCargaPorFilial(_carregamento.TipoDeCarga, retornoTipoCargaCarregamento, _pesquisaMontegemCarga.Filial);
    //else
    // Rarefa 9391 devolvida... adicionado parametro no tipo de carga.
    BuscarTiposdeCarga(_carregamento.TipoDeCarga, retornoTipoCargaCarregamento, null, null, null, null, null, true);

    var consultaPreCarga = new BuscarPreCarga(_carregamento.PreCarga, null, _carregamento.Filial, true);

    consultaPreCarga.SetFiltro('SemCarga', true);

    if (_CONFIGURACAO_TMS.UtilizaEmissaoMultimodal == true) {
        _carregamento.PedidoViagemNavio.required = true;

        _carregamento.ModeloVeicularCarga.required = false;
        _carregamento.ModeloVeicularCarga.visible(false);
        $("#divPesoMedida").hide();
    }

    BuscarTiposSeparacao(_carregamento.TipoSeparacao);

    loadCarregamentoTransporte();
    loadCarregamentoPedido();
    loadCarregamentoCarga();
    loadCarregamentoFilial();
    loadDirecoesGoogleMaps();
    loadPedidoMapa();
    loadBlocosCarregamento();
    loadCarregamentoAutorizacao();
    _precisarSetarPedidosSelecionadosTabelaMontagemCarga = true;
    PEDIDOS_SELECIONADOS.subscribe(PedidosSelecionadosChange);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        definirDataCarregamentoPorFilial(_CONFIGURACAO_TMS.InformaHorarioCarregamentoMontagemCarga ? Global.DataHoraAtual() : Global.DataAtual());

    if (_CONFIGURACAO_TMS.MontagemCarga.DataAtualNovoCarregamento)
        definirDataCarregamentoPorFilial(_CONFIGURACAO_TMS.InformaHorarioCarregamentoMontagemCarga ? Global.DataHoraAtual() : Global.DataAtual());
}

function setConfiguracaoColeta() {
    _carregamento.Recebedor.required(_gerarCargasDeColeta);
    _carregamento.Recebedor.visible(_gerarCargasDeColeta);
}

function atualizarCarregamentoClick(callback) {
    var valido = true;

    if (!ValidarCamposObrigatorios(_carregamento))
        valido = false;

    if (!ValidarCamposObrigatorios(_carregamentoTransporte))
        valido = false;

    if (valido) {
        if ((_CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.AgruparCargas && _carregamentoTransporte.Motoristas.basicTable.BuscarRegistros().length <= 0) || (_CONFIGURACAO_TMS.MotoristaObrigatorioMontagemCarga && _carregamentoTransporte.Motoristas.basicTable.BuscarRegistros().length <= 0 && !_CONFIGURACAO_TMS.DesativarMultiplosMotoristasMontagemCarga)) {
            exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.MotoristaObrigatorio, Localization.Resources.Cargas.MontagemCarga.ObrigatorioInformarMotorista);
            return;
        }

        _carregamento.CarregamentoRedespacho.val(_objPesquisaMontagem.GerarCargasDeRedespacho);

        preencherListaMotorista();

        var carregamento = obterCarregamentoSalvar();

        if (!carregamento)
            return;

        executarReST("MontagemCarga/SalvarCarregamento", carregamento, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    if (ocultaGerarCarregamentosMontagemCarga())
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.MontagemCarga.Successo, Localization.Resources.Cargas.MontagemCarga.Carregamento + arg.Data.Descricao + Localization.Resources.Cargas.MontagemCarga.SalvoComSucesso, 6000);
                    else
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.MontagemCarga.Successo, Localization.Resources.Cargas.MontagemCarga.SalvoComSucesso);

                    _carregamento.Carregamento.codEntity(arg.Data.Codigo);
                    _carregamento.Carregamento.val(arg.Data.Descricao);

                    setarOpcoesCarregamento();
                    atualizarDadosPedidosSelecionados();

                    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS && _carregamento.TipoMontagemCarga.val() == EnumTipoMontagemCarga.NovaCarga)
                        _crudCarregamento.Imprimir.visible(true);

                    if (callback instanceof Function) {
                        PesquisarCarregamentos();
                        callback();
                        return;
                    }

                    if (_CONFIGURACAO_TMS.LimparTelaAoSalvarMontagemCarga) {
                        limparDadosCarregamento();
                        BuscarDadosMontagemCarga();
                        return;
                    }
                }
                else
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.Atencao, arg.Msg, 8000);
            }
            else {
                exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, arg.Msg);
            }
        });
    }
    else
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.CamposObrigatorios, Localization.Resources.Cargas.MontagemCarga.InformeOsCamposObrigatorios);
}

function AutorizarVeiculoClick() {
    exibirConfirmacao(Localization.Resources.Cargas.MontagemCarga.Confirmacao, Localization.Resources.Cargas.MontagemCarga.RealmenteDesejaAutorizarGeracaoDaCargaDoVeiculoSelecionado, function () {
        var data = { Codigo: _carregamento.Carregamento.codEntity() };
        executarReST("MontagemCarga/AutorizarVeiculo", data, function (arg) {
            if (arg.Success) {
                if (arg.Data !== false) {
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.MontagemCarga.AutorizadoComSucesso);
                    _crudCarregamento.AutorizarVeiculo.visible(false);

                } else {
                    exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.Atencao, arg.Msg);
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.Atencao, arg.Msg);
            }
        });
    });
}

function ImprimirClick(e, sender) {
    var data = { Codigo: _carregamento.Carregamento.codEntity(), Carregamento: true };
    executarReST("Pedido/GerarRelatorio", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.MontagemCarga.Successo, Localization.Resources.Cargas.MontagemCarga.AguardeQueSeuRelatorioEstaSendoGerado);
            } else {
                exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.MontagemCarga.Aviso, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.aviso, Localization.Resources.Cargas.MontagemCarga.Aviso, arg.Msg);
        }
    });
}

function iniciarNovoCarregamentoClick() {
    limparDadosCarregamento();
}

function gerarCargaClick() {
    if (PEDIDOS_SELECIONADOS().some(item => item.ValorMercadoInferior === true)) {
        let pedidosFiltrados = ko.utils.arrayFilter(PEDIDOS_SELECIONADOS(), function (pedido) {
            return pedido.ValorMercadoInferior === true;
        }).map(function (pedido) {
            return pedido.NumeroPedidoEmbarcador;
        });

        let menssagem = Localization.Resources.Cargas.MontagemCarga.OsPedidos + pedidosFiltrados.join(', ') + Localization.Resources.Cargas.MontagemCarga.NaoFaraoParteCarregamento;
        exibirConfirmacao(Localization.Resources.Cargas.MontagemCarga.Confirmacao, menssagem, removePedidosValorMercadoInferior, function () { return; });
    } else {
        gerarCargaValida();
    }
}

function cancelarCarregamentoClick() {
    exibirConfirmacao(Localization.Resources.Cargas.MontagemCarga.Confirmacao, Localization.Resources.Cargas.MontagemCarga.RealmenteDesejaCancelarCarregamento, function () {
        var codigo = _carregamento.Carregamento.codEntity();
        if (codigo > 0) {
            executarReST("MontagemCarga/CancelarCarregamento", { Codigo: codigo }, function (arg) {
                if (arg.Success) {
                    if (arg.Data !== false) {
                        exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.MontagemCarga.Successo, Localization.Resources.Cargas.MontagemCarga.CanceladoComSucesso);
                        limparDadosCarregamento();
                        BuscarDadosMontagemCarga();
                    } else {
                        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.Atencao, arg.Msg);
                    }
                } else {
                    exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, arg.Msg);
                }
            });
        } else {
            exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.MontagemCarga.Successo, Localization.Resources.Cargas.MontagemCarga.CanceladoComSucesso);
            limparDadosCarregamento();
        }
    });
}

function retornoCarregamento(carregamento) {
    executarReST("MontagemCarga/BuscarPorCodigo", carregamento, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                PreencherCarregamento(arg.Data);
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, arg.Msg);
        }
    });
}

function modeloVeicularCargaBlur() {
    if (_carregamento.ModeloVeicularCarga.val() == "")
        limparDadosModeloVeicularCarga();
}

function retornoModeloVeicular(dadosModelo) {
    preencherModeloVeicularCarga(dadosModelo);
}

function PedidosSelecionadosChange() {
    if (EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS === true) return;
    var peso = 0;
    var pesoLiquido = 0;
    var pallets = 0;
    var cubagem = 0;
    var pesoRestante = 0;
    var volume = 0;
    var valor = 0;

    var pedidos = PEDIDOS_SELECIONADOS();
    for (var i in pedidos) {
        var pedido = pedidos[i];

        peso += Globalize.parseFloat(pedido.PesoSaldoRestante);//.Peso);
        pesoLiquido += Globalize.parseFloat(pedido.PesoLiquido);//.Peso);
        cubagem += Globalize.parseFloat(pedido.Cubagem);
        pallets += Globalize.parseFloat(pedido.PalletSaldoRestante); // pedido.TotalPallets
        pesoRestante += Globalize.parseFloat(pedido.PesoSaldoRestante);
        volume += Globalize.parseFloat(pedido.Volumes);
        valor += Globalize.parseFloat(pedido.Valor);
    }

    // Atualizando totais pedidos selecionados area pedidos.
    _AreaPedido.TotalPedidosSelecionados.val(pedidos.length);
    _AreaPedido.PesoTotalSelecionados.val(Globalize.format(peso, "n3"));
    _AreaPedido.PesoLiquidoTotalSelecionados.val(Globalize.format(pesoLiquido, "n3"));
    _AreaPedido.PesoSaldoRestanteSelecionados.val(Globalize.format(pesoRestante, "n3"));
    _AreaPedido.VolumeTotalSelecionados.val(volume);
    _AreaPedido.ValorPedidosSelecionados.val(valor);

    ajustarCapacidades(peso, cubagem, pallets, volume);
    RenderizarGridMotagemPedidos();
    VerificarVisibilidadeBuscaSugestaoPedido();

    _carregamento.PreCarga.required(isCampoPreCargaObrigatorio());

    if (PEDIDOS_SELECIONADOS() != null && PEDIDOS_SELECIONADOS().some(p => p.TipoOperacaoExigirInformarDataPrevisaoInicioViagem)) {
        _carregamento.DataInicioViagemPrevista.visible(true);
        _carregamento.DataInicioViagemPrevista.required(true);
    }

    RemarcarPontosPedidosMapa();

    if (_carregamento.Carregamento.codEntity() == 0) {
        var tiposDeOperacao = PEDIDOS_SELECIONADOS().map(item => item.TipoOperacao).filter((value, index, self) => self.indexOf(value) === index);

        if (tiposDeOperacao.length == 1 && !PEDIDOS_SELECIONADOS()[0].OcultarTipoDeOperacaoNaMontagemDaCarga) {
            var tiposDeOperacaoCodigos = PEDIDOS_SELECIONADOS().map(item => item.CodigoTipoOperacao).filter((value, index, self) => self.indexOf(value) === index);

            _carregamentoTransporte.TipoOperacao.val(tiposDeOperacao[0]);
            _carregamentoTransporte.TipoOperacao.entityDescription(tiposDeOperacao[0]);
            _carregamentoTransporte.TipoOperacao.codEntity(tiposDeOperacaoCodigos[0]);
            _carregamentoTransporte.NecessarioConfirmacaoMotorista.val(PEDIDOS_SELECIONADOS()[0].NecessarioConfirmacaoMotorista);
            _carregamentoTransporte.TempoLimiteConfirmacaoMotorista.val(PEDIDOS_SELECIONADOS()[0].TempoLimiteConfirmacaoMotorista);
            _carregamentoTransporte.Recebedor.visible(PEDIDOS_SELECIONADOS()[0].TipoOperacaoInformarRecebedor);
            _carregamento.Unidade.visible(PEDIDOS_SELECIONADOS()[0].TipoOperacaoControlarCapacidadePorUnidade);
            //_carregamento.Peso.visible(!PEDIDOS_SELECIONADOS()[0].TipoOperacaoControlarCapacidadePorUnidade);

        } else {
            _carregamentoTransporte.TipoOperacao.val('');
            _carregamentoTransporte.TipoOperacao.entityDescription('');
            _carregamentoTransporte.TipoOperacao.codEntity(0);
            _carregamento.Unidade.visible(false);
            _carregamentoTransporte.Recebedor.visible(false)
            //_carregamento.Peso.visible(true);
        }

        var tiposDeCarga = PEDIDOS_SELECIONADOS().map(item => item.TipoCarga).filter((value, index, self) => self.indexOf(value) === index);

        if (tiposDeCarga.length == 1) {
            var tiposDeCargaCodigos = PEDIDOS_SELECIONADOS().map(item => item.CodigoTipoCarga).filter((value, index, self) => self.indexOf(value) === index);

            _carregamentoTransporte.TipoDeCarga.val(tiposDeCarga[0]);
            _carregamentoTransporte.TipoDeCarga.entityDescription(tiposDeCarga[0]);
            _carregamentoTransporte.TipoDeCarga.codEntity(tiposDeCargaCodigos[0]);
        }
        else {
            LimparCampo(_carregamentoTransporte.TipoDeCarga);
            limparDadosTipoDeCarga();
        }

        var modelosVeicularesCarga = PEDIDOS_SELECIONADOS().map(item => item.ModeloVeicularCarga).filter((value, index, self) => self.map(item => item.Codigo).indexOf(value.Codigo) === index);

        if ((modelosVeicularesCarga.length == 1) && (modelosVeicularesCarga[0].Codigo > 0))
            preencherModeloVeicularCarga(modelosVeicularesCarga[0]);
        else {
            LimparCampo(_carregamento.ModeloVeicularCarga);
            limparDadosModeloVeicularCarga();
        }

        if (_CONFIGURACAO_TMS.InformarTipoCondicaoPagamentoMontagemCarga) {
            var tiposCondicaoPagamento = PEDIDOS_SELECIONADOS().map(item => item.TipoCondicaoPagamento).filter((value, index, self) => self.indexOf(value) === index);

            if (tiposCondicaoPagamento.length == 1)
                _carregamento.TipoCondicaoPagamento.val(tiposCondicaoPagamento[0]);
            else
                _carregamento.TipoCondicaoPagamento.val(EnumTipoCondicaoPagamento.Todos);
        }

        const codigosExpedidores = PEDIDOS_SELECIONADOS().map(item => item.CodigoExpedidor).filter((value, index, self) => self.indexOf(value) === index);

        if (codigosExpedidores.length == 1) {
            if (PEDIDOS_SELECIONADOS()[0].CodigoExpedidor > 0) {
                _carregamentoTransporte.Expedidor.val(PEDIDOS_SELECIONADOS()[0].Expedidor);
                _carregamentoTransporte.Expedidor.entityDescription(PEDIDOS_SELECIONADOS()[0].Expedidor);
                _carregamentoTransporte.Expedidor.codEntity(PEDIDOS_SELECIONADOS()[0].CodigoExpedidor);
            } else {
                _carregamentoTransporte.Expedidor.val('');
                _carregamentoTransporte.Expedidor.entityDescription('');
                _carregamentoTransporte.Expedidor.codEntity(0);
            }
        } else {
            _carregamentoTransporte.Expedidor.val('');
            _carregamentoTransporte.Expedidor.entityDescription('');
            _carregamentoTransporte.Expedidor.codEntity(0);
        }
    }

    atualizarDadosCarregamentoPorFilial();
    buscarCapacidadeJanelaCarregamento(buscarPeriodoCarregamento);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        ObterDetalhesPedidoParaMontagemCarga(pedidos);

    if (_precisarSetarPedidosSelecionadosTabelaMontagemCarga)
        setarPedidosSelecionadosMontagemCarga();
}

function ObterDetalhesPedidoParaMontagemCarga(pedidos) {
    var codigosPedidos = new Array();

    for (var i = 0; i < pedidos.length; i++)
        codigosPedidos.push(pedidos[i].Codigo);

    executarReST("MontagemCarga/ObterDetalhesPedidosMontagemCarga", { Pedidos: JSON.stringify(codigosPedidos), TipoOperacao: _carregamentoTransporte.TipoOperacao.codEntity() }, function (r) {
        if (r.Success) {
            if (r.Data) {
                if (!string.IsNullOrWhiteSpace(r.Data.Mensagem)) {
                    $("#divAlertaPedidosCarregamento").html('<i class="fal fa-info-circle"></i>&nbsp;<span>' + r.Data.Mensagem + '</span>');
                    $("#divAlertaPedidosCarregamento").show();
                } else {
                    $("#divAlertaPedidosCarregamento").hide();
                }

                if (_carregamento.Carregamento.codEntity() <= 0) {
                    _carregamentoTransporte.TipoOperacao.val(r.Data.DadosTransporte.TipoOperacao.Descricao);
                    _carregamentoTransporte.TipoOperacao.codEntity(r.Data.DadosTransporte.TipoOperacao.Codigo);
                    _carregamentoTransporte.TipoDeCarga.val(r.Data.DadosTransporte.TipoCarga.Descricao);
                    _carregamentoTransporte.TipoDeCarga.codEntity(r.Data.DadosTransporte.TipoCarga.Codigo);
                    _carregamentoTransporte.Veiculo.val(r.Data.DadosTransporte.Veiculo.Descricao);
                    _carregamentoTransporte.Veiculo.codEntity(r.Data.DadosTransporte.Veiculo.Codigo);
                    _carregamentoTransporte.Veiculo.val(r.Data.DadosTransporte.Veiculo.Descricao);
                    _carregamentoTransporte.Veiculo.codEntity(r.Data.DadosTransporte.Veiculo.Codigo);
                    _carregamento.ModeloVeicularCarga.val(r.Data.DadosTransporte.ModeloVeicular.Descricao);
                    _carregamento.ModeloVeicularCarga.codEntity(r.Data.DadosTransporte.ModeloVeicular.Codigo);
                    _carregamentoTransporte.Empresa.val(r.Data.DadosTransporte.Empresa.Descricao);
                    _carregamentoTransporte.Empresa.codEntity(r.Data.DadosTransporte.Empresa.Codigo);
                    _carregamentoTransporte.RaizCNPJEmpresa.val(r.Data.DadosTransporte.RaizCNPJEmpresa);

                    var dataGrid = r.Data.DadosTransporte.Motoristas;

                    _gridMotoristas.CarregarGrid(dataGrid);
                }

            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.Atencao, r.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, r.Msg);
        }
    });
}

function downloadEDIClick(e) {
    executarDownload("MontagemCarga/BaixarEDICarregamento", { Codigo: _carregamento.Carregamento.codEntity() });
}

//*******MÉTODOS*******

function gerarCarga(permitirHorarioCarregamentoInferiorAoAtual, permitirHorarioCarregamentoComLimiteAtingido, permitirHorarioDescarregamentoInferiorAoAtual, permitirHorarioDescarregamentoComLimiteAtingido, permitirGerarCargaSemJanelaDescarregamento) {
    var carregamento = obterCarregamentoSalvar();

    if (!carregamento)
        return;

    carregamento["PermitirGerarCargaSemJanelaDescarregamento"] = Boolean(permitirGerarCargaSemJanelaDescarregamento);
    carregamento["PermitirHorarioCarregamentoComLimiteAtingido"] = Boolean(permitirHorarioCarregamentoComLimiteAtingido);
    carregamento["PermitirHorarioCarregamentoInferiorAoAtual"] = Boolean(permitirHorarioCarregamentoInferiorAoAtual);
    carregamento["PermitirHorarioDescarregamentoComLimiteAtingido"] = Boolean(permitirHorarioDescarregamentoComLimiteAtingido);
    carregamento["PermitirHorarioDescarregamentoInferiorAoAtual"] = Boolean(permitirHorarioDescarregamentoInferiorAoAtual);

    executarReST("MontagemCarga/GerarCarga", carregamento, function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                if (arg.Data.HorarioCarregamentoInferiorAtual) {
                    exibirConfirmacao(Localization.Resources.Cargas.MontagemCarga.Confirmacao, arg.Msg + " " + Localization.Resources.Cargas.MontagemCarga.DesejaSobreporEssaRegra, function () {
                        permitirHorarioCarregamentoInferiorAoAtual = true;
                        gerarCarga(permitirHorarioCarregamentoInferiorAoAtual, permitirHorarioCarregamentoComLimiteAtingido, permitirHorarioDescarregamentoInferiorAoAtual, permitirHorarioDescarregamentoComLimiteAtingido, permitirGerarCargaSemJanelaDescarregamento);
                    });
                    return;
                }

                if (arg.Data.HorarioLimiteCarregamentoAtingido) {
                    exibirConfirmacao(Localization.Resources.Cargas.MontagemCarga.Confirmacao, arg.Msg + " " + Localization.Resources.Cargas.MontagemCarga.DesejaSobreporEssaRegra, function () {
                        permitirHorarioCarregamentoComLimiteAtingido = true;
                        gerarCarga(permitirHorarioCarregamentoInferiorAoAtual, permitirHorarioCarregamentoComLimiteAtingido, permitirHorarioDescarregamentoInferiorAoAtual, permitirHorarioDescarregamentoComLimiteAtingido, permitirGerarCargaSemJanelaDescarregamento);
                    });
                    return;
                }

                if (arg.Data.HorarioDescarregamentoInferiorAtual) {
                    exibirConfirmacao(Localization.Resources.Cargas.MontagemCarga.Confirmacao, arg.Msg + " " + Localization.Resources.Cargas.MontagemCarga.DesejaSobreporEssaRegra, function () {
                        permitirHorarioDescarregamentoInferiorAoAtual = true;
                        gerarCarga(permitirHorarioCarregamentoInferiorAoAtual, permitirHorarioCarregamentoComLimiteAtingido, permitirHorarioDescarregamentoInferiorAoAtual, permitirHorarioDescarregamentoComLimiteAtingido, permitirGerarCargaSemJanelaDescarregamento);
                    });
                    return;
                }

                if (arg.Data.HorarioLimiteDescarregamentoAtingido) {
                    exibirConfirmacao(Localization.Resources.Cargas.MontagemCarga.Confirmacao, arg.Msg + " " + Localization.Resources.Cargas.MontagemCarga.DesejaSobreporEssaRegra, function () {
                        permitirHorarioDescarregamentoComLimiteAtingido = true;
                        gerarCarga(permitirHorarioCarregamentoInferiorAoAtual, permitirHorarioCarregamentoComLimiteAtingido, permitirHorarioDescarregamentoInferiorAoAtual, permitirHorarioDescarregamentoComLimiteAtingido, permitirGerarCargaSemJanelaDescarregamento);
                    });
                    return;
                }

                if (arg.Data.HorarioDescarregamentoIndisponivel) {
                    exibirConfirmacao(Localization.Resources.Cargas.MontagemCarga.Confirmacao, arg.Msg + " " + Localization.Resources.Cargas.MontagemCarga.DesejaSobreporEssaRegra, function () {
                        permitirGerarCargaSemJanelaDescarregamento = true;
                        gerarCarga(permitirHorarioCarregamentoInferiorAoAtual, permitirHorarioCarregamentoComLimiteAtingido, permitirHorarioDescarregamentoInferiorAoAtual, permitirHorarioDescarregamentoComLimiteAtingido, permitirGerarCargaSemJanelaDescarregamento);
                    });
                    return;
                }

                if (arg.Data.CarregamentoAguardandoAprovacao)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.MontagemCarga.Successo, Localization.Resources.Cargas.MontagemCarga.CarregamentoAguardandoAprovacaoPara + " " + (ocultar ? Localization.Resources.Cargas.MontagemCarga.Agendar : Localization.Resources.Cargas.MontagemCarga.Gerar) + " " + Localization.Resources.Cargas.MontagemCarga.Carga, 6000);
                else if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS || _CONFIGURACAO_TMS.ExibirListagemNotasFiscais)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.MontagemCarga.Successo, Localization.Resources.Cargas.MontagemCarga.Carga + " " + arg.Data.NumerosCargasGeradas + " " + Localization.Resources.Cargas.MontagemCarga.GeradaComSucesso, 6000);
                else if (ocultar)
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.MontagemCarga.Successo, Localization.Resources.Cargas.MontagemCarga.Carga + arg.Data.NumerosCargasGeradas + " " + Localization.Resources.Cargas.MontagemCarga.AgendadaComSucesso, 6000);
                else
                    exibirMensagem(tipoMensagem.ok, Localization.Resources.Cargas.MontagemCarga.Successo, Localization.Resources.Cargas.MontagemCarga.CargaGeradaComSucesso);

                limparDadosCarregamento();
                BuscarDadosMontagemCarga();
            }
            else
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.Atencao, arg.Msg, 15000);
        }
        else
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Cargas.MontagemCarga.Falha, arg.Msg);
    });
}

function PesquisarCarregamentos() {
    //if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.Todos || _CONFIGURACAO_TMS.TipoMontagemCargaPadrao === EnumTipoMontagemCarga.AgruparCargas) {
    $("#" + _AreaCarregamento.Carregamentos.id).html("");
    _AreaCarregamento.Inicio.val(0);
    _knoutsCarregamentos = new Array();
    buscarCarregamentos();
    //}
}

function buscarInformacoesTipoMontagem() {
    if (_carregamento.TipoMontagemCarga.val() == EnumTipoMontagemCarga.NovaCarga) {
        $("#liPedidos").show();
        //$("#liPedidos a").click();
        $("#liCargas").hide();

        if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao !== EnumTipoMontagemCarga.Todos) {
            $("#liAreaPedidos").show();
            $("#liAreaPedidos a").click();
            $("#liAreaCargas").hide();

            if (_CONFIGURACAO_TMS.PermiteAdicionarNotaManualmente)
                $("#liMapa").show();
        }

        _carregamento.Cargas.val(new Array());

        desmarcarKnoutsCarga();
        RenderizarGridMotagemCargas();
    }
    else {
        $("#liCargas").show();
        //$("#liCargas a").click();
        $("#liPedidos").hide();
        $("#liAreaCargas").show();
        $("#liAreaCargas a").click();
        $("#liAreaPedidos").hide();
        if (_CONFIGURACAO_TMS.TipoMontagemCargaPadrao !== EnumTipoMontagemCarga.Todos) {
            $("#liAreaCargas").show();
            $("#liAreaCargas a").click();
            $("#liAreaPedidos").hide();
            $("#liMapa").hide();
        }
        PEDIDOS_SELECIONADOS.removeAll();
        _carregamento.PreCarga.required(false);

        LimparCampoEntity(_carregamento.PreCarga);

        LimparPedidosSelecionados();
        RenderizarGridMotagemPedidos();
        buscarCapacidadeJanelaCarregamento();
    }
}

function setarOpcoesCarregamento() {
    if (_carregamento.TipoMontagemCarga.val() == EnumTipoMontagemCarga.NovaCarga) {
        let permitirEditarCarregamento = EnumSituacaoCarregamento.permitirEditarCarregamento(_carregamento.Situacao.val());

        _crudCarregamento.Atualizar.visible(permitirEditarCarregamento);
        _crudCarregamento.GerarCarga.visible(permitirEditarCarregamento);
        _crudCarregamento.Roteirizacao.visible(permitirEditarCarregamento);
        _crudCarregamento.SimularFrete.visible(permitirEditarCarregamento);
        _crudCarregamento.DownloadEDI.visible(false);
        _crudCarregamento.Bloco.visible(!_carregamento.CarregamentoRedespacho.val() && permitirEditarCarregamento);

        if (EnumSituacaoCarregamento.permitirCancelarCarregamento(_carregamento.Situacao.val()) &&
            (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe))
            _crudCarregamento.Cancelar.visible(true);
    }

    _carregamento.TipoMontagemCarga.enable(false);
}

function PreencherCarregamento(carregamento) {
    limparCarregamentoTransporte();
    LimparPedidosSelecionados();

    try {
        _preenchendoDadosCarregamento = true;

        PreencherObjetoKnout(_carregamento, { Data: carregamento.Carregamento });

        _crudCarregamento.SimularFrete.visible(false);
        _crudCarregamento.Bloco.visible(false);
        _crudCarregamento.DownloadEDI.visible(false);
        _carregamento.PreCarga.required(isCampoPreCargaObrigatorio());

        if (carregamento.Carregamento.VeiculoBloqueado) {
            if (VerificaSePossuiPermissaoEspecial(EnumPermissaoPersonalizada.MontagemCarga_LiberacaoVeiculo, _PermissoesPersonalizadasCarga))
                _crudCarregamento.AutorizarVeiculo.visible(true);
            else
                _crudCarregamento.AutorizarVeiculo.visible(false);
        }
        else
            _crudCarregamento.AutorizarVeiculo.visible(false);

        if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS && _carregamento.TipoMontagemCarga.val() == EnumTipoMontagemCarga.NovaCarga)
            _crudCarregamento.Imprimir.visible(true);

        preencherDadosTransporte(carregamento.Transporte);
        preencherDadosCarregamentoPorFilial(carregamento.Carregamento);
        controlarCamposPorGrupoTransportadorInformado();

        if (_carregamento.CarregamentoRedespacho.val() != _objPesquisaMontagem.GerarCargasDeRedespacho) {
            _pesquisaMontegemCarga.GerarCargasDeRedespacho.val(_carregamento.CarregamentoRedespacho.val());
            BuscarDadosMontagemCarga();
        };


        var peso = 0;
        var cubagem = 0;
        var pallets = 0;
        var volume = 0;

        var totalizadoresPedidos = CarregarPedidosCarragmento(carregamento.Carregamento.Pedidos, carregamento.Roteirizacao);
        peso = totalizadoresPedidos.peso;
        cubagem = totalizadoresPedidos.cubagem;
        pallets = totalizadoresPedidos.pallets;
        volume = totalizadoresPedidos.volume;

        RenderizarGridMotagemPedidos();
        desmarcarKnoutsCarga();

        for (var i = 0; i < carregamento.Carregamento.Cargas.length; i++) {
            var carga = carregamento.Carregamento.Cargas[i];
            var index = obterIndiceKnoutCarga(carga);
            if (index >= 0) {
                _knoutsCargas[index].InfoCarga.cssClass("card card-carga-selecionada no-padding padding-5");
            }
            peso += Globalize.parseFloat(carga.Peso);
            cubagem += Globalize.parseFloat(carga.Cubagem);
            pallets += Globalize.parseFloat(carga.TotalPallets);
            _carregamentoTransporte.RaizCNPJEmpresa.val(carga.RaizCNPJEmpresa);
        }
        RenderizarGridMotagemCargas();
        preencherModeloVeicularCarga(carregamento.Carregamento.ModeloVeicularCarga);
        ajustarCapacidades(peso, cubagem, pallets, volume);
        setarOpcoesCarregamento();
        buscarInformacoesTipoMontagem();
        VerificarCompatibilidasKnoutsCarga();
        ValidarFronteira();
        buscarCapacidadeJanelaCarregamento();
        preencherCarregamentoAutorizacao(carregamento.Carregamento.Carregamento.Codigo);

        NOTAS_FISCAIS_SELECIONADAS(carregamento.NotasFiscaisEnviar);

        if (_CONFIGURACAO_TMS.PermiteAdicionarNotaManualmente)
            AtualizarPontosFaltantes();
    }
    finally {
        _preenchendoDadosCarregamento = false;
    }
}

function preencherModeloVeicularCarga(dadosModeloVeicularCarga) {
    _carregamento.ModeloVeicularCarga.codEntity(dadosModeloVeicularCarga.Codigo);
    _carregamento.ModeloVeicularCarga.val(dadosModeloVeicularCarga.Descricao);
    _carregamento.ModeloVeicularCarga.numeroReboques = dadosModeloVeicularCarga.NumeroReboques;
    _carregamento.ModeloVeicularCarga.exigirDefinicaoReboquePedido = dadosModeloVeicularCarga.ExigirDefinicaoReboquePedido;
    _carregamento.ModeloVeicularCarga.OcupacaoCubicaPaletes = dadosModeloVeicularCarga.OcupacaoCubicaPaletes;
    _carregamento.CapacidadePeso.val(dadosModeloVeicularCarga.CapacidadePesoTransporte);
    _carregamento.CapacidadeUnidade.val(dadosModeloVeicularCarga.CapacidadePesoTransporte);
    _carregamento.CapacidadeCubagem.val(dadosModeloVeicularCarga.Cubagem);
    _carregamento.CapacidadePallets.val(dadosModeloVeicularCarga.NumeroPaletes);
    _carregamento.ToleranciaMinimaPaletes.val(dadosModeloVeicularCarga.ToleranciaMinimaPaletes);
    _carregamento.ToleranciaMinimaCubagem.val(dadosModeloVeicularCarga.ToleranciaMinimaCubagem);
    _carregamento.ToleranciaPesoMenor.val(dadosModeloVeicularCarga.ToleranciaPesoMenor);

    var unidadeCapacidade;

    if (EnumUnidadeCapacidade.Unidade == dadosModeloVeicularCarga.UnidadeCapacidade)
        unidadeCapacidade = true
    else if (EnumUnidadeCapacidade.Unidade == dadosModeloVeicularCarga.Peso)
        unidadeCapacidade = false
    else
        unidadeCapacidade = false

    _carregamento.Peso.visible(!unidadeCapacidade);
    _carregamento.Unidade.visible(unidadeCapacidade);

    if (dadosModeloVeicularCarga.ModeloControlaCubagem)
        _carregamento.Cubagem.visible(true);
    else
        _carregamento.Cubagem.visible(false);

    if (dadosModeloVeicularCarga.VeiculoPaletizado)
        _carregamento.Pallets.visible(true);
    else
        _carregamento.Pallets.visible(false);

    obterPesosEAjustarCapacidade();
    VerificarVisibilidadeBuscaSugestaoPedido();
    carregarDefinicaoReboquePedidosSelecionados();
    carregarDefinicaoTipoCarregamentoPedidosSelecionados();
}

function CarregarPedidosCarragmento(pedidosCarregamento, dadosRoteirizacao) {
    var pedidos = PEDIDOS();

    var peso = 0;
    var cubagem = 0;
    var pallets = 0;
    var volume = 0;

    EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = true;
    for (var i = 0; i < pedidosCarregamento.length; i++) {
        var pedido = pedidosCarregamento[i];

        var pedidoListado = PEDIDOS.update(function (ped) { return ped.Codigo == pedido.Codigo }, function (ped) { ped.Selecionado = true; return ped; });
        if (!pedidoListado) {
            pedido.Selecionado = true;
            PEDIDOS_NAO_LISTADOS.push(pedido);
        }

        if (isNaN(parseFloat(pedido.PalletPedidoCarregamento)) || parseFloat(pedido.PalletPedidoCarregamento) == 0)
            pedido.PalletPedidoCarregamento = parseFloat(pedido.PalletSaldoRestante);

        VerificarPontosFaltantes(pedido);
        peso += Globalize.parseFloat(pedido.Peso);
        cubagem += Globalize.parseFloat(pedido.Cubagem);
        volume += Globalize.parseFloat(pedido.Volumes);

        var tmpPallets = pedido.PalletPedidoCarregamento;
        if (tmpPallets == 0) {
            tmpPallets = Globalize.parseFloat(pedido.TotalPallets);
        }
        pallets += tmpPallets;
        PEDIDOS_SELECIONADOS.push(pedido);
    }

    // Ajustar ordem se tiver Roteirizacao... #33780
    if (dadosRoteirizacao) {
        for (var i = 0; i < dadosRoteirizacao.Pedidos.length; i++) {
            var index = PEDIDOS_SELECIONADOS().findIndex(function (item) { return item.Codigo == dadosRoteirizacao.Pedidos[i]; });
            if (index >= 0)
                PEDIDOS_SELECIONADOS()[index].Ordem = (i + 1);
        }
    }

    EVITAR_SUBSCRIBE_PEDIDOS_SELECIONADOS = false;
    _precisarSetarPedidosSelecionadosTabelaMontagemCarga = true;
    PedidosSelecionadosChange();

    pedidos = PEDIDOS_NAO_LISTADOS().concat(pedidos);
    PEDIDOS(pedidos);

    return {
        peso: peso,
        cubagem: cubagem,
        pallets: pallets,
        volume: volume
    }
}

function obterPesosEAjustarCapacidade() {
    var speso = _carregamento.Peso.val();
    speso = speso.replaceAll('.', '');
    speso = speso.replaceAll('.', '');
    var peso = Globalize.parseFloat(speso);

    var svolume = _carregamento.Unidade.val();
    svolume = svolume.replaceAll('.', '');
    svolume = svolume.replaceAll('.', '');
    var volume = Globalize.parseFloat(svolume);

    var spallet = _carregamento.Pallets.val();
    spallet = spallet.replaceAll('.', '');
    spallet = spallet.replaceAll('.', '');
    var pallets = Globalize.parseFloat(spallet);

    //Atualizando o peso do pedido no carregamento.
    if (PEDIDOS_SELECIONADOS().length == 1 && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador && _CONFIGURACAO_TMS.MontagemCarga.TipoControleSaldoPedido == EnumTipoControleSaldoPedido.Peso)
        PEDIDOS_SELECIONADOS()[0].PesoPedidoCarregamento = peso;

    //Atualizando o pallet do pedido no carregamento.
    if (PEDIDOS_SELECIONADOS().length == 1 && _CONFIGURACAO_TMS.TipoServicoMultisoftware != EnumTipoServicoMultisoftware.MultiEmbarcador && _CONFIGURACAO_TMS.MontagemCarga.TipoControleSaldoPedido == EnumTipoControleSaldoPedido.Pallet)
        PEDIDOS_SELECIONADOS()[0].PalletPedidoCarregamento = pallets;
    else
        pallets = Globalize.parseFloat(_carregamento.Pallets.val());

    var cubagem = Globalize.parseFloat(_carregamento.Cubagem.val()) - Globalize.parseFloat(_carregamento.CubagemPaletes.val());

    ajustarCapacidades(peso, cubagem, pallets, volume);
}

function ajustarCapacidades(peso, cubagem, pallets, volume) {

    if (_carregamento.Carregamento.codEntity() > 0 && PEDIDOS_SELECIONADOS().length == 1 && _CONFIGURACAO_TMS.MontagemCarga.TipoControleSaldoPedido == EnumTipoControleSaldoPedido.Peso) {
        peso = Globalize.parseFloat(_carregamento.PesoCarregamento.val());
    }

    if (_carregamento.Carregamento.codEntity() > 0 && PEDIDOS_SELECIONADOS().length == 1 && _CONFIGURACAO_TMS.MontagemCarga.TipoControleSaldoPedido == EnumTipoControleSaldoPedido.Pallet && Globalize.parseFloat(_carregamento.PalletCarregamento.val()) > 0)
        pallets = Globalize.parseFloat(_carregamento.PalletCarregamento.val());

    var cubagemPaletes = _carregamento.TipoDeCarga.Paletizado ? Globalize.parseFloat(_carregamento.ModeloVeicularCarga.OcupacaoCubicaPaletes) : 0;

    _carregamento.CubagemPaletes.val(Globalize.format(cubagemPaletes, "n2"));

    cubagem += cubagemPaletes;

    var capacidadePeso = Globalize.parseFloat(_carregamento.CapacidadePeso.val());
    var capacidadePallets = Globalize.parseFloat(_carregamento.CapacidadePallets.val());
    var capacidadeCubagem = Globalize.parseFloat(_carregamento.CapacidadeCubagem.val());

    var toleranciaPesoMenor = Globalize.parseFloat(_carregamento.ToleranciaPesoMenor.val());

    var cor = "";
    var corAprovado = "#9dde88ad";
    var corReprovado = "#FF6347";
    var corExcedida = "#FFFF00";

    cor = "";
    if (peso > 0) {
        if (peso >= toleranciaPesoMenor)
            cor = (peso > capacidadePeso) ? corExcedida : corAprovado
        else
            cor = corReprovado;
    }
    $("#" + _carregamento.Peso.id).css("background-color", cor);

    var lotacaoPeso = 0;
    if (capacidadePeso > 0)
        lotacaoPeso = (peso * 100) / capacidadePeso;
    _carregamento.LotacaoPeso.val(Globalize.format(lotacaoPeso, "n4"));

    cor = "";
    var lotacaoPallets = 0;
    if (capacidadePallets > 0) {
        var toleranciaMinimaPaletes = Globalize.parseFloat(_carregamento.ToleranciaMinimaPaletes.val());
        if (pallets >= toleranciaMinimaPaletes)
            cor = (pallets > capacidadePallets) ? corExcedida : corAprovado;
        else
            cor = corReprovado;

        lotacaoPallets = (pallets * 100) / capacidadePallets;
        _carregamento.LotacaoPallets.val(Globalize.format(lotacaoPallets, "n2"));
    }
    $("#" + _carregamento.Pallets.id).css("background-color", cor);


    cor = "";
    var lotacaoCubagem = 0;
    if (capacidadeCubagem > 0) {
        var toleranciaMinimaCubagem = Globalize.parseFloat(_carregamento.ToleranciaMinimaCubagem.val());
        if (cubagem >= toleranciaMinimaCubagem)
            cor = (cubagem > capacidadeCubagem) ? corExcedida : corAprovado;
        else
            cor = corReprovado;

        lotacaoCubagem = (cubagem * 100) / capacidadeCubagem;
        _carregamento.LotacaoCubagem.val(Globalize.format(lotacaoCubagem, "n2"));
    }
    $("#" + _carregamento.Cubagem.id).css("background-color", cor);

    cor = "";
    if (volume > 0) {
        if (volume >= toleranciaPesoMenor)
            cor = (volume > capacidadePeso) ? corExcedida : corAprovado
        else
            cor = corReprovado;
    }
    $("#" + _carregamento.Unidade.id).css("background-color", cor);

    var lotacaoVolume = 0;
    if (capacidadePeso > 0)
        lotacaoVolume = (volume * 100) / capacidadePeso;
    _carregamento.LotacaoUnidade.val(Globalize.format(lotacaoVolume, "n4"));

    _carregamento.Peso.val(Globalize.format(peso, "n4"));
    _carregamento.Cubagem.val(Globalize.format(cubagem, "n2"));
    _carregamento.Pallets.val(Globalize.format(pallets, "n2"));
    _carregamento.Unidade.val(Globalize.format(volume, "n2"));

    if (_carregamento.Carregamento.codEntity() == 0) {
        _carregamento.PesoCarregamento.val(_carregamento.Peso.val());
        _carregamento.PalletCarregamento.val(_carregamento.Pallets.val());
    } else if (PEDIDOS_SELECIONADOS().length == 1 && _CONFIGURACAO_TMS.MontagemCarga.TipoControleSaldoPedido == EnumTipoControleSaldoPedido.Pallet && Globalize.parseFloat(_carregamento.PalletCarregamento.val()) == 0 && Globalize.parseFloat(_carregamento.Pallets.val()) != 0) {
        _carregamento.PalletCarregamento.val(_carregamento.Pallets.val());
    }
}

function reiniciarCapacidadesCarregamento() {
    _carregamento.CapacidadePeso.val(_carregamento.CapacidadePeso.def);
    _carregamento.LotacaoPeso.val(_carregamento.LotacaoPeso.def);
    _carregamento.CapacidadePallets.val(_carregamento.CapacidadePallets.def);
    _carregamento.LotacaoPallets.val(_carregamento.LotacaoPallets.def);
    _carregamento.CapacidadeCubagem.val(_carregamento.CapacidadeCubagem.def);
    _carregamento.LotacaoCubagem.val(_carregamento.LotacaoCubagem.def);
    _carregamento.CapacidadeUnidade.val(_carregamento.CapacidadeUnidade.def);
    _carregamento.LotacaoUnidade.val(_carregamento.LotacaoUnidade.def);
    _carregamento.Peso.visible(true);
    _carregamento.Pallets.visible(true);
    _carregamento.Cubagem.visible(true);
    _carregamento.Unidade.visible(true);
}

function limparDadosCarregamento() {
    $("#" + _carregamento.Peso.id).css("background-color", "");
    $("#" + _carregamento.Pallets.id).css("background-color", "");
    $("#" + _carregamento.Cubagem.id).css("background-color", "");

    reiniciarCapacidadesCarregamento();
    LimparCampos(_carregamento);
    limparCarregamentoCarga();
    limparCarregamentoPedido();
    limparCarregamentoTransporte();
    LimparSimulacaoFrete();

    _carregamento.ModeloVeicularCarga.numeroReboques = 0;
    _carregamento.ModeloVeicularCarga.exigirDefinicaoReboquePedido = false;
    _carregamento.ModeloVeicularCarga.OcupacaoCubicaPaletes = "0";
    _carregamento.TipoDeCarga.Paletizado = false;
    _carregamento.TipoMontagemCarga.enable(true);
    _carregamento.ModeloVeicularCarga.enable(true);
    _carregamento.PedidoViagemNavio.enable(true);
    _carregamento.PreCarga.required(false);

    let visible = _CONFIGURACAO_TMS.LimparTelaAoSalvarMontagemCarga;

    _crudCarregamento.Atualizar.visible(true);
    _crudCarregamento.AutorizarVeiculo.visible(false);
    _crudCarregamento.Bloco.visible(visible);
    _crudCarregamento.Cancelar.visible(visible && (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador || _CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiCTe));
    _crudCarregamento.GerarCarga.visible(visible);
    _crudCarregamento.Imprimir.visible(false);
    _crudCarregamento.Roteirizacao.visible(visible);
    _crudCarregamento.SimularFrete.visible(visible);

    desmarcarKnoutsCarga();
    LimparPedidosSelecionados();
    RenderizarGridMotagemCargas();
    RenderizarGridMotagemPedidos();
    limparCamposCapacidadeJanelaCarregamento();
    limparCamposPeriodoCarregamento();
    buscarInformacoesTipoMontagem();
    LimparPontoMarcados();
    carregarDefinicaoReboquePedidosSelecionados();
    carregarDefinicaoTipoCarregamentoPedidosSelecionados();
    limparCarregamentoAutorizacao();
    LimparModalRoteirizacao();
    NOTAS_FISCAIS_SELECIONADAS([]);


    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiTMS)
        definirDataCarregamentoPorFilial(_CONFIGURACAO_TMS.InformaHorarioCarregamentoMontagemCarga ? Global.DataHoraAtual() : Global.DataAtual());

    if (_CONFIGURACAO_TMS.MontagemCarga.DataAtualNovoCarregamento)
        definirDataCarregamentoPorFilial(_CONFIGURACAO_TMS.InformaHorarioCarregamentoMontagemCarga ? Global.DataHoraAtual() : Global.DataAtual());

}

function limparDadosModeloVeicularCarga() {
    _carregamento.ModeloVeicularCarga.numeroReboques = 0;
    _carregamento.ModeloVeicularCarga.exigirDefinicaoReboquePedido = false;
    _carregamento.ModeloVeicularCarga.OcupacaoCubicaPaletes = "0";

    reiniciarCapacidadesCarregamento();
    obterPesosEAjustarCapacidade();
    VerificarVisibilidadeBuscaSugestaoPedido();
    carregarDefinicaoReboquePedidosSelecionados();
    carregarDefinicaoTipoCarregamentoPedidosSelecionados();
}

function limparDadosTipoDeCarga() {
    _carregamento.TipoDeCarga.Paletizado = false;

    obterPesosEAjustarCapacidade();
    buscarCapacidadeJanelaCarregamento(buscarPeriodoCarregamento);
}

function isCampoPreCargaObrigatorio() {
    var pedidos = PEDIDOS_SELECIONADOS();
    for (var i in pedidos) {
        var pedido = pedidos[i];
        if (pedido.ExigirPreCargaMontagemCarga)
            return true;
    }

    return false;
}

function obterCarregamentoSalvar() {
    var carregamento = {
        Carregamento: JSON.stringify(RetornarObjetoPesquisa(_carregamento)),
        Pedidos: JSON.stringify(ObterPedidosSelecionados()),
        Transporte: JSON.stringify(RetornarObjetoPesquisa(_carregamentoTransporte)),
        ListaMotoristas: _carregamentoTransporte.ListaMotoristas.val(),
        CarregamentoRedespacho: _carregamento.CarregamentoRedespacho.val(),
        NotasParaEnviar: JSON.stringify(obterNotasEnviar()),
        SessaoRoteirizador: 0
    }

    if (!preencherDadosCarregamentoPorFilialSalvar(carregamento))
        return undefined;

    return carregamento;
}

function setarRecebedorCarregamento(pedido) {
    if ((_carregamento.Recebedor.val() == "")) {
        _carregamento.Recebedor.codEntity(pedido.CodigoRecebedorColeta);
        _carregamento.Recebedor.entityDescription(pedido.DescricaoRecebedorColeta);
        _carregamento.Recebedor.val(pedido.DescricaoRecebedorColeta);
    }

}

function setarDataCarregamento(pedido) {

    if (!_gerarCargasDeColeta)
        return;

    if (!_carregamentoFilial.UtilizarFilialPadrao.val())
        return;

    var dadosCarregamentoPorFilial = obterDadosCarregamentoPorFilial();

    if (dadosCarregamentoPorFilial.DataCarregamento != "")
        return;

    definirDataCarregamentoPorFilial(_CONFIGURACAO_TMS.InformaHorarioCarregamentoMontagemCarga ? pedido.DataHoraCarregamentoPedido : pedido.DataCarregamentoPedido);
}

function setarModeloVeicularCarga(pedido) {
    if ((_gerarCargasDeColeta) && (_carregamento.ModeloVeicularCarga.val() == ""))
        preencherModeloVeicularCarga(pedido.ModeloVeicularCarga);
}

function setarTipoCondicaoCarregamento(pedido) {
    if (_CONFIGURACAO_TMS.InformarTipoCondicaoPagamentoMontagemCarga && (_gerarCargasDeColeta) && (_carregamento.TipoCondicaoPagamento.val() == EnumTipoCondicaoPagamento.Todos))
        _carregamento.TipoCondicaoPagamento.val(pedido.TipoCondicaoPagamento);
}

function gerarCargaValida() {
    if (_CONFIGURACAO_TMS.MotoristaObrigatorioMontagemCarga && _carregamentoTransporte.Motoristas.basicTable.BuscarRegistros().length <= 0 && !_CONFIGURACAO_TMS.DesativarMultiplosMotoristasMontagemCarga) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Cargas.MontagemCarga.MotoristaObrigatorio, Localization.Resources.Cargas.MontagemCarga.ObrigatorioInformarMotorista);
        return;
    }

    var codigo = _carregamento.Carregamento.codEntity();
    preencherListaMotorista();

    if (PEDIDOS_SELECIONADOS().some(p => p.TipoOperacaoValidarValorMinimoCarga) && !_VALOR_MINIMO_VALIDADO) {
        ValidarValorMinimoCarga(_carregamentoTransporte.TipoOperacao.codEntity(), ObterCodigoPedidosSelecionados());
        return;
    }

    if (codigo == 0) {
        atualizarCarregamentoClick(gerarCargaClick);
        return;
    }

    //#33331 
    // Verificando se o tipo de operação do pedido não obriga roteirizar.
    var tipoOperacaoPedidoNaoRequerRoteirizacao = PEDIDOS_SELECIONADOS().some(item => item.NaoExigeRoteirizacaoMontagemCarga === true);

    if (_CONFIGURACAO_TMS.TipoServicoMultisoftware == EnumTipoServicoMultisoftware.MultiEmbarcador && tipoOperacaoPedidoNaoRequerRoteirizacao === false) {
        var polilinhaRoteirizacao = _roteirizadorCarregamento.PolilinhaRota.val();

        if (polilinhaRoteirizacao == "") {
            roteirizarCarregamentoSemModal(gerarCargaClick, false);
            return;
        }
    }

    ocultar = ocultaGerarCarregamentosMontagemCarga();
    var mensagemConfirmacao = (ocultar ? Localization.Resources.Cargas.MontagemCarga.RealmenteDesejaAgendarEsteCarregamento : Localization.Resources.Cargas.MontagemCarga.RealmenteDesejaUmaCargaDesseCarregamento);

    exibirConfirmacao(Localization.Resources.Cargas.MontagemCarga.Confirmacao, mensagemConfirmacao, gerarCarga);
}

function removePedidosValorMercadoInferior() {
    PEDIDOS_SELECIONADOS.remove(function (pedido) {
        return pedido.ValorMercadoInferior === true;
    });
    gerarCargaValida();
}

function ValidarValorMinimoCarga(codigoTipoOperacao, codigosPedidos) {
    executarReST("MontagemCarga/ValidarValorMinimoPorCarga", { CodigoTipoOperacao: codigoTipoOperacao, CodigosPedidos: JSON.stringify(codigosPedidos) }, function (arg) {
        if (arg.Success) {
            if (arg.Data.CargasNaoAtingiramValorMinimo != null && arg.Data.CargasNaoAtingiramValorMinimo.length > 0) {
                const mensagem = MontarTabelaRetornoComCargaNaoAtingiramValorMinimo(arg.Data.CargasNaoAtingiramValorMinimo);
                exibirConfirmacaoComTamanhoMaior(Localization.Resources.Cargas.MontagemCarga.Confirmacao, mensagem, function () {
                    _VALOR_MINIMO_VALIDADO = true;
                    PEDIDOS_SELECIONADOS.remove(function (pedido) {
                        return arg.Data.PedidosParaRemoverCarregamento.includes(pedido.Codigo);
                    });

                    PedidosSelecionadosChange();

                    removerPedidosCarregamento(_carregamento.Carregamento.codEntity(), arg.Data.PedidosParaRemoverCarregamento, function () {
                        gerarCargaValida()
                    });
                });
            } else {
                _VALOR_MINIMO_VALIDADO = true;
                gerarCargaValida();
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}


function removerPedidosCarregamento(cod_carregamento, codigos_pedidos, callbackSucess) {
    executarReST("MontagemCarga/RemoverPedidosCarregamento", { Codigo: cod_carregamento, PedidosCodigo: JSON.stringify(codigos_pedidos) }, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                if (callbackSucess != undefined) {
                    callbackSucess();
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }
    });
}

function MontarTabelaRetornoComCargaNaoAtingiramValorMinimo(pedidosCargasNaoAtingiramValorMinimo) {
    let html = "<div>";
    html += "<h4>Serão removidas as cargas que não atingiram o valor mínimo previsto no Tipo de Operação. Deseja confirmar a remoção?</h4> <br>";
    html += "<table class='table' > <thead> <tr> <th scope='col'>Números pedidos</th> <th scope='col'>Filial</th> <th scope='col'>Valor da carga</th> <th scope='col'>Valor mínimo da carga</th> </tr> </thead>";
    html += "<tbody>";
    for (let i in pedidosCargasNaoAtingiramValorMinimo) {
        html += "<tr>";
        html += `<td>${pedidosCargasNaoAtingiramValorMinimo[i].NumerosPedido}</td>`;
        html += `<td>${pedidosCargasNaoAtingiramValorMinimo[i].Filial}</td>`;
        html += `<td>${pedidosCargasNaoAtingiramValorMinimo[i].ValorTotal}</td>`;
        html += `<td>${pedidosCargasNaoAtingiramValorMinimo[i].ValorMinimoCarga}</td>`;
        html += '</tr>';
    }

    html += "</tbody> </table > </div >";

    return html;
}
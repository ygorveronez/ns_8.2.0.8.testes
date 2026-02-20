/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../../Enumeradores/EnumSituacaoCarregamento.js" />
/// <reference path="../../Consultas/Tranportador.js" />
/// <reference path="consultarandamentopedido.js" />

var _acompanhamentoPedido;
var _corEtapaConcluida = "#85cc00";
var _corEtapaNaoConcluida = "#f4e912";
var _gridItensPendentes;
var _gridDetalhesRemessa;

var _iconeEtapa = [
    { text: "05", value: "../../../../img/AndamentoPedido/order-icon-3.png" },
    { text: "10", value: "../../../../img/AndamentoPedido/comercial-icon-1.png" },
    { text: "15", value: "../../../../img/AndamentoPedido/finance-icon-1.png" },
    { text: "25", value: "../../../../img/AndamentoPedido/order-icon-2.png" },
    { text: "30", value: "../../../../img/AndamentoPedido/calendar-icon-1.png" },
    { text: "35", value: "../../../../img/AndamentoPedido/box-icon-1.png" },
    { text: "40", value: "../../../../img/AndamentoPedido/boxes-icon-2.png" },
    { text: "45", value: "../../../../img/AndamentoPedido/truck-icon-1.png" }
];

var AndamentoPedido = function () {

    this.TamanhoEtapa = PropertyEntity({ type: types.local, def: "", val: ko.observable("") });

    this.NomeCliente = PropertyEntity({ text: "Cliente:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });
    this.NumOv = PropertyEntity({ text: "Status do Pedido nº:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });
    this.CodCentro = PropertyEntity({ text: "Unidade:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });
    this.TpCarregamento = PropertyEntity({ text: "Tipo:", getType: typesKnockout.string, val: ko.observable(""), visible: ko.observable(true) });

    this.FluxoEtapas = PropertyEntity({ def: new Array(), val: ko.observable(new Array()), visible: true });
    this.Remessas = ko.observableArray([]);
    this.ItensPendentes = PropertyEntity({ def: new Array(), val: ko.observable(new Array()), visible: true });

    this.GridItens = PropertyEntity({ type: types.local });
    this.GridsRemessas = PropertyEntity({ type: types.local });

    this.BotaoRemessa = PropertyEntity({
        eventClick: function (e) {
            if (e.BotaoRemessa.visibleFade()) {
                e.BotaoRemessa.visibleFade(false);
                e.BotaoRemessa.icon("fa fa-angle-down");
            } else {
                e.BotaoRemessa.visibleFade(true);
                e.BotaoRemessa.icon("fa fa-angle-up");
            }
        }, type: types.event, text: "Visualizar Remessas", idFade: guid(), icon: ko.observable("fa fa-angle-down"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.Limpar = PropertyEntity({ text: "Realizar nova busca", type: types.event, eventClick: limparClick });
};

var RemessaPedido = function (remessas) {
    this.DescricaoRemessa = PropertyEntity({ val: ko.observable(""), def: "" });
    this.FluxoRemessa = PropertyEntity({ def: new Array(), val: ko.observable(new Array()), visible: true });
    this.DetalhesRemessa = PropertyEntity({ def: new Array(), val: ko.observable(new Array()), visible: true });

    this.VisualizarDetalhes = PropertyEntity({
        eventClick: function (e) {
            if (e.VisualizarDetalhes.visibleFade()) {
                e.VisualizarDetalhes.visibleFade(false);
                e.VisualizarDetalhes.icon("fa fa-angle-down");
            } else {
                e.VisualizarDetalhes.visibleFade(true);
                e.VisualizarDetalhes.icon("fa fa-angle-up");
            }
        }, type: types.event, text: "Visualizar Detalhes", idFade: guid(), icon: ko.observable("fa fa-angle-down"), visibleFade: ko.observable(false), visible: ko.observable(true)
    });

    this.NumeroRemessa = PropertyEntity({ text: "Remessa:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.NumeroNotaFiscal = PropertyEntity({ text: "Nota Fiscal:", val: ko.observable(""), def: "", visible: ko.observable(true) });
    this.DataEmissao = PropertyEntity({ text: "Emissão", val: ko.observable(""), def: "", visible: ko.observable(true) });

    this.GridDetalhes = PropertyEntity({ type: types.local });

    PreencherObjetoKnout(this, { Data: remessas });
};

function loadAndamentoPedido() {
    _acompanhamentoPedido = new AndamentoPedido();
    KoBindings(_acompanhamentoPedido, "knockoutPedido");
}

function BuscarAndamentoPedido(numOv, codEmpresa, CodUserSap, tipoUsuario) {
    var p = new promise.Promise();
    var data = new Object();
    data.numOv = numOv;
    data.codEmpresa = codEmpresa;
    data.codUserSap = CodUserSap;
    data.tipoUsuario = tipoUsuario;

    executarReST("ConsultarAndamentoPedido/ObterDadosPedido", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {

                if (arg.Data != undefined) {
                    preencherKnoutAndamentoPedido(arg.Data);
                    $("#knockoutPedido").show();
                }
                window.scrollTo(0, 0);

            } else {
                $("#divProblemas").show();
                $("#divDados").hide();

                exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.Atencao, arg.Msg);
            }
        } else {

            $("#divProblemas").show();
            $("#divDados").hide();

            exibirMensagem(tipoMensagem.falha, Localization.Resources.Gerais.Geral.Falha, arg.Msg);
        }

        p.done();
    });

    return p;
}

function preencherKnoutAndamentoPedido(data) {
    if (data.NumeroPedido == null) {
        $("#divNenhumRegistroEncontrado").show();
        $("#divDados").hide();
    }

    _acompanhamentoPedido.NomeCliente.val(data.NomeCliente);
    _acompanhamentoPedido.NumOv.val(data.NumeroPedido);
    _acompanhamentoPedido.TpCarregamento.val(data.TipoCarregamento);
    _acompanhamentoPedido.CodCentro.val(data.CodigoCentro);
    _acompanhamentoPedido.Remessas.removeAll();

    var etapas = data.FluxoPedido;
    var itensPendentes = data.ItensPendentes;
    var remessas = data.Remessas;

    if (etapas != undefined && Array.isArray(etapas) && etapas.length > 0) {
        var tamanhoEtapaPedido = 95 / etapas.length;

        etapas.forEach(function (etapa, i) {
            etapa.tamanhoEtapa = `${tamanhoEtapaPedido.toFixed(2)}%`;
            var icone = _iconeEtapa.find(icone => icone.text == etapa.codEtapa);
            etapa.caminhoIcone = icone != undefined && icone != null ? icone.value : "";

            if (etapa.codStatus == "20") {
                etapa.codCorStatus = _corEtapaConcluida;
                etapa.checked = true;
            } else if (etapa.codStatus == "10") {
                etapa.codCorStatus = _corEtapaNaoConcluida;
                etapa.checked = false;
            } else {
                etapa.codCorStatus = _corEtapaNaoConcluida;
                etapa.checked = false;
            }

            etapa.lineVisible = true;
        });

        etapas[etapas.length - 1].lineVisible = false;

        _acompanhamentoPedido.FluxoEtapas.val(etapas);
    } else
        _acompanhamentoPedido.FluxoEtapas.val([]);

    if (itensPendentes != undefined && Array.isArray(itensPendentes) && itensPendentes.length > 0)
        _acompanhamentoPedido.ItensPendentes.val(itensPendentes);
    else
        _acompanhamentoPedido.ItensPendentes.val([]);

    if (remessas != undefined && Array.isArray(remessas) && remessas.length > 0) {
        remessas.forEach(function (remessa, i) {
            var tamanhoEtapaRemessa = 95 / remessa.FluxoRemessa.length;

            remessa.FluxoRemessa.forEach(function (fluxo, i) {
                fluxo.tamanhoEtapa = `${tamanhoEtapaRemessa.toFixed(2)}%`;
                var icone = _iconeEtapa.find(icone => icone.text == fluxo.codEtapa);
                fluxo.caminhoIcone = icone != undefined && icone != null ? icone.value : "";

                if (fluxo.codStatus == "20") {
                    fluxo.codCorStatus = _corEtapaConcluida;
                    fluxo.checked = true;
                }
                else if (fluxo.codStatus == "10") {
                    fluxo.codCorStatus = _corEtapaNaoConcluida;
                    fluxo.checked = false;
                } else {
                    fluxo.codCorStatus = _corEtapaNaoConcluida;
                    fluxo.checked = false;
                }

                fluxo.lineVisible = true;
            });

            remessa.FluxoRemessa[remessa.FluxoRemessa.length - 1].lineVisible = false;

            var knoutRemessaPedido = new RemessaPedido(remessa);
            knoutRemessaPedido.DescricaoRemessa.val(`Remessa ${i + 1}`);

            _acompanhamentoPedido.Remessas.push(knoutRemessaPedido);

            var header = [
                { data: "NumeroItem", visible: false },
                { data: "CodigoMaterial", title: "PRODUTO", width: "23%", className: "text-align-left" },
                { data: "DescricaoMaterial", title: "DESCRIÇÃO", width: "40%", className: "text-align-left" },
                { data: "QuantidadePendente", title: "QUANTIDADE", width: "22%", className: "text-align-right" },
                { data: "UnidadeMedida", title: "UNIDADE", width: "15%", className: "text-align-left" }
            ];

            _gridDetalhesRemessa = new BasicDataTable(knoutRemessaPedido.GridDetalhes.id, header, null);

            carregarGridDetalhesRemessa(remessa.DetalhesRemessa);

            $("#grid-" + knoutRemessaPedido.GridDetalhes.id).show();
        });
    }
    else
        _acompanhamentoPedido.Remessas([]);

    var header = [
        { data: "NumeroItem", visible: false },
        { data: "CodigoMaterial", title: "PRODUTO", width: "23%", className: "text-align-left" },
        { data: "DescricaoMaterial", title: "DESCRIÇÃO", width: "40%", className: "text-align-left" },
        { data: "QuantidadePendente", title: "QUANTIDADE", width: "22%", className: "text-align-right" },
        { data: "UnidadeMedida", title: "UNIDADE", width: "15%", className: "text-align-left" }
    ];

    _gridItensPendentes = new BasicDataTable(_acompanhamentoPedido.GridItens.id, header, null, { column: 1, dir: orderDir.asc });
    carregarGridItensPendentes();
}

function carregarGridItensPendentes() {
    var data = new Array();
    $.each(_acompanhamentoPedido.ItensPendentes.val(), function (i, item) {
        var itemPendente = new Object();

        itemPendente.NumeroItem = item.numItem;
        itemPendente.CodigoMaterial = item.codMaterial;
        itemPendente.DescricaoMaterial = item.descMaterial;
        itemPendente.QuantidadePendente = item.numQuantPend;
        itemPendente.UnidadeMedida = item.unidMedida

        data.push(itemPendente);
    });

    _gridItensPendentes.CarregarGrid(data);
}

function carregarGridDetalhesRemessa(dados) {
    var data = new Array();
    $.each(dados, function (i, detalhe) {
        var detalheRemessa = new Object();

        detalheRemessa.NumeroItem = detalhe.codItemRemessa;
        detalheRemessa.CodigoMaterial = detalhe.codMaterial;
        detalheRemessa.DescricaoMaterial = detalhe.descMaterial;
        detalheRemessa.QuantidadePendente = detalhe.numQuant;
        detalheRemessa.UnidadeMedida = detalhe.unidMedida;

        data.push(detalheRemessa);
    });

    _gridDetalhesRemessa.CarregarGrid(data);
}

function limparClick() {
    _acompanhamentoPedido.BotaoRemessa.visibleFade(false);
    LimparCampos(_acompanhamentoPedido);
    MostrarPesquisaNumeroPedido();
    $("#divDadosPedido").hide();
}
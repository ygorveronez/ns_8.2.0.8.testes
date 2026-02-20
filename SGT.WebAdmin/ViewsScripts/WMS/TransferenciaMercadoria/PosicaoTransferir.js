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
/// <reference path="../../Consultas/Filial.js" />
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Deposito.js" />
/// <reference path="TransferenciaMercadoria.js" />
/// <reference path="PosicaoAtual.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _HTMLDetalhePosicaoTransferir;
var _areaPosicaoTransferir;
var _detalhePosicaoTransferir;

var _knoutsPosicaoTransferir = new Array();

var AreaPosicaoTransferir = function () {
    this.Posicoes = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), visible: ko.observable(true) });

    this.DepositoPosicao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Posição:", required: false, idBtnSearch: guid(), enable: ko.observable(true) });
    this.DepositoBloco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Bloco:", required: false, idBtnSearch: guid(), enable: ko.observable(true) });
    this.DepositoRua = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rua:", required: false, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Deposito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Depósito:", required: false, idBtnSearch: guid(), enable: ko.observable(true) });
    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto do Embarcador:", required: false, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            pesquisarPosicaoTransferir();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.PosicaoTransferir = PropertyEntity();
    this.CarregandoPosicaoTransferir = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Total = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, eventChange: PosicaoTransferirPesquisaScroll });
}

var DetalhePosicaoTransferir = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Armazenamento = PropertyEntity({ text: "Local: ", val: ko.observable("") });

    this.PesoMaximo = PropertyEntity({ text: "Peso Máx.: ", val: ko.observable("") });
    this.PesoAtual = PropertyEntity({ text: "Peso: ", val: ko.observable("") });
    this.QuantidadePaletMaximo = PropertyEntity({ text: "Qtd. Palet Máx: ", val: ko.observable("") });
    this.QuantidadePaletAtual = PropertyEntity({ text: "Qtd. Palet: ", val: ko.observable("") });
    this.MetroCubicoMaximo = PropertyEntity({ text: "M³ Máx: ", val: ko.observable("") });
    this.MetroCubicoAtual = PropertyEntity({ text: "M³: ", val: ko.observable("") });
    
    this.InfoPosicaoTransferir = PropertyEntity({ cssClass: ko.observable("card no-padding padding-5") });
}

function loadAreaPosicaoTransferir(callback) {
    _areaPosicaoTransferir = new AreaPosicaoTransferir();
    KoBindings(_areaPosicaoTransferir, "knoutPosicaoTransferir");

    new BuscarProdutos(_areaPosicaoTransferir.ProdutoEmbarcador);
    new BuscarDeposito(_areaPosicaoTransferir.Deposito);
    new BuscarDepositoRua(_areaPosicaoTransferir.DepositoRua);
    new BuscarDepositoBloco(_areaPosicaoTransferir.DepositoBloco);
    new BuscarDepositoPosicao(_areaPosicaoTransferir.DepositoPosicao);

    $.get("Content/Static/WMS/DetalhePosicaoTransferir.html?dyn=" + guid(), function (data) {
        _HTMLDetalhePosicaoTransferir = data;
        if (callback != null)
            callback();
    });

}

function PosicaoTransferirPesquisaScroll(e, sender) {
    var elem = sender.target;
    if (_areaPosicaoTransferir.Inicio.val() < _areaPosicaoTransferir.Total.val() &&
        elem.scrollTop >= (elem.scrollHeight - elem.offsetHeight)) {
        buscarPosicaoTransferir();
    }
}

function buscarPosicaoTransferir() {
    var data = RetornarObjetoPesquisa(_areaPosicaoTransferir);
    data.Inicio = _areaPosicaoTransferir.Inicio.val();
    data.Limite = 52;
    _ControlarManualmenteProgresseTransferencia = true;
    _areaPosicaoTransferir.CarregandoPosicaoTransferir.val(true);

    executarReST("TransferenciaMercadoria/ObterPosicaoTransferir", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var retorno = arg.Data;
                _areaPosicaoTransferir.Total.val(retorno.Quantidade);
                _areaPosicaoTransferir.Inicio.val(_areaPosicaoTransferir.Inicio.val() + data.Limite);

                for (var i = 0; i < retorno.Registros.length; i++) {
                    var posicaoTransferir = retorno.Registros[i];
                    var knoutDetalhePosicaoTransferir = new DetalhePosicaoTransferir();
                    var knoutPosicaoTransferir = "PosicaoAtual_" + knoutDetalhePosicaoTransferir.InfoPosicaoTransferir.id + "_" + posicaoTransferir.Codigo;
                    var html = _HTMLDetalhePosicaoTransferir.replace(/#detalhePosicaoTransferir/g, knoutPosicaoTransferir);
                    $("#" + _areaPosicaoTransferir.PosicaoTransferir.id).append(html);
                    KoBindings(knoutDetalhePosicaoTransferir, knoutPosicaoTransferir);
                    var dataKnout = { Data: posicaoTransferir };
                    PreencherObjetoKnout(knoutDetalhePosicaoTransferir, dataKnout);
                    _knoutsPosicaoTransferir.push(knoutDetalhePosicaoTransferir);

                    index = obterIndicePosicaoTransferir(posicaoTransferir);
                    if (index >= 0)
                        knoutDetalhePosicaoTransferir.InfoPosicaoTransferir.cssClass("card");

                    $("#" + knoutPosicaoTransferir).droppable({
                        drop: droppableItemRetirar,
                        hoverClass: "ui-state-active backgroundDropHover",
                        activate: activateItemWMS,
                        deactivate: deactivateItemWMS
                    });
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Falha", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
        _ControlarManualmenteProgresseTransferencia = false;
        _areaPosicaoTransferir.CarregandoPosicaoTransferir.val(false);
    });
}

function obterIndicePosicaoTransferir(posicaoTransferir) {
    if (!NavegadorIEInferiorVersao12()) {
        return _areaPosicaoTransferir.Posicoes.val().findIndex(function (item) { return item.Codigo == posicaoTransferir.Codigo });
    } else {
        for (var i = 0; i < _areaPosicaoTransferir.Posicoes.val().length; i++) {
            if (posicaoTransferir.Codigo == _areaPosicaoTransferir.Posicoes.val()[i].Codigo)
                return i;
        }
        return -1;
    }
}

function pesquisarPosicaoTransferir(e) {
    $("#" + _areaPosicaoTransferir.PosicaoTransferir.id).html("");
    _areaPosicaoTransferir.Inicio.val(0);
    _knoutsPosicaoTransferir = new Array();
    buscarPosicaoTransferir()
}
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
/// <reference path="../../Consultas/Produto.js" />
/// <reference path="../../Consultas/Deposito.js" />
/// <reference path="../../Consultas/ProdutoEmbarcadorLote.js" />
/// <reference path="TransferenciaMercadoria.js" />
/// <reference path="PosicaoTransferir.js" />


//*******MAPEAMENTO KNOUCKOUT*******

var _HTMLDetalhePosicaoAtual;
var _areaPosicaoAtual;
var _detalhePosicaoAtual;

var _knoutsPosicaoAtual = new Array();

var AreaPosicaoAtual = function () {
    this.Posicoes = PropertyEntity({ getType: typesKnockout.dynamic, val: ko.observable(new Array()), def: new Array(), visible: ko.observable(true) });

    this.DepositoPosicao = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Posição:", required: false, idBtnSearch: guid(), enable: ko.observable(true) });
    this.DepositoBloco = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Bloco:", required: false, idBtnSearch: guid(), enable: ko.observable(true) });
    this.DepositoRua = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Rua:", required: false, idBtnSearch: guid(), enable: ko.observable(true) });
    this.Deposito = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Depósito:", required: false, idBtnSearch: guid(), enable: ko.observable(true) });
    this.ProdutoEmbarcador = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Produto do Embarcador:", required: false, idBtnSearch: guid(), enable: ko.observable(true) });
    this.ProdutoEmbarcadorLote = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Lote do Produto:", required: false, idBtnSearch: guid(), enable: ko.observable(true) });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            pesquisarPosicaoAtual();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.PosicaoAtual = PropertyEntity();
    this.CarregandoPosicaoAtual = PropertyEntity({ val: ko.observable(false), def: false, getType: typesKnockout.bool });
    this.Inicio = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Total = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int, eventChange: PosicaoAtualPesquisaScroll });
}

var DetalhePosicaoAtual = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoProdutoEmbarcador = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.CodigoArmazenamento = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "Descrição: ", val: ko.observable("") });
    this.CodigoBarras = PropertyEntity({ text: "Cód. Barras: ", val: ko.observable("") });
    this.NumeroLote = PropertyEntity({ text: "Número Lote: ", val: ko.observable("") });
    this.DataVencimento = PropertyEntity({ text: "Data Vencimento: ", val: ko.observable("") });
    this.QuantidadeAtual = PropertyEntity({ text: "Quantidade: ", val: ko.observable("") });
    this.Peso = PropertyEntity({ text: "Peso: ", val: ko.observable("") });
    this.QuantidadePalet = PropertyEntity({ text: "Qtd. Palet: ", val: ko.observable("") });
    this.Armazenamento = PropertyEntity({ text: "Local: ", val: ko.observable("") });
    this.Cubagem = PropertyEntity({ text: "M³: ", val: ko.observable("") });
    
    this.InfoPosicaoAtual = PropertyEntity({ cssClass: ko.observable("card no-padding padding-5") });
}

function loadAreaPosicaoAtual(callback) {
    _areaPosicaoAtual = new AreaPosicaoAtual();
    KoBindings(_areaPosicaoAtual, "knoutPosicaoAtual");

    new BuscarProdutos(_areaPosicaoAtual.ProdutoEmbarcador);
    new BuscarProdutoEmbarcadorLote(_areaPosicaoAtual.ProdutoEmbarcadorLote);
    new BuscarDeposito(_areaPosicaoAtual.Deposito);
    new BuscarDepositoRua(_areaPosicaoAtual.DepositoRua);
    new BuscarDepositoBloco(_areaPosicaoAtual.DepositoBloco);
    new BuscarDepositoPosicao(_areaPosicaoAtual.DepositoPosicao);

    $.get("Content/Static/WMS/DetalheLoteProdutoEmbarcador.html?dyn=" + guid(), function (data) {
        _HTMLDetalhePosicaoAtual = data;
        if (callback != null)
            callback();
    });
}

function PosicaoAtualPesquisaScroll(e, sender) {
    var elem = sender.target;
    if (_areaPosicaoAtual.Inicio.val() < _areaPosicaoAtual.Total.val() &&
        elem.scrollTop >= (elem.scrollHeight - elem.offsetHeight)) {
        buscarPosicaoAtual();
    }
}

function buscarPosicaoAtual() {
    var data = RetornarObjetoPesquisa(_areaPosicaoAtual);
    data.Inicio = _areaPosicaoAtual.Inicio.val();
    data.Limite = 52;
    _ControlarManualmenteProgresseTransferencia = true;
    _areaPosicaoAtual.CarregandoPosicaoAtual.val(true);

    executarReST("TransferenciaMercadoria/ObterPosicaoAtuals", data, function (arg) {
        if (arg.Success) {
            if (arg.Data !== false) {
                var retorno = arg.Data;
                _areaPosicaoAtual.Total.val(retorno.Quantidade);
                _areaPosicaoAtual.Inicio.val(_areaPosicaoAtual.Inicio.val() + data.Limite);

                for (var i = 0; i < retorno.Registros.length; i++) {
                    var posicaoAtual = retorno.Registros[i];
                    var knoutDetalhePosicaoAtual = new DetalhePosicaoAtual();
                    var knoutPosicaoAtual = "PosicaoAtual_" + knoutDetalhePosicaoAtual.InfoPosicaoAtual.id + "_" + posicaoAtual.Codigo;
                    var html = _HTMLDetalhePosicaoAtual.replace(/#detalhePosicaoAtual/g, knoutPosicaoAtual);
                    $("#" + _areaPosicaoAtual.PosicaoAtual.id).append(html);
                    KoBindings(knoutDetalhePosicaoAtual, knoutPosicaoAtual);
                    var dataKnout = { Data: posicaoAtual };
                    PreencherObjetoKnout(knoutDetalhePosicaoAtual, dataKnout);
                    _knoutsPosicaoAtual.push(knoutDetalhePosicaoAtual);

                    index = obterIndicePosicaoAtual(posicaoAtual);
                    if (index >= 0)
                        knoutDetalhePosicaoAtual.InfoPosicaoAtual.cssClass("card no-padding padding-5");

                    $("#" + knoutPosicaoAtual).draggable({
                        helper: "clone",
                        revert: true,
                        cursor: "move"
                    });
                }
            } else {
                exibirMensagem(tipoMensagem.atencao, "Falha", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
        _ControlarManualmenteProgresseTransferencia = false;
        _areaPosicaoAtual.CarregandoPosicaoAtual.val(false);
    });

}

function obterIndicePosicaoAtual(posicaoAtual) {
    if (!NavegadorIEInferiorVersao12()) {
        return _areaPosicaoAtual.Posicoes.val().findIndex(function (item) { return item.Codigo == posicaoAtual.Codigo });
    } else {
        for (var i = 0; i < _areaPosicaoAtual.Posicoes.val().length; i++) {
            if (posicaoAtual.Codigo == _areaPosicaoAtual.Posicoes.val()[i].Codigo)
                return i;
        }
        return -1;
    }
}

function pesquisarPosicaoAtual(e) {
    $("#" + _areaPosicaoAtual.PosicaoAtual.id).html("");
    _areaPosicaoAtual.Inicio.val(0);
    _knoutsPosicaoAtual = new Array();
    buscarPosicaoAtual();
}
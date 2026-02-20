///// <reference path="../../../js/libs/jquery-2.1.1.js" />
///// <reference path="../../../js/Global/CRUD.js" />
///// <reference path="../../../js/Global/knockout-3.1.0.js" />
///// <reference path="../../../js/Global/Rest.js" />
///// <reference path="../../../js/Global/Mensagem.js" />
///// <reference path="../../../js/Global/Grid.js" />
///// <reference path="../../../js/bootstrap/bootstrap.js" />
///// <reference path="../../../js/libs/jquery.blockui.js" />
///// <reference path="../../../js/Global/knoutViewsSlides.js" />
///// <reference path="../../../js/libs/jquery.maskMoney.js" />
///// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />
///// <reference path="../../Consultas/TipoOperacao.js" />
///// <reference path="OperadorFilial.js" />
///// <reference path="ConfigOperador.js" />
///// <reference path="OperadorTipoCarga.js" />
///// <reference path="TabelaFrete.js" />


////*******MAPEAMENTO KNOUCKOUT*******

//var _operadorTipoOperacao;
//var _gridTipoOperacao;


//var _operadorTipoOperacaoEmissao = [
//        { text: EnumTipoOperacaoEmissaoDescricao(EnumTipoOperacaoEmissao.vendaNormal), value: EnumTipoOperacaoEmissao.vendaNormal },
//        { text: EnumTipoOperacaoEmissaoDescricao(EnumTipoOperacaoEmissao.EntregaArmazem), value: EnumTipoOperacaoEmissao.EntregaArmazem },
//        { text: EnumTipoOperacaoEmissaoDescricao(EnumTipoOperacaoEmissao.VendaArmazemCliente), value: EnumTipoOperacaoEmissao.VendaArmazemCliente },
//        { text: EnumTipoOperacaoEmissaoDescricao(EnumTipoOperacaoEmissao.VendaComRedespacho) + " (Primeiro Trecho)", value: EnumTipoOperacaoEmissao.VendaComRedespacho }
//];

////*******EVENTOS*******

//var _operadorTipoOperacao;
//var _tipoOperacao;

//function OperadorTipoOperacao() {
//    this.TiposOperacaoEmissao = ko.observableArray();
//    this.VisualizaRedespacho = PropertyEntity({ getType: typesKnockout.bool, val: ko.observable(false), def: false, text: "Operador visualiza cargas de redespacho?" });
//}

//function TipoOperadorModel() {
//    this.TipoOperacaoEmissao = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
//    this.Descricao = PropertyEntity({ val: ko.observable(""), def: "", getType: typesKnockout.string });
//}

////*******EVENTOS*******

//function loadTipoOperacao() {
//    _tipoOperacao = new TipoOperadorModel();
//    _operadorTipoOperacao = new OperadorTipoOperacao();
//    $.each(_operadorTipoOperacaoEmissao, function (i, operacao) {
//        _operadorTipoOperacao.TiposOperacaoEmissao.push({ TipoOperacaoEmissao: operacao.value, Descricao: operacao.text });
//    });
//    KoBindings(_operadorTipoOperacao, "knockoutOperadorTipoOperacaoEmissao");

//}

//function selecionarTipoOperacao(tipoOperacao) {
//    if ($("#chkTipoOperacao_" + tipoOperacao.TipoOperacaoEmissao).prop("checked")) {
//        adicionarTipoOperacao(tipoOperacao);
//    } else {
//        removerTipoOperacao(tipoOperacao);
//    }
//    return true;
//}

//function adicionarTipoOperacao(tipoOperacao) {
//    var existe = false;

//    for (var i = 0; i < _operador.OperadorTiposOperacao.list.length; i++) {
//        if (_operador.OperadorTiposOperacao.list[i].TipoOperacaoEmissao.val == tipoOperacao.TipoOperacaoEmissao) {
//            existe = true;
//            break;
//        }
//    }

//    if (!existe) {
//        _tipoOperacao.TipoOperacaoEmissao.val(tipoOperacao.TipoOperacaoEmissao);
//        _operador.OperadorTiposOperacao.list.push(SalvarListEntity(_tipoOperacao));
//    }

//}

//function removerTipoOperacao(tipoOperacao) {
//    for (var i = 0; i < _operador.OperadorTiposOperacao.list.length; i++) {
//        if (_operador.OperadorTiposOperacao.list[i].TipoOperacaoEmissao.val == tipoOperacao.TipoOperacaoEmissao) {
//            _operador.OperadorTiposOperacao.list.splice(i, 1);
//            break;
//        }
//    }
//}

//function limparOperadorTiposDeOperacao() {
//    $("#divOperadorTiposOperacao input[type=checkbox]").each(function () {
//        $(this).prop("checked", false);
//    });
//}

//function recarregarOperadorTiposDeOperacao() {
//    limparOperadorTiposDeOperacao();
//    _operadorTipoOperacao.VisualizaRedespacho.val(_operador.VisualizaRedespacho.val());
//    for (var i = 0; i < _operador.OperadorTiposOperacao.list.length; i++) {
//        $("#chkTipoOperacao_" + _operador.OperadorTiposOperacao.list[i].TipoOperacaoEmissao.val).prop("checked", true);
//    }
//}
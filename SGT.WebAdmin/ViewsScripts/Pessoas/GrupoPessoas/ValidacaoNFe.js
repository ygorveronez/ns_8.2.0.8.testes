/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="GrupoPessoas.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

var _validacaoNFe;
var _ncmPallet;
var _gridNcmPallet;

//*******MAPEAMENTO KNOUCKOUT*******

var ValidacaoNFe = function () {
    this.ValidaPlacaNFe = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.PlacaNotaDeveSerMesmaCarga, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.ValidaEmitenteNFe = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.EmitenteNotaDeveSerMesmoPedido, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.LerNumeroPedidoDaObservacaoDaNota = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.LerNumeroPedidoObservacaoNota, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });

    this.ValidaDestinoNFe = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.DestinoNotaDeveSerMesmoPedido, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.ValidaOrigemNFe = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.OrigemNotaDeveSerMesmoPedido, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(false), enable: ko.observable(true) });
    this.ArmazenaProdutosXMLNFE = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ArmazenarProdutosContidosXMLNota, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });

    this.LerNumeroCargaObservacaoCTeSubcontratacao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.LerNumeroCargaSubcontratacao, val: ko.observable(false), def: false, getType: typesKnockout.bool, visible: ko.observable(true), enable: ko.observable(true) });
    this.RegexNumeroCargaObservacaoCTeSubcontratacao = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ExpressaoRegularExtrairNumeroCarga.getFieldDescription(), val: ko.observable(""), def: "", getType: typesKnockout.bool });

    this.ValidaPlacaNFe.val.subscribe(function (novoValor) {
        _grupoPessoas.ValidaPlacaNFe.val(novoValor);
    });

    this.ValidaEmitenteNFe.val.subscribe(function (novoValor) {
        _grupoPessoas.ValidaEmitenteNFe.val(novoValor);
    });

    this.ValidaDestinoNFe.val.subscribe(function (novoValor) {
        _grupoPessoas.ValidaDestinoNFe.val(novoValor);
    });

    this.ValidaOrigemNFe.val.subscribe(function (novoValor) {
        _grupoPessoas.ValidaOrigemNFe.val(novoValor);
    });

    this.ArmazenaProdutosXMLNFE.val.subscribe(function (novoValor) {
        _grupoPessoas.ArmazenaProdutosXMLNFE.val(novoValor);
    });

    this.LerNumeroPedidoDaObservacaoDaNota.val.subscribe(function (novoValor) {
        _grupoPessoas.LerNumeroPedidoDaObservacaoDaNota.val(novoValor);
    });
};

var NCMPallet = function () {
    this.Codigo = PropertyEntity({ getType: typesKnockout.string });
    this.NCM = PropertyEntity({ text: "*NCM: ", required: true, maxlength: 8 });

    this.ListaNCMPallet = PropertyEntity({ type: types.local, getType: typesKnockout.dynamic, def: new Array(), val: ko.observable(new Array()), idGrid: guid(), enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarNCMPallet, type: types.event, text: Localization.Resources.Pessoas.GrupoPessoas.Adicionar });

    //Campos fora da lista
    this.NaoAdicionarNotaNCMPalletCarga = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.NaoAdicionarCargaNotaNCM, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true), enable: ko.observable(true) });
    this.ZerarPesoNotaNCMPalletCarga = PropertyEntity({ text: Localization.Resources.Pessoas.GrupoPessoas.ZerarPesoNotaComEssesNCM, val: ko.observable(false), getType: typesKnockout.bool, def: false, visible: ko.observable(true), enable: ko.observable(true) });
    
    this.NaoAdicionarNotaNCMPalletCarga.val.subscribe(function (novoValor) {
        _grupoPessoas.NaoAdicionarNotaNCMPalletCarga.val(novoValor);
    });
    this.ZerarPesoNotaNCMPalletCarga.val.subscribe(function (novoValor) {
        _grupoPessoas.ZerarPesoNotaNCMPalletCarga.val(novoValor);
    });    
};

//*******EVENTOS*******

function loadValidacaoNFe() {

    _validacaoNFe = new ValidacaoNFe();
    KoBindings(_validacaoNFe, "knockoutValidacaoNFe");

    _ncmPallet = new NCMPallet();
    KoBindings(_ncmPallet, "knockoutNCMPallet");

    _grupoPessoas.LerNumeroCargaObservacaoCTeSubcontratacao = _validacaoNFe.LerNumeroCargaObservacaoCTeSubcontratacao;
    _grupoPessoas.RegexNumeroCargaObservacaoCTeSubcontratacao = _validacaoNFe.RegexNumeroCargaObservacaoCTeSubcontratacao;
    _grupoPessoas.NaoAdicionarNotaNCMPalletCarga = _ncmPallet.NaoAdicionarNotaNCMPalletCarga;
    _grupoPessoas.ZerarPesoNotaNCMPalletCarga = _ncmPallet.ZerarPesoNotaNCMPalletCarga;    

    $("#" + _ncmPallet.NCM.id).mask("00000000", { selectOnFocus: true, clearIfNotMatch: true });
    loadGridNCMPallet();
}

function adicionarNCMPallet() {
    if (!ValidarCamposObrigatorios(_ncmPallet)) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.CamposObrigatorios, Localization.Resources.Pessoas.GrupoPessoas.InformeCamposObrigatorios);
        return;
    }

    if (_ncmPallet.NCM.val().length != 8) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.NCMInvalido, Localization.Resources.Pessoas.GrupoPessoas.MesmoPrecisaConter8Caracteres);
        return;
    }

    var lista = obterListaNCMPallet();
    var valido = true;
    lista.forEach(function (ncm, i) {
        if (_ncmPallet.NCM.val() == ncm.NCM)
            valido = false;
    });

    if (!valido) {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Pessoas.GrupoPessoas.NCMExistente, Localization.Resources.Pessoas.GrupoPessoas.EsseNCMEstaLancado);
        return;
    }

    _ncmPallet.Codigo.val(guid());
    var ncmAdicionar = RetornarObjetoPesquisa(_ncmPallet);
    lista.push(ncmAdicionar);

    _ncmPallet.ListaNCMPallet.val(lista);

    LimparCampo(_ncmPallet.NCM);
    recarregarGridNCMPallet();
}

function excluirNCMPallet(registroSelecionado) {
    var lista = obterListaNCMPallet();

    lista.forEach(function (ncm, i) {
        if (registroSelecionado.Codigo == ncm.Codigo) {
            lista.splice(i, 1);
        }
    });

    _ncmPallet.ListaNCMPallet.val(lista);
    recarregarGridNCMPallet();
}

//*******MÉTODOS*******

function loadGridNCMPallet() {
    var excluir = { descricao: Localization.Resources.Pessoas.GrupoPessoas.Excluir, id: guid(), evento: "onclick", metodo: excluirNCMPallet, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(excluir);

    var header = [
        { data: "Codigo", visible: false },
        { data: "NCM", title: "NCM", width: "80%" }
    ];

    _gridNcmPallet = new BasicDataTable(_ncmPallet.ListaNCMPallet.idGrid, header, menuOpcoes, null, null, 5);
    _gridNcmPallet.CarregarGrid([]);
}

function recarregarGridNCMPallet() {
    _gridNcmPallet.CarregarGrid(obterListaNCMPallet());
}

function obterListaNCMPalletSalvar() {
    return JSON.stringify(obterListaNCMPallet());
}

function obterListaNCMPallet() {
    return _ncmPallet.ListaNCMPallet.val().slice();
}

function preencherCamposValidacaoNFe() {
    _ncmPallet.ListaNCMPallet.val(_grupoPessoas.NCMPalletsNFe.val());
    recarregarGridNCMPallet();
}

function limparCamposValidacaoNFe() {
    LimparCampos(_validacaoNFe);
    LimparCampos(_ncmPallet);
    _ncmPallet.ListaNCMPallet.val(new Array());
    recarregarGridNCMPallet();
}
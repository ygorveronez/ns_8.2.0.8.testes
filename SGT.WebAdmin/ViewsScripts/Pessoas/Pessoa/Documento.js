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
/// <reference path="Pessoa.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridListaDocumento;
var _listaDocumento;

var ListaDocumento = function () {
    this.Grid = PropertyEntity({ type: types.local });

    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.DataEmissao = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DataEmissao.getRequiredFieldDescription(), getType: typesKnockout.date, enable: ko.observable(true), required: true });
    this.DataVencimento = PropertyEntity({ text: Localization.Resources.Pessoas.Pessoa.DataVencimento.getRequiredFieldDescription(), getType: typesKnockout.date, enable: ko.observable(true), required: true });
    this.Descricao = PropertyEntity({ text: ko.observable(Localization.Resources.Gerais.Geral.Descricao.getRequiredFieldDescription()), required: true, maxlength: 1000, enable: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarListaDocumentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Adicionar, visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarListaDocumentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Atualizar, visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: limparCamposListaDocumento, type: types.event, text: Localization.Resources.Gerais.Geral.Cancelar, visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirListaDocumentoClick, type: types.event, text: Localization.Resources.Gerais.Geral.Excluir, visible: ko.observable(false) });
}


//*******EVENTOS*******

function loadListaDocumento() {

    _listaDocumento = new ListaDocumento();
    KoBindings(_listaDocumento, "tabDocumento");

    var menuOpcoes = { tipo: TypeOptionMenu.link, tamanho: 7, opcoes: [{ descricao: Localization.Resources.Gerais.Geral.Editar, id: guid(), metodo: editarListaDocumentoClick }] };

    var header = [{ data: "Codigo", visible: false },
        { data: "Descricao", title: Localization.Resources.Gerais.Geral.Descricao, width: "60%" },
        { data: "DataEmissao", title: Localization.Resources.Pessoas.Pessoa.DataEmissao, width: "12%" },
        { data: "DataVencimento", title: Localization.Resources.Pessoas.Pessoa.DataVencimento, width: "12%" }];

    _gridListaDocumento = new BasicDataTable(_listaDocumento.Grid.id, header, menuOpcoes, { column: 1, dir: orderDir.asc });

    recarregarGridListaDocumento();
}

function recarregarGridListaDocumento() {

    var data = new Array();

    $.each(_pessoa.ListaDocumento.list, function (i, listaDocumento) {
        var listaDocumentoGrid = new Object();

        listaDocumentoGrid.Codigo = listaDocumento.Codigo.val;
        listaDocumentoGrid.Descricao = listaDocumento.Descricao.val;
        listaDocumentoGrid.DataEmissao = listaDocumento.DataEmissao.val;
        listaDocumentoGrid.DataVencimento = listaDocumento.DataVencimento.val;

        data.push(listaDocumentoGrid);
    });

    _gridListaDocumento.CarregarGrid(data);
}


function excluirListaDocumentoClick(data) {
    var codigo = _listaDocumento.Codigo.val();

    $.each(_pessoa.ListaDocumento.list, function (i, listaDocumento) {
        if (codigo == listaDocumento.Codigo.val) {
            _pessoa.ListaDocumento.list.splice(i, 1);
            return false;
        }
    });

    limparCamposListaDocumento();
    recarregarGridListaDocumento();
}

function editarListaDocumentoClick(data) {
    var documento = null;

    $.each(_pessoa.ListaDocumento.list, function (i, listaDocumento) {
        if (data.Codigo == listaDocumento.Codigo.val) {
            documento = _pessoa.ListaDocumento.list[i];
            return;
        }
    });

    if (documento != null) {
        _listaDocumento.Codigo.val(documento.Codigo.val);
        _listaDocumento.Descricao.val(documento.Descricao.val);
        _listaDocumento.DataEmissao.val(documento.DataEmissao.val);
        _listaDocumento.DataVencimento.val(documento.DataVencimento.val);

        _listaDocumento.Adicionar.visible(false);
        _listaDocumento.Cancelar.visible(true);
        _listaDocumento.Excluir.visible(true);
        _listaDocumento.Atualizar.visible(true);
    }
}

function atualizarListaDocumentoClick(data) {
    if (ValidarCamposObrigatorios(_listaDocumento)) {
        var documento = SalvarListEntity(_listaDocumento);
        var codigo = _listaDocumento.Codigo.val();

        $.each(_pessoa.ListaDocumento.list, function (i, listaDocumento) {
            if (codigo == listaDocumento.Codigo.val) {
                _pessoa.ListaDocumento.list[i] = documento;
                return false;
            }
        });

        limparCamposListaDocumento();
        recarregarGridListaDocumento();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function adicionarListaDocumentoClick(e, sender) {
    if (ValidarCamposObrigatorios(_listaDocumento)) {
        _listaDocumento.Codigo.val(guid());

        var documento = SalvarListEntity(_listaDocumento);
        _pessoa.ListaDocumento.list.push(documento);

        recarregarGridListaDocumento();
        limparCamposListaDocumento();
    } else {
        exibirMensagem(tipoMensagem.atencao, Localization.Resources.Gerais.Geral.CamposObrigatorios, Localization.Resources.Gerais.Geral.InformeCamposObrigatorios);
    }
}

function limparCamposListaDocumento() {
    LimparCampos(_listaDocumento);


    _listaDocumento.Adicionar.visible(true);
    _listaDocumento.Cancelar.visible(false);
    _listaDocumento.Excluir.visible(false);
    _listaDocumento.Atualizar.visible(false);
}
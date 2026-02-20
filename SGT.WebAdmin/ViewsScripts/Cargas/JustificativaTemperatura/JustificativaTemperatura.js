/// <reference path="../../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../../js/Global/CRUD.js" />
/// <reference path="../../../js/Global/Rest.js" />
/// <reference path="../../../js/Global/Mensagem.js" />
/// <reference path="../../../js/Global/Grid.js" />
/// <reference path="../../../js/bootstrap/bootstrap.js" />
/// <reference path="../../../js/libs/jquery.blockui.js" />
/// <reference path="../../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../../js/plugin/datatables/jquery.dataTables.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridJustificativaTemperatura;
var _justificativaTemperatura;
var _pesquisaJustificativaTemperatura;

var PesquisaJustificativaTemperatura = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Status = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridJustificativaTemperatura.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade()) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
};

var JustificativaTemperatura = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 150 });
    this.Status = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
};

var CRUDJustificativaTemperatura = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadJustificativaTemperatura() {
    _justificativaTemperatura = new JustificativaTemperatura();
    KoBindings(_justificativaTemperatura, "knockoutCadastroJustificativaTemperatura");

    HeaderAuditoria("JustificativaTemperatura", _justificativaTemperatura);

    _crudJustificativaTemperatura = new CRUDJustificativaTemperatura();
    KoBindings(_crudJustificativaTemperatura, "knockoutCRUDJustificativaTemperatura");

    _pesquisaJustificativaTemperatura = new PesquisaJustificativaTemperatura();
    KoBindings(_pesquisaJustificativaTemperatura, "knockoutPesquisaJustificativaTemperatura", false, _pesquisaJustificativaTemperatura.Pesquisar.id);

    buscarJustificativaTemperatura();
}

function adicionarClick(e, sender) {
    Salvar(_justificativaTemperatura, "JustificativaTemperatura/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridJustificativaTemperatura.CarregarGrid();
                limparCamposJustificativaTemperatura();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_justificativaTemperatura, "JustificativaTemperatura/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridJustificativaTemperatura.CarregarGrid();
                limparCamposJustificativaTemperatura();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Justificativa " + _justificativaTemperatura.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_justificativaTemperatura, "JustificativaTemperatura/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                _gridJustificativaTemperatura.CarregarGrid();
                limparCamposJustificativaTemperatura();
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposJustificativaTemperatura();
}

//*******MÉTODOS*******

function buscarJustificativaTemperatura() {
    var opcaoEditar = { descricao: "Editar", id: guid(), evento: "onclick", metodo: editarJustificativaTemperatura, tamanho: "10", icone: "" };
    var menuOpcoes = { tipo: TypeOptionMenu.link, opcoes: [opcaoEditar] };
    var configuracoesExportacao = { url: "JustificativaTemperatura/ExportarPesquisa", titulo: "Justificativas de Temperaturas" };

    _gridJustificativaTemperatura = new GridViewExportacao(_pesquisaJustificativaTemperatura.Pesquisar.idGrid, "JustificativaTemperatura/Pesquisa", _pesquisaJustificativaTemperatura, menuOpcoes, configuracoesExportacao);
    _gridJustificativaTemperatura.CarregarGrid();
}

function editarJustificativaTemperatura(justificativaTemperaturaGrid) {
    limparCamposJustificativaTemperatura();
    _justificativaTemperatura.Codigo.val(justificativaTemperaturaGrid.Codigo);
    BuscarPorCodigo(_justificativaTemperatura, "JustificativaTemperatura/BuscarPorCodigo", function (arg) {
        _pesquisaJustificativaTemperatura.ExibirFiltros.visibleFade(false);
        _crudJustificativaTemperatura.Atualizar.visible(true);
        _crudJustificativaTemperatura.Cancelar.visible(true);
        _crudJustificativaTemperatura.Excluir.visible(true);
        _crudJustificativaTemperatura.Adicionar.visible(false);
    }, null);
}

function limparCamposJustificativaTemperatura() {
    _crudJustificativaTemperatura.Atualizar.visible(false);
    _crudJustificativaTemperatura.Cancelar.visible(false);
    _crudJustificativaTemperatura.Excluir.visible(false);
    _crudJustificativaTemperatura.Adicionar.visible(true);
    LimparCampos(_justificativaTemperatura);
}
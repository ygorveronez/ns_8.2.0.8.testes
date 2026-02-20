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

var _gridJustificativaOcorrencia;
var _justificativaOcorrencia;
var _pesquisaJustificativaOcorrencia;

var PesquisaJustificativaOcorrencia = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridJustificativaOcorrencia.CarregarGrid();
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

var JustificativaOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
};

var CRUDJustificativaOcorrencia = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadJustificativaOcorrencia() {
    _justificativaOcorrencia = new JustificativaOcorrencia();
    KoBindings(_justificativaOcorrencia, "knockoutCadastroJustificativaOcorrencia");

    HeaderAuditoria("JustificativaOcorrencia", _justificativaOcorrencia);

    _crudJustificativaOcorrencia = new CRUDJustificativaOcorrencia();
    KoBindings(_crudJustificativaOcorrencia, "knockoutCRUDJustificativaOcorrencia");

    _pesquisaJustificativaOcorrencia = new PesquisaJustificativaOcorrencia();
    KoBindings(_pesquisaJustificativaOcorrencia, "JustificativaOcorrencia", false, _pesquisaJustificativaOcorrencia.Pesquisar.id);

    buscarJustificativaOcorrencia();
}

function adicionarClick(e, sender) {
    Salvar(_justificativaOcorrencia, "JustificativaOcorrencia/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridJustificativaOcorrencia.CarregarGrid();
                limparCamposJustificativaOcorrencia();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_justificativaOcorrencia, "JustificativaOcorrencia/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridJustificativaOcorrencia.CarregarGrid();
                limparCamposJustificativaOcorrencia();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Justificativa de Ocorrência? " + _justificativaOcorrencia.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_justificativaOcorrencia, "JustificativaOcorrencia/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridJustificativaOcorrencia.CarregarGrid();
                    limparCamposJustificativaOcorrencia();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }
        }, null);
    });
}

function cancelarClick(e) {
    limparCamposJustificativaOcorrencia();
}

//*******MÉTODOS*******

function buscarJustificativaOcorrencia() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarJustificativaOcorrencia, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridJustificativaOcorrencia = new GridView(_pesquisaJustificativaOcorrencia.Pesquisar.idGrid, "JustificativaOcorrencia/Pesquisa", _pesquisaJustificativaOcorrencia, menuOpcoes);
    _gridJustificativaOcorrencia.CarregarGrid();
}

function editarJustificativaOcorrencia(justificativaOcorrenciaGrid) {
    limparCamposJustificativaOcorrencia();
    _justificativaOcorrencia.Codigo.val(justificativaOcorrenciaGrid.Codigo);
    BuscarPorCodigo(_justificativaOcorrencia, "JustificativaOcorrencia/BuscarPorCodigo", function (arg) {
        _pesquisaJustificativaOcorrencia.ExibirFiltros.visibleFade(false);
        _crudJustificativaOcorrencia.Atualizar.visible(true);
        _crudJustificativaOcorrencia.Cancelar.visible(true);
        _crudJustificativaOcorrencia.Excluir.visible(true);
        _crudJustificativaOcorrencia.Adicionar.visible(false);
    }, null);
}

function limparCamposJustificativaOcorrencia() {
    _crudJustificativaOcorrencia.Atualizar.visible(false);
    _crudJustificativaOcorrencia.Cancelar.visible(false);
    _crudJustificativaOcorrencia.Excluir.visible(false);
    _crudJustificativaOcorrencia.Adicionar.visible(true);
    LimparCampos(_justificativaOcorrencia);
}
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

var _gridTiposCausadoresOcorrencia;
var _tiposCausadoresOcorrencia;
var _pesquisaTiposCausadoresOcorrencia;

var PesquisaTiposCausadoresOcorrencia = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridTiposCausadoresOcorrencia.CarregarGrid();
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

var TiposCausadoresOcorrencia = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código Integração: ", required: ko.observable(false), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
};

var CRUDTiposCausadoresOcorrencia = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadTiposCausadoresOcorrencia() {
    _tiposCausadoresOcorrencia = new TiposCausadoresOcorrencia();
    KoBindings(_tiposCausadoresOcorrencia, "knockoutCadastroTiposCausadoresOcorrencia");

    HeaderAuditoria("TiposCausadoresOcorrencia", _tiposCausadoresOcorrencia);

    _crudTiposCausadoresOcorrencia = new CRUDTiposCausadoresOcorrencia();
    KoBindings(_crudTiposCausadoresOcorrencia, "knockoutCRUDTiposCausadoresOcorrencia");

    _pesquisaTiposCausadoresOcorrencia = new PesquisaTiposCausadoresOcorrencia();
    KoBindings(_pesquisaTiposCausadoresOcorrencia, "TiposCausadoresOcorrencia", false, _pesquisaTiposCausadoresOcorrencia.Pesquisar.id);

    buscarTiposCausadoresOcorrencia();
}

function adicionarClick(e, sender) {
    Salvar(_tiposCausadoresOcorrencia, "TiposCausadoresOcorrencia/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridTiposCausadoresOcorrencia.CarregarGrid();
                limparCamposTiposCausadoresOcorrencia();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_tiposCausadoresOcorrencia, "TiposCausadoresOcorrencia/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridTiposCausadoresOcorrencia.CarregarGrid();
                limparCamposTiposCausadoresOcorrencia();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o tipo de causador da ocorrência? " + _tiposCausadoresOcorrencia.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_tiposCausadoresOcorrencia, "TiposCausadoresOcorrencia/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridTiposCausadoresOcorrencia.CarregarGrid();
                    limparCamposTiposCausadoresOcorrencia();
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
    limparCamposTiposCausadoresOcorrencia();
}

//*******MÉTODOS*******

function buscarTiposCausadoresOcorrencia() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarTiposCausadoresOcorrencia, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridTiposCausadoresOcorrencia = new GridView(_pesquisaTiposCausadoresOcorrencia.Pesquisar.idGrid, "TiposCausadoresOcorrencia/Pesquisa", _pesquisaTiposCausadoresOcorrencia, menuOpcoes);
    _gridTiposCausadoresOcorrencia.CarregarGrid();
}

function editarTiposCausadoresOcorrencia(tiposCausadoresOcorrenciaGrid) {
    limparCamposTiposCausadoresOcorrencia();
    _tiposCausadoresOcorrencia.Codigo.val(tiposCausadoresOcorrenciaGrid.Codigo);
    BuscarPorCodigo(_tiposCausadoresOcorrencia, "TiposCausadoresOcorrencia/BuscarPorCodigo", function (arg) {
        _pesquisaTiposCausadoresOcorrencia.ExibirFiltros.visibleFade(false);
        _crudTiposCausadoresOcorrencia.Atualizar.visible(true);
        _crudTiposCausadoresOcorrencia.Cancelar.visible(true);
        _crudTiposCausadoresOcorrencia.Excluir.visible(true);
        _crudTiposCausadoresOcorrencia.Adicionar.visible(false);
    }, null);
}

function limparCamposTiposCausadoresOcorrencia() {
    _crudTiposCausadoresOcorrencia.Atualizar.visible(false);
    _crudTiposCausadoresOcorrencia.Cancelar.visible(false);
    _crudTiposCausadoresOcorrencia.Excluir.visible(false);
    _crudTiposCausadoresOcorrencia.Adicionar.visible(true);
    LimparCampos(_tiposCausadoresOcorrencia);
}
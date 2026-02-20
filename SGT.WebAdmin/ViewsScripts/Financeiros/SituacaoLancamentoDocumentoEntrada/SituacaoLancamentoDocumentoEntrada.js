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

var _gridSituacaoLancamentoDocumentoEntrada;
var _SituacaoLancamentoDocumentoEntrada;
var _pesquisaSituacaoLancamentoDocumentoEntrada;

var PesquisaSituacaoLancamentoDocumentoEntrada = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSituacaoLancamentoDocumentoEntrada.CarregarGrid();
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

var SituacaoLancamentoDocumentoEntrada = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 500 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
};

var CRUDSituacaoLancamentoDocumentoEntrada = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadSituacaoLancamentoDocumentoEntrada() {
    _SituacaoLancamentoDocumentoEntrada = new SituacaoLancamentoDocumentoEntrada();

    KoBindings(_SituacaoLancamentoDocumentoEntrada, "knockoutCadastroSituacaoLancamentoDocumentoEntrada");
    HeaderAuditoria("SituacaoLancamentoDocumentoEntrada", _SituacaoLancamentoDocumentoEntrada);

    _crudSituacaoLancamentoDocumentoEntrada = new CRUDSituacaoLancamentoDocumentoEntrada();
    KoBindings(_crudSituacaoLancamentoDocumentoEntrada, "knockoutCRUDSituacaoLancamentoDocumentoEntrada");

    _pesquisaSituacaoLancamentoDocumentoEntrada = new PesquisaSituacaoLancamentoDocumentoEntrada();
    KoBindings(_pesquisaSituacaoLancamentoDocumentoEntrada, "knockoutPesquisaSituacaoLancamentoDocumentoEntrada", false, _pesquisaSituacaoLancamentoDocumentoEntrada.Pesquisar.id);

    buscarSituacaoLancamentoDocumentoEntrada();
}

function adicionarClick(e, sender) {
    Salvar(_SituacaoLancamentoDocumentoEntrada, "SituacaoLancamentoDocumentoEntrada/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridSituacaoLancamentoDocumentoEntrada.CarregarGrid();
                limparCamposSituacaoLancamentoDocumentoEntrada();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_SituacaoLancamentoDocumentoEntrada, "SituacaoLancamentoDocumentoEntrada/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridSituacaoLancamentoDocumentoEntrada.CarregarGrid();
                limparCamposSituacaoLancamentoDocumentoEntrada();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a Marca EPI " + _SituacaoLancamentoDocumentoEntrada.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_SituacaoLancamentoDocumentoEntrada, "SituacaoLancamentoDocumentoEntrada/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridSituacaoLancamentoDocumentoEntrada.CarregarGrid();
                    limparCamposSituacaoLancamentoDocumentoEntrada();
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
    limparCamposSituacaoLancamentoDocumentoEntrada();
}

//*******MÉTODOS*******

function buscarSituacaoLancamentoDocumentoEntrada() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarSituacaoLancamentoDocumentoEntrada, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridSituacaoLancamentoDocumentoEntrada = new GridView(_pesquisaSituacaoLancamentoDocumentoEntrada.Pesquisar.idGrid, "SituacaoLancamentoDocumentoEntrada/Pesquisa", _pesquisaSituacaoLancamentoDocumentoEntrada, menuOpcoes);
    _gridSituacaoLancamentoDocumentoEntrada.CarregarGrid();
}

function editarSituacaoLancamentoDocumentoEntrada(SituacaoLancamentoDocumentoEntradaGrid) {
    limparCamposSituacaoLancamentoDocumentoEntrada();
    _SituacaoLancamentoDocumentoEntrada.Codigo.val(SituacaoLancamentoDocumentoEntradaGrid.Codigo);
    BuscarPorCodigo(_SituacaoLancamentoDocumentoEntrada, "SituacaoLancamentoDocumentoEntrada/BuscarPorCodigo", function (arg) {
        _pesquisaSituacaoLancamentoDocumentoEntrada.ExibirFiltros.visibleFade(false);
        _crudSituacaoLancamentoDocumentoEntrada.Atualizar.visible(true);
        _crudSituacaoLancamentoDocumentoEntrada.Cancelar.visible(true);
        _crudSituacaoLancamentoDocumentoEntrada.Excluir.visible(true);
        _crudSituacaoLancamentoDocumentoEntrada.Adicionar.visible(false);
    }, null);
}

function limparCamposSituacaoLancamentoDocumentoEntrada() {
    _crudSituacaoLancamentoDocumentoEntrada.Atualizar.visible(false);
    _crudSituacaoLancamentoDocumentoEntrada.Cancelar.visible(false);
    _crudSituacaoLancamentoDocumentoEntrada.Excluir.visible(false);
    _crudSituacaoLancamentoDocumentoEntrada.Adicionar.visible(true);
    LimparCampos(_SituacaoLancamentoDocumentoEntrada);
}
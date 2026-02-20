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
/// <reference path="../../Configuracao/ConfiguracaoTMS.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridSeguradora;
var _seguradora;
var _pesquisaSeguradora;

var PesquisaSeguradora = function () {
    this.Nome = PropertyEntity({ text: "Nome: " });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _statusPesquisa, def: true, text: "Situação: " });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridSeguradora.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var Seguradora = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.ClienteSeguradora = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "*Seguradora:",issue: 262, idBtnSearch: guid(), required: true, visible: ko.observable(true) });
    this.Nome = PropertyEntity({ text: "*Nome:", issue: 586, required: true, maxlength: 150 });
    this.Observacao = PropertyEntity({ text: "Observação:", issue: 593, maxlength: 450 });
    this.Ativo = PropertyEntity({ val: ko.observable(true), options: _status, def: true, text: "*Situação: ", issue: 593 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******

function loadSeguradora() {

    _seguradora = new Seguradora();
    KoBindings(_seguradora, "knockoutCadastroSeguradora");

    HeaderAuditoria("Seguradora", _seguradora);

    _pesquisaSeguradora = new PesquisaSeguradora();
    KoBindings(_pesquisaSeguradora, "knockoutPesquisaSeguradora", _pesquisaSeguradora.Pesquisar.id);
    new BuscarClientes(_seguradora.ClienteSeguradora, retornoSeguradora, true);
    buscarTiposOperacao();
}

function retornoSeguradora(data) {
    _seguradora.ClienteSeguradora.codEntity(data.Codigo);
    _seguradora.ClienteSeguradora.val("(" + data.CPF_CNPJ + ") " + data.Nome);
    if (_seguradora.Nome.val() == "") {
        _seguradora.Nome.val(data.Nome);
    }
}

function adicionarClick(e, sender) {
    Salvar(e, "Seguradora/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridSeguradora.CarregarGrid();
                limparCamposSeguradora();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(e, "Seguradora/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridSeguradora.CarregarGrid();
                limparCamposSeguradora();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir a seguradora " + _seguradora.Nome.val() + "?", function () {
        ExcluirPorCodigo(_seguradora, "Seguradora/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso.");
                    _gridSeguradora.CarregarGrid();
                    limparCamposSeguradora();
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
    limparCamposSeguradora();
}

//*******MÉTODOS*******

function editarSeguradora(seguradora) {
    limparCamposSeguradora();
    _seguradora.Codigo.val(seguradora.Codigo);
    BuscarPorCodigo(_seguradora, "Seguradora/BuscarPorCodigo", function (arg) {
        _pesquisaSeguradora.ExibirFiltros.visibleFade(false);
        _seguradora.Atualizar.visible(true);
        _seguradora.Cancelar.visible(true);
        _seguradora.Excluir.visible(true);
        _seguradora.Adicionar.visible(false);
    }, null);
}

function buscarTiposOperacao() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarSeguradora, tamanho: "20", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridSeguradora = new GridView(_pesquisaSeguradora.Pesquisar.idGrid, "Seguradora/Pesquisa", _pesquisaSeguradora, menuOpcoes, null);
    _gridSeguradora.CarregarGrid();
}

function limparCamposSeguradora() {
    _seguradora.Atualizar.visible(false);
    _seguradora.Cancelar.visible(false);
    _seguradora.Excluir.visible(false);
    _seguradora.Adicionar.visible(true);
    LimparCampos(_seguradora);
}


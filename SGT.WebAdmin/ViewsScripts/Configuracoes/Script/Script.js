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

var _script;
var _pesquisaScript;
var _gridScript;

var PesquisaScript = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", getType: typesKnockout.string, val: ko.observable("") });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridScript.CarregarGrid();
        }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true)
    });
    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            if (e.ExibirFiltros.visibleFade() == true) {
                e.ExibirFiltros.visibleFade(false);
            } else {
                e.ExibirFiltros.visibleFade(true);
            }
        }, type: types.event, text: Localization.Resources.Gerais.Geral.ExibirFiltros, idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
}

var Script = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "*Observação: ", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.ScriptSQL = PropertyEntity({ text: "*Script: ", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 20000 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

function loadScript() {
    _pesquisaScript = new PesquisaScript();
    KoBindings(_pesquisaScript, "knockoutPesquisaScript", false, _pesquisaScript.Pesquisar.id);

    _script = new Script();
    KoBindings(_script, "knockoutScript");

    HeaderAuditoria("Script", _script);

    BuscarScript();
}

function adicionarClick(e, sender) {
    Salvar(_script, "Script/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridScript.CarregarGrid();
                LimparCamposScript();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_script, "Script/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridScript.CarregarGrid();
                LimparCamposScript();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_script, "Script/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridScript.CarregarGrid();
                    LimparCamposScript();
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
    LimparCamposScript();
}

function editarScriptClick(itemGrid) {
    LimparCamposScript();

    _script.Codigo.val(itemGrid.Codigo);
    BuscarPorCodigo(_script, "Script/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaScript.ExibirFiltros.visibleFade(false);

                _script.Atualizar.visible(true);
                _script.Excluir.visible(true);
                _script.Cancelar.visible(true);
                _script.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function BuscarScript() {
    let editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarScriptClick, tamanho: "20", icone: "" };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridScript = new GridView(_pesquisaScript.Pesquisar.idGrid, "Script/Pesquisa", _pesquisaScript, menuOpcoes, null);
    _gridScript.CarregarGrid();
}

function LimparCamposScript() {
    _script.Atualizar.visible(false);
    _script.Cancelar.visible(false);
    _script.Excluir.visible(false);
    _script.Adicionar.visible(true);

    LimparCampos(_script);
}
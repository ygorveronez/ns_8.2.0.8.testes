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

var _justificativaAutorizacaoCarga;
var _pesquisJustificativaAutorizacaoCarga;
var _gridJustificativaAutorizacaoCarga;

var PesquisaJustificativaAutorizacaoCarga = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Situação", val: ko.observable(""), options: Global.ObterOpcoesPesquisaBooleano("Ativo", "Inativo"), def: "" });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridJustificativaAutorizacaoCarga.CarregarGrid();
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

var JustificativaAutorizacaoCarga = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:", required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 1000 });
    this.Situacao = PropertyEntity({ text: "Situação", val: ko.observable(""), options: Global.ObterOpcoesBooleano("Ativo", "Inativo"), def: true });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.CodigoIntegracao = PropertyEntity({ text: "Código de Integração:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 50 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

function loadJustificativaAutorizacaoCarga() {
    _pesquisJustificativaAutorizacaoCarga = new PesquisaJustificativaAutorizacaoCarga();
    KoBindings(_pesquisJustificativaAutorizacaoCarga, "knockoutPesquisaJustificativaAutorizacaoCarga", false, _pesquisJustificativaAutorizacaoCarga.Pesquisar.id);

    _justificativaAutorizacaoCarga = new JustificativaAutorizacaoCarga();
    KoBindings(_justificativaAutorizacaoCarga, "knockoutJustificativaAutorizacaoCarga");

    BuscarJustificativaAutorizacaoCarga();
}

function adicionarClick(e, sender) {
    Salvar(_justificativaAutorizacaoCarga, "JustificativaAutorizacaoCarga/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridJustificativaAutorizacaoCarga.CarregarGrid();
                LimparCamposJustificativaAutorizacaoCarga();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_justificativaAutorizacaoCarga, "JustificativaAutorizacaoCarga/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridJustificativaAutorizacaoCarga.CarregarGrid();
                LimparCamposJustificativaAutorizacaoCarga();
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
        ExcluirPorCodigo(_justificativaAutorizacaoCarga, "JustificativaAutorizacaoCarga/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridJustificativaAutorizacaoCarga.CarregarGrid();
                    LimparCamposJustificativaAutorizacaoCarga();
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
    LimparCamposJustificativaAutorizacaoCarga();
}

function editarScriptClick(itemGrid) {
    LimparCamposJustificativaAutorizacaoCarga();

    _justificativaAutorizacaoCarga.Codigo.val(itemGrid.Codigo);
    BuscarPorCodigo(_justificativaAutorizacaoCarga, "JustificativaAutorizacaoCarga/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisJustificativaAutorizacaoCarga.ExibirFiltros.visibleFade(false);

                _justificativaAutorizacaoCarga.Atualizar.visible(true);
                _justificativaAutorizacaoCarga.Excluir.visible(true);
                _justificativaAutorizacaoCarga.Cancelar.visible(true);
                _justificativaAutorizacaoCarga.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function BuscarJustificativaAutorizacaoCarga() {
    let editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarScriptClick, tamanho: "20", icone: "" };

    let menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridJustificativaAutorizacaoCarga = new GridView(_pesquisJustificativaAutorizacaoCarga.Pesquisar.idGrid, "JustificativaAutorizacaoCarga/Pesquisa", _pesquisJustificativaAutorizacaoCarga, menuOpcoes, null);
    _gridJustificativaAutorizacaoCarga.CarregarGrid();
}

function LimparCamposJustificativaAutorizacaoCarga() {
    _justificativaAutorizacaoCarga.Atualizar.visible(false);
    _justificativaAutorizacaoCarga.Cancelar.visible(false);
    _justificativaAutorizacaoCarga.Excluir.visible(false);
    _justificativaAutorizacaoCarga.Adicionar.visible(true);

    LimparCampos(_justificativaAutorizacaoCarga);
}
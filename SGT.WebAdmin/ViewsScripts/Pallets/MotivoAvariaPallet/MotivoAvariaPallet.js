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

var _motivoAvariaPallet;
var _pesquisaMotivoAvariaPallet;
var _gridMotivoAvariaPallet;

var MotivoAvariaPallet = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _status, def: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var PesquisaMotivoAvariaPallet = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:",issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ",issue: 557, val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
    this.Pesquisar = PropertyEntity({ eventClick: function () { _gridMotivoAvariaPallet.CarregarGrid(); }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

function loadMotivoAvariaPallet() {
    _pesquisaMotivoAvariaPallet = new PesquisaMotivoAvariaPallet();
    KoBindings(_pesquisaMotivoAvariaPallet, "knockoutPesquisaMotivoAvariaPallet", false, _pesquisaMotivoAvariaPallet.Pesquisar.id);

    _motivoAvariaPallet = new MotivoAvariaPallet();
    KoBindings(_motivoAvariaPallet, "knockoutMotivoAvariaPallet");

    HeaderAuditoria("MotivoAvariaPallet", _motivoAvariaPallet);

    buscarMotivoAvariaPallet();
}

function adicionarClick(e, sender) {
    Salvar(_motivoAvariaPallet, "MotivoAvariaPallet/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivoAvariaPallet.CarregarGrid();
                limparCamposMotivoAvariaPallet();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoAvariaPallet, "MotivoAvariaPallet/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoAvariaPallet.CarregarGrid();
                limparCamposMotivoAvariaPallet();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function buscarMotivoAvariaPallet() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var configuracoesExportacao = {
        url: "MotivoAvariaPallet/ExportarPesquisa",
        titulo: "Motivo Avaria Pallet"
    };

    _gridMotivoAvariaPallet = new GridViewExportacao(_pesquisaMotivoAvariaPallet.Pesquisar.idGrid, "MotivoAvariaPallet/Pesquisa", _pesquisaMotivoAvariaPallet, menuOpcoes, configuracoesExportacao);

    _gridMotivoAvariaPallet.CarregarGrid();
}

function cancelarClick(e) {
    limparCamposMotivoAvariaPallet();
}

function controlarBotoesHabilitados(isEdicao) {
    _motivoAvariaPallet.Atualizar.visible(isEdicao);
    _motivoAvariaPallet.Excluir.visible(isEdicao);
    _motivoAvariaPallet.Cancelar.visible(isEdicao);
    _motivoAvariaPallet.Adicionar.visible(!isEdicao);
}

function editarClick(registroSelecionado) {
    limparCamposMotivoAvariaPallet();

    _motivoAvariaPallet.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoAvariaPallet, "MotivoAvariaPallet/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaMotivoAvariaPallet.ExibirFiltros.visibleFade(false);

                var isEdicao = true;

                controlarBotoesHabilitados(isEdicao);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir esse cadastro?", function () {
        ExcluirPorCodigo(_motivoAvariaPallet, "MotivoAvariaPallet/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoAvariaPallet.CarregarGrid();
                    limparCamposMotivoAvariaPallet();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function limparCamposMotivoAvariaPallet() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);

    LimparCampos(_motivoAvariaPallet);
}
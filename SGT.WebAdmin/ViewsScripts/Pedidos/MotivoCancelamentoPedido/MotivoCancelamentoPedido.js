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

var _motivoCancelamentoPedido;
var _pesquisaMotivoCancelamentoPedido;
var _gridMotivoCancelamentoPedido;

var MotivoCancelamentoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.Motivo = PropertyEntity({ text: "Motivo de Cancelamento:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _status, def: true });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var PesquisaMotivoCancelamentoPedido = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", issue: 557, val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
    this.Pesquisar = PropertyEntity({ eventClick: function () { _gridMotivoCancelamentoPedido.CarregarGrid(); }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

function loadMotivoCancelamento() {
    _pesquisaMotivoCancelamentoPedido = new PesquisaMotivoCancelamentoPedido();
    KoBindings(_pesquisaMotivoCancelamentoPedido, "knockoutPesquisaMotivoCancelamentoPedido", false, _pesquisaMotivoCancelamentoPedido.Pesquisar.id);

    _motivoCancelamentoPedido = new MotivoCancelamentoPedido();
    KoBindings(_motivoCancelamentoPedido, "knockoutMotivoCancelamentoPedido");

    HeaderAuditoria("MotivoCancelamentoPedido", _motivoCancelamentoPedido);

    buscarMotivoCancelamento();
}

function adicionarClick(e, sender) {
    Salvar(_motivoCancelamentoPedido, "MotivoCancelamentoPedido/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivoCancelamentoPedido.CarregarGrid();
                limparCamposMotivoCancelamentoPedido();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoCancelamentoPedido, "MotivoCancelamentoPedido/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoCancelamentoPedido.CarregarGrid();
                limparCamposMotivoCancelamentoPedido();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function buscarMotivoCancelamento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var configuracoesExportacao = {
        url: "MotivoCancelamentoPedido/ExportarPesquisa",
        titulo: "Motivo de Cancelamento Pedido"
    };

    _gridMotivoCancelamentoPedido = new GridViewExportacao(_pesquisaMotivoCancelamentoPedido.Pesquisar.idGrid, "MotivoCancelamentoPedido/Pesquisa", _pesquisaMotivoCancelamentoPedido, menuOpcoes, configuracoesExportacao);

    _gridMotivoCancelamentoPedido.CarregarGrid();
}

function cancelarClick(e) {
    limparCamposMotivoCancelamentoPedido();
}

function controlarBotoesHabilitados(isEdicao) {
    _motivoCancelamentoPedido.Atualizar.visible(isEdicao);
    _motivoCancelamentoPedido.Excluir.visible(isEdicao);
    _motivoCancelamentoPedido.Cancelar.visible(isEdicao);
    _motivoCancelamentoPedido.Adicionar.visible(!isEdicao);
}

function editarClick(registroSelecionado) {
    limparCamposMotivoCancelamentoPedido();

    _motivoCancelamentoPedido.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoCancelamentoPedido, "MotivoCancelamentoPedido/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaMotivoCancelamentoPedido.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_motivoCancelamentoPedido, "MotivoCancelamentoPedido/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoCancelamentoPedido.CarregarGrid();
                    limparCamposMotivoCancelamentoPedido();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function limparCamposMotivoCancelamentoPedido() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);

    LimparCampos(_motivoCancelamentoPedido);
}
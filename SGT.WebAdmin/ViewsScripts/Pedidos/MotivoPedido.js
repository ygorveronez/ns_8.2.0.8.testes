/// <reference path="../../js/libs/jquery-2.1.1.js" />
/// <reference path="../../js/Global/CRUD.js" />
/// <reference path="../../js/Global/knockout-3.1.0.js" />
/// <reference path="../../js/Global/Rest.js" />
/// <reference path="../../js/Global/Mensagem.js" />
/// <reference path="../../js/Global/Grid.js" />
/// <reference path="../../js/bootstrap/bootstrap.js" />
/// <reference path="../../js/libs/jquery.blockui.js" />
/// <reference path="../../js/Global/knoutViewsSlides.js" />
/// <reference path="../../js/libs/jquery.maskMoney.js" />
/// <reference path="../../js/plugin/datatables/jquery.dataTables.js" />
/// <reference path="../Enumeradores/EnumTipoMotivoPedido.js" />

var _motivoPedido;
var _pesquisaMotivoPedido;
var _gridMotivoPedido;

var MotivoPedido = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable(""), maxlength: 200 });
    this.Observacao = PropertyEntity({ text: "Observação:", getType: typesKnockout.string, val: ko.observable(""), maxlength: 2000 });
    this.Status = PropertyEntity({ text: "Situação: ", val: ko.observable(true), options: _status, def: true });
    this.TipoMotivo = PropertyEntity({ text: "*Tipo: ", val: ko.observable(true), required: true, options: EnumTipoMotivoPedido.obterOpcoes(), def: true, visible: ko.observable(true) });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
}

var PesquisaMotivoPedido = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Situação: ", issue: 557, val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.ExibirFiltros = PropertyEntity({
        eventClick: function (e) {
            e.ExibirFiltros.visibleFade(!e.ExibirFiltros.visibleFade());
        }, type: types.event, text: "Filtros de Pesquisa", idFade: guid(), visibleFade: ko.observable(false), visible: ko.observable(true)
    });
    this.Pesquisar = PropertyEntity({ eventClick: function () { _gridMotivoPedido.CarregarGrid(); }, type: types.event, text: "Pesquisar", idGrid: guid(), visible: ko.observable(true) });
}

function loadMotivoPedido() {
    _pesquisaMotivoPedido = new PesquisaMotivoPedido();
    KoBindings(_pesquisaMotivoPedido, "knockoutPesquisaMotivoPedido", false, _pesquisaMotivoPedido.Pesquisar.id);

    _motivoPedido = new MotivoPedido();
    KoBindings(_motivoPedido, "knockoutMotivoPedido");

    HeaderAuditoria("MotivoPedido", _motivoPedido);

    buscarMotivoPedido();
}

function adicionarClick(e, sender) {
    Salvar(_motivoPedido, "MotivoPedido/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivoPedido.CarregarGrid();
                limparCamposMotivoPedido();
            }
            else
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
        }
        else
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoPedido, "MotivoPedido/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoPedido.CarregarGrid();
                limparCamposMotivoPedido();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function buscarMotivoPedido() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarClick, tamanho: "10", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    var configuracoesExportacao = {
        url: "MotivoPedido/ExportarPesquisa",
        titulo: "Motivo Pedido"
    };

    _gridMotivoPedido = new GridViewExportacao(_pesquisaMotivoPedido.Pesquisar.idGrid, "MotivoPedido/Pesquisa", _pesquisaMotivoPedido, menuOpcoes, configuracoesExportacao);

    _gridMotivoPedido.CarregarGrid();
}

function cancelarClick(e) {
    limparCamposMotivoPedido();
}

function controlarBotoesHabilitados(isEdicao) {
    _motivoPedido.Atualizar.visible(isEdicao);
    _motivoPedido.Excluir.visible(isEdicao);
    _motivoPedido.Cancelar.visible(isEdicao);
    _motivoPedido.Adicionar.visible(!isEdicao);
}

function editarClick(registroSelecionado) {
    limparCamposMotivoPedido();

    _motivoPedido.Codigo.val(registroSelecionado.Codigo);

    BuscarPorCodigo(_motivoPedido, "MotivoPedido/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaMotivoPedido.ExibirFiltros.visibleFade(false);

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
        ExcluirPorCodigo(_motivoPedido, "MotivoPedido/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoPedido.CarregarGrid();
                    limparCamposMotivoPedido();
                } else {
                    exibirMensagem(tipoMensagem.aviso, "Sugestão", arg.Msg, 16000);
                }
            } else {
                exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
            }

        }, null);
    });
}

function limparCamposMotivoPedido() {
    var isEdicao = false;

    controlarBotoesHabilitados(isEdicao);

    LimparCampos(_motivoPedido);
}
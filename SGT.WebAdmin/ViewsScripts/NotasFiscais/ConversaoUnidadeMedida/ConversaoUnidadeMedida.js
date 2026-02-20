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

//*******MAPEAMENTO KNOUCKOUT*******

var _conversaoUnidadeMedida;
var _pesquisaConversaoUnidadeMedida;
var _gridConversaoUnidadeMedida;

var ConversaoUnidadeMedida = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Fator = PropertyEntity({ def: "0,00000", val: ko.observable("0,00000"), text: "Fator: ", required: false, getType: typesKnockout.decimal, maxlength: 22, configDecimal: { precision: 5, allowZero: true, allowNegative: true }, issue: 1047 });
    this.UnidadeMedidaOrigem = PropertyEntity({ val: ko.observable(EnumUnidadeMedida.Quilograma), options: EnumUnidadeMedida.obterOpcoes(), text: "Unidade de Medida Origem: ", def: EnumUnidadeMedida.Quilograma, issue: 88 });
    this.UnidadeMedidaDestino = PropertyEntity({ val: ko.observable(EnumUnidadeMedida.Quilograma), options: EnumUnidadeMedida.obterOpcoes(), text: "Unidade de Medida Destino: ", def: EnumUnidadeMedida.Quilograma, issue: 88 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

var PesquisaConversaoUnidadeMedida = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridConversaoUnidadeMedida.CarregarGrid();
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

//*******EVENTOS*******
function loadConversaoUnidadeMedida() {
    _pesquisaConversaoUnidadeMedida = new PesquisaConversaoUnidadeMedida();
    KoBindings(_pesquisaConversaoUnidadeMedida, "knockoutPesquisaConversaoUnidadeMedida", false, _pesquisaConversaoUnidadeMedida.Pesquisar.id);

    _conversaoUnidadeMedida = new ConversaoUnidadeMedida();
    KoBindings(_conversaoUnidadeMedida, "knockoutConversaoUnidadeMedida");

    HeaderAuditoria("ConversaoUnidadeMedida", _conversaoUnidadeMedida);

    CarregarGridConversaoUnidadeMedida();
}

function adicionarClick(e, sender) {
    Salvar(_conversaoUnidadeMedida, "ConversaoUnidadeMedida/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridConversaoUnidadeMedida.CarregarGrid();
                LimparCamposConversaoUnidadeMedida();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_conversaoUnidadeMedida, "ConversaoUnidadeMedida/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridConversaoUnidadeMedida.CarregarGrid();
                LimparCamposConversaoUnidadeMedida();
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
        ExcluirPorCodigo(_conversaoUnidadeMedida, "ConversaoUnidadeMedida/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridConversaoUnidadeMedida.CarregarGrid();
                    LimparCamposConversaoUnidadeMedida();
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
    LimparCamposConversaoUnidadeMedida();
}

function editarConversaoUnidadeMedidaClick(itemGrid) {
    LimparCamposConversaoUnidadeMedida();
    _conversaoUnidadeMedida.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_conversaoUnidadeMedida, "ConversaoUnidadeMedida/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaConversaoUnidadeMedida.ExibirFiltros.visibleFade(false);

                _conversaoUnidadeMedida.Atualizar.visible(true);
                _conversaoUnidadeMedida.Excluir.visible(true);
                _conversaoUnidadeMedida.Cancelar.visible(true);
                _conversaoUnidadeMedida.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******
function CarregarGridConversaoUnidadeMedida() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarConversaoUnidadeMedidaClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridConversaoUnidadeMedida = new GridView(_pesquisaConversaoUnidadeMedida.Pesquisar.idGrid, "ConversaoUnidadeMedida/Pesquisa", _pesquisaConversaoUnidadeMedida, menuOpcoes, null);
    _gridConversaoUnidadeMedida.CarregarGrid();
}

function LimparCamposConversaoUnidadeMedida() {
    _conversaoUnidadeMedida.Atualizar.visible(false);
    _conversaoUnidadeMedida.Cancelar.visible(false);
    _conversaoUnidadeMedida.Excluir.visible(false);
    _conversaoUnidadeMedida.Adicionar.visible(true);
    LimparCampos(_conversaoUnidadeMedida);
}
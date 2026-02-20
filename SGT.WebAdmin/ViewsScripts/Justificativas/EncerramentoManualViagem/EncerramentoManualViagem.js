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

var _encerramentoManualViagem;
var _pesquisaEncerramentoManual;
var _gridEncerramentoManual;

var PesquisaMoedaEncerramentoManualViagem = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Status: ", val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridEncerramentoManual.CarregarGrid();
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

var EncerramentoManualViagem = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição:",required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Situacao = PropertyEntity({ text: "Status: ", val: ko.observable(true), options: _status, def: true });
    this.Observacao = PropertyEntity({ text: "Observação: ", maxlength: 3000 });

    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
}

//*******EVENTOS*******
function loadJustificativaEncerramentoViagem() {
    _pesquisaEncerramentoManual = new PesquisaMoedaEncerramentoManualViagem();
    KoBindings(_pesquisaEncerramentoManual, "knockoutPesquisaEncerramentoManual", false, _pesquisaEncerramentoManual.Pesquisar.id);

    _encerramentoManualViagem = new EncerramentoManualViagem();
    KoBindings(_encerramentoManualViagem, "knockoutJustificativaManualViagem");

    HeaderAuditoria("JustificativaManualViagem", _encerramentoManualViagem);

    BuscarEncerramentoManualViagem();
}

function adicionarClick(e, sender) {
    Salvar(_encerramentoManualViagem, "EncerramentoManualViagem/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridEncerramentoManual.CarregarGrid();
                LimparCamposMoedaEncerramentoManualViagem();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_encerramentoManualViagem, "EncerramentoManualViagem/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridEncerramentoManual.CarregarGrid();
                LimparCamposMoedaEncerramentoManualViagem();
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
        ExcluirPorCodigo(_encerramentoManualViagem, "EncerramentoManualViagem/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridEncerramentoManual.CarregarGrid();
                    LimparCamposMoedaEncerramentoManualViagem();
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
    LimparCamposMoedaEncerramentoManualViagem();
}

function editarPesquisaMoedaEncerramentoManualViagemClick(itemGrid) {
    LimparCamposMoedaEncerramentoManualViagem();

    _encerramentoManualViagem.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_encerramentoManualViagem, "EncerramentoManualViagem/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaEncerramentoManual.ExibirFiltros.visibleFade(false);
                _encerramentoManualViagem.Atualizar.visible(true);
                _encerramentoManualViagem.Excluir.visible(true);
                _encerramentoManualViagem.Cancelar.visible(true);
                _encerramentoManualViagem.Adicionar.visible(false);
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}



//*******MÉTODOS*******
function BuscarEncerramentoManualViagem() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarPesquisaMoedaEncerramentoManualViagemClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridEncerramentoManual = new GridView(_pesquisaEncerramentoManual.Pesquisar.idGrid, "EncerramentoManualViagem/Pesquisa", _pesquisaEncerramentoManual, menuOpcoes, null);
    _gridEncerramentoManual.CarregarGrid();
}

function LimparCamposMoedaEncerramentoManualViagem() {
    _encerramentoManualViagem.Atualizar.visible(false);
    _encerramentoManualViagem.Cancelar.visible(false);
    _encerramentoManualViagem.Excluir.visible(false);
    _encerramentoManualViagem.Adicionar.visible(true);
    LimparCampos(_encerramentoManualViagem);
}
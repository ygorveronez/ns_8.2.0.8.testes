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
/// <reference path="../../Enumeradores/EnumTipoMotivoRecusaCancelamento.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _motivoRecusaCancelamento;
var _crudMotivoRecusaCancelamento;
var _pesquisaObjeto;
var _gridObjeto;
var _gridMotivoRecusaCancelamento;

var MotivoRecusaCancelamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });
    this.Descricao = PropertyEntity({ text: "*Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", issue: 557, val: ko.observable(true), options: _status, def: true });
    this.Observacao = PropertyEntity({ text: "Observação:", issue: 593, getType: typesKnockout.string, val: ko.observable("") });
    this.TipoMotivoRecusaCancelamento = PropertyEntity({ text: "Tipo:  ", val: ko.observable(EnumTipoMotivoRecusaCancelamento.Todos), options: EnumTipoMotivoRecusaCancelamento.obterOpcoes(), def: EnumTipoMotivoRecusaCancelamento.Todos, required: ko.observable(true) });
};

var CRUDMotivoRecusaCancelamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

var PesquisaMotivoRecusaCancelamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição:", issue: 586, required: true, getType: typesKnockout.string, val: ko.observable("") });
    this.Status = PropertyEntity({ text: "Status: ", issue: 556, val: ko.observable(0), options: _statusPesquisa, def: 0 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoRecusaCancelamento.CarregarGrid();
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

function loadMotivoRecusaCancelamento() {
    _pesquisaMotivoRecusaCancelamento = new PesquisaMotivoRecusaCancelamento();
    KoBindings(_pesquisaMotivoRecusaCancelamento, "knockoutPesquisaMotivoRecusaCancelamento", false, _pesquisaMotivoRecusaCancelamento.Pesquisar.id);

    _motivoRecusaCancelamento = new MotivoRecusaCancelamento();
    KoBindings(_motivoRecusaCancelamento, "knockoutMotivoRecusaCancelamento");

    HeaderAuditoria("MotivoRecusaCancelamento", _motivoRecusaCancelamento);

    _crudMotivoRecusaCancelamento = new CRUDMotivoRecusaCancelamento();
    KoBindings(_crudMotivoRecusaCancelamento, "knockoutCRUDMotivoRecusaCancelamento");

    buscarMotivoRecusaCancelamento();
}

function adicionarClick(e, sender) {
    Salvar(_motivoRecusaCancelamento, "MotivoRecusaCancelamento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso.");
                _gridMotivoRecusaCancelamento.CarregarGrid();
                limparCamposMotivoRecusaCancelamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoRecusaCancelamento, "MotivoRecusaCancelamento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoRecusaCancelamento.CarregarGrid();
                limparCamposMotivoRecusaCancelamento();
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
        ExcluirPorCodigo(_motivoRecusaCancelamento, "MotivoRecusaCancelamento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoRecusaCancelamento.CarregarGrid();
                    limparCamposMotivoRecusaCancelamento();
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
    limparCamposMotivoRecusaCancelamento();
}

function editarMotivoRecusaCancelamentoClick(itemGrid) {
    limparCamposMotivoRecusaCancelamento();
    _motivoRecusaCancelamento.Codigo.val(itemGrid.Codigo);

    BuscarPorCodigo(_motivoRecusaCancelamento, "MotivoRecusaCancelamento/BuscarPorCodigo", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                _pesquisaMotivoRecusaCancelamento.ExibirFiltros.visibleFade(false);

                _crudMotivoRecusaCancelamento.Atualizar.visible(true);
                _crudMotivoRecusaCancelamento.Excluir.visible(true);
                _crudMotivoRecusaCancelamento.Cancelar.visible(true);
                _crudMotivoRecusaCancelamento.Adicionar.visible(false);

            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, null);
}

//*******MÉTODOS*******

function buscarMotivoRecusaCancelamento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMotivoRecusaCancelamentoClick, tamanho: "20", icone: "" };

    var menuOpcoes = {
        tipo: TypeOptionMenu.link,
        opcoes: [editar]
    };

    _gridMotivoRecusaCancelamento = new GridView(_pesquisaMotivoRecusaCancelamento.Pesquisar.idGrid, "MotivoRecusaCancelamento/Pesquisa", _pesquisaMotivoRecusaCancelamento, menuOpcoes, null);
    _gridMotivoRecusaCancelamento.CarregarGrid();
}

function limparCamposMotivoRecusaCancelamento() {
    _crudMotivoRecusaCancelamento.Atualizar.visible(false);
    _crudMotivoRecusaCancelamento.Cancelar.visible(false);
    _crudMotivoRecusaCancelamento.Excluir.visible(false);
    _crudMotivoRecusaCancelamento.Adicionar.visible(true);
    LimparCampos(_motivoRecusaCancelamento);
}

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
/// <reference path="../../Consultas/TipoResponsavelAtrasoEntrega.js" />

//*******MAPEAMENTO KNOUCKOUT*******

var _gridMotivoReagendamento;
var _motivoReagendamento;
var _pesquisaMotivoReagendamento;

var PesquisaMotivoReagendamento = function () {
    this.Descricao = PropertyEntity({ text: "Descrição: " });
    this.Ativo = PropertyEntity({ text: "Status: ", val: ko.observable(1), options: _statusPesquisa, def: 1 });

    this.Pesquisar = PropertyEntity({
        eventClick: function (e) {
            _gridMotivoReagendamento.CarregarGrid();
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

var MotivoReagendamento = function () {
    this.Codigo = PropertyEntity({ val: ko.observable(0), def: 0, getType: typesKnockout.int });

    this.Descricao = PropertyEntity({ text: "*Descrição: ", required: ko.observable(true), maxlength: 200 });
    this.Ativo = PropertyEntity({ text: "*Status: ", val: ko.observable(true), options: _status, def: true, required: ko.observable(true), enable: ko.observable(true) });
    this.TipoResponsavelAtrasoEntrega = PropertyEntity({ type: types.entity, codEntity: ko.observable(0), text: "Tipo de Responsável pelo Atraso da Entrega:", idBtnSearch: guid() });
    this.Observacao = PropertyEntity({ text: "Observação: ", val: ko.observable(""), def: "" });
    this.ConsiderarOnTime = PropertyEntity({ text: "Considerar On Time ", val: ko.observable(""), def: "" });
};

var CRUDMotivoReagendamento = function () {
    this.Adicionar = PropertyEntity({ eventClick: adicionarClick, type: types.event, text: "Adicionar", visible: ko.observable(true) });
    this.Atualizar = PropertyEntity({ eventClick: atualizarClick, type: types.event, text: "Atualizar", visible: ko.observable(false) });
    this.Excluir = PropertyEntity({ eventClick: excluirClick, type: types.event, text: "Excluir", visible: ko.observable(false) });
    this.Cancelar = PropertyEntity({ eventClick: cancelarClick, type: types.event, text: "Cancelar", visible: ko.observable(false) });
};

//*******EVENTOS*******

function loadMotivoReagendamento() {
    _motivoReagendamento = new MotivoReagendamento();
    KoBindings(_motivoReagendamento, "knockoutCadastroMotivoReagendamento");

    HeaderAuditoria("MotivoReagendamento", _motivoReagendamento);

    _crudMotivoReagendamento = new CRUDMotivoReagendamento();
    KoBindings(_crudMotivoReagendamento, "knockoutCRUDMotivoReagendamento");

    _pesquisaMotivoReagendamento = new PesquisaMotivoReagendamento();
    KoBindings(_pesquisaMotivoReagendamento, "knockoutPesquisaMotivoReagendamento", false, _pesquisaMotivoReagendamento.Pesquisar.id);

    new BuscarTipoResponsavelAtrasoEntrega(_motivoReagendamento.TipoResponsavelAtrasoEntrega);

    buscarMotivoReagendamento();
}

function adicionarClick(e, sender) {
    Salvar(_motivoReagendamento, "MotivoReagendamento/Adicionar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Cadastrado com sucesso");
                _gridMotivoReagendamento.CarregarGrid();
                limparCamposMotivoReagendamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function atualizarClick(e, sender) {
    Salvar(_motivoReagendamento, "MotivoReagendamento/Atualizar", function (arg) {
        if (arg.Success) {
            if (arg.Data) {
                exibirMensagem(tipoMensagem.ok, "Sucesso", "Atualizado com sucesso");
                _gridMotivoReagendamento.CarregarGrid();
                limparCamposMotivoReagendamento();
            } else {
                exibirMensagem(tipoMensagem.aviso, "Aviso", arg.Msg);
            }
        } else {
            exibirMensagem(tipoMensagem.falha, "Falha", arg.Msg);
        }
    }, sender);
}

function excluirClick(e, sender) {
    exibirConfirmacao("Confirmação", "Realmente deseja excluir o Tipo de MotivoReagendamento " + _motivoReagendamento.Descricao.val() + "?", function () {
        ExcluirPorCodigo(_motivoReagendamento, "MotivoReagendamento/ExcluirPorCodigo", function (arg) {
            if (arg.Success) {
                if (arg.Data) {
                    exibirMensagem(tipoMensagem.ok, "Sucesso", "Excluído com sucesso");
                    _gridMotivoReagendamento.CarregarGrid();
                    limparCamposMotivoReagendamento();
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
    limparCamposMotivoReagendamento();
}

//*******MÉTODOS*******

function buscarMotivoReagendamento() {
    var editar = { descricao: "Editar", id: "clasEditar", evento: "onclick", metodo: editarMotivoReagendamento, tamanho: "15", icone: "" };
    var menuOpcoes = new Object();
    menuOpcoes.tipo = TypeOptionMenu.link;
    menuOpcoes.opcoes = new Array();
    menuOpcoes.opcoes.push(editar);

    _gridMotivoReagendamento = new GridView(_pesquisaMotivoReagendamento.Pesquisar.idGrid, "MotivoReagendamento/Pesquisa", _pesquisaMotivoReagendamento, menuOpcoes);
    _gridMotivoReagendamento.CarregarGrid();
}

function editarMotivoReagendamento(MotivoReagendamentoGrid) {
    limparCamposMotivoReagendamento();
    _motivoReagendamento.Codigo.val(MotivoReagendamentoGrid.Codigo);
    BuscarPorCodigo(_motivoReagendamento, "MotivoReagendamento/BuscarPorCodigo", function (arg) {
        _pesquisaMotivoReagendamento.ExibirFiltros.visibleFade(false);
        _crudMotivoReagendamento.Atualizar.visible(true);
        _crudMotivoReagendamento.Cancelar.visible(true);
        _crudMotivoReagendamento.Excluir.visible(true);
        _crudMotivoReagendamento.Adicionar.visible(false);
    }, null);
}

function limparCamposMotivoReagendamento() {
    _crudMotivoReagendamento.Atualizar.visible(false);
    _crudMotivoReagendamento.Cancelar.visible(false);
    _crudMotivoReagendamento.Excluir.visible(false);
    _crudMotivoReagendamento.Adicionar.visible(true);
    LimparCampos(_motivoReagendamento);
}